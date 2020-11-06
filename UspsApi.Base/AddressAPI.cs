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

namespace UspsApiBase
{
    public class AddressAPI
    {
        #region Address Validation
        internal async Task<List<Address>> Validate(List<Address> input)
        {
            // limit is 5 addresses per request
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

                index += 5;

                XmlSerializer xsSubmit = new XmlSerializer(typeof(AddressValidateRequest));
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
                    new KeyValuePair<string,string>("API", "Verify"),
                    new KeyValuePair<string, string>("XML", xml)
                });

                HttpClient httpClient = new HttpClient();
                var response = await httpClient.PostAsync(uspsUrl, formData);
                var content = await response.Content.ReadAsStringAsync();

                try
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(AddressValidateResponse));
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
                    AddressValidateResponse responseJson = (AddressValidateResponse)deserializer.Deserialize(ms);

                    // todo: save response data to correct input data
                    foreach (Address validatedAddress in responseJson.Address)
                    {
                        Address orig = input.First(i => i.AddressDetailId == validatedAddress.AddressDetailId);
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

        public Address ValidateAddress(Address input)
        {
            List<Address> list = new List<Address> { input };
            return Validate(list).Result.First();
        }

        public List<Address> ValidateAddress(List<Address> input)
        {
            return Validate(input).Result;
        }
        #endregion

        #region City State lookup
        internal async Task<List<ZipCode>> CityStateLookup(List<ZipCode> input)
        {
            // limit is 5 addresses per request
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

                index += 5;

                XmlSerializer xsSubmit = new XmlSerializer(typeof(CityStateLookupRequest));
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
                    new KeyValuePair<string,string>("API", "CityStateLookup"),
                    new KeyValuePair<string, string>("XML", xml)
                });

                HttpClient httpClient = new HttpClient();
                var response = await httpClient.PostAsync(uspsUrl, formData);
                var content = await response.Content.ReadAsStringAsync();

                try
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(CityStateLookupResponse));
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
                    CityStateLookupResponse responseJson = (CityStateLookupResponse)deserializer.Deserialize(ms);

                    // todo: save response data to correct input data
                    foreach (ZipCode zip in responseJson.ZipCode)
                    {
                        // preserve ID
                        ZipCode orig = input.First(i => i.Zip5 == zip.Zip5);
                        orig.City = zip.City;
                        orig.State = zip.State;
                        output.Add(orig);
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

        public ZipCode LookupCityState(ZipCode input)
        {
            List<ZipCode> list = new List<ZipCode> { input };
            return CityStateLookup(list).Result.First();
        }

        public List<ZipCode> LookupCityState(List<ZipCode> input)
        {
            return CityStateLookup(input).Result;
        }
        #endregion

        #region Zip Code Lookup
        internal async Task<List<Address>> ZipCodeLookup(List<Address> input)
        {
            // limit is 5 addresses per request
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

                index += 5;

                XmlSerializer xsSubmit = new XmlSerializer(typeof(ZipCodeLookupRequest));
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
                    new KeyValuePair<string,string>("API", "ZipCodeLookup"),
                    new KeyValuePair<string, string>("XML", xml)
                });

                HttpClient httpClient = new HttpClient();
                var response = await httpClient.PostAsync(uspsUrl, formData);
                var content = await response.Content.ReadAsStringAsync();

                try
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(ZipCodeLookupResponse));
                    var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
                    ZipCodeLookupResponse responseJson = (ZipCodeLookupResponse)deserializer.Deserialize(ms);

                    // todo: save response data to correct input data
                    foreach (Address validatedAddress in responseJson.Address)
                    {
                        Address orig = input.First(i => i.ID == validatedAddress.ID);
                        orig.Zip5 = validatedAddress.Zip5;
                        orig.Zip4 = validatedAddress.Zip4;
                        output.Add(orig);
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

        public Address LookupZipCode(Address input)
        {
            List<Address> list = new List<Address> { input };
            return ZipCodeLookup(list).Result.First();
        }

        public List<Address> LookupZipCode(List<Address> input)
        {
            return ZipCodeLookup(input).Result;
        }
        #endregion
    }
}