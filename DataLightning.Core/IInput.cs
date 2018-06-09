﻿namespace DataLightning.Core
{
    public interface IInput : ICalcUnitSubscriber
    {
        object Value { get; }
        object Key { get; }
    }
}