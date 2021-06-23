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
using UspsApi.Models;
using UspsApi.Models.RateAPI;
using UspsApi.Models.RateAPI.Response;
using static UspsApi.Settings;

namespace UspsApi
{
    internal static class RateAPI
    {
        internal static async Task<List<Models.RateAPI.Response.Package>> FetchRatesAsync(List<Models.RateAPI.Request.Package> input)
        {
            // limit is 25 packages per request
            string requestGuid = Guid.NewGuid().ToString();
            Log.Information("{area}: New request for {packageTotal} packages. {requestGuid}", "FetchRates()", input.Count, requestGuid);

            List<Models.RateAPI.Response.Package> output = new();
            RateV4Request request;
            int index = 0;

            while (index < input.Count)
            {
                request = new RateV4Request
                {
                    USERID = UserId,
                    Revision = "2",
                    Package = input.Skip(index).Take(25).ToList()
                };

                Log.Information("{area}: Fetching rates for {packageCount} package(s). {requestGuid}", "FetchRates()", request.Package.Count, requestGuid);

                XmlSerializer xsSubmit = new(typeof(RateV4Request));
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
                    new KeyValuePair<string,string>("API", "RateV4"),
                    new KeyValuePair<string, string>("XML", xml)
                });

                HttpClient httpClient = new()
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
                        Log.Error("{area}: USPS Failed to Respond after " + MaxRetries + " attempts. {requestGuid}", "FetchRates()", retryCount, requestGuid);
                        throw new Exception("408: After many attempts, the request to the USPS API did not recieve a response. Please try again later.");
                    }

                    if (retryCount > 0)
                        Log.Warning("{area}: USPS Failed to Respond after " + retryCount + " seconds. Attempt {retryCount}. {requestGuid}", "FetchRates()", retryCount, requestGuid);

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
                Log.Information("{area}: USPS response received in {responseTime} ms. {requestGuid}", "FetchRates()", responseTime.Milliseconds, requestGuid);

                try
                {
                    XmlSerializer deserializer = new(typeof(RateV4Response));
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
                    RateV4Response responseJson = (RateV4Response)deserializer.Deserialize(ms);
                    index += 25;

                    foreach (Models.RateAPI.Response.Package pkg in responseJson.Package)
                    {
                        if (pkg.Error != null)
                            Log.Warning("{area}: USPS Returned Error: {uspsErrorNumber} {uspsErrorDescription} {requestGuid}", "FetchRates()", pkg.Error.Number, pkg.Error.Description, requestGuid);

                        output.Add(pkg);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("{area}: Exception: {ex} {requestGuid}", "FetchRates()", ex.ToString(), requestGuid);
                    throw;
                }
            }

            if (output.Count != input.Count)
            {
                // something went wrong because counts should always match
                Log.Error("{area}: Counts did not match between input and output. {requestGuid}", "FetchRates()", requestGuid);
                throw new Exception("Counts did not match between input and output");
            }

            return output;
        }
    }
}