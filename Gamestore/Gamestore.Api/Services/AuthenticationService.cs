using System.Net.Http.Headers;
using System.Text;
using Gamestore.Api.Services.Interfaces;
using Newtonsoft.Json;

namespace Gamestore.Api.Services;

public class AuthenticationService : IAuthenticationService, IDisposable
{
    private readonly HttpClient _client;

    public AuthenticationService()
    {
        _client = new HttpClient();
    }

    public async Task<string> Authenticate(string username, string password)
    {
        var uri = "https://localhost:5037/api/authenticate"; 
        var data = new { Username = username, Password = password }; 

        var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(uri, content);

        if (response.IsSuccessStatusCode)
        {
            var token = await response.Content.ReadAsStringAsync();
            return token;
        }

        return null;
    }

    public async Task<HttpResponseMessage> ExecuteSecureAction(string token, string url)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync(url);

        return response;
    }

    public void Dispose()
    {
        _client.Dispose();
        GC.SuppressFinalize(this);
    }
}
