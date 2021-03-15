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
using UspsOpenApi.Models.AddressAPI;

namespace UspsOpenApi.ase
{
    public class AddressAPI
    {
        #region Address Validation
        internal static List<Address> Validate(List<Address> input)
        {
            // limit is 5 addresses per request
            string requestGuid = Guid.NewGuid().ToString();
            Log.Information("{area}: New request for {packageTotal} packages. {requestGuid}", "Validate()", input.Count, requestGuid);

            List<Address> output = new List<Address>();
            string userId = ***REMOVED***;
            AddressValidateRequest request;
            int index = 0;

            while (index < input.Count)
            {
                request = new AddressValidateRequest
                {
                    Address = input.Skip(index).Take(5).ToList(),
                    USERID = userId,
                };

                Log.Information("{area}: Fetching rates for {packageCount} package(s). {requestGuid}", "Validate()", input.Count, requestGuid);

                XmlSerializer xsSubmit = new XmlSerializer(typeof(AddressValidateRequest));
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
                    new KeyValuePair<string,string>("API", "Verify"),
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
                        Log.Warning("{area}: USPS Failed to Respond after " + retryCount + " seconds. Attempt {retryCount}. {requestGuid}", "Validate()", retryCount, requestGuid);

                    response = httpClient.PostAsync(uspsUrl, formData).Result;
                    Thread.Sleep(1000 * retryCount);
                    httpClient.CancelPendingRequests();

                    retryCount++;

                    if (retryCount > 50)
                    {
                        Log.Error("{area}: USPS Failed to Respond after 50 attempts. {requestGuid}", "Validate()", retryCount, requestGuid);
                        throw new UspsOpenApiException("408: After many attempts, the request to the USPS API did not recieve a response. Please try again later.");
                    }
                }

                TimeSpan responseTime = DateTime.Now.TimeOfDay.Subtract(responseTimer.TimeOfDay);
                var content = response.Content.ReadAsStringAsync().Result;
                Log.Information("{area}: USPS response received in {responseTime} ms. {requestGuid}", "Validate()", responseTime.Milliseconds, requestGuid);

                try
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(AddressValidateResponse));
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
                    AddressValidateResponse responseJson = (AddressValidateResponse)deserializer.Deserialize(ms);

                    index += 5;

                    if (responseJson.Error != null)
                        Log.Warning("{area}: USPS Returned Error: {uspsErrorNumber} {uspsErrorDescription} {requestGuid}", "Validate()", responseJson.Error.Number, responseJson.Error.Description, requestGuid);

                    foreach (Address validatedAddress in responseJson.Address)
                    {
                        Address orig = input.First(i => i.AddressDetailId == validatedAddress.AddressDetailId);

                        if (validatedAddress.Error != null)
                        {
                            Log.Warning("{area}: USPS Returned Error: {uspsErrorNumber} {uspsErrorDescription} {requestGuid}", "Validate()", validatedAddress.Error.Number, validatedAddress.Error.Description, requestGuid);
                            orig.Error = validatedAddress.Error;
                        }

                        orig.Business = validatedAddress.Business;
                        orig.CarrierRoute = validatedAddress.CarrierRoute;
                        orig.CentralDeliveryPoint = validatedAddress.CentralDeliveryPoint;
                        orig.CityAbbreviation = validatedAddress.CityAbbreviation;
                        orig.DeliveryPoint = validatedAddress.DeliveryPoint;
                        orig.DPVCMRA = validatedAddress.DPVCMRA;
                        orig.DPVConfirmation = validatedAddress.DPVConfirmation;
                        orig.DPVFootnotes = validatedAddress.DPVFootnotes;
                        orig.Footnotes = validatedAddress.Footnotes;
                        orig.Vacant = validatedAddress.Vacant;

                        // backup origingal address and overwrite with validated
                        orig.OriginalAddress1 = orig.Address1;
                        orig.Address1 = validatedAddress.Address1;
                        orig.OriginalAddress2 = orig.Address2;
                        orig.Address2 = validatedAddress.Address2;
                        orig.OriginalCity = orig.City;
                        orig.City = validatedAddress.City;
                        orig.OriginalState = orig.State;
                        orig.State = validatedAddress.State;
                        orig.OriginalZip = orig.MailZip;
                        orig.Zip5 = validatedAddress.Zip5 + "-" + validatedAddress.Zip4;
                        output.Add(orig);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("{area}: Exception: {ex} {requestGuid}", "Validate()", ex.ToString(), requestGuid);
                    throw new UspsOpenApiException(ex);
                }
            }

            if (output.Count != input.Count)
            {
                // something went wrong because counts should always match
                Console.WriteLine("Counts did not match between input and output");
                Log.Error("{area}: Counts did not match between input and output. {requestGuid}", "Validate()", requestGuid);
                throw new UspsOpenApiException("Counts did not match between input and output");
            }

            return output;
        }

        public static Address ValidateAddress(Address input)
        {
            List<Address> list = new List<Address> { input };
            List<Address> resp = Validate(list);
            return resp.First();
        }

        public static List<Address> ValidateAddress(List<Address> input)
        {
            return Validate(input);
        }
        #endregion

        #region City State lookup
        internal static List<ZipCode> CityStateLookup(List<ZipCode> input)
        {
            // limit is 5 addresses per request
            string requestGuid = Guid.NewGuid().ToString();
            Log.Information("{area}: New request for {packageTotal} packages. {requestGuid}", "CityStateLookup()", input.Count, requestGuid);

            List<ZipCode> output = new List<ZipCode>();
            string userId = ***REMOVED***;
            CityStateLookupRequest request;
            int index = 0;

            while (index < input.Count)
            {
                request = new CityStateLookupRequest
                {
                    ZipCode = input.Skip(index).Take(5).ToList(),
                    USERID = userId,
                };

                Log.Information("{area}: Fetching rates for {packageCount} package(s). {requestGuid}", "CityStateLookup()", input.Count, requestGuid);

                XmlSerializer xsSubmit = new XmlSerializer(typeof(CityStateLookupRequest));
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
                    new KeyValuePair<string,string>("API", "CityStateLookup"),
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
                        Log.Error("{area}: USPS Failed to Respond after 50 attempts. {requestGuid}", "CityStateLookup()", retryCount, requestGuid);
                        throw new UspsOpenApiException("408: After many attempts, the request to the USPS API did not recieve a response. Please try again later.");
                    }

                    if (retryCount > 0)
                        Log.Warning("{area}: USPS Failed to Respond after " + retryCount + " seconds. Attempt {retryCount}. {requestGuid}", "CityStateLookup()", retryCount, requestGuid);

                    try
                    {
                        response = httpClient.PostAsync(uspsUrl, formData).Result;
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
                var content = response.Content.ReadAsStringAsync().Result;
                Log.Information("{area}: USPS response received in {responseTime} ms. {requestGuid}", "CityStateLookup()", responseTime.Milliseconds, requestGuid);

                try
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(CityStateLookupResponse));
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
                    CityStateLookupResponse responseJson = (CityStateLookupResponse)deserializer.Deserialize(ms);

                    index += 5;

                    foreach (ZipCode zip in responseJson.ZipCode)
                    {
                        if (zip.Error != null)
                        {
                            Log.Warning("{area}: USPS Returned Error: {uspsErrorNumber} {uspsErrorDescription} {requestGuid}", "CityStateLookup()", zip.Error.Number, zip.Error.Description, requestGuid);
                            output.Add(zip);
                        }
                        else
                        {
                            // preserve ID
                            ZipCode orig = input.First(i => i.Zip5 == zip.Zip5);
                            orig.City = zip.City;
                            orig.State = zip.State;
                            output.Add(orig);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("{area}: Exception: {ex} {requestGuid}", "CityStateLookup()", ex.ToString(), requestGuid);
                    throw new UspsOpenApiException(ex);
                }
            }

            if (output.Count != input.Count)
            {
                // something went wrong because counts should always match
                Console.WriteLine("Counts did not match between input and output");
                Log.Error("{area}: Counts did not match between input and output. {requestGuid}", "CityStateLookup()", requestGuid);
                throw new UspsOpenApiException("Counts did not match between input and output");
            }

            return output;
        }

        public static ZipCode LookupCityState(ZipCode input)
        {
            List<ZipCode> list = new List<ZipCode> { input };
            list = CityStateLookup(list);
            return list.First();
        }

        public static List<ZipCode> LookupCityState(List<ZipCode> input)
        {
            return CityStateLookup(input);
        }
        #endregion

        #region Zip Code Lookup
        internal static List<Address> ZipCodeLookup(List<Address> input)
        {
            // limit is 5 addresses per request
            string requestGuid = Guid.NewGuid().ToString();
            Log.Information("{area}: New request for {packageTotal} packages. {requestGuid}", "ZipCodeLookup()", input.Count, requestGuid);

            List<Address> output = new List<Address>();
            string userId = ***REMOVED***;
            ZipCodeLookupRequest request;
            int index = 0;

            while (index < input.Count)
            {
                request = new ZipCodeLookupRequest
                {
                    Address = input.Skip(index).Take(5).ToList(),
                    USERID = userId,
                };

                Log.Information("{area}: Fetching rates for {packageCount} package(s). {requestGuid}", "ZipCodeLookup()", input.Count, requestGuid);

                XmlSerializer xsSubmit = new XmlSerializer(typeof(ZipCodeLookupRequest));
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
                    new KeyValuePair<string,string>("API", "ZipCodeLookup"),
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
                        Log.Warning("{area}: USPS Failed to Respond after " + retryCount + " seconds. Attempt {retryCount}. {requestGuid}", "ZipCodeLookup()", retryCount, requestGuid);

                    response = httpClient.PostAsync(uspsUrl, formData).Result;
                    Thread.Sleep(1000 * retryCount);
                    httpClient.CancelPendingRequests();

                    retryCount++;

                    if (retryCount > 50)
                    {
                        Log.Error("{area}: USPS Failed to Respond after 50 attempts. {requestGuid}", "ZipCodeLookup()", retryCount, requestGuid);
                        throw new UspsOpenApiException("408: After many attempts, the request to the USPS API did not recieve a response. Please try again later.");
                    }
                }

                TimeSpan responseTime = DateTime.Now.TimeOfDay.Subtract(responseTimer.TimeOfDay);
                var content = response.Content.ReadAsStringAsync().Result;
                Log.Information("{area}: USPS response received in {responseTime} ms. {requestGuid}", "ZipCodeLookup()", responseTime.Milliseconds, requestGuid);

                try
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(ZipCodeLookupResponse));
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
                    ZipCodeLookupResponse responseJson = (ZipCodeLookupResponse)deserializer.Deserialize(ms);

                    index += 5;

                    foreach (Address validatedAddress in responseJson.Address)
                    {
                        Address orig = input.First(i => i.ID == validatedAddress.ID);

                        if (validatedAddress.Error != null)
                        {
                            Log.Warning("{area}: USPS Returned Error: {uspsErrorNumber} {uspsErrorDescription} {requestGuid}", "ZipCodeLookup()", validatedAddress.Error.Number, validatedAddress.Error.Description, requestGuid);
                            orig.Error = validatedAddress.Error;
                        }

                        orig.Zip5 = validatedAddress.Zip5;
                        orig.Zip4 = validatedAddress.Zip4;
                        output.Add(orig);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("{area}: Exception: {ex} {requestGuid}", "ZipCodeLookup()", ex.ToString(), requestGuid);
                    throw new UspsOpenApiException(ex);
                }
            }

            if (output.Count != input.Count)
            {
                // something went wrong because counts should always match
                Console.WriteLine("Counts did not match between input and output");
                Log.Error("{area}: Counts did not match between input and output. {requestGuid}", "ZipCodeLookup()", requestGuid);
                throw new UspsOpenApiException("Counts did not match between input and output");
            }

            return output;
        }

        public static Address LookupZipCode(Address input)
        {
            List<Address> list = new List<Address> { input };
            list = ZipCodeLookup(list);
            return list.First();
        }

        public static List<Address> LookupZipCode(List<Address> input)
        {
            return ZipCodeLookup(input);
        }
        #endregion
    }
}