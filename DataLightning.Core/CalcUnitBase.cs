using System.Collections.Generic;
using System.Linq;

namespace DataLightning.Core
{
    public abstract class CalcUnitBase<TInput, TOutput> : ISubscribable<TOutput>
    {
        private readonly Dictionary<object, IInput<TInput>> _inputs = new();
        private TOutput _outputValue;
        private readonly SubscriptionManager<TOutput> _subscriptionManager;

        protected CalcUnitBase(IEnumerable<ISubscribable<TInput>> inputKeys)
        {
            _subscriptionManager = new SubscriptionManager<TOutput>();

            foreach (var key in inputKeys)
            {
                var input = new Input<TInput>(key);
                input.Changed += Input_Changed;
                _inputs[key] = input;
                key.Subscribe(input);
            }
        }

        private void Calculate(object inputKey)
        {
            var args = _inputs.Keys.ToDictionary(k => k, k => _inputs[k].Value);

            var result = Execute(args, inputKey);

            if (!Equals(result, _outputValue))
            {
                _outputValue = result;

                _subscriptionManager.NotifySubscribers(_outputValue);
            }
        }

        protected abstract TOutput Execute(IDictionary<object, TInput> args, object changedInput);

        private void Input_Changed(object sender, System.EventArgs e)
        {
            Calculate(((IInput<TInput>)sender).Key);
        }

        public ISubscription Subscribe(ISubscriber<TOutput> subscriber) => 
            _subscriptionManager.Subscribe(subscriber);
    }
}