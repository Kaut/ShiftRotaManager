namespace ShiftRotaManager.Data.Models
{
    public class Rule
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public RuleType Type { get; set; } // e.g., MinStaff, NightShiftFollowedByEarly
        public string Parameters { get; set; } = string.Empty; // JSON string for rule-specific parameters
    }
    public enum RuleType
    {
        MinStaffPerShift,
        NoNightBeforeEarly,
        MaxConsecutiveShifts,
        MinRestBetweenShifts,
        MaxHoursPerWeek,
        SpecificTeamMemberExclusion,
        TrainingPairing
    }
}