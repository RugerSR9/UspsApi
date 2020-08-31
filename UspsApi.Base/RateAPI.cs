using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using UsaCommonModel;
using UspsApi.Models.AddressAPI;
using UspsApi.Models.RateAPI;
using UspsApi.Models.TrackingAPI;

namespace UspsApiBase
{
    // public class RateAPI
    // 
    //    public async Task<List<Package>> GetRates(List<Package> input)
    //    {
    //        // limit is 5 addresses per request
    //        List<Package> output = new List<Package>();
    //        string userId = ***REMOVED***;
    //        RateV4Request request;
    //        int index = 0;

    //        while (index <= input.Count)
    //        {
    //            request = new RateV4Request
    //            {
    //                USERID = userId,
    //                Revision = "1",
    //                Package = input.Skip(index).Take(5).ToList()
    //            };

    //            index += 5;

    //            XmlSerializer xsSubmit = new XmlSerializer(typeof(RateV4Request));
    //            var xml = "";

    //            using (var sww = new StringWriter())
    //            {
    //                using (XmlWriter writer = XmlWriter.Create(sww))
    //                {
    //                    xsSubmit.Serialize(writer, request);
    //                    xml = sww.ToString();
    //                }
    //            }

    //            string uspsUrl = "https://secure.shippingapis.com/ShippingAPI.dll";
    //            var formData = new FormUrlEncodedContent(new[]
    //            {
    //                new KeyValuePair<string,string>("API", "RateV4"),
    //                new KeyValuePair<string, string>("XML", xml)
    //            });

    //            HttpClient httpClient = new HttpClient();
    //            var response = await httpClient.PostAsync(uspsUrl, formData);
    //            var content = await response.Content.ReadAsStringAsync();

    //            try
    //            {
    //                XmlSerializer deserializer = new XmlSerializer(typeof(RateV4Response));
    //                var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
    //                RateV4Response responseJson = (RateV4Response)deserializer.Deserialize(ms);

    //                // todo: save response data to correct input data
    //                foreach (Package pkg in responseJson.Package)
    //                {
    //                    output.Add(pkg);
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                XmlSerializer serializer = new XmlSerializer(typeof(Error));
    //                var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
    //                Error error = (Error)serializer.Deserialize(ms);
    //                Console.WriteLine(ex.Message);
    //                Console.WriteLine(error);
    //                return null; // maybe do something except return null
    //                //return NotFound(ex.Message);
    //            }
    //        }

    //        if (output.Count != input.Count)
    //        {
    //            // something went wrong because counts should always match
    //            Console.WriteLine("Counts did not match between input and output");
    //            return null; // maybe do something except return null
    //        }

    //        return output;
    //    }

    //    public Package GetRates(Package pkg)
    //    {
    //        List<Package> list = new List<Package> { pkg };
    //        return GetRates(list).Result.First();
    //    }
    //}
}