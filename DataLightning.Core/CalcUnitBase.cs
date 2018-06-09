using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DataLightning.Core
{
    public abstract class CalcUnitBase<TInput, TOutput> : ICalcUnit<TInput, TOutput>
    {
        private readonly Dictionary<object, IInput<TInput>> _inputs = new Dictionary<object, IInput<TInput>>();
        private readonly IList<ICalcUnitSubscriber<TOutput>> _subscribers = new List<ICalcUnitSubscriber<TOutput>>();
        private TOutput _outputValue;

        protected CalcUnitBase(IEnumerable<object> inputKeys)
        {
            foreach (var key in inputKeys)
            {
                var input = new Input<TInput>(key);
                input.Changed += Input_Changed;
                _inputs[key] = input;

                if (key is ISubscribable<TInput> c)
                    c.Subscribe(input);
            }
        }

        public IReadOnlyDictionary<object, IInput<TInput>> Inputs => new ReadOnlyDictionary<object, IInput<TInput>>(_inputs);

        public ISubscription Subscribe(ICalcUnitSubscriber<TOutput> subscriptor)
        {
            _subscribers.Add(subscriptor);
            return new Subscription<TOutput>(_subscribers, subscriptor);
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

        protected abstract TOutput Execute(IDictionary<object, TInput> args, object changedInput);

        private void Input_Changed(object sender, System.EventArgs e)
        {
            Calculate(((IInput<TInput>)sender).Key);
        }
    }
}