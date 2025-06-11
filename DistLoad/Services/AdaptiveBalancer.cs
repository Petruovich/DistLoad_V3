//using DistLoad.Interfaces;
//using DistLoad.Models;

//namespace DistLoad.Services
//using DistLoad.Interfaces;
//using DistLoad.Models;

//{
    //public class AdaptiveBalancer : ILoadBalancer
    //{
    //    private readonly List<ServerInstance> _servers;

    //    public AdaptiveBalancer(List<ServerInstance> servers)
    //    {
    //        _servers = servers;
    //    }

    //    public Task<ServerInstance> GetNextServerAsync()
    //    {
    //        if (_servers.Count == 0)
    //            throw new Exception("No available servers");

    //        var bestServer = _servers
    //            .Where(s => s.IsAvailable)
    //            .OrderBy(s => s.ActiveRequests)
    //            .ThenBy(s => s.CpuUsage) 
    //            .FirstOrDefault();

    //        if (bestServer == null)
    //            throw new Exception("No available servers");

    //        bestServer.ActiveRequests++;

    //        return Task.FromResult(bestServer);
    //    }

    //    public void ReleaseServer(ServerInstance server)
    //    {
    //        var target = _servers.FirstOrDefault(s => s.Id == server.Id);
    //        if (target != null && target.ActiveRequests > 0)
    //        {
    //            target.ActiveRequests--;
    //        }
    //    }
    //    public List<ServerInstance> GetServers() => _servers;
    //}









    //public class AdaptiveBalancer : ILoadBalancer
    //{
    //    private readonly List<ServerInstance> _servers;

    //    public AdaptiveBalancer(List<ServerInstance> servers)
    //    {
    //        _servers = servers;
    //    }

    //    public Task<ServerInstance> GetNextServerAsync()
    //    {
    //        if (_servers.Count == 0)
    //            throw new Exception("No available servers");

    //        // Сортируем по минимальному ActiveRequests, затем по минимальному CpuUsage
    //        var bestServer = _servers
    //            .Where(s => s.IsAvailable)
    //            .OrderBy(s => s.ActiveRequests)
    //            .ThenBy(s => s.CpuUsage)
    //            .FirstOrDefault();

    //        if (bestServer == null)
    //            throw new Exception("No available servers");

    //        // Увеличиваем счётчик активных запросов, чтобы следующий выбор учёл это
    //        bestServer.ActiveRequests++;

    //        return Task.FromResult(bestServer);
    //    }

    //    public void ReleaseServer(ServerInstance server)
    //    {
    //        var target = _servers.FirstOrDefault(s => s.Id == server.Id);
    //        if (target != null && target.ActiveRequests > 0)
    //        {
    //            target.ActiveRequests--;
    //        }
    //    }

    //    public List<ServerInstance> GetServers() => _servers;
    //}






    using DistLoad.Interfaces;
    using DistLoad.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

namespace DistLoad.Services
{
    public class AdaptiveBalancer : ILoadBalancer
    {
        private readonly List<ServerInstance> _servers;
        private const double Epsilon = 0.001;

        public AdaptiveBalancer(List<ServerInstance> servers)
        {
            _servers = servers;
        }

        public Task<ServerInstance> GetNextServerAsync()
        {
            if (_servers.Count == 0)
                throw new Exception("No available servers");

            // Фільтруємо ті сервери, які он­лайн і не перевантажені
            var candidates = _servers
                .Where(s => s.IsOnline && s.IsAvailable && !s.IsOverloaded)
                .ToList();

            if (!candidates.Any())
                throw new Exception("No available servers");

            // Обчислюємо вагу Weight_i для кожного сервера:
            //    Weight_i = (C_i * M_i) / ( log(AvgRespTime_i + 1) + log(FailureRate_i + 1) )
            // (у нашому прикладі C_i та M_i ми просто беремо певні довільні або налагоджувальні значення,
            //  бо “хардкодимо” – наприклад, C_i = кількість віртуальних CPU, M_i = обсяг пам’яті. 
            //  Тут для прикладу просто встановлюємо C_i=M_i=1.0.)
            //
            // Обчислюємо Score(S_i) згідно:
            //    Score(S_i) = [ log(ActiveRequests_i + 1)
            //                 + log(CpuUsage_i + 1)
            //                 + log(RequestCount_i + 1)
            //                 + log(ResponseTime_i + 1) ]
            //                / ( Weight_i + ε )
            //
            // І беремо сервер із мінімальним Score.

            var scored = candidates.Select(s =>
            {
                // Даний простий приклад: вважатимемо, що кожен сервер має 1 віртуальне ядро і 1 ГБ пам’яті:
                double Ci = 1.0;
                double Mi = 1.0;

                // Для AvgResponseTime беремо зі Status (якщо немає – беремо заглушку 1 ms)
                double avgResp = (s.LastState?.ResponseTime ?? 1);
                // Для FailureRate беремо зі Status (якщо немає – 0.0)
                double failRate = (s.LastState?.FailureRate ?? 0.0);

                // Використовуємо формулу (2):
                double denom = Math.Log(avgResp + 1) + Math.Log(failRate + 1);
                double weight = (Ci * Mi) / (denom + Epsilon);

                // Обчислюємо Score (1):
                double ar = s.ActiveRequests;
                double cpu = s.CpuUsage;
                double total = s.RequestCount;
                double resp = (s.LastState?.ResponseTime ?? 1);

                double numerator =
                    Math.Log(ar + 1) +
                    Math.Log(cpu + 1) +
                    Math.Log(total + 1) +
                    Math.Log(resp + 1);

                double score = numerator / (weight + Epsilon);

                return new { Server = s, Score = score };
            })
            .OrderBy(x => x.Score)
            .ToList();

            var bestServer = scored.First().Server;

            // Як тільки обрали – збільшуємо лічильник активних запитів,
            // аби на наступному виклику вибір врахував це
            bestServer.ActiveRequests++;
            return Task.FromResult(bestServer);
        }

        public void ReleaseServer(ServerInstance server)
        {
            var target = _servers.FirstOrDefault(s => s.Id == server.Id);
            if (target != null && target.ActiveRequests > 0)
            {
                target.ActiveRequests--;
            }
        }

        public List<ServerInstance> GetServers() => _servers;
    }
}

