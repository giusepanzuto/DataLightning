using System;
using System.Collections.Generic;
using System.Linq;

namespace DataLightning.Core.Operators
{
    public class Join<TLeftInput, TRightInput> : CalcUnitBase<object, (IList<TLeftInput> Left, IList<TRightInput> Right)>
    {
        private readonly (object Key, Func<TLeftInput, object> KeyGetter, Dictionary<object, Dictionary<object, TLeftInput>> Data) _left;
        private readonly (object Key, Func<TRightInput, object> KeyGetter, Dictionary<object, Dictionary<object, TRightInput>> Data) _right;
        private readonly Func<TLeftInput, object> _leftEntityKeyGetter;
        private readonly Func<TRightInput, object> _rightEntityKeyGetter;

        public Join(
            ISubscribable<TLeftInput> leftInput, ISubscribable<TRightInput> rightInput,
            Func<TLeftInput, object> leftKeyGetter, Func<TRightInput, object> rightKeyGetter,
            Func<TLeftInput, object> leftEntityKeyGetter, Func<TRightInput, object> rightEntityKeyGetter) :
            base(new ISubscribable<object>[] {
                new SubscriberAdapter<TLeftInput, object>(leftInput),
                new SubscriberAdapter<TRightInput, object>(rightInput) })
        {
            _left = (Key: leftInput, KeyGetter: leftKeyGetter, Data: new Dictionary<object, Dictionary<object, TLeftInput>>());
            _right = (Key: rightInput, KeyGetter: rightKeyGetter, Data: new Dictionary<object, Dictionary<object, TRightInput>>());
            _leftEntityKeyGetter = leftEntityKeyGetter;
            _rightEntityKeyGetter = rightEntityKeyGetter;
        }

        protected override (IList<TLeftInput> Left, IList<TRightInput> Right) Execute(IDictionary<object, object> args, object changedInput)
        {
            if (args[changedInput] == null)
                return (new List<TLeftInput>(), new List<TRightInput>());

            object key;

            if (changedInput == _left.Key || (changedInput is SubscriberAdapter<TLeftInput, object> leftAdapter && leftAdapter.Adaptee == _left.Key))
            {
                TLeftInput leftInput = (TLeftInput)args[changedInput];
                key = _left.KeyGetter(leftInput);
                if (!_left.Data.ContainsKey(key))
                    _left.Data[key] = new Dictionary<object, TLeftInput>();
                _left.Data[key][_leftEntityKeyGetter(leftInput)] = leftInput;

                if (_right.Data.ContainsKey(key))
                    return (_left.Data[key].Values.ToList(), _right.Data[key].Values.ToList());

                return (_left.Data[key].Values.ToList(), new List<TRightInput>());
            }
            else if (changedInput == _right.Key || (changedInput is SubscriberAdapter<TRightInput, object> rightAdapter && rightAdapter.Adaptee == _right.Key))
            {
                TRightInput rightInput = (TRightInput)args[changedInput];
                key = _right.KeyGetter(rightInput);
                if (!_right.Data.ContainsKey(key))
                    _right.Data[key] = new Dictionary<object, TRightInput>();
                _right.Data[key][_rightEntityKeyGetter(rightInput)] = rightInput;

                if (_left.Data.ContainsKey(key))
                    return (_left.Data[key].Values.ToList(), _right.Data[key].Values.ToList());

                return (new List<TLeftInput>(), _right.Data[key].Values.ToList());
            }
            else
                throw new ArgumentException();
        }
    }
}