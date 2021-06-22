using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UspsApi.Models.AddressAPI;
using UspsApi.Models.TrackingAPI;
using static UspsApi.Settings;

namespace UspsApi
{
    public class UspsApi
    {
        public UspsApi(string userId, int maxRetries = 5, int retryDelay = 2500)
        {
            UserId = userId;
            MaxRetries = maxRetries;
            RetryDelay = retryDelay;
        }

        /// <summary>
        /// Fetch rates for a single Package.
        /// Upon USPS API communication issues, this request will continue to retry. You will need to set a timeout handler or cancel the request from the calling app if this is an issue.
        /// </summary>
        /// <param name="pkg"></param>
        /// <returns></returns>
        public Models.RateAPI.Response.Package GetRates(Models.RateAPI.Request.Package pkg)
        {
            List<Models.RateAPI.Request.Package> list = new() { pkg };

            List<Models.RateAPI.Response.Package> resp = RateAPI.FetchRatesAsync(list).Result;
            Models.RateAPI.Response.Package result = resp.First();

            if (result.Error != null)
                return result;

            result.Postage.First().TotalPostage = Convert.ToDecimal(result.Postage.First().Rate);

            if (pkg.SpecialServices.SpecialService != null && pkg.SpecialServices.SpecialService.Count > 0)
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
        public List<Models.RateAPI.Response.Package> GetRates(List<Models.RateAPI.Request.Package> pkgs)
        {
            List<Models.RateAPI.Response.Package> result = RateAPI.FetchRatesAsync(pkgs).Result;

            foreach (var pkg in result)
            {
                if (pkg.Error != null)
                    continue;

                pkg.Postage.First().TotalPostage = Convert.ToDecimal(pkg.Postage.First().Rate);

                Models.RateAPI.Request.Package inputPkg = pkgs.First(o => o.ID == pkg.ID);

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
        public async Task<Models.RateAPI.Response.Package> GetRatesAsync(Models.RateAPI.Request.Package pkg)
        {
            List<Models.RateAPI.Request.Package> list = new() { pkg };

            List<Models.RateAPI.Response.Package> resp = await RateAPI.FetchRatesAsync(list);
            Models.RateAPI.Response.Package result = resp.First();

            if (result.Error != null)
                return result;

            result.Postage.First().TotalPostage = Convert.ToDecimal(result.Postage.First().Rate);

            if (pkg.SpecialServices.SpecialService != null && pkg.SpecialServices.SpecialService.Count > 0)
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
        public async Task<List<Models.RateAPI.Response.Package>> GetRatesAsync(List<Models.RateAPI.Request.Package> pkgs)
        {
            List<Models.RateAPI.Response.Package> result = await RateAPI.FetchRatesAsync(pkgs);

            foreach (var pkg in result)
            {
                if (pkg.Error != null)
                    continue;

                pkg.Postage.First().TotalPostage = Convert.ToDecimal(pkg.Postage.First().Rate);

                Models.RateAPI.Request.Package inputPkg = pkgs.First(o => o.ID == pkg.ID);

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

        public TrackInfo Track(string trackingNumber)
        {
            List<TrackID> list = new() { new TrackID() { ID = trackingNumber } };
            List<TrackInfo> resp = TrackingAPI.TrackAsync(list).Result;
            return resp.First();
        }

        public List<TrackInfo> Track(List<string> trackingNumbers)
        {
            List<TrackID> list = new();
            foreach (string id in trackingNumbers)
                list.Add(new TrackID() { ID = id });
            List<TrackInfo> resp = TrackingAPI.TrackAsync(list).Result;
            return resp;
        }


        public async Task<TrackInfo> TrackAsync(string trackingNumber)
        {
            List<TrackID> list = new() { new TrackID() { ID = trackingNumber } };
            List<TrackInfo> resp = await TrackingAPI.TrackAsync(list);
            return resp.First();
        }

        public async Task<List<TrackInfo>> TrackAsync(List<string> trackingNumbers)
        {
            List<TrackID> list = new();

            foreach (string id in trackingNumbers)
                list.Add(new TrackID() { ID = id });

            List<TrackInfo> resp = await TrackingAPI.TrackAsync(list);
            return resp;
        }

        public Address ValidateAddress(Address input)
        {
            List<Address> list = new() { input };
            List<Address> resp = AddressAPI.ValidateAsync(list).Result;
            return resp.First();
        }

        public List<Address> ValidateAddress(List<Address> input)
        {
            return AddressAPI.ValidateAsync(input).Result;
        }

        public async Task<Address> ValidateAddressAsync(Address input)
        {
            List<Address> list = new() { input };
            List<Address> resp = await AddressAPI.ValidateAsync(list);
            return resp.First();
        }

        public async Task<List<Address>> ValidateAddressAsync(List<Address> input)
        {
            return await AddressAPI.ValidateAsync(input);
        }

        public ZipCode LookupCityState(ZipCode input)
        {
            List<ZipCode> list = new() { input };
            list = AddressAPI.CityStateLookupAsync(list).Result;
            return list.First();
        }

        public List<ZipCode> LookupCityState(List<ZipCode> input)
        {
            return AddressAPI.CityStateLookupAsync(input).Result;
        }

        public async Task<ZipCode> LookupCityStateAsync(ZipCode input)
        {
            List<ZipCode> list = new() { input };
            list = await AddressAPI.CityStateLookupAsync(list);
            return list.First();
        }

        public async Task<List<ZipCode>> LookupCityStateAsync(List<ZipCode> input)
        {
            return await AddressAPI.CityStateLookupAsync(input);
        }

        public Address LookupZipCode(Address input)
        {
            List<Address> list = new() { input };
            list = AddressAPI.ZipCodeLookupAsync(list).Result;
            return list.First();
        }

        public List<Address> LookupZipCode(List<Address> input)
        {
            return AddressAPI.ZipCodeLookupAsync(input).Result;
        }

        public async Task<Address> LookupZipCodeAsync(Address input)
        {
            List<Address> list = new() { input };
            list = await AddressAPI.ZipCodeLookupAsync(list);
            return list.First();
        }

        public async Task<List<Address>> LookupZipCodeAsync(List<Address> input)
        {
            return await AddressAPI.ZipCodeLookupAsync(input);
        }

        public PTSRreResult RequestPODEmail(PTSRreRequest input)
        {
            // must track first
            TrackInfo tracking = Track(input.TrackId);
            input.MpDate = tracking.MPDATE;
            input.MpSuffix = tracking.MPSUFFIX;
            input.TableCode = tracking.TABLECODE;
            List<PTSRreResult> list = ProofOfDeliveryAPI.RequestPODViaEmailAsync(new List<PTSRreRequest> { input }).Result;
            return list.First();
        }

        public List<PTSRreResult> RequestPODEmail(List<PTSRreRequest> input)
        {
            // must track first
            List<TrackInfo> tracking = Track(input.Select(o => o.TrackId).ToList());
            input.AsParallel().ForAll(o =>
            {
                TrackInfo trackInfo = tracking.First(p => p.ID == o.TrackId);
                o.MpDate = trackInfo.MPDATE;
                o.MpSuffix = trackInfo.MPSUFFIX;
                o.TableCode = trackInfo.TABLECODE;
            });

            return ProofOfDeliveryAPI.RequestPODViaEmailAsync(input).Result;
        }

        public async Task<PTSRreResult> RequestPODEmailAsync(PTSRreRequest input)
        {
            // must track first
            TrackInfo tracking = Track(input.TrackId);
            input.MpDate = tracking.MPDATE;
            input.MpSuffix = tracking.MPSUFFIX;
            input.TableCode = tracking.TABLECODE;
            List<PTSRreResult> list = await ProofOfDeliveryAPI.RequestPODViaEmailAsync(new List<PTSRreRequest> { input });
            return list.First();
        }

        public async Task<List<PTSRreResult>> RequestPODEmailAsync(List<PTSRreRequest> input)
        {
            // must track first
            List<TrackInfo> tracking = Track(input.Select(o => o.TrackId).ToList());
            input.AsParallel().ForAll(o =>
            {
                TrackInfo trackInfo = tracking.First(p => p.ID == o.TrackId);
                o.MpDate = trackInfo.MPDATE;
                o.MpSuffix = trackInfo.MPSUFFIX;
                o.TableCode = trackInfo.TABLECODE;
            });

            return await ProofOfDeliveryAPI.RequestPODViaEmailAsync(input);
        }
    }
}
