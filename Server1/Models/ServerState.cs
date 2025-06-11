namespace Server1.Models
{
        public class ServerState
        {
        public int CpuUsage { get; set; }

        // Поточна кількість активних запитів.
        public int ActiveRequests { get; set; }

        // Загальна кількість усіх оброблених запитів.
        public int TotalRequests { get; set; }

        // Середній час відповіді сервера (у мс).
        public int ResponseTime { get; set; }

        // Частка невдалих запитів (0.0–1.0).
        public double FailureRate { get; set; }

        // Чи доступний сервер (true/false).
        public bool IsAvailable { get; set; }
    }
    }