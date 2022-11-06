using System.Collections.Generic;

namespace DataLightning.Core
{
    public abstract class CalcUnitBase<TInput, TOutput> : ISubscribable<TOutput>
    {
        private readonly SubscriptionManager<TOutput> _subscriptionManager;
        private readonly InputManager _inputManager;
        private readonly Dictionary<object, TInput> _inputs = new();

        private TOutput _outputValue;

        protected CalcUnitBase(IEnumerable<ISubscribable<TInput>> inputKeys)
        {
            _subscriptionManager = new SubscriptionManager<TOutput>();
            _inputManager = new InputManager();

            foreach (var key in inputKeys)
            {
                _inputManager.Add(string.Empty, key, (_, value) =>
                {
                    _inputs[key] = value;
                    Calculate2(key);
                });
            }
        }

        private void Calculate2(object inputKey)
        {
            var result = Execute(_inputs, inputKey);

            if (!Equals(result, _outputValue))
            {
                _outputValue = result;

                _subscriptionManager.NotifySubscribers(_outputValue);
            }
        }

        protected abstract TOutput Execute(IDictionary<object, TInput> args, object changedInput);

        public ISubscription Subscribe(ISubscriber<TOutput> subscriber) => 
            _subscriptionManager.Subscribe(subscriber);
    }
}