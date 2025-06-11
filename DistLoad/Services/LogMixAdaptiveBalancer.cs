using DistLoad.Models;

namespace DistLoad.Services
{
    public class LogMixAdaptiveBalancer
    {
        //    private readonly List<ServerInstance> _servers;
        //    private const double Epsilon = 0.001;

        //    public LogMixAdaptiveBalancer(List<ServerInstance> servers)
        //    {
        //        _servers = servers;
        //    }

        //    public Task<ServerInstance> GetNextServerAsync()
        //    {
        //        // 1) Фильтруем только доступные (онлайн && не перегружен)
        //        var candidates = _servers
        //            .Where(s => s.IsOnline && !IsOverloaded(s))
        //            .ToList();

        //        if (!candidates.Any())
        //            throw new Exception("No available servers");

        //        ServerInstance best = null;
        //        double bestScore = double.MaxValue;

        //        foreach (var s in candidates)
        //        {
        //            // Метрики берем напрямую из ServerInstance (хардкод)
        //            double activeReq = s.ActiveRequests;              // (например, 0…100)
        //            double cpuUsage = s.CpuUsage;                    // (0…100)
        //            double totalReq = s.RequestCount;                 // суммарные запросы
        //            double respTime = s.LastState?.ResponseTime ?? 0; // в мс
        //            double failureRate = s.LastState?.FailureRate ?? 0;  // 0.0…1.0

        //            // 2) Вычисляем weight:
        //            //    Cores_i * RAM_i / [ log(AvgResponseTime_i + 1) + log(FailureRate_i + 1) ]
        //            // Предполагаем, что Cores_i и RAM_i (Гб) хранятся в LastState:
        //            double cores = s.LastState?.CpuCores ?? 1;        
        //            double ram = s.LastState?.AvailableRAM ?? 1;    
        //            double avgResp = s.LastState?.AvgResponseTime ?? 0; 

        //            double denom = Math.Log(avgResp + 1.0) + Math.Log(failureRate + 1.0);
        //            if (denom <= 0) denom = Epsilon;
        //            double weight = (cores * ram) / denom;

        //            double num = Math.Log(activeReq + 1.0)
        //                       + Math.Log(cpuUsage + 1.0)
        //                       + Math.Log(totalReq + 1.0)
        //                       + Math.Log(respTime + 1.0);

        //            double score = num / (weight + Epsilon);

        //            if (score < bestScore)
        //            {
        //                bestScore = score;
        //                best = s;
        //            }
        //        }

        //        if (best == null)
        //            throw new Exception("No available servers");

        //        best.ActiveRequests++;

        //        return Task.FromResult(best);
        //    }

        //    private bool IsOverloaded(ServerInstance s)
        //    {
        //        if (s.CpuUsage >= s.CpuCriticalThreshold) return true;
        //        if (s.ActiveRequests > s.MaxActiveRequests) return true;
        //        return false;
        //    }

        //    public List<ServerInstance> GetServers() => _servers;

        //    public void ReleaseServer(ServerInstance server)
        //    {
        //        var target = _servers.FirstOrDefault(x => x.Id == server.Id);
        //        if (target != null && target.ActiveRequests > 0)
        //        {
        //            target.ActiveRequests--;
        //        }
        //    }
    }
}
