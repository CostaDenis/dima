namespace Dima.Core.Requests.Stripe;

public class CreateSessionRequest : BaseRequest
{
    public string OrderNumber { get; set; } = string.Empty;
    public string ProductTitle { get; set; } = string.Empty;
    public string ProductDescription { get; set; } = string.Empty;
    public long OrderTotal { get; set; }
}