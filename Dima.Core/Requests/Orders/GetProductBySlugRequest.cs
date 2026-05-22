namespace Dima.Core.Requests.Orders;

public class GetProductBySlugRequest : BaseRequest
{
    public string Slug { get; set; } = string.Empty;
}