//using Prometheus;
//using Server2.Models;

//namespace Server2.Services
//{
//    public class ServerMetricsService
//    {
//        private readonly ServerState _state = new();
//        private readonly Random _random = new();

//        private readonly Gauge _cpuUsage = Metrics.CreateGauge("server_cpu_usage", "CPU Usage");
//        private readonly Gauge _activeRequests = Metrics.CreateGauge("server_active_requests", "Active Requests");

//        public ServerMetricsService()
//        {

//            new Timer(_ =>
//            {
//                _state.CpuUsage = _random.Next(10, 90);
//                _cpuUsage.Set(_state.CpuUsage);
//            }, null, 0, 5000);
//        }

//        public void IncreaseRequests()
//        {
//            _state.ActiveRequests++;
//            _activeRequests.Set(_state.ActiveRequests);
//        }

//        public void DecreaseRequests()
//        {
//            if (_state.ActiveRequests > 0)
//                _state.ActiveRequests--;

//            _activeRequests.Set(_state.ActiveRequests);
//        }

//        public ServerState GetServerState()
//        {
//            return _state;
//        }
//    }
//}
// Проект Server2/Services/ServerMetricsService.cs







//using Prometheus;
//using Server2.Models;
//using System;
//using System.Collections.Generic;
//using System.Threading;

//namespace Server2.Services
//{
//    public class ServerMetricsService
//    {
//        private readonly List<ServerState> _pattern;
//        private int _currentIndex;
//        private readonly object _lock = new();
//        private readonly Gauge _cpuUsage = Metrics.CreateGauge("server_cpu_usage", "CPU Usage");
//        private readonly Gauge _activeRequests = Metrics.CreateGauge("server_active_requests", "Active Requests");

//        public ServerMetricsService()
//        {
//            // Паттерн «постоянного перегруза»
//            _pattern = new List<ServerState>
//            {
//                new ServerState { CpuUsage = 85, ActiveRequests = 20, IsAvailable = true },
//                new ServerState { CpuUsage = 90, ActiveRequests = 25, IsAvailable = true },
//                new ServerState { CpuUsage = 95, ActiveRequests = 30, IsAvailable = true },
//                new ServerState { CpuUsage = 92, ActiveRequests = 28, IsAvailable = true },
//            };
//            _currentIndex = 0;

//            new Timer(_ => Next(), null, 0, 5000);
//        }

//        private void Next()
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
//            => new ServerState
//            {
//                CpuUsage = _cpuUsage.Value,
//                ActiveRequests = (int)_activeRequests.Value,
//                IsAvailable = true
//            };
//    }
//}






using Prometheus;
using Server2.Models;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Server2.Services
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
            // Гірший сервер: більший CPU і більше запитів, ближчий до порогу
            _pattern = new List<ServerState>
            {
                new ServerState { CpuUsage = 60, ActiveRequests = 20, TotalRequests = 20, ResponseTime = 150, FailureRate = 0.10, IsAvailable = true },
                new ServerState { CpuUsage = 75, ActiveRequests = 25, TotalRequests = 40, ResponseTime = 200, FailureRate = 0.15, IsAvailable = true },
                new ServerState { CpuUsage = 85, ActiveRequests = 30, TotalRequests = 70, ResponseTime = 250, FailureRate = 0.20, IsAvailable = true },
                new ServerState { CpuUsage = 90, ActiveRequests = 35, TotalRequests = 100,ResponseTime = 300, FailureRate = 0.30, IsAvailable = false }, // зробимо недоступним
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
                    TotalRequests = (int)(_activeRequests.Value),
                    ResponseTime = (int)_responseTime.Value,
                    FailureRate = _failureRate.Value,
                    IsAvailable = _failureRate.Value < 0.25 // наприклад, якщо failRate>0.25, server недоступний
                };
            }
        }
    }
}


