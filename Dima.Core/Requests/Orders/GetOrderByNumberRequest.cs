namespace Dima.Core.Requests.Orders;

public class GetOrderByNumberRequest : BaseRequest
{
    public string Number { get; set; } = string.Empty;
}