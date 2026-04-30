namespace NuovaPrevidenza.API.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Matricola { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal InstalmentAmount { get; set; }
        public int NumberOfInstalments { get; set; }
        public DateTime DecreeDate { get; set; }
        public string DecreeProtocol { get; set; } = string.Empty;
        public DateTime NotificationDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public DateTime ResultDate { get; set; }
        public string PlannedDecree { get; set; } = string.Empty;
        public string TrafficLight { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string WithholdingProtocol { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
