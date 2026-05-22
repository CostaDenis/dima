using Dima.Core.Enums;

namespace Dima.Core.Models;

public class Order
{
    public long Id { get; set; }
    //N -> Guid sem traços
    //[..8] -> pega os 8 primeiros caracteres
    public string Number { get; set; } = Guid.NewGuid().ToString("N")[..8];
    
    public DateTime CreateAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    
    public EPaymentGateway Gateway { get; set; } = EPaymentGateway.Stripe;
    public string? ExternalReference { get; set; } //id gerado pelo gateway

    public EOrderStatus Status { get; set; } = EOrderStatus.WaitingPayment;
     
    public long ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public long? VoucherId { get; set; }
    public Voucher? Voucher { get; set; }
    
    public string UserId { get; set; } = string.Empty;
    //prop computada
    public decimal Total => Product.Price - (Voucher?.Amount ?? 0);
}