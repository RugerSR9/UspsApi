using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace UspsApi.Models.Contracts
{
    [ServiceContract(Name = "UspsApi.RateEstimate")]
    public interface IRateEstimate
    {
        ValueTask<List<RateAPI.Response.Package>> FetchRate(List<RateAPI.Request.Package> request);
    }
}
