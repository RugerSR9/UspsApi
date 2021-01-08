using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UspsApi.Models.Contracts;
using UspsApiBase;

namespace UspsApi.gRPC.Services
{
    public class RateEstimate : IRateEstimate
    {
        private readonly ILogger<RateEstimate> _logger;
        public RateEstimate(ILogger<RateEstimate> logger)
        {
            _logger = logger;
        }

        private static RateAPI _rateApi = new RateAPI();

        public ValueTask<List<Models.RateAPI.Response.Package>> FetchRate(List<Models.RateAPI.Request.Package> request)
        {
            _logger.LogInformation("FetchRate() hit");
            Console.WriteLine("FetchRate() hit");
            var result = _rateApi.GetRates(request);
            return new ValueTask<List<Models.RateAPI.Response.Package>>(result);
        }
    }
}
