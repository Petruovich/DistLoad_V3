namespace DistLoad.Models
{
    public class ServerState
    {
        //internal int ResponseTime;

        //public bool IsAvailable { get; set; } = true;
        //public int ActiveRequests { get; set; } = 0;
        //public double CpuUsage { get; set; } = 0;
        public int CpuUsage { get; set; }
        public int ActiveRequests { get; set; }
        public int TotalRequests { get; set; }
        public bool IsAvailable { get; set; }
        public int ResponseTime { get; set; }
        public double FailureRate { get; set; }
    }
}
