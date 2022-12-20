using IdentityModel.Client;

namespace Tamboliya.Services
{
    public interface ITokenService
    {
        Task<TokenResponse> GetToken(string scope);
    }

    public class TokenService : ITokenService
    {
        private DiscoveryDocumentResponse _discDocument { get; set; } = null!;

        public async Task<TokenResponse> GetToken(string scope)
        {
            using (var client = new HttpClient())
            {
                _discDocument = await client.GetDiscoveryDocumentAsync("https://localhost:7212/.well-known/openid-configuration");

                var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = _discDocument.TokenEndpoint,
                    ClientId = "cwm.client",
                    Scope = scope,
                    ClientSecret = "secret"
                });
                if (tokenResponse.IsError)
                {
                    throw new Exception("Token Error");
                }
                return tokenResponse;
            }
        }
    }
}