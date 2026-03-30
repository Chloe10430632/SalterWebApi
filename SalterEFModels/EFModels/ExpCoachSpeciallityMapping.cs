using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ExpCoachSpeciallityMapping
{
    public int CoachId { get; set; }

    public int SpecialitiesId { get; set; }

    public int Id { get; set; }

    public virtual ExpCoach Coach { get; set; } = null!;

    public virtual ExpSpeciality Specialities { get; set; } = null!;
}
