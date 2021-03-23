using System;
using MIG.RateDataService;

using System.Collections.Generic;
namespace MIG.Display
{
    class Program
    {
        static void Main(string[] args)
        {
            RateService newDisplay = new RateService(@"myNewDb6.db");
            newDisplay.DisplayCurrent();

        }
    }
}
