//using Prometheus;
//using Server3.Models;

//namespace Server3.Services
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


// Проект Server3/Services/ServerMetricsService.cs
using Prometheus;
using Server3.Models;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Server3.Services
{
    public class ServerMetricsService
    {
        private readonly List<ServerState> _pattern;
        private int _idx;
        private readonly object _lock = new();
        private readonly Gauge _cpuUsage = Metrics.CreateGauge("server_cpu_usage", "CPU Usage");
        private readonly Gauge _activeRequests = Metrics.CreateGauge("server_active_requests", "Active Requests");

        public ServerMetricsService()
        {
            // «скачкообразный» паттерн с периодическим unavailability
            _pattern = new List<ServerState>
            {
                new ServerState { CpuUsage = 15, ActiveRequests =  2, IsAvailable = true },
                new ServerState { CpuUsage = 60, ActiveRequests =  5, IsAvailable = true },
                new ServerState { CpuUsage = 10, ActiveRequests =  1, IsAvailable = true}, // отказ
                new ServerState { CpuUsage = 50, ActiveRequests =  4, IsAvailable = true },
            };
            _idx = 0;
            new Timer(_ => Next(), null, 0, 5000);
        }

        private void Next()
        {
            ServerState next;
            lock (_lock)
            {
                next = _pattern[_idx];
                _idx = (_idx + 1) % _pattern.Count;
            }
            _cpuUsage.Set(next.CpuUsage);
            _activeRequests.Set(next.ActiveRequests);
            // доступность передаётся в контроллер статуса
            CurrentAvailability = next.IsAvailable;
        }

        // Для StatusController
        public bool CurrentAvailability { get; private set; } = true;

        public void IncreaseRequests()
        {
            lock (_lock)
            {
                var v = (int)_activeRequests.Value + 1;
                _activeRequests.Set(v);
            }
        }

        public void DecreaseRequests()
        {
            lock (_lock)
            {
                var v = Math.Max(0, (int)_activeRequests.Value - 1);
                _activeRequests.Set(v);
            }
        }

        public ServerState GetServerState()
            => new ServerState
            {
                CpuUsage = _cpuUsage.Value,
                ActiveRequests = (int)_activeRequests.Value,
                IsAvailable = CurrentAvailability
            };
    }
}

