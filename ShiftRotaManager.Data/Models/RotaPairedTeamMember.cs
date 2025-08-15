using System;

namespace ShiftRotaManager.Data.Models;

    public class RotaPairedTeamMember
    {
        public Guid RotaId { get; set; }
        public Rota Rota { get; set; } = null!;

        public Guid TeamMemberId { get; set; }
        public TeamMember TeamMember { get; set; } = null!;
    }
