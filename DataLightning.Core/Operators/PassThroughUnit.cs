using System.Linq;

namespace DataLightning.Core.Operators
{
    public class PassThroughUnit<T> : GenericCalcUnit<T, T>
    {
        public PassThroughUnit() : base(new[] { "Input" }, args => args.Values.Single())
        {
        }

        public IInput<T> Input => Inputs["Input"];
    }
}