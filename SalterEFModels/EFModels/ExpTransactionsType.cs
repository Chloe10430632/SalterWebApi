using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ExpTransactionsType
{
    public int Id { get; set; }

    public string? TransType { get; set; }

    public virtual ICollection<ExpTransaction> ExpTransactions { get; set; } = new List<ExpTransaction>();
}
