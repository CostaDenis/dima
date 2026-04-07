using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace Dima.Web.Security;

public class CookieHandler : DelegatingHandler  //Interceptador de requisições http
{
    //Toda vez que é feito uma requisição via HttpClient dispara este método
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        //Atribuir as requisições do Browser às credenciais
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include); 
        request.Headers.Add("X-Requested-With", ["XMLHttpRequest"]);
        
        return base.SendAsync(request, cancellationToken);
    }
}