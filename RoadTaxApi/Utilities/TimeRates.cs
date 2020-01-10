using System;
using System.Collections;
using System.Collections.Generic;

namespace RoadTaxApi.Utilities
{
    public struct TimeRates
    {
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public int Fee { get; set; }

        public TimeRates(TimeSpan start, TimeSpan end, int amount)
        {
            Start = start;
            End = end;
            Fee = amount;
        }
    }
}