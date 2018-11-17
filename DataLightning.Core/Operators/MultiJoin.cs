using System.Collections.Generic;
using System.Linq;

namespace DataLightning.Core.Operators
{
    public class MultiJoin : CalcUnitBase<object, IDictionary<string, IList<object>>>
    {
        private readonly Dictionary<IJoinDefinition, Dictionary<object, Dictionary<object, object>>> _data = new Dictionary<IJoinDefinition, Dictionary<object, Dictionary<object, object>>>();

        public MultiJoin(params IJoinDefinition[] inputs) : base(inputs)
        {
        }

        protected override IDictionary<string, IList<object>> Execute(IDictionary<object, object> args, object changedInput)
        {
            var result = new Dictionary<string, IList<object>>();
            var value = args[changedInput];

            if (value == null)
                return result;

            var input = changedInput as IJoinDefinition;

            var key = input.GetJoinKey(value);

            SaveValue(value, input, key);

            foreach (var joinDefinition in _data.Keys)
                if (_data[joinDefinition].ContainsKey(key))
                    result[joinDefinition.Name] = _data[joinDefinition][key].Values.ToList();

            return result;
        }

        private void SaveValue(object value, IJoinDefinition input, object key)
        {
            if (!_data.ContainsKey(input))
                _data[input] = new Dictionary<object, Dictionary<object, object>>();

            if (!_data[input].ContainsKey(key))
                _data[input][key] = new Dictionary<object, object>();

            _data[input][key][input.GetEntityKey(value)] = value;
        }
    }
}