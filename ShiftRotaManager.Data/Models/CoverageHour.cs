namespace ShiftRotaManager.Data.Models
{
    public class CoverageHour
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int RequiredStaffCount { get; set; }
    }
}