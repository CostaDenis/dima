using System.ComponentModel.DataAnnotations;
using Dima.Core.Enums;

namespace Dima.Core.Requests.Transactions;

public class UpdateTransactionRequest : BaseRequest
{

    public long Id { get; set; }

    [Required(ErrorMessage = "Insira o título")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Insira o tipo")]
    public ETransactionType Type { get; set; }

    [Required(ErrorMessage = "Insira o valor")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Insira a categoria")]
    public long CategoryId { get; set; }

    [Required(ErrorMessage = "Data inválida")]
    public DateTime? PaidOrReceivedAt { get; set; }
}
