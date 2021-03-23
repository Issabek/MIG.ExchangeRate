using System;
using System.Collections.Generic;
using MIG.ExchangeRateData;
using Tengri.DAL;
using System.Linq;
using System.Threading;

namespace MIG.RateDataService
{
    public class RateService
    {

        public List<Rate> RateHistory { get; set; }
        private LiteDbEntity db = null;
        public RateEntity LastEntity { get; set; } = null;
        public List<RateEntity> AllEntities { get; set; }
        public RateService(string connectionString)
        {
            db = new LiteDbEntity(connectionString);
            AllEntities = db.getCollection<RateEntity>();
            if (AllEntities.Count != 0)
            {
                LastEntity = AllEntities.Last();
            }
        }


        //public Rate showSingleUser(int userID)
        //{
        //    Rate user = db.getCollection<Rate>().Where(x => x.Id == userID).FirstOrDefault();
        //    return user;
            
        //}


        public void showMinMax(RateEntity rateEntity)
        {
            if (LastEntity == null)
                LastEntity = rateEntity;

            Console.WriteLine("=============Верхний порог значений===========");
            CurrencyComparator(rateEntity.MaxRate.USD_in,LastEntity.MaxRate.USD_in, true);
            Console.Write("USD IN - USD OUT");
            CurrencyComparator(rateEntity.MaxRate.USD_out, LastEntity.MaxRate.USD_out, false);
            Console.WriteLine("\n");

            CurrencyComparator(rateEntity.MaxRate.EUR_in, LastEntity.MaxRate.EUR_in, true);
            Console.Write("EUR IN - EUR OUT");
            CurrencyComparator(rateEntity.MaxRate.EUR_out, LastEntity.MaxRate.EUR_out, false);
            Console.WriteLine("\n");

            CurrencyComparator(rateEntity.MaxRate.RUB_in, LastEntity.MaxRate.RUB_in, true);
            Console.Write("RUB IN - RUB OUT");
            CurrencyComparator(rateEntity.MaxRate.RUB_out, LastEntity.MaxRate.RUB_out, false);
            Console.WriteLine("\n");

            CurrencyComparator(rateEntity.MaxRate.UAH_in, LastEntity.MaxRate.UAH_in, true);
            Console.Write("UAH IN - UAH OUT");
            CurrencyComparator(rateEntity.MaxRate.UAH_out, LastEntity.MaxRate.UAH_out, false);
            Console.WriteLine("\n");
            Console.WriteLine("===============================================");

            Console.WriteLine("\n\n");

            Console.WriteLine("=============Нижний порог значений===========");
            CurrencyComparator(rateEntity.MinRate.USD_in, LastEntity.MinRate.USD_in, true);
            Console.Write("USD IN - USD OUT");
            CurrencyComparator(rateEntity.MinRate.USD_out, LastEntity.MinRate.USD_out, false);
            Console.WriteLine("\n");

            CurrencyComparator(rateEntity.MinRate.EUR_in, LastEntity.MinRate.EUR_in, true);
            Console.Write("EUR IN - EUR OUT");
            CurrencyComparator(rateEntity.MinRate.EUR_out, LastEntity.MinRate.EUR_out, false);
            Console.WriteLine("\n");

            CurrencyComparator(rateEntity.MinRate.RUB_in, LastEntity.MinRate.RUB_in, true);
            Console.Write("RUB IN - RUB OUT");
            CurrencyComparator(rateEntity.MinRate.RUB_out, LastEntity.MinRate.RUB_out, false);
            Console.WriteLine("\n");

            CurrencyComparator(rateEntity.MinRate.UAH_in, LastEntity.MinRate.UAH_in, true);
            Console.Write("UAH IN - UAH OUT");
            CurrencyComparator(rateEntity.MinRate.UAH_out, LastEntity.MinRate.UAH_out, false);
            Console.WriteLine("\n");
            Console.WriteLine("===============================================");

        }

        private void CurrencyComparator(string current, string previous, bool isIn)
        {
            double val1 = double.Parse(current), val2 = double.Parse(previous);
            
            if (isIn) {
                if (val1 > val2)
                {
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                    Console.Write(" [ {0} ] ",val1);
                }
                else if(val1< val2)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Write(" [ {0} ] ", val1);
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(" [ {0} ] ", val1);
                }
            }
            else
            {
                if (val1 < val2)
                {
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                    Console.Write(" [ {0} ] ", val1);
                }
                else if (val1 > val2)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Write(" [ {0} ] ", val1);
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Write(" [ {0} ] ", val1);
                }
            }
            Console.ResetColor();
        }

        public bool RateRegistration(List<Rate> rates, RateEntity rateEntity)
        {
            string tempStr = null;
            RateHistory = rates;

            if (!db.CreateRecord<RateEntity>(rateEntity, out tempStr))
            {
                throw new Exception(tempStr);
            }
            else
            {
                foreach (Rate rate in rates)
                {
                    rate.EntityID = rateEntity.Id;
                }
                db.CreateRecords(rates,out tempStr);
                Rate minRate = null;
                Rate maxRate = null;
                GetMinMax(out minRate, out maxRate,RateHistory);
                rateEntity.MinRate = minRate;
                rateEntity.MaxRate = maxRate;
                AllEntities = db.getCollection<RateEntity>();
                db.UpdateRecord<RateEntity>(rateEntity, out tempStr);
            }
            return true;
        }




        public void DisplayCurrent()
        {
            RateEntity rateEntity = new RateEntity();
            List<Rate> rates = DataFetch.GetRecentRate();
            Rate minRate = null;
            Rate maxRate = null;
            bool temp1 = false;
            bool temp2 = false;
            if (LastEntity != null)
            {
                Console.WriteLine("Курсы валют на {0}", DateTime.Now.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("fr-FR")));
                showMinMax(LastEntity);
            }
            Console.WriteLine("Enter time in minutes to wat until next reload:");
            int sleepTime = int.Parse(Console.ReadLine())*60000;
            while (true)
            {
                Thread.Sleep(sleepTime);
                GetMinMax(out minRate, out maxRate, rates);
                AllEntities.ToList();
                if (LastEntity != null)
                {
                    temp1 = LastEntity.MinRate.Equals(minRate);
                    temp2 = LastEntity.MaxRate.Equals(maxRate);
                }

                if (!temp1 || !temp2)
                {
                    Console.Clear();
                    if (RateRegistration(rates, rateEntity))
                    {
                        Console.Clear();
                        Console.WriteLine("Курсы валют на {0}", DateTime.Now.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("fr-FR")));
                        showMinMax(rateEntity);
                        LastEntity = rateEntity;
                    }
                    else
                    {
                        Console.WriteLine("Something went wrong");
                    }
                }
            }
        }


        public List<Rate> GetRatesByEntity(RateEntity rateEntity)
        {
            return db.getCollection<Rate>().Where(x => x.EntityID == rateEntity.Id).ToList();
        }

        /// <summary>
        /// получает минимальное значение конкретной валюты из списка
        /// </summary>
        /// <param name="Unit"></param>
        /// <param name="IndexOfRateInHistory"> Индекс конкретной записи в БД</param>
        /// <returns></returns>
        //private string getMinRate(string Unit, RateEntity rateEnt)
        //{
        //    List<Rate> ratesList = GetRatesByEntity(rateEnt);
        //    double minValue = 0;

        //    if (double.TryParse(ratesList[0].GetType().GetProperties()
        //                            .Single(pi => pi.Name == Unit)
        //                            .GetValue(ratesList[0], null).ToString(),
        //                            out minValue))
        //    {

        //        foreach (Rate rate in ratesList)
        //            if (double.Parse(rate.GetType().GetProperties().Single(pi => pi.Name == Unit).GetValue(rate, null).ToString()) < minValue) //Сравнивает конкретную валюту и ее значение
        //                minValue = double.Parse(rate.GetType().GetProperties().Single(pi => pi.Name == Unit).GetValue(rate, null).ToString());

        //    }
        //    return minValue.ToString();
        //}
        /// <summary>
        /// получает максимальной значение конкретной валюты из списка
        /// </summary>
        /// <param name="Unit"></param>
        /// <param name="IndexOfRateInHistory"> Индекс конкретной записи в БД</param>
        /// <returns></returns>
        /// 
        //private object getMaxRate(string Unit, RateEntity rateEnt)
        //{
        //    List<Rate> ratesList = GetRatesByEntity(rateEnt);
        //    double maxValue = 0;

        //    if (double.TryParse(ratesList[0].GetType().GetProperties()
        //                            .Single(pi => pi.Name == Unit)
        //                            .GetValue(ratesList[0], null).ToString(),
        //                            out maxValue))
        //    {

        //        foreach (Rate rate in ratesList)
        //            if (double.Parse(rate.GetType().GetProperties().Single(pi => pi.Name == Unit).GetValue(rate, null).ToString()) > maxValue) //Сравнивает конкретную валюту и ее значение
        //                maxValue = double.Parse(rate.GetType().GetProperties().Single(pi => pi.Name == Unit).GetValue(rate, null).ToString());

        //    }
        //    return maxValue.ToString();
        //}
        public void GetMinMax(out Rate MinRate, out Rate MaxRate,List<Rate> rates )
        {
            MinRate = rates[0];
            MaxRate = rates[1];
            foreach (Rate rate in rates)
            {
                #region MaxComparator
                //if (double.Parse(rate.CAD_in) > double.Parse(MaxRate.CAD_in))
                //    MaxRate.CAD_in = rate.CAD_in;
                //if (double.Parse(rate.CAD_out) > double.Parse(MaxRate.CAD_out))
                //    MaxRate.CAD_in = rate.CAD_out;
                //if (double.Parse(rate.CHF_in) > double.Parse(MaxRate.CHF_in))
                //    MaxRate.CAD_in = rate.CHF_in;
                //if (double.Parse(rate.CHF_out) > double.Parse(MaxRate.CHF_out))
                //    MaxRate.CAD_in = rate.CHF_out;
                //if (double.Parse(rate.CNY_in) > double.Parse(MaxRate.CNY_in))
                //    MaxRate.CAD_in = rate.CNY_in;
                //if (double.Parse(rate.CNY_out) > double.Parse(MaxRate.CNY_out))
                //    MaxRate.CAD_in = rate.CNY_out;
                //if (double.Parse(rate.CZK_in) > double.Parse(MaxRate.CZK_in))
                //    MaxRate.CAD_in = rate.CZK_in;
                //if (double.Parse(rate.CZK_out) > double.Parse(MaxRate.CZK_out))
                //    MaxRate.CAD_in = rate.CZK_out;
                if (double.Parse(rate.EUR_in) > double.Parse(MaxRate.EUR_in))
                    MaxRate.EUR_in = rate.EUR_in;
                if (double.Parse(rate.EUR_out) > double.Parse(MaxRate.EUR_out))
                    MaxRate.EUR_out = rate.EUR_out;
                //if (double.Parse(rate.GBP_in) > double.Parse(MaxRate.GBP_in))
                //    MaxRate.CAD_in = rate.GBP_in;
                //if (double.Parse(rate.GBP_out) > double.Parse(MaxRate.GBP_out))
                //    MaxRate.CAD_in = rate.GBP_out;
                //if (double.Parse(rate.JPY_in) > double.Parse(MaxRate.JPY_in))
                //    MaxRate.CAD_in = rate.JPY_in;
                //if (double.Parse(rate.JPY_in) > double.Parse(MaxRate.JPY_in))
                //    MaxRate.CAD_in = rate.JPY_in;
                //if (double.Parse(rate.JPY_out) > double.Parse(MaxRate.JPY_out))
                //    MaxRate.CAD_in = rate.JPY_out;
                //if (double.Parse(rate.NOK_in) > double.Parse(MaxRate.NOK_in))
                //    MaxRate.CAD_in = rate.NOK_in;
                //if (double.Parse(rate.NOK_out) > double.Parse(MaxRate.NOK_out))
                //    MaxRate.CAD_in = rate.NOK_out;
                //if (double.Parse(rate.PLN_in) > double.Parse(MaxRate.PLN_in))
                //    MaxRate.CAD_in = rate.PLN_in;
                //if (double.Parse(rate.PLN_out) > double.Parse(MaxRate.PLN_out))
                //    MaxRate.CAD_in = rate.PLN_out;
                //if (double.Parse(rate.RUB_EUR_in) > double.Parse(MaxRate.RUB_EUR_in))
                //    MaxRate.CAD_in = rate.RUB_EUR_in;
                //if (double.Parse(rate.RUB_EUR_out) > double.Parse(MaxRate.RUB_EUR_out))
                //    MaxRate.CAD_in = rate.RUB_EUR_out;
                if (double.Parse(rate.RUB_in) > double.Parse(MaxRate.RUB_in))
                    MaxRate.RUB_in = rate.RUB_in;
                if (double.Parse(rate.RUB_out) > double.Parse(MaxRate.RUB_out))
                    MaxRate.RUB_out = rate.RUB_out;
                //if (double.Parse(rate.SEK_in) > double.Parse(MaxRate.SEK_in))
                //    MaxRate.CAD_in = rate.SEK_in;
                //if (double.Parse(rate.SEK_out) > double.Parse(MaxRate.SEK_out))
                //    MaxRate.CAD_in = rate.SEK_out;
                if (double.Parse(rate.UAH_in) > double.Parse(MaxRate.UAH_in))
                    MaxRate.UAH_in = rate.UAH_in;
                if (double.Parse(rate.UAH_out) > double.Parse(MaxRate.UAH_out))
                    MaxRate.UAH_out = rate.UAH_out;
                //if (double.Parse(rate.USD_EUR_in) > double.Parse(MaxRate.USD_EUR_in))
                //    MaxRate.CAD_in = rate.USD_EUR_in;
                //if (double.Parse(rate.USD_EUR_out) > double.Parse(MaxRate.USD_EUR_out))
                //    MaxRate.CAD_in = rate.USD_EUR_out;
                if (double.Parse(rate.USD_in) > double.Parse(MaxRate.USD_in))
                    MaxRate.USD_in = rate.USD_in;
                if (double.Parse(rate.USD_out) > double.Parse(MaxRate.USD_out))
                    MaxRate.USD_out = rate.USD_out;
                //if (double.Parse(rate.USD_RUB_in) > double.Parse(MaxRate.USD_RUB_in))
                //    MaxRate.CAD_in = rate.USD_RUB_in;
                //if (double.Parse(rate.USD_RUB_out) > double.Parse(MaxRate.USD_RUB_out))
                //    MaxRate.CAD_in = rate.USD_RUB_out;
                #endregion
                #region MinComparator
                //if (double.Parse(rate.CAD_in) < double.Parse(MinRate.CAD_in))
                //    MinRate.CAD_in = rate.CAD_in;
                //if (double.Parse(rate.CAD_out) < double.Parse(MinRate.CAD_out))
                //    MinRate.CAD_in = rate.CAD_out;
                //if (double.Parse(rate.CHF_in) < double.Parse(MinRate.CHF_in))
                //    MinRate.CAD_in = rate.CHF_in;
                //if (double.Parse(rate.CHF_out) < double.Parse(MinRate.CHF_out))
                //    MinRate.CAD_in = rate.CHF_out;
                //if (double.Parse(rate.CNY_in) < double.Parse(MinRate.CNY_in))
                //    MinRate.CAD_in = rate.CNY_in;
                //if (double.Parse(rate.CNY_out) < double.Parse(MinRate.CNY_out))
                //    MinRate.CAD_in = rate.CNY_out;
                //if (double.Parse(rate.CZK_in) < double.Parse(MinRate.CZK_in))
                //    MinRate.CAD_in = rate.CZK_in;
                //if (double.Parse(rate.CZK_out) < double.Parse(MinRate.CZK_out))
                //    MinRate.CAD_in = rate.CZK_out;
                if (double.Parse(rate.EUR_in) < double.Parse(MinRate.EUR_in))
                    MinRate.EUR_in = rate.EUR_in;
                if (double.Parse(rate.EUR_out) < double.Parse(MinRate.EUR_out))
                    MinRate.EUR_out = rate.EUR_out;
                //if (double.Parse(rate.GBP_in) < double.Parse(MinRate.GBP_in))
                //    MinRate.CAD_in = rate.GBP_in;
                //if (double.Parse(rate.GBP_out) < double.Parse(MinRate.GBP_out))
                //    MinRate.CAD_in = rate.GBP_out;
                //if (double.Parse(rate.JPY_in) < double.Parse(MinRate.JPY_in))
                //    MinRate.CAD_in = rate.JPY_in;
                //if (double.Parse(rate.JPY_in) < double.Parse(MinRate.JPY_in))
                //    MinRate.CAD_in = rate.JPY_in;
                //if (double.Parse(rate.JPY_out) < double.Parse(MinRate.JPY_out))
                //    MinRate.CAD_in = rate.JPY_out;
                //if (double.Parse(rate.NOK_in) < double.Parse(MinRate.NOK_in))
                //    MinRate.CAD_in = rate.NOK_in;
                //if (double.Parse(rate.NOK_out) < double.Parse(MinRate.NOK_out))
                //    MinRate.CAD_in = rate.NOK_out;
                //if (double.Parse(rate.PLN_in) < double.Parse(MinRate.PLN_in))
                //    MinRate.CAD_in = rate.PLN_in;
                //if (double.Parse(rate.PLN_out) < double.Parse(MinRate.PLN_out))
                //    MinRate.CAD_in = rate.PLN_out;
                //if (double.Parse(rate.RUB_EUR_in) < double.Parse(MinRate.RUB_EUR_in))
                //    MinRate.CAD_in = rate.RUB_EUR_in;
                //if (double.Parse(rate.RUB_EUR_out) < double.Parse(MinRate.RUB_EUR_out))
                //    MinRate.CAD_in = rate.RUB_EUR_out;
                if (double.Parse(rate.RUB_in) < double.Parse(MinRate.RUB_in))
                    MinRate.RUB_in = rate.RUB_in;
                if (double.Parse(rate.RUB_out) < double.Parse(MinRate.RUB_out))
                    MinRate.RUB_out = rate.RUB_out;
                //if (double.Parse(rate.SEK_in) < double.Parse(MinRate.SEK_in))
                //    MinRate.CAD_in = rate.SEK_in;
                //if (double.Parse(rate.SEK_out) < double.Parse(MinRate.SEK_out))
                //MinRate.CAD_in = rate.SEK_out;
                if (double.Parse(rate.UAH_in) < double.Parse(MinRate.UAH_in))
                    MinRate.UAH_in = rate.UAH_in;
                if (double.Parse(rate.UAH_out) < double.Parse(MinRate.UAH_out))
                    MinRate.UAH_out = rate.UAH_out;
                //if (double.Parse(rate.USD_EUR_in) < double.Parse(MinRate.USD_EUR_in))
                //    MinRate.CAD_in = rate.USD_EUR_in;
                //if (double.Parse(rate.USD_EUR_out) < double.Parse(MinRate.USD_EUR_out))
                //    MinRate.CAD_in = rate.USD_EUR_out;
                if (double.Parse(rate.USD_in) < double.Parse(MinRate.USD_in))
                    MinRate.USD_in = rate.USD_in;
                if (double.Parse(rate.USD_out) < double.Parse(MinRate.USD_out))
                    MinRate.USD_out = rate.USD_out;
                //if (double.Parse(rate.USD_RUB_in) < double.Parse(MinRate.USD_RUB_in))
                //    MinRate.CAD_in = rate.USD_RUB_in;
                //if (double.Parse(rate.USD_RUB_out) < double.Parse(MinRate.USD_RUB_out))
                //    MinRate.CAD_in = rate.USD_RUB_out;
                #endregion

            }
        }

        //public Rate GetMinRateExchange()
        //{
        //    Rate MinRate = new Rate();
        //    List<string> Units = GetUnitsList(MinRate);
        //    foreach(string unit in Units)
        //    {
        //        var tempVal = getMinRate(unit, LastEntity);
        //        MinRate.GetType().GetProperty(unit).SetValue(MinRate, tempVal);
        //    }

        //    return MinRate;
        //}

        //public Rate GetMaxRateExchange()
        //{
        //    Rate MaxRate = new Rate();
        //    List<string> Units = GetUnitsList(MaxRate);
        //    foreach (string unit in Units)
        //    {
        //        MaxRate.GetType().GetProperty(unit).SetValue(MaxRate, getMaxRate(unit, LastEntity));
        //    }

        //    return MaxRate;
        //}

        private List<string> GetUnitsList(Rate anyRate)
        {
            List<string> Units = new List<string>();
            foreach (var rate in anyRate.GetType().GetProperties())
            {
                if(rate.Name.Contains("in")|| rate.Name.Contains("out"))
                {
                    Units.Add(rate.Name);
                }
            }
            return Units;
        }

        public RateEntity this[int i]
        {
            get { return AllEntities[i]; }
        }
    }
}
