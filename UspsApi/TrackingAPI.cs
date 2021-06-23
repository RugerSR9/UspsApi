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
using UspsApi.Models.TrackingAPI;
using static UspsApi.Settings;

namespace UspsApi
{
    internal static class TrackingAPI
    {
        public static async Task<List<TrackInfo>> TrackAsync(List<TrackID> input)
        {
            // limit is 10 tracking numbers per request
            string requestGuid = Guid.NewGuid().ToString();
            Log.Information("{area}: New request for {packageTotal} package(s). {requestGuid}", "Track()", input.Count, requestGuid);

            List<TrackInfo> output = new List<TrackInfo>();
            TrackFieldRequest request;
            int index = 0;

            while (index < input.Count)
            {
                request = new TrackFieldRequest
                {
                    USERID = UserId,
                    Revision = "1",
                    ClientIp = ClientIP,
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

                HttpClient httpClient = new HttpClient()
                {
                    Timeout = TimeSpan.FromSeconds(120)
                };
                HttpResponseMessage response = null;
                int retryCount = 0;
                DateTime responseTimer = DateTime.Now;

            retry:
                while (response == null || response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    if (retryCount > MaxRetries)
                    {
                        Log.Error("{area}: USPS Failed to Respond after " + MaxRetries + " attempts. {requestGuid}", "Track()", retryCount, requestGuid);
                        throw new Exception("408: After many attempts, the request to the USPS API did not recieve a response. Please try again later.");
                    }

                    if (retryCount > 0)
                        Log.Warning("{area}: USPS Failed to Respond after " + retryCount + " seconds. Attempt {retryCount}. {requestGuid}", "TrackAsync()", retryCount, requestGuid);

                    try
                    {
                        response = await httpClient.PostAsync(uspsUrl, formData).ConfigureAwait(false);
                        Thread.Sleep(RetryDelay);
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
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                Log.Information("{area}: USPS response received in {responseTime} ms. {requestGuid}", "TrackAsync()", responseTime.Milliseconds, requestGuid);

                try
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(TrackResponse));
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
                    TrackResponse responseJson = (TrackResponse)deserializer.Deserialize(ms);

                    // todo: save response data to correct input data
                    foreach (TrackInfo trackInfo in responseJson.TrackInfo)
                    {
                        if (trackInfo.Error != null)
                            Log.Warning("{area}: USPS Returned Error: {uspsErrorNumber} {uspsErrorDescription} {requestGuid}", "TrackAsync()", trackInfo.Error.Number, trackInfo.Error.Description, requestGuid);

                        output.Add(trackInfo);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("{area}: Exception: {ex} {requestGuid}", "TrackAsync()", ex.ToString(), requestGuid);
                    throw;
                }
            }

            if (output.Count != input.Count)
            {
                // something went wrong because counts should always match
                Log.Error("{area}: Counts did not match between input and output", "TrackAsync()");
                throw new Exception("Counts did not match between input and output");
            }

            return output;
        }
    }
}