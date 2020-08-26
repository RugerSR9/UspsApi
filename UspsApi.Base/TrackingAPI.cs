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
    public class TrackingAPI
    {
        public async Task<List<TrackInfo>> Track(List<TrackID> input)
        {
            // limit is 5 addresses per request
            List<TrackInfo> output = new List<TrackInfo>();
            string userId = ***REMOVED***;
            TrackFieldRequest request;
            int index = 0;

            while (index <= input.Count)
            {
                request = new TrackFieldRequest
                {
                    USERID = userId,
                    Revision = "1",
                    ClientIp = "12.174.118.186",
                    TrackID = input.Skip(index).Take(5).ToList(),
                    SourceId = "MYUSPS"
                };

                index += 5;

                XmlSerializer xsSubmit = new XmlSerializer(typeof(TrackFieldRequest));
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
                    new KeyValuePair<string,string>("API", "TrackV2"),
                    new KeyValuePair<string, string>("XML", xml)
                });

                HttpClient httpClient = new HttpClient();
                var response = await httpClient.PostAsync(uspsUrl, formData);
                var content = await response.Content.ReadAsStringAsync();

                try
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(TrackResponse));
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
                    TrackResponse responseJson = (TrackResponse)deserializer.Deserialize(ms);

                    // todo: save response data to correct input data
                    foreach (TrackInfo trackInfo in responseJson.TrackInfo)
                    {
                        output.Add(trackInfo);
                    }
                }
                catch (Exception ex)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Error));
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
                    Error error = (Error)serializer.Deserialize(ms);
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(error);
                    return null; // maybe do something except return null
                    //return NotFound(ex.Message);
                }
            }

            if (output.Count != input.Count)
            {
                // something went wrong because counts should always match
                Console.WriteLine("Counts did not match between input and output");
                return null; // maybe do something except return null
            }

            return output;
        }

        public TrackInfo Track(TrackID trackingNumber)
        {
            List<TrackID> list = new List<TrackID> { trackingNumber };
            return Track(list).Result.First();
        }
    }
}