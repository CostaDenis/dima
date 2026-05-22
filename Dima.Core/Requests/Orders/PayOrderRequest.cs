namespace Dima.Core.Requests.Orders;

public class PayOrderRequest : BaseRequest
{
    public long Id { get; set; }
    public string ExternalReference { get; set; } = string.Empty;
}