using System;
using System.Collections.Generic;
using System.Linq;

namespace DataLightning.Core.Operators
{
    public class Join2<TLeftInput, TRightInput> : ISubscribable<(IList<TLeftInput> Left, IList<TRightInput> Right)>
    {
        private readonly SubscriptionManager<(IList<TLeftInput> Left, IList<TRightInput> Right)> _subscriptionManager = new();
        private readonly InputManager _inputManager = new();

        private readonly (object Key, Func<TLeftInput, object> KeyGetter, Dictionary<object, Dictionary<object, TLeftInput>> Data) _left;
        private readonly (object Key, Func<TRightInput, object> KeyGetter, Dictionary<object, Dictionary<object, TRightInput>> Data) _right;
        private readonly Func<TLeftInput, object> _leftEntityKeyGetter;
        private readonly Func<TRightInput, object> _rightEntityKeyGetter;

        public Join2(
            ISubscribable<TLeftInput> leftInput, ISubscribable<TRightInput> rightInput,
            Func<TLeftInput, object> leftKeyGetter, Func<TRightInput, object> rightKeyGetter,
            Func<TLeftInput, object> leftEntityKeyGetter, Func<TRightInput, object> rightEntityKeyGetter) 
        {
            _inputManager.Add("left", leftInput, (_, value) => OnLeftChanged(value));
            _inputManager.Add("right", rightInput, (_, value) => OnRightChanged(value));

            _left = (Key: leftInput, KeyGetter: leftKeyGetter, Data: new Dictionary<object, Dictionary<object, TLeftInput>>());
            _right = (Key: rightInput, KeyGetter: rightKeyGetter, Data: new Dictionary<object, Dictionary<object, TRightInput>>());
            _leftEntityKeyGetter = leftEntityKeyGetter;
            _rightEntityKeyGetter = rightEntityKeyGetter;
        }

        private void OnRightChanged(TRightInput rightInput)
        {
            var key = _right.KeyGetter(rightInput);
            if (!_right.Data.ContainsKey(key))
                _right.Data[key] = new Dictionary<object, TRightInput>();
            _right.Data[key][_rightEntityKeyGetter(rightInput)] = rightInput;

            if (_left.Data.ContainsKey(key))
                _subscriptionManager.NotifySubscribers(
                    (_left.Data[key].Values.ToList(), _right.Data[key].Values.ToList()));
            else
                _subscriptionManager.NotifySubscribers(
                    (new List<TLeftInput>(), _right.Data[key].Values.ToList()));
        }

        private void OnLeftChanged(TLeftInput leftInput)
        {
            var key = _left.KeyGetter(leftInput);
            if (!_left.Data.ContainsKey(key))
                _left.Data[key] = new Dictionary<object, TLeftInput>();
            _left.Data[key][_leftEntityKeyGetter(leftInput)] = leftInput;

            if (_right.Data.ContainsKey(key))
                _subscriptionManager.NotifySubscribers(
                    (_left.Data[key].Values.ToList(), _right.Data[key].Values.ToList()));
            else
                _subscriptionManager.NotifySubscribers(
                    (_left.Data[key].Values.ToList(), new List<TRightInput>()));
        }

        public ISubscription Subscribe(ISubscriber<(IList<TLeftInput> Left, IList<TRightInput> Right)> subscriber) => 
            _subscriptionManager.Subscribe(subscriber);
    }
}