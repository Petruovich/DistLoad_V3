namespace DistLoad.Models
{
    public class ServerInstance
    {
        //public string Id { get; set; }
        //public string Address { get; set; } 
        //public int ActiveRequests { get; set; }
        //public bool IsOnline { get; set; } = true;
        //public bool IsAvailable { get; set; }
        //public int CpuUsage { get; set; } = 0;
        //public int RequestCount { get; set; } = 0;
        //public ServerState LastState { get; internal set; }

        public string Id { get; set; }

        // Адрес (URL) API этого сервера, откуда мы будем дергать /api/status
        public string Address { get; set; }

        // Число активных запросов (Real‐time метрика, обновляемая MetricsLogger-ом)
        public int ActiveRequests { get; set; } = 0;

        // Флаг «онлайн / доступен» (изначально true, но балансировщик может помечать false)
        public bool IsOnline { get; set; } = true;

        // Флаг «доступен ли для распределения» (можно выставлять в false, 
        // если, скажем, ActiveRequests > MaxActiveRequests или CpuUsage >= CpuCriticalThreshold)
        public bool IsAvailable { get; set; } = true;

        // Текущая загрузка CPU (%) – будет обновляться MetricsLogger-ом раз в 5 секунд
        public int CpuUsage { get; set; } = 0;

        // Общее число запросов, которое уже принял этот сервер (TotalRequests)
        public int RequestCount { get; set; } = 0;

        // Последнее состояние (полный ServerState), пришедшее из /api/status
        public ServerState LastState { get; internal set; }

        // Новые поля: порог «критического» CPU и максимальное «безопасное» число активных запросов
        // Можно делать эти значения настраиваемыми в конфиге, но для простоты пусть будут вот такими:
        public int CpuCriticalThreshold { get; set; } = 85;     // если CPU >= 85%, считаем near‐critical
        public int MaxActiveRequests { get; set; } = 50;     // если ActiveRequests > 50, near‐critical

        // Флаг «перегружен» (устанавливаем в MetricsLogger-е или в самом балансировщике)
        public bool IsOverloaded { get; set; } = false;
    }
}
