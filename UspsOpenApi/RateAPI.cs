﻿using Serilog;
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
using UspsOpenApi.Models.RateAPI;
using UspsOpenApi.Models.RateAPI.Response;

namespace UspsOpenApi
{
    public class RateAPI
    {
        internal async Task<List<UspsOpenApi.Models.RateAPI.Response.Package>> FetchRates(List<UspsOpenApi.Models.RateAPI.Request.Package> input)
        {
            // limit is 25 packages per request
            string requestGuid = Guid.NewGuid().ToString();
            Log.Information("{area}: New request for {packageTotal} packages. {requestGuid}", "FetchRates()", input.Count, requestGuid);

            List<UspsOpenApi.Models.RateAPI.Response.Package> output = new List<UspsOpenApi.Models.RateAPI.Response.Package>();
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

                Log.Information("{area}: Fetching rates for {packageCount} package(s). {requestGuid}", "FetchRates()", request.Package.Count, requestGuid);

                XmlSerializer xsSubmit = new XmlSerializer(typeof(RateV4Request));
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

                HttpClient httpClient = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(50)
                };
                HttpResponseMessage response = null;
                int retryCount = 0;
                DateTime responseTimer = DateTime.Now;

                while (response == null || response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    if (retryCount > 0)
                        Log.Warning("{area}: USPS Failed to Respond after " + retryCount + " seconds. Attempt {retryCount}. {requestGuid}", "FetchRates()", retryCount, requestGuid);

                    response = await httpClient.PostAsync(uspsUrl, formData);
                    Thread.Sleep(1000 * retryCount);
                    httpClient.CancelPendingRequests();

                    retryCount++;

                    if (retryCount > 50)
                    {
                        Log.Error("{area}: USPS Failed to Respond after 50 attempts. {requestGuid}", "FetchRates()", retryCount, requestGuid);
                        throw new UspsOpenApiException("408: After many attempts, the request to the USPS API did not recieve a response. Please try again later.");
                    }
                }

                TimeSpan responseTime = DateTime.Now.TimeOfDay.Subtract(responseTimer.TimeOfDay);
                var content = await response.Content.ReadAsStringAsync();
                Log.Information("{area}: USPS response received in {responseTime} ms. {requestGuid}", "FetchRates()", responseTime.Milliseconds, requestGuid);

                try
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(RateV4Response));
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
                    RateV4Response responseJson = (RateV4Response)deserializer.Deserialize(ms);
                    index += 25;

                    foreach (UspsOpenApi.Models.RateAPI.Response.Package pkg in responseJson.Package)
                    {
                        if (pkg.Error != null)
                            Log.Warning("{area}: USPS Returned Error: {uspsErrorNumber} {uspsErrorDescription} {requestGuid}", "FetchRates()", pkg.Error.Number, pkg.Error.Description, requestGuid);

                        output.Add(pkg);
                    }

                }
                catch (Exception ex)
                {
                    Log.Error("{area}: Exception: {ex} {requestGuid}", "FetchRates()", ex.ToString(), requestGuid);
                    throw new UspsOpenApiException(ex);
                }
            }

            if (output.Count != input.Count)
            {
                // something went wrong because counts should always match
                Console.WriteLine("Counts did not match between input and output");
                Log.Error("{area}: Counts did not match between input and output. {requestGuid}", "FetchRates()", requestGuid);
                throw new UspsOpenApiException("Counts did not match between input and output");
            }

            return output;
        }

        /// <summary>
        /// Fetch rates for a single Package.
        /// Upon USPS API communication issues, this request will continue to retry. You will need to set a timeout handler or cancel the request from the calling app if this is an issue.
        /// </summary>
        /// <param name="pkg"></param>
        /// <returns></returns>
        public async Task<UspsOpenApi.Models.RateAPI.Response.Package> GetRates(UspsOpenApi.Models.RateAPI.Request.Package pkg)
        {
            List<UspsOpenApi.Models.RateAPI.Request.Package> list = new List<UspsOpenApi.Models.RateAPI.Request.Package> { pkg };

            Package result = FetchRates(list).Result.First();

            if (result.Error != null)
                return result;

            result.Postage.First().TotalPostage = Convert.ToDecimal(result.Postage.First().Rate);

            if (pkg.SpecialServices.SpecialService != null && pkg.SpecialServices.SpecialService.Count() > 0)
            {
                foreach (var service in pkg.SpecialServices.SpecialService)
                {
                    if (result.Postage.First().SpecialServices.SpecialService.Any(o => o.ServiceID == service.ToString()))
                        result.Postage.First().TotalPostage += Convert.ToDecimal(result.Postage.First().SpecialServices.SpecialService.First(o => o.ServiceID == service.ToString()).Price);
                }
            }

            return result;
        }

        /// <summary>
        /// Fetch rates for a List of Package
        /// Upon USPS API communication issues, this request will continue to retry. You will need to set a timeout handler or cancel the request from the calling app if this is an issue.
        /// </summary>
        /// <param name="pkgs"></param>
        /// <returns></returns>
        public async Task<List<UspsOpenApi.Models.RateAPI.Response.Package>> GetRates(List<UspsOpenApi.Models.RateAPI.Request.Package> pkgs)
        {
            List<UspsOpenApi.Models.RateAPI.Response.Package> result = FetchRates(pkgs).Result;

            foreach (var pkg in result)
            {
                if (pkg.Error != null)
                    continue;

                pkg.Postage.First().TotalPostage = Convert.ToDecimal(pkg.Postage.First().Rate);

                UspsOpenApi.Models.RateAPI.Request.Package inputPkg = pkgs.First(o => o.ID == pkg.ID);

                if (inputPkg.SpecialServices.SpecialService != null && inputPkg.SpecialServices.SpecialService.Count() > 0)
                {
                    foreach (var service in inputPkg.SpecialServices.SpecialService)
                    {
                        if (pkg.Postage.First().SpecialServices.SpecialService.Any(o => o.ServiceID == service.ToString()))
                            pkg.Postage.First().TotalPostage += Convert.ToDecimal(pkg.Postage.First().SpecialServices.SpecialService.First(o => o.ServiceID == service.ToString()).Price);
                    }
                }
            }

            return result;
        }
    }
}