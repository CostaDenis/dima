namespace Dima.Core.Requests.Orders;

public class GetVoucherByNumberRequest : PagedRequest
{
    public string Number { get; set; } = string.Empty;
}