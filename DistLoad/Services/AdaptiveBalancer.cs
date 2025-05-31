using DistLoad.Interfaces;
using DistLoad.Models;

namespace DistLoad.Services
{
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
    public class AdaptiveBalancer : ILoadBalancer
    {
        private readonly List<ServerInstance> _servers;

        public AdaptiveBalancer(List<ServerInstance> servers)
        {
            _servers = servers;
        }

        public Task<ServerInstance> GetNextServerAsync()
        {
            if (_servers.Count == 0)
                throw new Exception("No available servers");

            // Сортируем по минимальному ActiveRequests, затем по минимальному CpuUsage
            var bestServer = _servers
                .Where(s => s.IsAvailable)
                .OrderBy(s => s.ActiveRequests)
                .ThenBy(s => s.CpuUsage)
                .FirstOrDefault();

            if (bestServer == null)
                throw new Exception("No available servers");

            // Увеличиваем счётчик активных запросов, чтобы следующий выбор учёл это
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
