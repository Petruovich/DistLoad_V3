//using Prometheus;
//using Server1.Models;
//using System;
//using System.Collections.Generic;
//using System.Threading;

//namespace Server1.Services
//{
//public class ServerMetricsService
//{
//    private readonly ServerState _state = new();
//    private readonly Random _random = new();

//    private readonly Gauge _cpuUsage = Metrics.CreateGauge("server_cpu_usage", "CPU Usage");
//    private readonly Gauge _activeRequests = Metrics.CreateGauge("server_active_requests", "Active Requests");

//    public ServerMetricsService()
//    {
//        //new Timer(_ =>
//        //{
//        //    _state.CpuUsage = _random.Next(10, 90);
//        //    _cpuUsage.Set(*_state.CpuUsage*/);
//        //}, null, 0, 5000);
//        new Timer(_ =>
//        {
//           var de = _state.CpuUsage = _random.Next(10, 90);
//            _cpuUsage.Set(de/*_state.CpuUsage*/);
//        }, null, 0, 5000);

//    }

//    public void IncreaseRequests()
//    {
//        _state.ActiveRequests++;
//        _activeRequests.Set(_state.ActiveRequests);
//    }

//    public void DecreaseRequests()
//    {
//        if (_state.ActiveRequests > 0)
//            _state.ActiveRequests--;

//        _activeRequests.Set(_state.ActiveRequests);
//    }



//    public ServerState GetServerState()
//    {
//        _state.CpuUsage = _cpuUsage.Value;  
//        _state.ActiveRequests = (int)_activeRequests.Value;  
//        return _state;
//    }

//}



//    public class ServerMetricsService
//    {
//        private readonly List<ServerState> _pattern;
//        private int _currentIndex;
//        private readonly Gauge _cpuUsage = Metrics.CreateGauge("server_cpu_usage", "CPU Usage");
//        private readonly Gauge _activeRequests = Metrics.CreateGauge("server_active_requests", "Active Requests");
//        private readonly object _lock = new();

//        public ServerMetricsService()
//        {

//            _pattern = new List<ServerState>
//        {
//            new ServerState { CpuUsage = 10, ActiveRequests =  5, IsAvailable = true },
//            new ServerState { CpuUsage = 20, ActiveRequests = 10, IsAvailable = true },
//            new ServerState { CpuUsage = 50, ActiveRequests = 15, IsAvailable = true },
//            new ServerState { CpuUsage = 80, ActiveRequests =  8, IsAvailable = true },

//        };
//            _currentIndex = 0;


//            new Timer(_ =>
//            {
//                LogNextPattern();
//            }, null, 0, 5000);
//        }

//        private void LogNextPattern()
//        {
//            ServerState next;
//            lock (_lock)
//            {
//                next = _pattern[_currentIndex];
//                _currentIndex = (_currentIndex + 1) % _pattern.Count;
//            }

//            _cpuUsage.Set(next.CpuUsage);
//            _activeRequests.Set(next.ActiveRequests);
//        }

//        public void IncreaseRequests()
//        {
//            lock (_lock)
//            {
//                var v = (int)_activeRequests.Value + 1;
//                _activeRequests.Set(v);
//            }
//        }

//        public void DecreaseRequests()
//        {
//            lock (_lock)
//            {
//                var v = Math.Max(0, (int)_activeRequests.Value - 1);
//                _activeRequests.Set(v);
//            }
//        }

//        public ServerState GetServerState()
//        {

//            return new ServerState
//            {
//                CpuUsage = _cpuUsage.Value,
//                ActiveRequests = (int)_activeRequests.Value,
//                IsAvailable = true
//            };
//        }
//    }
//}


// 1) Эмулируем метрики












//    public class ServerMetricsService
//    {
//        private readonly List<ServerState> _pattern = new()
//    {
//        new() { CpuUsage = 10, ActiveRequests =  5, IsAvailable = true },
//        new() { CpuUsage = 20, ActiveRequests = 10, IsAvailable = true },
//        new() { CpuUsage = 50, ActiveRequests = 15, IsAvailable = true },
//        new() { CpuUsage = 80, ActiveRequests =  8, IsAvailable = true },
//    };

//        private int _idx = 0;
//        private readonly object _lock = new();

//        public ServerMetricsService()
//        {
//            // каждую пятую секунду переключаемся на следующий паттерн
//            new Timer(_ =>
//            {
//                lock (_lock)
//                {
//                    _idx = (_idx + 1) % _pattern.Count;
//                }
//            }, null, 0, 5000);
//        }

//        public ServerState GetServerState()
//        {
//            lock (_lock)
//            {
//                return _pattern[_idx];
//            }
//        }
//    }

//    // 2) Контроллер отдаёт JSON-статус

//}





using Prometheus;
using Server1.Models;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Server1.Services
{
    public class ServerMetricsService
    {
        private readonly List<ServerState> _pattern;
        private int _currentIndex;
        private readonly Gauge _cpuUsage = Metrics.CreateGauge("server_cpu_usage", "CPU Usage");
        private readonly Gauge _activeRequests = Metrics.CreateGauge("server_active_requests", "Active Requests");
        private readonly Gauge _responseTime = Metrics.CreateGauge("server_response_time_ms", "Response Time (ms)");
        private readonly Gauge _failureRate = Metrics.CreateGauge("server_failure_rate", "Failure Rate");
        private readonly object _lock = new();

        public ServerMetricsService()
        {
            // Цей патерн описує “поведінку” Server1 (гарні значення)
            _pattern = new List<ServerState>
            {
                new ServerState { CpuUsage = 10, ActiveRequests =  5, TotalRequests =  5, ResponseTime =  50, FailureRate = 0.01, IsAvailable = true },
                new ServerState { CpuUsage = 20, ActiveRequests = 10, TotalRequests = 15, ResponseTime =  60, FailureRate = 0.02, IsAvailable = true },
                new ServerState { CpuUsage = 50, ActiveRequests = 15, TotalRequests = 30, ResponseTime = 100, FailureRate = 0.05, IsAvailable = true },
                new ServerState { CpuUsage = 15, ActiveRequests =  7, TotalRequests = 40, ResponseTime =  70, FailureRate = 0.03, IsAvailable = true },
            };
            _currentIndex = 0;

            new Timer(_ =>
            {
                LogNextPattern();
            }, null, 0, 5000);
        }

        private void LogNextPattern()
        {
            ServerState next;
            lock (_lock)
            {
                next = _pattern[_currentIndex];
                _currentIndex = (_currentIndex + 1) % _pattern.Count;
            }

            _cpuUsage.Set(next.CpuUsage);
            _activeRequests.Set(next.ActiveRequests);
            _responseTime.Set(next.ResponseTime);
            _failureRate.Set(next.FailureRate);
        }

        public ServerState GetServerState()
        {
            lock (_lock)
            {
                return new ServerState
                {
                    CpuUsage = (int)_cpuUsage.Value,
                    ActiveRequests = (int)_activeRequests.Value,
                    TotalRequests = (int)(_activeRequests.Value), // тут можна окремий лічильник
                    ResponseTime = (int)_responseTime.Value,
                    FailureRate = _failureRate.Value,
                    IsAvailable = true
                };
            }
        }
    }
}
