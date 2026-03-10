using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ExpEquipment
{
    public int Id { get; set; }

    public int? CoachId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public decimal? Price { get; set; }

    public int? StockQuantity { get; set; }

    public bool? IsRental { get; set; }

    public DateTime? ListedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public byte Status { get; set; }

    public string? ReviewRemark { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public int? ReviewAdminId { get; set; }

    public virtual ExpCoach? Coach { get; set; }

    public virtual ICollection<ExpEquipmentOrder> ExpEquipmentOrders { get; set; } = new List<ExpEquipmentOrder>();

    public virtual ICollection<ExpEquipmentPicture> ExpEquipmentPictures { get; set; } = new List<ExpEquipmentPicture>();

    public virtual UserAdmin? ReviewAdmin { get; set; }
}
