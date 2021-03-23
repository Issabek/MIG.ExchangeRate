using System;
using System.Linq;
using System.Collections.Generic;

namespace MIG.ExchangeRateData
{
    public class RateEntity
    {
        public Rate MinRate { get; set; }
        public Rate MaxRate { get; set; }
        public DateTime CreateDate { get; } = DateTime.Now;
        public int Id { get; set; }

        public RateEntity() :this(null,null)
        {
            //RateRecord.Keys.ToList()[0] = DateTime.Now;
        }

        public RateEntity(Rate minRate, Rate maxRate)
        {
            MaxRate = maxRate;
            MinRate = minRate;
        }

    }
}
