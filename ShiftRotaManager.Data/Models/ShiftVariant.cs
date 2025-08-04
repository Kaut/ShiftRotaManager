namespace ShiftRotaManager.Data.Models
{
    public class ShiftVariant
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;

        // Foreign key to the base Shift
        public Guid BaseShiftId { get; set; }
        public Shift BaseShift { get; set; } = null!; // Navigation property

        public TimeSpan StartTimeOffset { get; set; } // Offset from base shift's start time
        public TimeSpan EndTimeOffset { get; set; }   // Offset from base shift's end time
    }
}