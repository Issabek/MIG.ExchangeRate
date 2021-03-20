using System;
using RestSharp;
using log4net;
using log4net.Config;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MIG.ExchangeRateData
{
    public class DataFetch
    {
        private static ILog log = LogManager.GetLogger("LOGGER");

        public DataFetch() {
        }

            public static List<Rate> GetRecentRate()
            {
                XmlConfigurator.Configure();
                try
                {
                    var request = new RestRequest("/api/kursExchange", Method.GET);
                    IRestResponse response = GetResponse(GetClient(), request);
                    var error = response.ErrorException;
                    if (response.Content.Length <= 100)
                        throw new Exception("Service is on maintaince");
                    else 
                       return JsonConvert.DeserializeObject<List<Rate>>(response.Content);
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message + " Ошибка проиозшла в главной области");
                
                    Console.WriteLine(ex.Message);
                    return null;

                }
            }

            private static RestClient GetClient()
            {
                RestClient client = new RestClient("https://belarusbank.by");
                return client;
            }

            private static IRestResponse GetResponse(RestClient client, RestRequest request)
            {

                try
                {
                    IRestResponse response = client.Get(request);
                    return response;
                }

                catch (System.NullReferenceException nex)
                {
                    log.Error(nex.Message);
                }

                catch (Exception ex)
                {
                    log.Fatal(ex.Message);
                }

                return null;
            }
        
    }
}
    
    
        
    


    

