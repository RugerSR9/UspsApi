using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace UspsOpenApi.Models.Contracts
{
    [ServiceContract(Name = "UspsOpenApi.RateEstimate")]
    public interface IRateEstimate
    {
        ValueTask<List<RateAPI.Response.Package>> FetchRate(List<RateAPI.Request.Package> request);
    }
}
