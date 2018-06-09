using System;
using System.Collections.Generic;
using System.Linq;

namespace DataLightning.Core.Operators
{
    public class Join : CalcUnitBase
    {
        private readonly IDictionary<object, (Func<object, object> KeyGetter, IDictionary<object, IList<object>> Data)> _inputConfigs = new Dictionary<object, (Func<object, object> KeyGetter, IDictionary<object, IList<object>> Data)>();
        private readonly object _leftInput;
        private readonly object _rightInput;

        public Join(Func<object, object> leftKeyGetter, Func<object, object> rightKeyGetter): this("left", "right", leftKeyGetter, rightKeyGetter)
        {

        }

        public Join(object leftInput, object rightInput, Func<object, object> leftKeyGetter, Func<object, object> rightKeyGetter) : base(new[] { leftInput, rightInput })
        {
            _inputConfigs[leftInput] = (leftKeyGetter, new Dictionary<object, IList<object>>());
            _inputConfigs[rightInput] = (rightKeyGetter, new Dictionary<object, IList<object>>());
            _leftInput = leftInput;
            _rightInput = rightInput;
        }

        protected override object Execute(IDictionary<object, object> args, object changedInput)
        {
            if (args[changedInput] == null)
                return null;

            var (KeyGetter, Data) = _inputConfigs[changedInput];
            var key = KeyGetter(args[changedInput]);

            if (!_inputConfigs[changedInput].Data.ContainsKey(key))
                _inputConfigs[changedInput].Data[key] = new List<object>();

            if (!_inputConfigs[changedInput].Data[key].Contains(args[changedInput]))
                _inputConfigs[changedInput].Data[key].Add(args[changedInput]);

            if (_inputConfigs.Values.Any(i => !i.Data.ContainsKey(key)))
                return null;

            return _inputConfigs.Keys.ToDictionary(k => k, k => _inputConfigs[k].Data[key]);
        }
    }
}