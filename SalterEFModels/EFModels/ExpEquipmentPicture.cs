using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ExpEquipmentPicture
{
    public int Id { get; set; }

    public int? ExpEquipmentId { get; set; }

    public string? EPhotoUrl { get; set; }

    public DateTime? UploadedAt { get; set; }

    public virtual ExpEquipment? ExpEquipment { get; set; }
}
