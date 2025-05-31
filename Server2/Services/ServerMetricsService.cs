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
        private readonly object _lock = new();
        private readonly Gauge _cpuUsage = Metrics.CreateGauge("server_cpu_usage", "CPU Usage");
        private readonly Gauge _activeRequests = Metrics.CreateGauge("server_active_requests", "Active Requests");

        public ServerMetricsService()
        {
            // Паттерн «постоянного перегруза»
            _pattern = new List<ServerState>
            {
                new ServerState { CpuUsage = 85, ActiveRequests = 20, IsAvailable = true },
                new ServerState { CpuUsage = 90, ActiveRequests = 25, IsAvailable = true },
                new ServerState { CpuUsage = 95, ActiveRequests = 30, IsAvailable = true },
                new ServerState { CpuUsage = 92, ActiveRequests = 28, IsAvailable = true },
            };
            _currentIndex = 0;

            new Timer(_ => Next(), null, 0, 5000);
        }

        private void Next()
        {
            ServerState next;
            lock (_lock)
            {
                next = _pattern[_currentIndex];
                _currentIndex = (_currentIndex + 1) % _pattern.Count;
            }
            _cpuUsage.Set(next.CpuUsage);
            _activeRequests.Set(next.ActiveRequests);
        }

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
                IsAvailable = true
            };
    }
}

