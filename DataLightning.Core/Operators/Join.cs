using System;
using System.Collections.Generic;

namespace DataLightning.Core.Operators
{
    public class Join<TLeftInput, TRightInput> : CalcUnitBase<object, (IList<TLeftInput> Left, IList<TRightInput> Right)>
    {
        private readonly (object Key, Func<TLeftInput, object> KeyGetter, Dictionary<object, List<TLeftInput>> Data) _left;
        private readonly (object Key, Func<TRightInput, object> KeyGetter, Dictionary<object, List<TRightInput>> Data) _right;

        public Join(Func<TLeftInput, object> leftKeyGetter, Func<TRightInput, object> rightKeyGetter) : this("left", "right", leftKeyGetter, rightKeyGetter)
        {
        }

        public Join(object leftInput, object rightInput, Func<TLeftInput, object> leftKeyGetter, Func<TRightInput, object> rightKeyGetter) : base(new[] { leftInput, rightInput })
        {
            _left = (Key: leftInput, KeyGetter: leftKeyGetter, Data: new Dictionary<object, List<TLeftInput>>());
            _right = (Key: rightInput, KeyGetter: rightKeyGetter, Data: new Dictionary<object, List<TRightInput>>());
        }

        protected override (IList<TLeftInput> Left, IList<TRightInput> Right) Execute(IDictionary<object, object> args, object changedInput)
        {
            if (args[changedInput] == null)
                return (new List<TLeftInput>(), new List<TRightInput>());

            object key;

            if (changedInput == _left.Key)
            {
                TLeftInput leftInput = (TLeftInput)args[changedInput];
                key = _left.KeyGetter(leftInput);
                if (!_left.Data.ContainsKey(key))
                    _left.Data[key] = new List<TLeftInput>();
                _left.Data[key].Add(leftInput);

                if (_right.Data.ContainsKey(key))
                    return (_left.Data[key], _right.Data[key]);

                return (_left.Data[key], new List<TRightInput>());
            }
            else
            {
                TRightInput rightInput = (TRightInput)args[changedInput];
                key = _right.KeyGetter(rightInput);
                if (!_right.Data.ContainsKey(key))
                    _right.Data[key] = new List<TRightInput>();
                _right.Data[key].Add(rightInput);

                if (_left.Data.ContainsKey(key))
                    return (_left.Data[key], _right.Data[key]);

                return (new List<TLeftInput>(), _right.Data[key]);
            }
        }
    }
}