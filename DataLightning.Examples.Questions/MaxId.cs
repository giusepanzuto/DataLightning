using DataLightning.Core;
using System;
using System.Collections.Generic;

namespace DataLightning.Examples.Questions
{
    public class MaxId<T> : CalcUnitBase<T, int>
    {
        private readonly Func<T, int> _keyGetter;
        private int _max = 0;

        public MaxId(object input, Func<T, int> keyGetter) : base(new[] { input })
        {
            _keyGetter = keyGetter;
        }

        protected override int Execute(IDictionary<object, T> args, object changedInput)
        {
            var currentId = _keyGetter(args[changedInput]);
            return _max = Math.Max(_max, currentId);
        }
    }
}