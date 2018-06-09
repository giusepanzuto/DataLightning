using System.Collections.Generic;
using System.Linq;

namespace DataLightning.Core.Operators
{
    public class Sum : CalcUnitBase<decimal, decimal>
    {
        public Sum(IEnumerable<object> inputKeys) : base(inputKeys)
        {
        }

        protected override decimal Execute(IDictionary<object, decimal> args, object changedInput) =>
            args.Values.Sum();
    }
}