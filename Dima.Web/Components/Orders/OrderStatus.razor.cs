using Dima.Core.Enums;
using Microsoft.AspNetCore.Components;

namespace Dima.Web.Components.Orders;

public partial class OrderStatusComponent : ComponentBase
{

    #region Parameters
    
    //EditorRequired -> Parametro obrigatorio
    [Parameter, EditorRequired] public EOrderStatus Status { get; set; }

    #endregion
}