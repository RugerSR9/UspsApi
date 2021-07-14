using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UspsApi
{
    public class UspsApi
    {
        public static string UspsApiUsername { get; set; }
        /// <summary>
        /// Helpers for XML requests to the USPS API. https://www.usps.com/business/web-tools-apis/documentation-updates.htm
        /// </summary>
        /// <param name="username">Your username for accessing the USPS API. You can obtain this by visiting https://registration.shippingapis.com/ </param>
        public UspsApi(string username)
        {
            UspsApiUsername = username;
        }

        #region RateAPI
        /// <summary>
        /// Fetch rates for a single Package.
        /// Upon USPS API communication issues, this request will continue to retry. You will need to set a timeout handler or cancel the request from the calling app if this is an issue.
        /// </summary>
        /// <param name="pkg"></param>
        /// <returns></returns>
        public Models.RateAPI.Response.Package GetRates(Models.RateAPI.Request.Package package)
        {
            List<Models.RateAPI.Request.Package> list = new() { package };

            List<Models.RateAPI.Response.Package> resp = RateAPI.FetchRatesAsync(list, UspsApiUsername).Result;
            Models.RateAPI.Response.Package result = resp.First();

            if (result.Error != null)
                return result;

            result.Postage.First().TotalPostage = Convert.ToDecimal(result.Postage.First().Rate);

            if (package.SpecialServices.SpecialService != null && package.SpecialServices.SpecialService.Count > 0)
            {
                foreach (var service in package.SpecialServices.SpecialService)
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
        public List<Models.RateAPI.Response.Package> GetRates(List<Models.RateAPI.Request.Package> packages)
        {
            List<Models.RateAPI.Response.Package> result = RateAPI.FetchRatesAsync(packages, UspsApiUsername).Result;

            foreach (var pkg in result)
            {
                if (pkg.Error != null)
                    continue;

                pkg.Postage.First().TotalPostage = Convert.ToDecimal(pkg.Postage.First().Rate);

                Models.RateAPI.Request.Package inputPkg = packages.First(o => o.ID == pkg.ID);

                if (inputPkg.SpecialServices.SpecialService != null && inputPkg.SpecialServices.SpecialService.Count > 0)
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


        /// <summary>
        /// Fetch rates for a single Package.
        /// Upon USPS API communication issues, this request will continue to retry. You will need to set a timeout handler or cancel the request from the calling app if this is an issue.
        /// </summary>
        /// <param name="pkg"></param>
        /// <returns></returns>
        public async Task<Models.RateAPI.Response.Package> GetRatesAsync(Models.RateAPI.Request.Package package)
        {
            List<Models.RateAPI.Request.Package> list = new() { package };

            List<Models.RateAPI.Response.Package> resp = await RateAPI.FetchRatesAsync(list, UspsApiUsername);
            Models.RateAPI.Response.Package result = resp.First();

            if (result.Error != null)
                return result;

            result.Postage.First().TotalPostage = Convert.ToDecimal(result.Postage.First().Rate);

            if (package.SpecialServices.SpecialService != null && package.SpecialServices.SpecialService.Count > 0)
            {
                foreach (var service in package.SpecialServices.SpecialService)
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
        public async Task<List<Models.RateAPI.Response.Package>> GetRatesAsync(List<Models.RateAPI.Request.Package> packages)
        {
            List<Models.RateAPI.Response.Package> result = await RateAPI.FetchRatesAsync(packages, UspsApiUsername);

            foreach (var pkg in result)
            {
                if (pkg.Error != null)
                    continue;

                pkg.Postage.First().TotalPostage = Convert.ToDecimal(pkg.Postage.First().Rate);

                Models.RateAPI.Request.Package inputPkg = packages.First(o => o.ID == pkg.ID);

                if (inputPkg.SpecialServices.SpecialService != null && inputPkg.SpecialServices.SpecialService.Count > 0)
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
        #endregion

        #region TrackingAPI
        /// <summary>
        /// Retrieve all tracking for a USPS package via tracking number.
        /// Upon USPS API communication issues, this request will continue to retry. You will need to set a timeout handler or cancel the request from the calling app if this is an issue.
        /// </summary>
        /// <param name="trackingNumber"></param>
        /// <returns></returns>
        public Models.TrackingAPI.TrackInfo Track(string trackingNumber)
        {
            List<Models.TrackingAPI.TrackID> list = new() { new Models.TrackingAPI.TrackID() { ID = trackingNumber } };
            List<Models.TrackingAPI.TrackInfo> resp = TrackingAPI.TrackAsync(list, UspsApiUsername).Result;
            return resp.First();
        }

        /// <summary>
        /// Asynchronously retrieve all tracking for a USPS package via tracking number.
        /// Upon USPS API communication issues, this request will continue to retry. You will need to set a timeout handler or cancel the request from the calling app if this is an issue.
        /// </summary>
        /// <param name="trackingNumber"></param>
        /// <returns></returns>
        public async Task<Models.TrackingAPI.TrackInfo> TrackAsync(string trackingNumber)
        {
            List<Models.TrackingAPI.TrackID> list = new() { new Models.TrackingAPI.TrackID() { ID = trackingNumber } };
            List<Models.TrackingAPI.TrackInfo> resp = await TrackingAPI.TrackAsync(list, UspsApiUsername);
            return resp.First();
        }

        /// <summary>
        /// Retrieve all tracking for a list of USPS packages via tracking number.
        /// Upon USPS API communication issues, this request will continue to retry. You will need to set a timeout handler or cancel the request from the calling app if this is an issue.
        /// </summary>
        /// <param name="trackingNumber"></param>
        /// <returns></returns>
        public List<Models.TrackingAPI.TrackInfo> Track(List<string> trackingNumbers)
        {
            List<Models.TrackingAPI.TrackID> list = new();
            foreach (string id in trackingNumbers)
                list.Add(new Models.TrackingAPI.TrackID() { ID = id });
            List<Models.TrackingAPI.TrackInfo> resp = TrackingAPI.TrackAsync(list, UspsApiUsername).Result;
            return resp;
        }

        /// <summary>
        /// Asynchronously retrieve all tracking for a list of USPS packages via tracking number.
        /// Upon USPS API communication issues, this request will continue to retry. You will need to set a timeout handler or cancel the request from the calling app if this is an issue.
        /// </summary>
        /// <param name="trackingNumber"></param>
        /// <returns></returns>
        public async Task<List<Models.TrackingAPI.TrackInfo>> TrackAsync(List<string> trackingNumbers)
        {
            List<Models.TrackingAPI.TrackID> list = new();

            foreach (string id in trackingNumbers)
                list.Add(new Models.TrackingAPI.TrackID() { ID = id });

            List<Models.TrackingAPI.TrackInfo> resp = await TrackingAPI.TrackAsync(list, UspsApiUsername);
            return resp;
        }
        #endregion

        #region AddressAPI

        #region AddressValidation
        /// <summary>
        /// Validate a corrupt or incomplete address to USPS standards.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Models.AddressAPI.Address ValidateAddress(Models.AddressAPI.Address address)
        {
            List<Models.AddressAPI.Address> list = new() { address };
            List<Models.AddressAPI.Address> resp = AddressAPI.ValidateAsync(list, UspsApiUsername).Result;
            return resp.First();
        }

        /// <summary>
        /// Validate a list of corrupt or incomplete addresses to USPS standards.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<Models.AddressAPI.Address> ValidateAddress(List<Models.AddressAPI.Address> addresses)
        {
            return AddressAPI.ValidateAsync(addresses, UspsApiUsername).Result;
        }

        /// <summary>
        /// Asynchronously validate a corrupt or incomplete address to USPS standards.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Models.AddressAPI.Address> ValidateAddressAsync(Models.AddressAPI.Address address)
        {
            List<Models.AddressAPI.Address> list = new() { address };
            List<Models.AddressAPI.Address> resp = await AddressAPI.ValidateAsync(list, UspsApiUsername);
            return resp.First();
        }

        /// <summary>
        /// Asynchronously validate a list of corrupt or incomplete addresses to USPS standards.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<Models.AddressAPI.Address>> ValidateAddressAsync(List<Models.AddressAPI.Address> addresses)
        {
            return await AddressAPI.ValidateAsync(addresses, UspsApiUsername);
        }
        #endregion

        #region CityStateLookup
        /// <summary>
        /// Lookup City, State by Zip Code.
        /// </summary>
        /// <param name="zipCode"></param>
        /// <returns></returns>
        public Models.AddressAPI.ZipCode LookupCityState(Models.AddressAPI.ZipCode zipCode)
        {
            List<Models.AddressAPI.ZipCode> list = new() { zipCode };
            list = AddressAPI.CityStateLookupAsync(list, UspsApiUsername).Result;
            return list.First();
        }

        // <summary>
        /// Lookup City, State by list of Zip Codes.
        /// </summary>
        /// <param name="zipCodes"></param>
        /// <returns></returns>
        public List<Models.AddressAPI.ZipCode> LookupCityState(List<Models.AddressAPI.ZipCode> zipCodes)
        {
            return AddressAPI.CityStateLookupAsync(zipCodes, UspsApiUsername).Result;
        }

        // <summary>
        /// Asynchronously lookup City, State by Zip Code.
        /// </summary>
        /// <param name="zipCode"></param>
        /// <returns></returns>
        public async Task<Models.AddressAPI.ZipCode> LookupCityStateAsync(Models.AddressAPI.ZipCode zipCode)
        {
            List<Models.AddressAPI.ZipCode> list = new() { zipCode };
            list = await AddressAPI.CityStateLookupAsync(list, UspsApiUsername);
            return list.First();
        }

        // <summary>
        /// Asynchronously lookup City, State by list of Zip Codes.
        /// </summary>
        /// <param name="zipCodes"></param>
        /// <returns></returns>
        public async Task<List<Models.AddressAPI.ZipCode>> LookupCityStateAsync(List<Models.AddressAPI.ZipCode> zipCodes)
        {
            return await AddressAPI.CityStateLookupAsync(zipCodes, UspsApiUsername);
        }
        #endregion

        #region ZipCodeLookup
        /// <summary>
        /// Lookup Zip Code by City, State
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public Models.AddressAPI.Address LookupZipCode(Models.AddressAPI.Address address)
        {
            List<Models.AddressAPI.Address> list = new() { address };
            list = AddressAPI.ZipCodeLookupAsync(list, UspsApiUsername).Result;
            return list.First();
        }

        /// <summary>
        /// Lookup Zip Codes by City, State
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public List<Models.AddressAPI.Address> LookupZipCode(List<Models.AddressAPI.Address> addresses)
        {
            return AddressAPI.ZipCodeLookupAsync(addresses, UspsApiUsername).Result;
        }

        /// <summary>
        /// Asynchronously lookup Zip Code by City, State
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public async Task<Models.AddressAPI.Address> LookupZipCodeAsync(Models.AddressAPI.Address address)
        {
            List<Models.AddressAPI.Address> list = new() { address };
            list = await AddressAPI.ZipCodeLookupAsync(list, UspsApiUsername);
            return list.First();
        }

        /// <summary>
        /// Asynchronously lookup Zip Codes by City, State
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public async Task<List<Models.AddressAPI.Address>> LookupZipCodeAsync(List<Models.AddressAPI.Address> addresses)
        {
            return await AddressAPI.ZipCodeLookupAsync(addresses, UspsApiUsername);
        }
        #endregion

        #endregion

        #region ProofOfDeliverAPI
        /// <summary>
        /// Request a Proof of Delivery email for the specified tracking number.
        /// </summary>
        /// <param name="ptsRreRequest"></param>
        /// <returns></returns>
        public Models.TrackingAPI.PTSRreResult RequestPODEmail(Models.TrackingAPI.PTSRreRequest ptsRreRequest)
        {
            // must track first
            Models.TrackingAPI.TrackInfo tracking = Track(ptsRreRequest.TrackId);
            ptsRreRequest.MpDate = tracking.MPDATE;
            ptsRreRequest.MpSuffix = tracking.MPSUFFIX;
            ptsRreRequest.TableCode = tracking.TABLECODE;
            List<Models.TrackingAPI.PTSRreResult> list = ProofOfDeliveryAPI.RequestPODViaEmailAsync(new List<Models.TrackingAPI.PTSRreRequest> { ptsRreRequest }, UspsApiUsername).Result;
            return list.First();
        }

        /// <summary>
        /// Request a Proof of Delivery email for the specified tracking numbers.
        /// </summary>
        /// <param name="ptsRreRequests"></param>
        /// <returns></returns>
        public List<Models.TrackingAPI.PTSRreResult> RequestPODEmail(List<Models.TrackingAPI.PTSRreRequest> ptsRreRequests)
        {
            // must track first
            List<Models.TrackingAPI.TrackInfo> tracking = Track(ptsRreRequests.Select(o => o.TrackId).ToList());
            ptsRreRequests.AsParallel().ForAll(o =>
            {
                Models.TrackingAPI.TrackInfo trackInfo = tracking.First(p => p.ID == o.TrackId);
                o.MpDate = trackInfo.MPDATE;
                o.MpSuffix = trackInfo.MPSUFFIX;
                o.TableCode = trackInfo.TABLECODE;
            });

            return ProofOfDeliveryAPI.RequestPODViaEmailAsync(ptsRreRequests, UspsApiUsername).Result;
        }

        /// <summary>
        /// Asynchronously request a Proof of Delivery email for the specified tracking number.
        /// </summary>
        /// <param name="ptsRreRequest"></param>
        /// <returns></returns>
        public async Task<Models.TrackingAPI.PTSRreResult> RequestPODEmailAsync(Models.TrackingAPI.PTSRreRequest ptsRreRequest)
        {
            // must track first
            Models.TrackingAPI.TrackInfo tracking = Track(ptsRreRequest.TrackId);
            ptsRreRequest.MpDate = tracking.MPDATE;
            ptsRreRequest.MpSuffix = tracking.MPSUFFIX;
            ptsRreRequest.TableCode = tracking.TABLECODE;
            List<Models.TrackingAPI.PTSRreResult> list = await ProofOfDeliveryAPI.RequestPODViaEmailAsync(new List<Models.TrackingAPI.PTSRreRequest> { ptsRreRequest }, UspsApiUsername);
            return list.First();
        }

        /// <summary>
        /// Asynchronously request a Proof of Delivery email for the specified tracking numbers.
        /// </summary>
        /// <param name="ptsRreRequest"></param>
        /// <returns></returns>
        public async Task<List<Models.TrackingAPI.PTSRreResult>> RequestPODEmailAsync(List<Models.TrackingAPI.PTSRreRequest> ptsRreRequests)
        {
            // must track first
            List<Models.TrackingAPI.TrackInfo> tracking = Track(ptsRreRequests.Select(o => o.TrackId).ToList());
            ptsRreRequests.AsParallel().ForAll(o =>
            {
                Models.TrackingAPI.TrackInfo trackInfo = tracking.First(p => p.ID == o.TrackId);
                o.MpDate = trackInfo.MPDATE;
                o.MpSuffix = trackInfo.MPSUFFIX;
                o.TableCode = trackInfo.TABLECODE;
            });

            return await ProofOfDeliveryAPI.RequestPODViaEmailAsync(ptsRreRequests, UspsApiUsername);
        }
        #endregion
    }
}
