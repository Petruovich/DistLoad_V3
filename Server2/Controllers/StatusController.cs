//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Server2.Services;

//namespace Server2.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
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
using Server2.Services;
using Server2.Models;

namespace Server2.Controllers
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

