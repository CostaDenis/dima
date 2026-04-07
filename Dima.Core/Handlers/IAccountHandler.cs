using Dima.Core.Requests.Account;
using Dima.Core.Responses;

namespace Dima.Core.Handlers;

public interface IAccountHandler
{
    Task<Response<string>> LoginAsyc(LoginRequest request);
    Task<Response<string>> RegisterAsyc(RegisterRequest request);
    Task LogoutAsync();
}