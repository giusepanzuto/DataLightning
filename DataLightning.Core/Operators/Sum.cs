using System.Collections.Generic;
using System.Linq;

namespace DataLightning.Core.Operators
{
    public class Sum : CalcUnitBase
    {
        public Sum(IEnumerable<object> inputKeys) : base(inputKeys)
        {
        }

        protected override object Execute(IDictionary<object, object> args, object changedInput) =>
            args.Values.Cast<int?>().Sum();
    }
}