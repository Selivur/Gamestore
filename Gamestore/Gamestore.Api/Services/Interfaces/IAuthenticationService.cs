namespace Gamestore.Api.Services.Interfaces;

public interface IAuthenticationService
{
    Task<string> Authenticate(string username, string password);
    Task<HttpResponseMessage> ExecuteSecureAction(string token, string url);
}
