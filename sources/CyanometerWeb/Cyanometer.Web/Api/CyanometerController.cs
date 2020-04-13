using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cyanometer.Web.Api
{
    public abstract class CyanometerController<T>: ControllerBase
        where T: ControllerBase
    {
        protected readonly ILogger<T> logger;
        public CyanometerController(ILogger<T> logger)
        {
            this.logger = logger;
        }
    }
}
