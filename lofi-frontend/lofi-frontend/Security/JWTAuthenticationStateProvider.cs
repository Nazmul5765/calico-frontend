using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using lofi_frontend.Services;

namespace lofi_frontend.Security;

public class JWTAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly AccessTokenService _accessTokenService;

    public JWTAuthenticationStateProvider(AccessTokenService accessTokenService)
    {
        _accessTokenService = accessTokenService;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _accessTokenService.GetToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                return await MarkAsUnAuthorised();
            }

            var readJwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var identity = new ClaimsIdentity(readJwt.Claims, "jwt");
            var principal = new ClaimsPrincipal(identity);
            
            return await Task.FromResult(new AuthenticationState(principal));
        }
        catch (Exception e)
        {
            return await MarkAsUnAuthorised();
        }
    }

    public void NotifyUserLogin()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void NotifyUserLogout()
    {
        NotifyAuthenticationStateChanged(MarkAsUnAuthorised());
    }

    private async Task<AuthenticationState> MarkAsUnAuthorised()
    {
        try
        {
            var state =  new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            NotifyAuthenticationStateChanged(Task.FromResult(state));

            return state;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }
}