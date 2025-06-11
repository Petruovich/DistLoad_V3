//using Prometheus;
//using System;
//using System.Collections.Generic;
//using System.Threading;
//using System.Net.Http;
//using System.Text.Json;
//using DistLoad.Models;

//namespace DistLoad.Metrics
//{
//    public class MetricsLogger
//    {
//        private readonly List<ServerInstance> _servers;
//        private readonly object _lock = new();

//        // для зовнішнього Prometheus
//        public static readonly Gauge CpuUsageGauge = global::Prometheus.Metrics.CreateGauge("server_cpu_usage", "CPU Usage");
//        public static readonly Gauge ActiveRequestsGauge = global::Prometheus.Metrics.CreateGauge("server_active_requests", "Active Requests");
//        public static readonly Counter TotalRequestsCounter = global::Prometheus.Metrics.CreateCounter("server_total_requests", "Total requests");

//        public MetricsLogger(List<ServerInstance> servers)
//        {
//            _servers = servers;
//            var t = new Thread(Loop) { IsBackground = true };
//            t.Start();
//        }

//        public void Loop()
//        {
//            while (true)
//            {
//                LogAll();
//                Thread.Sleep(5000);
//            }
//        }

//        private void LogAll()
//        {
//            lock (_lock)
//            {
//                // Блок 1: Метрики серверів
//                Console.ForegroundColor = ConsoleColor.Green;
//                Console.WriteLine("=== Server Metrics (every 5s) ===");
//                foreach (var s in _servers)
//                {
//                    Console.WriteLine($"• {s.Id} @ {s.Address}");
//                    Console.WriteLine($"    CPU Usage:       {CpuUsageGauge.Value}%");
//                    Console.WriteLine($"    Active Requests: {ActiveRequestsGauge.Value}");
//                    Console.WriteLine($"    Total Requests:  {TotalRequestsCounter.Value}");
//                }

//                // Блок 2: Логи балансувальника
//                Console.ForegroundColor = ConsoleColor.Yellow;
//                Console.WriteLine("\n=== Load Balancer Dispatch Counts ===");
//                foreach (var kv in LoadBalancerMiddleware.ServerCounters)
//                {
//                    Console.WriteLine($"Server {kv.Key}: {kv.Value} requests");
//                }

//                Console.ResetColor();
//                Console.WriteLine(); // відступ для читабельності
//            }
//        }
//    }
//}




//using Prometheus;
//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Text.Json;
//using System.Threading;
//using System.Threading.Tasks;
//using DistLoad.Models;

//namespace DistLoad.Metrics
//{
//    public class MetricsLogger
//    {
//        private readonly List<ServerInstance> _servers;
//        private readonly HttpClient _http = new();
//        private readonly object _lock = new();

//        // ці ж Gauge використовуються для зовнішнього Prometheus
//        public static readonly Gauge CpuUsageGauge = global::Prometheus.Metrics.CreateGauge("server_cpu_usage", "CPU Usage");
//        public static readonly Gauge ActiveRequestsGauge = global::Prometheus.Metrics.CreateGauge("server_active_requests", "Active Requests");
//        public static readonly Counter TotalRequestsCounter = global::Prometheus.Metrics.CreateCounter("server_total_requests", "Total requests");

//        private readonly JsonSerializerOptions _jsonOptions =
//            new() { PropertyNameCaseInsensitive = true };

//        public MetricsLogger(List<ServerInstance> servers)
//        {
//            _servers = servers;
//            // стартуємо один фон-цикл, який одночасно оновлює метрики і лог
//            var thread = new Thread(async () => await Loop()) { IsBackground = true };
//            thread.Start();
//        }

//        private async Task Loop()
//        {
//            while (true)
//            {
//                await UpdateMetricsFromServers();
//                LogAll();
//                Thread.Sleep(5000);
//            }
//        }

//        private async Task UpdateMetricsFromServers()
//        {
//            foreach (var s in _servers)
//            {
//                try
//                {
//                    var json = await _http.GetStringAsync($"{s.Address}/api/status");
//                    var st = JsonSerializer.Deserialize<ServerState>(json, _jsonOptions);
//                    if (st != null)
//                    {
//                        lock (_lock)
//                        {
//                            CpuUsageGauge.Set(st.CpuUsage);
//                            ActiveRequestsGauge.Set(st.ActiveRequests);
//                            // якщо хочете встановлювати точне значення, а не інкремент:
//                            // TotalRequestsCounter.IncTo(st.TotalRequests);
//                            TotalRequestsCounter.Inc();
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Console.ForegroundColor = ConsoleColor.Red;
//                    Console.WriteLine($"[MetricsLogger] Помилка при зверненні до {s.Address}: {ex.Message}");
//                    Console.ResetColor();
//                }
//            }
//        }

//        private void LogAll()
//        {
//            lock (_lock)
//            {
//                // Блок 1: Метрики серверів
//                Console.ForegroundColor = ConsoleColor.Green;
//                Console.WriteLine("=== Server Metrics (every 5s) ===");
//                foreach (var s in _servers)
//                {
//                    Console.WriteLine($"• {s.Id} @ {s.Address}");
//                    Console.WriteLine($"    CPU Usage:       {CpuUsageGauge.Value}%");
//                    Console.WriteLine($"    Active Requests: {ActiveRequestsGauge.Value}");
//                    Console.WriteLine($"    Total Requests:  {TotalRequestsCounter.Value}");
//                }

//                // Блок 2: Логи балансувальника
//                Console.ForegroundColor = ConsoleColor.Yellow;
//                Console.WriteLine("\n=== Load Balancer Dispatch Counts ===");
//                foreach (var kv in LoadBalancerMiddleware.ServerCounters)
//                {
//                    Console.WriteLine($"Server {kv.Key}: {kv.Value} requests");
//                }

//                Console.ResetColor();
//                Console.WriteLine(); // відступ
//            }
//        }
//    }

//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Text.Json;
//using System.Threading;
//using DistLoad.Models;

//namespace DistLoad.Metrics
//{
//    public class MetricsLogger
//    {
//        private readonly List<ServerInstance> _servers;
//        private readonly HttpClient _http = new();
//        private readonly object _lock = new();

//        public MetricsLogger(List<ServerInstance> servers)
//        {
//            _servers = servers;
//            var thread = new Thread(Loop) { IsBackground = true };
//            thread.Start();
//        }

//        public void Loop()
//        {
//            while (true)
//            {
//                LogAll();
//                Thread.Sleep(5000);
//            }
//        }

//        private void LogAll()
//        {
//            lock (_lock)
//            {
//                Console.ForegroundColor = ConsoleColor.Green;
//                Console.WriteLine("=== Server Metrics (every 5s) ===");

//                foreach (var s in _servers)
//                {
//                    try
//                    {
//                        var response = _http.GetStringAsync($"{s.Address}/api/status").Result;
//                        var state = JsonSerializer.Deserialize<ServerState>(response,
//                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

//                        if (state != null)
//                        {
//                            Console.WriteLine($"• {s.Id} @ {s.Address}");
//                            Console.WriteLine($"    CPU Usage:       {state.CpuUsage}%");
//                            Console.WriteLine($"    Active Requests: {state.ActiveRequests}");
//                        }
//                        else
//                        {
//                            Console.WriteLine($"• {s.Id} @ {s.Address}  -- failed to parse");
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine($"• {s.Id} @ {s.Address}  -- error: {ex.Message}");
//                    }

//                    Console.WriteLine(new string('-', 40));
//                }

//                Console.ForegroundColor = ConsoleColor.Yellow;
//                Console.WriteLine("\n=== Load Balancer Dispatch Counts ===");
//                foreach (var kv in LoadBalancerMiddleware.ServerCounters)
//                {
//                    Console.WriteLine($"Server {kv.Key}: {kv.Value} requests");
//                }

//                Console.ResetColor();
//                Console.WriteLine();
//            }
//        }
//    }


//using DistLoad.Models;
//using Prometheus;
//using System.Text.Json;

//public class MetricsLogger
//{
//    private readonly List<ServerInstance> _servers;
//    private readonly HttpClient _http = new();
//    private readonly object _lock = new();

//    // 1) Gauge/Counter с одним лейблом server_id:
//    public static readonly Gauge CpuUsageGauge = Metrics
//        .CreateGauge("server_cpu_usage", "CPU Usage (%)",
//            new GaugeConfiguration { LabelNames = new[] { "server_id" } });

//    public static readonly Gauge ActiveRequestsGauge = Metrics
//        .CreateGauge("server_active_requests", "Active Requests",
//            new GaugeConfiguration { LabelNames = new[] { "server_id" } });

//    public static readonly Counter TotalRequestsCounter = Metrics
//        .CreateCounter("server_total_requests", "Total requests",
//            new CounterConfiguration { LabelNames = new[] { "server_id" } });

//    private readonly JsonSerializerOptions _jsonOptions =
//        new() { PropertyNameCaseInsensitive = true };

//    public MetricsLogger(List<ServerInstance> servers)
//    {
//        _servers = servers;
//        // запускаем фоновый цикл
//        var thread = new Thread(async () => await Loop()) { IsBackground = true };
//        thread.Start();
//    }

//    public async Task Loop()
//    {
//        while (true)
//        {
//            await UpdateMetricsFromServers();
//            LogAll();
//            Thread.Sleep(5000);
//        }
//    }

//    private async Task UpdateMetricsFromServers()
//    {
//        foreach (var s in _servers)
//        {
//            try
//            {
//                var json = await _http.GetStringAsync($"{s.Address}/api/status");
//                var st = JsonSerializer.Deserialize<ServerState>(json, _jsonOptions);
//                if (st != null)
//                {
//                    lock (_lock)
//                    {
//                        // 2) Указываем лейбл server_id
//                        CpuUsageGauge.WithLabels(s.Id).Set(st.CpuUsage);
//                        ActiveRequestsGauge.WithLabels(s.Id).Set(st.ActiveRequests);
//                        TotalRequestsCounter.WithLabels(s.Id).Inc();
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.ForegroundColor = ConsoleColor.Red;
//                Console.WriteLine($"[MetricsLogger] Error fetching {s.Address}: {ex.Message}");
//                Console.ResetColor();
//            }
//        }
//    }

//    public void LogAll()
//    {
//        lock (_lock)
//        {
//            // Блок 1: Метрики серверов
//            Console.ForegroundColor = ConsoleColor.Green;
//            Console.WriteLine("=== Server Metrics (every 5s) ===");
//            foreach (var s in _servers)
//            {
//                // правки здесь:
//                var cpu = CpuUsageGauge.WithLabels(s.Id).Value;
//                var act = ActiveRequestsGauge.WithLabels(s.Id).Value;
//                var total = TotalRequestsCounter.WithLabels(s.Id).Value;

//                Console.WriteLine($"• {s.Id} @ {s.Address}");
//                Console.WriteLine($"    CPU Usage:       {cpu:0.##}%");
//                Console.WriteLine($"    Active Requests: {act}");
//                Console.WriteLine($"    Total Requests:  {total}");
//                Console.WriteLine(new string('-', 40));
//            }

//            // Блок 2: Логи балансировщика
//            Console.ForegroundColor = ConsoleColor.Yellow;
//            Console.WriteLine("\n=== Load Balancer Dispatch Counts ===");
//            foreach (var kv in LoadBalancerMiddleware.ServerCounters)
//            {
//                Console.WriteLine($"Server {kv.Key}: {kv.Value} requests");
//            }

//            Console.ResetColor();
//            Console.WriteLine(); // отступ для читабельности
//        }
//    }

//}
// MetricsLogger.cs







//using DistLoad.Models;
//using Prometheus;
//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Text.Json;
//using System.Threading;
//using System.Threading.Tasks;

//namespace DistLoad.Metrics
//{
//    public class MetricsLogger
//    {
//        private readonly List<ServerInstance> _servers;
//        private readonly HttpClient _http = new();
//        private readonly object _lock = new();

//        // 1) Gauge/Counter с лейблом server_id
//        public static readonly Gauge CpuUsageGauge = global::Prometheus.Metrics
//            .CreateGauge("server_cpu_usage", "CPU Usage (%)",
//                new GaugeConfiguration { LabelNames = new[] { "server_id" } });

//        public static readonly Gauge ActiveRequestsGauge = global::Prometheus.Metrics
//            .CreateGauge("server_active_requests", "Active Requests",
//                new GaugeConfiguration { LabelNames = new[] { "server_id" } });

//        public static readonly Counter TotalRequestsCounter = global::Prometheus.Metrics
//            .CreateCounter("server_total_requests", "Total requests",
//                new CounterConfiguration { LabelNames = new[] { "server_id" } });

//        private readonly JsonSerializerOptions _jsonOptions =
//            new() { PropertyNameCaseInsensitive = true };

//        public MetricsLogger(List<ServerInstance> servers)
//        {
//            _servers = servers;
//            // запускаем фоновый цикл
//            var thread = new Thread(async () => await Loop()) { IsBackground = true };
//            thread.Start();
//        }

//        public async Task Loop()
//        {
//            while (true)
//            {
//                await UpdateMetricsFromServers();
//                LogAll();
//                Thread.Sleep(5000);
//            }
//        }

//        private async Task UpdateMetricsFromServers()
//        {
//            foreach (var s in _servers)
//            {
//                try
//                {
//                    var json = await _http.GetStringAsync($"{s.Address}/api/status");
//                    var st = JsonSerializer.Deserialize<ServerState>(json, _jsonOptions);
//                    if (st != null)
//                    {
//                        lock (_lock)
//                        {
//                            // 2) Обновляем Prometheus-метрики с указанием лейбла
//                            CpuUsageGauge.WithLabels(s.Id).Set(st.CpuUsage);
//                            ActiveRequestsGauge.WithLabels(s.Id).Set(st.ActiveRequests);
//                            TotalRequestsCounter.WithLabels(s.Id).Inc();

//                            // 3) И сразу же записываем в модель, чтобы балансировщик видел актуальные данные
//                            s.CpuUsage = (int)st.CpuUsage;
//                            s.ActiveRequests = st.ActiveRequests;
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Console.ForegroundColor = ConsoleColor.Red;
//                    Console.WriteLine($"[MetricsLogger] Error fetching from {s.Address}: {ex.Message}");
//                    Console.ResetColor();
//                }
//            }
//        }

//        public void LogAll()
//        {
//            lock (_lock)
//            {
//                // Блок 1: Метрики серверов
//                Console.ForegroundColor = ConsoleColor.Green;
//                Console.WriteLine("=== Server Metrics (every 5s) ===");
//                foreach (var s in _servers)
//                {
//                    var cpu = CpuUsageGauge.WithLabels(s.Id).Value;
//                    var act = ActiveRequestsGauge.WithLabels(s.Id).Value;
//                    var total = TotalRequestsCounter.WithLabels(s.Id).Value;

//                    Console.WriteLine($"• {s.Id} @ {s.Address}");
//                    Console.WriteLine($"    CPU Usage:       {cpu:0.##}%");
//                    Console.WriteLine($"    Active Requests: {act}");
//                    Console.WriteLine($"    Total Requests:  {total}");
//                    Console.WriteLine(new string('-', 40));
//                }

//                // Блок 2: Логи балансировщика
//                Console.ForegroundColor = ConsoleColor.Yellow;
//                Console.WriteLine("\n=== Load Balancer Dispatch Counts ===");
//                foreach (var kv in LoadBalancerMiddleware.ServerCounters)
//                {
//                    Console.WriteLine($"Server {kv.Key}: {kv.Value} requests");
//                }

//                Console.ResetColor();
//                Console.WriteLine(); // отступ
//            }
//        }
//    }
//}


// MetricsLogger.cs













//using DistLoad.Models;
//using Prometheus;
//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Text.Json;
//using System.Threading;
//using System.Threading.Tasks;

//namespace DistLoad.Metrics
//{
//    public class MetricsLogger
//    {
//        private readonly List<ServerInstance> _servers;
//        private readonly HttpClient _http = new();
//        private readonly object _lock = new();
//        private readonly JsonSerializerOptions _jsonOptions =
//            new() { PropertyNameCaseInsensitive = true };

//        // 1) Создаём gauge/counter с меткой server_id
//        public static readonly Gauge CpuUsageGauge = global::Prometheus.Metrics
//            //            .Metrics
//            .CreateGauge("server_cpu_usage", "CPU Usage (%)",
//                new GaugeConfiguration { LabelNames = new[] { "server_id" } });

//        public static readonly Gauge ActiveRequestsGauge = global::Prometheus.Metrics
//            //            .Metrics
//            .CreateGauge("server_active_requests", "Active Requests",
//                new GaugeConfiguration { LabelNames = new[] { "server_id" } });

//        public static readonly Counter TotalRequestsCounter = global::Prometheus.Metrics
//            //            .Metrics
//            .CreateCounter("server_total_requests", "Total requests",
//                new CounterConfiguration { LabelNames = new[] { "server_id" } });

//        public MetricsLogger(List<ServerInstance> servers)
//        {
//            _servers = servers;
//            // запускаем цикл в фоне
//            var thread = new Thread(() => Loop().GetAwaiter().GetResult())
//            {
//                IsBackground = true
//            };
//            thread.Start();
//        }

//        public async Task Loop()
//        {
//            while (true)
//            {
//                await UpdateMetricsFromServers();
//                LogAll();
//                Thread.Sleep(5000);
//            }
//        }

//        private async Task UpdateMetricsFromServers()
//        {
//            foreach (var s in _servers)
//            {
//                try
//                {
//                    var json = await _http.GetStringAsync($"{s.Address}/api/status");
//                    var st = JsonSerializer.Deserialize<ServerState>(json, _jsonOptions);
//                    if (st != null)
//                    {
//                        lock (_lock)
//                        {
//                            // 2) обновляем Prometheus-метрики с меткой
//                            CpuUsageGauge.WithLabels(s.Id).Set(st.CpuUsage);
//                            ActiveRequestsGauge.WithLabels(s.Id).Set(st.ActiveRequests);
//                            TotalRequestsCounter.WithLabels(s.Id).Inc();

//                            // 3) и пушим в модель, чтобы балансировщик видел
//                            s.CpuUsage = (int)st.CpuUsage;
//                            s.ActiveRequests = st.ActiveRequests;
//                            s.IsAvailable = st.IsAvailable;
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Console.ForegroundColor = ConsoleColor.Red;
//                    Console.WriteLine($"[MetricsLogger] Error fetching {s.Address}: {ex.Message}");
//                    Console.ResetColor();
//                }
//            }
//        }

//        public void LogAll()
//        {
//            lock (_lock)
//            {
//                Console.ForegroundColor = ConsoleColor.Green;
//                Console.WriteLine("=== Server Metrics (every 5s) ===");
//                foreach (var s in _servers)
//                {
//                    var cpu = CpuUsageGauge.WithLabels(s.Id).Value;
//                    var act = ActiveRequestsGauge.WithLabels(s.Id).Value;
//                    var total = TotalRequestsCounter.WithLabels(s.Id).Value;
//                    Console.WriteLine($"• {s.Id} @ {s.Address}");
//                    Console.WriteLine($"    CPU Usage:       {cpu:0.##}%");
//                    Console.WriteLine($"    Active Requests: {act}");
//                    Console.WriteLine($"    Total Requests:  {total}");
//                    Console.WriteLine(new string('-', 40));
//                }

//                Console.ForegroundColor = ConsoleColor.Yellow;
//                Console.WriteLine("\n=== Load Balancer Dispatch Counts ===");
//                foreach (var kv in LoadBalancerMiddleware.ServerCounters)
//                    Console.WriteLine($"Server {kv.Key}: {kv.Value} requests");

//                Console.ResetColor();
//                Console.WriteLine();
//            }
//        }
//    }






using DistLoad.Models;
using Prometheus;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DistLoad.Metrics
{
    public class MetricsLogger
    {
        private readonly List<ServerInstance> _servers;
        private readonly HttpClient _http = new();
        private readonly object _lock = new();
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        // Gauge/Counter із лейблом "server_id"
        public static readonly Gauge CpuUsageGauge = global::Prometheus.Metrics.CreateGauge(
            "server_cpu_usage", "CPU Usage (%)",
            new GaugeConfiguration { LabelNames = new[] { "server_id" } });

        public static readonly Gauge ActiveRequestsGauge = global::Prometheus.Metrics.CreateGauge(
            "server_active_requests", "Active Requests",
            new GaugeConfiguration { LabelNames = new[] { "server_id" } });

        public static readonly Counter TotalRequestsCounter = global::Prometheus.Metrics.CreateCounter(
            "server_total_requests", "Total requests",
            new CounterConfiguration { LabelNames = new[] { "server_id" } });

        public static readonly Gauge ResponseTimeGauge = global::Prometheus.Metrics.CreateGauge(
            "server_response_time_ms", "Average Response Time (ms)",
            new GaugeConfiguration { LabelNames = new[] { "server_id" } });

        public static readonly Gauge FailureRateGauge = global::Prometheus.Metrics.CreateGauge(
            "server_failure_rate", "Failure Rate (0.0–1.0)",
            new GaugeConfiguration { LabelNames = new[] { "server_id" } });

        public MetricsLogger(List<ServerInstance> servers)
        {
            _servers = servers;
            // Запускаємо нескінченний цикл, що кожні 5 секунд звертається до серверів
            var thread = new Thread(() => Loop().GetAwaiter().GetResult())
            {
                IsBackground = true
            };
            thread.Start();
        }

        public async Task Loop()
        {
            while (true)
            {
                await UpdateMetricsFromServers();
                LogAll();
                Thread.Sleep(5000);
            }
        }

        private async Task UpdateMetricsFromServers()
        {
            foreach (var s in _servers)
            {
                try
                {
                    // Отримуємо JSON зі статусом сервера: /api/status (повертає ServerState)
                    var json = await _http.GetStringAsync($"{s.Address}/api/status");
                    var st = JsonSerializer.Deserialize<ServerState>(json, _jsonOptions);

                    if (st != null)
                    {
                        lock (_lock)
                        {
                            // Оновлюємо Prometheus-метрики
                            CpuUsageGauge.WithLabels(s.Id).Set(st.CpuUsage);
                            ActiveRequestsGauge.WithLabels(s.Id).Set(st.ActiveRequests);
                            TotalRequestsCounter.WithLabels(s.Id).IncTo(st.TotalRequests);
                            ResponseTimeGauge.WithLabels(s.Id).Set(st.ResponseTime);
                            FailureRateGauge.WithLabels(s.Id).Set(st.FailureRate);

                            // Оновлюємо модель у пам’яті, щоб AdaptiveBalancer “бачив” актуальні дані
                            s.CpuUsage = st.CpuUsage;
                            s.ActiveRequests = st.ActiveRequests;
                            s.RequestCount = st.TotalRequests;
                            s.LastState = st;
                            s.IsAvailable = st.IsAvailable;
                            // Перевіряємо порогові значення
                            s.IsOverloaded = (s.CpuUsage >= s.CpuCriticalThreshold)
                                          || (s.ActiveRequests >= s.MaxActiveRequests)
                                          || (!st.IsAvailable);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[MetricsLogger] Error fetching from {s.Address}: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }

        public void LogAll()
        {
            lock (_lock)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("=== Server Metrics (every 5s) ===");
                foreach (var s in _servers)
                {
                    var cpu = CpuUsageGauge.WithLabels(s.Id).Value;
                    var act = ActiveRequestsGauge.WithLabels(s.Id).Value;
                    var total = TotalRequestsCounter.WithLabels(s.Id).Value;
                    var resp = ResponseTimeGauge.WithLabels(s.Id).Value;
                    var fail = FailureRateGauge.WithLabels(s.Id).Value;

                    Console.WriteLine($"• {s.Id} @ {s.Address}");
                    Console.WriteLine($"    CPU Usage:       {cpu:0.##}%   (Threshold: {s.CpuCriticalThreshold}%)");
                    Console.WriteLine($"    Active Requests: {act}   (MaxAllowed: {s.MaxActiveRequests})");
                    Console.WriteLine($"    Total Requests:  {total}");
                    Console.WriteLine($"    Response Time:   {resp:0.##} ms");
                    Console.WriteLine($"    Failure Rate:    {fail:0.##}");
                    Console.WriteLine($"    Overloaded:      {s.IsOverloaded}");
                    Console.WriteLine(new string('-', 40));
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n=== Load Balancer Dispatch Counts ===");
                foreach (var kv in LoadBalancerMiddleware.ServerCounters)
                {
                    Console.WriteLine($"Server {kv.Key}: {kv.Value} requests");
                }
                Console.ResetColor();
                Console.WriteLine();
            }
        }
    }
}







