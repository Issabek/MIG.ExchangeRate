using System;

namespace MIG.ExchangeRateData
{ 
    public class Rate
    {
        public string USD_in { get; set; }
        public string USD_out { get; set; }
        public string EUR_in { get; set; }
        public string EUR_out { get; set; }
        public string RUB_in { get; set; }
        public string RUB_out { get; set; }
        public string GBP_in { get; set; }
        public string GBP_out { get; set; }
        public string CAD_in { get; set; }
        public string CAD_out { get; set; }
        public string PLN_in { get; set; }
        public string PLN_out { get; set; }
        public string UAH_in { get; set; }
        public string UAH_out { get; set; }
        public string SEK_in { get; set; }
        public string SEK_out { get; set; }
        public string CHF_in { get; set; }
        public string CHF_out { get; set; }
        public string USD_EUR_in { get; set; }
        public string USD_EUR_out { get; set; }
        public string USD_RUB_in { get; set; }
        public string USD_RUB_out { get; set; }
        public string RUB_EUR_in { get; set; }
        public string RUB_EUR_out { get; set; }
        public string JPY_in { get; set; }
        public string JPY_out { get; set; }
        public string CNY_in { get; set; }
        public string CNY_out { get; set; }
        public string CZK_in { get; set; }
        public string CZK_out { get; set; }
        public string NOK_in { get; set; }
        public string NOK_out { get; set; }
        public string filial_id { get; set; }
        public string sap_id { get; set; }
        public string info_worktime { get; set; }
        public string street_type { get; set; }
        public string street { get; set; }
        public string filials_text { get; set; }
        public string home_number { get; set; }
        public string name { get; set; }
        public string name_type { get; set; }
        public int id { get; set; }
        public int EntityID { get; set; }
        public DateTime CreteTime { get; } = DateTime.Now;


        public override bool Equals(object item)
        {
            var obj = item as Rate;
            if (obj.USD_in != this.USD_in ||
                obj.RUB_in != this.RUB_in ||
                obj.EUR_in != this.EUR_in ||
                obj.UAH_in != this.UAH_in ||
                obj.USD_out != this.USD_out ||
                obj.RUB_out != this.RUB_out ||
                obj.EUR_out != this.EUR_out ||
                obj.UAH_out != this.UAH_out)
            {
                return false;
            }
            return true;
        }
    }

}
