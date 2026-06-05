namespace Dima.Core.Requests.Stripe;

public class GetTransactionsByOrderNumberRequest : BaseRequest
{
    public string Number { get; set; } = string.Empty;
}