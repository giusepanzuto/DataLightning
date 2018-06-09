using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DataLightning.Core
{
    public class CalcUnit : ICalcUnit
    {
        private readonly Func<IDictionary<object, object>, object> _function;
        private readonly Dictionary<object, IInput> _inputs = new Dictionary<object, IInput>();
        private readonly IList<ICalcUnitSubscriber> _subscribers = new List<ICalcUnitSubscriber>();
        private object _outputValue;

        public CalcUnit(IEnumerable<object> inputKeys, Func<IDictionary<object, object>, object> function)
        {
            foreach (var key in inputKeys)
            {
                var input = new Input();
                input.Changed += Input_Changed;
                _inputs[key] = input;
                if (key is ICalcUnit c)
                    c.Subscribe(input);
            }

            _function = function;
        }

        public IReadOnlyDictionary<object, IInput> Inputs => new ReadOnlyDictionary<object, IInput>(_inputs);

        public ISubscription Subscribe(ICalcUnitSubscriber subscriptor)
        {
            _subscribers.Add(subscriptor);
            return new Subscription(_subscribers, subscriptor);
        }

        private void Calculate()
        {
            var args = _inputs.Keys.ToDictionary(k => k, k => _inputs[k].Value);

            var result = _function(args);

            if (!Equals(result, _outputValue))
            {
                _outputValue = result;

                foreach (var s in _subscribers)
                    s.OnNext(_outputValue);
            }
        }

        private void Input_Changed(object sender, System.EventArgs e)
        {
            Calculate();
        }
    }
}