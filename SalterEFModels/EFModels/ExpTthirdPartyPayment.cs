using System;
using System.Collections.Generic;

namespace SalterEFModels.EFModels;

public partial class ExpTthirdPartyPayment
{
    public int Id { get; set; }

    public int? TransactionId { get; set; }

    public string? TradeNo { get; set; }

    public string? MerchantTradeNo { get; set; }

    public DateTime? MerchantTradeDate { get; set; }

    public int? TradeAmt { get; set; }

    public string? TradeDesc { get; set; }

    public string? ItemName { get; set; }

    public int? RtnCode { get; set; }

    public string? RtnMsg { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? PaymentType { get; set; }

    public int? SimulatePaid { get; set; }

    public string? CustomField1 { get; set; }

    public virtual ExpTransaction? Transaction { get; set; }
}
