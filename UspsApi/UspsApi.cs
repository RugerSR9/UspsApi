using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public static Models.RateAPI.Response.Package GetRates(Models.RateAPI.Request.Package pkg)
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
        public static List<Models.RateAPI.Response.Package> GetRates(List<Models.RateAPI.Request.Package> pkgs)
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
        public static async Task<Models.RateAPI.Response.Package> GetRatesAsync(Models.RateAPI.Request.Package pkg)
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
        public static async Task<List<Models.RateAPI.Response.Package>> GetRatesAsync(List<Models.RateAPI.Request.Package> pkgs)
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

        public static TrackInfo Track(string trackingNumber)
        {
            List<TrackID> list = new() { new TrackID() { ID = trackingNumber } };
            List<TrackInfo> resp = TrackingAPI.TrackAsync(list).Result;
            return resp.First();
        }

        public static List<TrackInfo> Track(List<string> trackingNumbers)
        {
            List<TrackID> list = new();
            foreach (string id in trackingNumbers)
                list.Add(new TrackID() { ID = id });
            List<TrackInfo> resp = TrackingAPI.TrackAsync(list).Result;
            return resp;
        }


        public static async Task<TrackInfo> TrackAsync(string trackingNumber)
        {
            List<TrackID> list = new() { new TrackID() { ID = trackingNumber } };
            List<TrackInfo> resp = await TrackingAPI.TrackAsync(list);
            return resp.First();
        }

        public static async Task<List<TrackInfo>> TrackAsync(List<string> trackingNumbers)
        {
            List<TrackID> list = new();

            foreach (string id in trackingNumbers)
                list.Add(new TrackID() { ID = id });

            List<TrackInfo> resp = await TrackingAPI.TrackAsync(list);
            return resp;
        }
    }
}
