//using Microsoft.AspNetCore.Mvc;
//using Server3.Services;

//namespace Server3.Controllers
//{
//    public class StatusController : ControllerBase
//    {
//        private readonly ServerMetricsService _metricsService;

//        public StatusController(ServerMetricsService metricsService)
//        {
//            _metricsService = metricsService;
//        }

//        [HttpGet]
//        public IActionResult GetStatus()
//        {
//            return Ok(_metricsService.GetServerState());
//        }
//    }
//}




using Microsoft.AspNetCore.Mvc;
using Server3.Services;
using Server3.Models;

namespace Server3.Controllers
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

