using System;
using System.Collections.Generic;

namespace MIG.ExchangeRateData
{
    public class RateEntity
    {
        public Rate MinRate { get; set; }
        public Rate MaxRate { get; set; }
        public Dictionary<DateTime,List<Rate>> RateRecord;
        public int Id { get; set; }

        public RateEntity()
        {
        }
        public RateEntity(List<Rate> Rates,Rate minRate, Rate maxRate)
        {
            MaxRate = MaxRate;
            MinRate = minRate;
            RateRecord.Add(DateTime.Now,Rates);
        }
        public RateEntity(List<Rate> Rates) : this(Rates, null, null)
        {
            RateRecord.Add(DateTime.Now, Rates);

        }
        public RateEntity( Dictionary<DateTime,List<Rate>> Rates)
        {
            this.RateRecord = Rates;
        }
    }
}
