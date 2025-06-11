//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Server1.Services;

//namespace Server1.Controllers
//{
//    [ApiController]
//    [Route("api/status")]
//    public class StatusController : ControllerBase
//    {
//        private readonly ServerMetricsService _svc;
//        public StatusController(ServerMetricsService svc) => _svc = svc;

//        [HttpGet]
//        public IActionResult Get() => Ok(_svc.GetServerState());
//    }
//}




using Microsoft.AspNetCore.Mvc;
using Server1.Services;
using Server1.Models;

namespace Server1.Controllers
{
    [ApiController]
    [Route("api/status")]
    public class StatusController : ControllerBase
    {
        private readonly ServerMetricsService _metricsService;

        public StatusController(ServerMetricsService metricsService)
        {
            _metricsService = metricsService;
        }

        [HttpGet]
        public ActionResult<ServerState> GetStatus()
        {
            // Повертаємо поточний стан, який оновлюється кожні 5 сек
            return Ok(_metricsService.GetServerState());
        }
    }
}

