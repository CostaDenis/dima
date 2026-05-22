namespace Dima.Core.Requests.Orders;

public class CreateOrderRequest : BaseRequest
{
    public long ProductId { get; set; }
    public long? VoucherId { get; set; }
}