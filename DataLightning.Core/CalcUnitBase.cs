using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DataLightning.Core
{
    public abstract class CalcUnitBase : ICalcUnit
    {
        private readonly Dictionary<object, IInput> _inputs = new Dictionary<object, IInput>();
        private readonly IList<ICalcUnitSubscriber> _subscribers = new List<ICalcUnitSubscriber>();
        private object _outputValue;

        protected CalcUnitBase(IEnumerable<object> inputKeys)
        {
            foreach (var key in inputKeys)
            {
                var input = new Input(key);
                input.Changed += Input_Changed;
                _inputs[key] = input;

                if (key is ICalcUnit c)
                    c.Subscribe(input);
            }
        }

        public IReadOnlyDictionary<object, IInput> Inputs => new ReadOnlyDictionary<object, IInput>(_inputs);

        public ISubscription Subscribe(ICalcUnitSubscriber subscriptor)
        {
            _subscribers.Add(subscriptor);
            return new Subscription(_subscribers, subscriptor);
        }

        private void Calculate(object inputKey)
        {
            var args = _inputs.Keys.ToDictionary(k => k, k => _inputs[k].Value);

            var result = Execute(args, inputKey);

            if (!Equals(result, _outputValue))
            {
                _outputValue = result;

                foreach (var s in _subscribers)
                    s.OnNext(_outputValue);
            }
        }

        protected abstract object Execute(IDictionary<object, object> args, object changedInput);

        private void Input_Changed(object sender, System.EventArgs e)
        {
            Calculate(((IInput)sender).Key);
        }
    }
}