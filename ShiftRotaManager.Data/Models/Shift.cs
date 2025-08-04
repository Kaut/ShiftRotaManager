namespace ShiftRotaManager.Data.Models
{
    public class Shift
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan Duration { get; set; } // Calculated or explicitly set
        public bool IsNightShift { get; set; }
        public int MinStaffRequired { get; set; }
        public int MaxStaffAllowed { get; set; }

        // Navigation property for variants
        public ICollection<ShiftVariant> Variants { get; set; } = new List<ShiftVariant>();
        // Navigation property for rotas
        public ICollection<Rota> Rotas { get; set; } = new List<Rota>();
    }
}