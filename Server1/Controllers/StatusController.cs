using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server1.Services;

namespace Server1.Controllers
{
    [ApiController]
    [Route("api/status")]
    public class StatusController : ControllerBase
    {
        private readonly ServerMetricsService _svc;
        public StatusController(ServerMetricsService svc) => _svc = svc;

        [HttpGet]
        public IActionResult Get() => Ok(_svc.GetServerState());
    }
}
