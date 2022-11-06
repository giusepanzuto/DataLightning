using System.Collections.Generic;
using System.Linq;

namespace DataLightning.Core
{
    public abstract class CalcUnitBase<TInput, TOutput> : SubscribableBase<TOutput>
    {
        private readonly Dictionary<object, IInput<TInput>> _inputs = new();
        private TOutput _outputValue;

        protected CalcUnitBase(IEnumerable<ISubscribable<TInput>> inputKeys)
        {
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

                PushToSubscribers(_outputValue);
            }
        }

        protected abstract TOutput Execute(IDictionary<object, TInput> args, object changedInput);

        private void Input_Changed(object sender, System.EventArgs e)
        {
            Calculate(((IInput<TInput>)sender).Key);
        }
    }
}