using System;
using System.Collections.Generic;

namespace DataLightning.Core
{
    public class GenericCalcUnit<TInput, TOutput> : CalcUnitBase<TInput, TOutput>
    {
        private readonly Func<IDictionary<object, TInput>, TOutput> _function;

        public GenericCalcUnit(IEnumerable<object> inputKeys, Func<IDictionary<object, TInput>, TOutput> function) : base(inputKeys)
        {
            _function = function;
        }

        protected override TOutput Execute(IDictionary<object, TInput> args, object changedInput) =>
            _function(args);
    }
}