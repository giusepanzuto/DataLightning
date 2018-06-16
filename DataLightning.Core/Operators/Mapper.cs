using System;
using System.Collections.Generic;

namespace DataLightning.Core.Operators
{
    public class Mapper<Tin, Tout> : CalcUnitBase<Tin, Tout>
    {
        private readonly Func<Tin, Tout> _mapper;

        public Mapper(ISubscribable<Tin> input, Func<Tin, Tout> mapper) : base(new[] { input })
        {
            _mapper = mapper;
        }

        protected override Tout Execute(IDictionary<object, Tin> args, object changedInput) =>
            _mapper(args[changedInput]);
    }
}