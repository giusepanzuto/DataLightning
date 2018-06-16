using System.Collections.Generic;
using System.Linq;

namespace DataLightning.Core.Operators
{
    public class MultiJoin : CalcUnitBase<object, IDictionary<string, IList<object>>>
    {
        private readonly IEnumerable<ISubscribable<object>> _inputs;
        private readonly Dictionary<IJoinDefinition, Dictionary<object, List<object>>> _data = new Dictionary<IJoinDefinition, Dictionary<object, List<object>>>();

        public MultiJoin(params IJoinDefinition[] inputs) : base(inputs)
        {
            _inputs = inputs;
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
                    result[joinDefinition.Name] = _data[joinDefinition][key].ToList();

            return result;
        }

        private void SaveValue(object value, IJoinDefinition input, object key)
        {
            if (!_data.ContainsKey(input))
                _data[input] = new Dictionary<object, List<object>>();

            if (!_data[input].ContainsKey(key))
                _data[input][key] = new List<object>();

            _data[input][key].Add(value);
        }
    }
}