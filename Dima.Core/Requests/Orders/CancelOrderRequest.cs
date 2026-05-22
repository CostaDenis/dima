using System.Security.AccessControl;

namespace Dima.Core.Requests.Orders;

public class CancelOrderRequest : BaseRequest
{
    public long Id { get; set; }
}