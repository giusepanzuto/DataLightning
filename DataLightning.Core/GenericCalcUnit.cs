using System;
using System.Collections.Generic;

namespace DataLightning.Core
{
    public class GenericCalcUnit : CalcUnitBase
    {
        private readonly Func<IDictionary<object, object>, object> _function;

        public GenericCalcUnit(IEnumerable<object> inputKeys, Func<IDictionary<object, object>, object> function) : base(inputKeys)
        {
            _function = function;
        }

        protected override object Execute(IDictionary<object, object> args, object changedInput) =>
            _function(args);
    }
}