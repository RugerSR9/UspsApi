using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using UspsApi.Models;
using UspsApi.Models.TrackingAPI;

namespace UspsApi
{
    public class ProofOfDeliveryAPI
    {
        internal static async Task<List<PTSRreResult>> RequestPODViaEmailAsync(List<PTSRreRequest> input, string UspsApiUsername)
        {
            string requestGuid = Guid.NewGuid().ToString();
            Log.Information("{area}: New request for {podTotal} PODs. {requestGuid}", "RequestPODViaEmailAsync()", input.Count, requestGuid);

            List<PTSRreResult> output = new List<PTSRreResult>();
            PTSRreRequest request;
            int index = 0;

            // 1 per request
            while (index < input.Count)
            {
                request = new PTSRreRequest(input[index].TrackId)
                {
                    // todo: finish this after adding tracking first
                    USERID = UspsApiUsername,
                    MpDate = input[index].MpDate,
                    MpSuffix = input[index].MpSuffix,
                    TableCode = input[index].TableCode,
                    Email1 = input[index].Email1,
                    FirstName = input[index].FirstName,
                    LastName = input[index].LastName
                };

                if (!string.IsNullOrEmpty(input[index].Email2))
                {
                    request.Email2 = input[index].Email2;

                    // to have an email3, you must also have an email2
                    if (!string.IsNullOrEmpty(input[index].Email3))
                        request.Email2 = input[index].Email3;
                }

                Log.Information("{area}: Requesting Proof of Delivery for {trackId}. {requestGuid}", "RequestPODViaEmailAsync()", request.TrackId, requestGuid);

                XmlSerializer xsSubmit = new XmlSerializer(typeof(PTSRreRequest));
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
                    new KeyValuePair<string,string>("API", "PTSRre"),
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
                        Log.Error("{area}: USPS Failed to Respond after 50 attempts. {requestGuid}", "RequestPODViaEmailAsync()", retryCount, requestGuid);
                        throw new UspsApiException("408: After many attempts, the request to the USPS API did not recieve a response. Please try again later.");
                    }

                    if (retryCount > 0)
                        Log.Warning("{area}: USPS Failed to Respond after " + retryCount + " seconds. Attempt {retryCount}. {requestGuid}", "RequestPODViaEmailAsync()", retryCount, requestGuid);

                    try
                    {
                        response = await httpClient.PostAsync(uspsUrl, formData).ConfigureAwait(false);
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
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                Log.Information("{area}: USPS response received in {responseTime} ms. {requestGuid}", "RequestPODViaEmailAsync()", responseTime.Milliseconds, requestGuid);

                try
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(PTSRreResult));
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
                    PTSRreResult responseJson = (PTSRreResult)deserializer.Deserialize(ms);

                    index++;

                    output.Add(responseJson);
                }
                catch (Exception ex)
                {
                    Log.Error("{area}: Exception: {ex} {requestGuid}", "RequestPODViaEmailAsync()", ex.ToString(), requestGuid);
                    throw new UspsApiException(ex);
                }
            }

            if (output.Count != input.Count)
            {
                // something went wrong because counts should always match
                Console.WriteLine("Counts did not match between input and output");
                Log.Error("{area}: Counts did not match between input and output. {requestGuid}", "RequestPODViaEmailAsync()", requestGuid);
                throw new UspsApiException("Counts did not match between input and output");
            }

            return output;
        }
    }
}
