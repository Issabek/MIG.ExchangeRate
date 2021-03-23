using System;
using MIG.ExchangeRateData;

using System.Collections.Generic;
namespace MIG.ExchangeUI
{
    class Program
    {
        static void Main(string[] args)
        {
            RateService newDisplay = new RateService(@"myNewDb3.db");
            newDisplay.DisplayCurrent();

        }
    }
}
