using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace UspsApi.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UspsApiController : ControllerBase
    {
        private readonly ILogger<UspsApiController> _logger;
        public UspsApiController(ILogger<UspsApiController> logger)
        {
            _logger = logger;
        }

        
    }


}
