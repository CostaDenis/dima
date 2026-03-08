using System.ComponentModel.DataAnnotations;

namespace Dima.Core.Requests.Categories;

public class CreateCategoryRequest : BaseRequest
{
    [Required(ErrorMessage = "Título é obrigatório")]
    [MaxLength(80, ErrorMessage = "O título deve conter no maximo 80 caracteres")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Descrição é obrigatória")]
    public string Description { get; set; } = string.Empty;
}
