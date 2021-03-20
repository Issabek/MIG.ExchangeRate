using System;
using System.Collections.Generic;
using MIG.ExchangeRateData;
using System.Linq;
namespace MIG.ExchangeUI
{
    public class Display
    {

        public List<RateEntity> RateHistory { get; set; }
        


        public static void DisplayWindow()
        {
            Console.WriteLine("Курсы валют на {0}", DateTime.Now.ToString("dddd, dd MMMM yyyy")) ;
            Console.WriteLine();
        }

        public Display(List<RateEntity> RateHistory)
        {
            this.RateHistory = RateHistory;
        }
        /// <summary>
        /// получает минимальное значение конкретной валюты из списка
        /// </summary>
        /// <param name="Unit"></param>
        /// <param name="IndexOfRateInHistory"> Индекс конкретной записи в БД</param>
        /// <returns></returns>
        private object getMinRate(string Unit,int IndexOfRateInHistory)
        {
            List<Rate> ratesList = RateHistory[IndexOfRateInHistory].RateRecord.Values.ToList()[0];
            var minValue = ratesList[0].GetType().GetProperties()
                                    .Single(pi => pi.Name == Unit)
                                    .GetValue(ratesList[0], null);


            foreach (Rate rate in ratesList)
                if((double)rate.GetType().GetProperties().Single(pi => pi.Name == Unit).GetValue(rate, null) < (double)minValue) //Сравнивает конкретную валюту и ее значение
                    minValue = rate.GetType().GetProperties().Single(pi => pi.Name == Unit).GetValue(rate, null);


            return minValue;
        }
        /// <summary>
        /// получает максимальной значение конкретной валюты из списка
        /// </summary>
        /// <param name="Unit"></param>
        /// <param name="IndexOfRateInHistory"> Индекс конкретной записи в БД</param>
        /// <returns></returns>
        /// 
        private string getMaxRate(string Unit, int IndexOfRateInHistory)
        {
            List<Rate> ratesList = RateHistory[IndexOfRateInHistory].RateRecord.Values.ToList()[0];
            var minValue = ratesList[0].GetType().GetProperties()
                                    .Single(pi => pi.Name == Unit)
                                    .GetValue(ratesList[0], null);


            foreach (Rate rate in ratesList)
                if ((double)rate.GetType().GetProperties().Single(pi => pi.Name == Unit).GetValue(rate, null) > (double)minValue) //Сравнивает конкретную валюту и ее значение
                    minValue = rate.GetType().GetProperties().Single(pi => pi.Name == Unit).GetValue(rate, null);


            return string.Format("{0}", minValue);
        }

        public Rate GetMinRateExchange()
        {
            Rate MinRate = new Rate();
            List<string> Units = GetUnitsList(MinRate);
            foreach(string unit in Units)
            {
                MinRate.GetType().GetProperty(unit).SetValue(MinRate, getMinRate(unit, RateHistory.Count() - 1));
            }

            return MinRate;
        }

        public Rate GetMaxRateExchange()
        {
            Rate MaxRate = new Rate();
            List<string> Units = GetUnitsList(MaxRate);
            foreach (string unit in Units)
            {
                MaxRate.GetType().GetProperty(unit).SetValue(MaxRate, getMaxRate(unit, RateHistory.Count() - 1));
            }

            return MaxRate;
        }

        private List<string> GetUnitsList(Rate anyRate)
        {
            List<string> Units = new List<string>();
            foreach(var rate in RateHistory[0].GetType().GetProperties())
            {
                if(rate.Name.Contains("In")|| rate.Name.Contains("Out"))
                {
                    Units.Add(rate.Name);
                }
            }
            return Units;
        }
    }
}
