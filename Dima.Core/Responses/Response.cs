using System.Text.Json.Serialization;

namespace Dima.Core.Responses;

public class Response<TData>
{
    [JsonConstructor]  //Fala para o JSON usar esse construtor
    public Response()
    {
        _code = Configuration.DefaultCode;
    }

    public Response(TData? data, int code = Configuration.DefaultCode, string? message = null)
    {
        Data = data;
        Message = message;
        _code = code;
    }

    private readonly int _code;
    public TData? Data { get; set; }
    public string? Message { get; set; }

    [JsonIgnore]  //Não exibe na tela
    public bool IsSuccess => _code is >= 200 and <= 299;
}
