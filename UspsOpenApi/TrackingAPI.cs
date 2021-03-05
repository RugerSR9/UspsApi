using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using UspsOpenApi.Models;
using UspsOpenApi.Models.TrackingAPI;

namespace UspsOpenApi
{
    public class TrackingAPI
    {
        internal static async Task<List<TrackInfo>> Track(List<TrackID> input)
        {
            // limit is 10 tracking numbers per request
            string requestGuid = Guid.NewGuid().ToString();
            Log.Information("{area}: New request for {packageTotal} package(s). {requestGuid}", "Track()", input.Count, requestGuid);

            List<TrackInfo> output = new List<TrackInfo>();
            string userId = ***REMOVED***;
            TrackFieldRequest request;
            int index = 0;

            while (index < input.Count)
            {
                request = new TrackFieldRequest
                {
                    USERID = userId,
                    Revision = "1",
                    ClientIp = "12.174.118.186",
                    TrackID = input.Skip(index).Take(10).ToList(),
                    SourceId = "MYUSPS"
                };

                index += 10;

                XmlSerializer xsSubmit = new XmlSerializer(typeof(TrackFieldRequest));
                var xml = "";

                using (var sww = new StringWriter())
                {
                    using XmlWriter writer = XmlWriter.Create(sww);
                    xsSubmit.Serialize(writer, request);
                    xml = sww.ToString();
                }

                string uspsUrl = "https://secure.shippingapis.com/ShippingAPI.dll";
                var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string,string>("API", "TrackV2"),
                    new KeyValuePair<string, string>("XML", xml)
                });

                HttpClient httpClient = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(120)
                };
                HttpResponseMessage response = null;
                int retryCount = 0;
                DateTime responseTimer = DateTime.Now;

            retry:
                while (response == null || response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    if (retryCount > 50)
                    {
                        Log.Error("{area}: USPS Failed to Respond after 50 attempts. {requestGuid}", "Track()", retryCount, requestGuid);
                        throw new UspsOpenApiException("408: After many attempts, the request to the USPS API did not recieve a response. Please try again later.");
                    }

                    if (retryCount > 0)
                        Log.Warning("{area}: USPS Failed to Respond after " + retryCount + " seconds. Attempt {retryCount}. {requestGuid}", "Track()", retryCount, requestGuid);

                    try
                    {
                        response = await httpClient.PostAsync(uspsUrl, formData);
                        Thread.Sleep(2500 * retryCount);
                        httpClient.CancelPendingRequests();
                        retryCount++;
                    }
                    catch
                    {
                        httpClient.CancelPendingRequests();
                        retryCount++;
                        goto retry;
                    }
                }

                TimeSpan responseTime = DateTime.Now.TimeOfDay.Subtract(responseTimer.TimeOfDay);
                var content = await response.Content.ReadAsStringAsync();
                Log.Information("{area}: USPS response received in {responseTime} ms. {requestGuid}", "FetchRates()", responseTime.Milliseconds, requestGuid);

                try
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(TrackResponse));
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
                    TrackResponse responseJson = (TrackResponse)deserializer.Deserialize(ms);

                    // todo: save response data to correct input data
                    foreach (TrackInfo trackInfo in responseJson.TrackInfo)
                    {
                        if (trackInfo.Error != null)
                            Log.Warning("{area}: USPS Returned Error: {uspsErrorNumber} {uspsErrorDescription} {requestGuid}", "Track()", trackInfo.Error.Number, trackInfo.Error.Description, requestGuid);

                        output.Add(trackInfo);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("{area}: Exception: {ex} {requestGuid}", "Track()", ex.ToString(), requestGuid);
                    throw new UspsOpenApiException(ex);
                }
            }

            if (output.Count != input.Count)
            {
                // something went wrong because counts should always match
                Console.WriteLine("Counts did not match between input and output");
                throw new UspsOpenApiException("Counts did not match between input and output");
            }

            return output;
        }

        public static async Task<TrackInfo> Track(string trackingNumber)
        {
            List<TrackID> list = new List<TrackID> { new TrackID() { ID = trackingNumber } };
            List<TrackInfo> resp = await Track(list);
            return resp.First();
        }

        public static async Task<List<TrackInfo>> Track(List<string> trackingNumbers)
        {
            List<TrackID> list = new List<TrackID>();
            foreach (string id in trackingNumbers)
                list.Add(new TrackID() { ID = id });
            List<TrackInfo> resp = await Track(list);
            return resp;
        }
    }
}