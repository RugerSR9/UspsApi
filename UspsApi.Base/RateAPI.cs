using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using UspsApi.Models.RateAPI;

namespace UspsApiBase
{
    public class RateAPI
    {
        internal async Task<List<UspsApi.Models.RateAPI.Response.Package>> FetchRates(List<UspsApi.Models.RateAPI.Request.Package> input)
        {
            // limit is 25 packages per request
            List<UspsApi.Models.RateAPI.Response.Package> output = new List<UspsApi.Models.RateAPI.Response.Package>();
            string userId = ***REMOVED***;
            RateV4Request request;
            int index = 0;

            while (index <= input.Count)
            {
                request = new RateV4Request
                {
                    USERID = userId,
                    Revision = "2",
                    Package = input.Skip(index).Take(25).ToList()
                };
                
                index += 25;

                XmlSerializer xsSubmit = new XmlSerializer(typeof(RateV4Request));
                var xml = "";

                using (var sww = new StringWriter())
                {
                    using (XmlWriter writer = XmlWriter.Create(sww))
                    {
                        xsSubmit.Serialize(writer, request);
                        xml = sww.ToString();
                    }
                }

                string uspsUrl = "https://secure.shippingapis.com/ShippingAPI.dll";
                var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string,string>("API", "RateV4"),
                    new KeyValuePair<string, string>("XML", xml)
                });

                HttpClient httpClient = new HttpClient();
                var response = await httpClient.PostAsync(uspsUrl, formData);
                var content = await response.Content.ReadAsStringAsync();

                try
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(RateV4Response));
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
                    RateV4Response responseJson = (RateV4Response)deserializer.Deserialize(ms);

                    // todo: save response data to correct input data
                    foreach (UspsApi.Models.RateAPI.Response.Package pkg in responseJson.Package)
                    {
                        output.Add(pkg);
                    }
                }
                catch (Exception ex)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Error));
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
                    Error error = (Error)serializer.Deserialize(ms);
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(error);
                    throw new Exception(ex.ToString());
                }
            }

            if (output.Count != input.Count)
            {
                // something went wrong because counts should always match
                Console.WriteLine("Counts did not match between input and output");
                throw new Exception("Counts did not match between input and output");
            }

            return output;
        }

        public UspsApi.Models.RateAPI.Response.Package GetRates(UspsApi.Models.RateAPI.Request.Package pkg)
        {
            List<UspsApi.Models.RateAPI.Request.Package> list = new List<UspsApi.Models.RateAPI.Request.Package> { pkg };
            return FetchRates(list).Result.First();
        }

        public List<UspsApi.Models.RateAPI.Response.Package> GetRates(List<UspsApi.Models.RateAPI.Request.Package> pkgs)
        {
            return FetchRates(pkgs).Result;
        }
    }
}