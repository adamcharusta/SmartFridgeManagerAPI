using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace SmartFridgeManagerAPI.IntegrationTests.Infrastructure;

public class FakePolicyEvaluator : IPolicyEvaluator
{
    public Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
    {
        ClaimsPrincipal claimsPrincipal = new();

        claimsPrincipal.AddIdentity(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, "1"), new Claim(ClaimTypes.Role, "Admin") }));

        AuthenticationTicket ticket = new(claimsPrincipal, "Test");
        AuthenticateResult result = AuthenticateResult.Success(ticket);
        return Task.FromResult(result);
    }

    public Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy,
        AuthenticateResult authenticationResult, HttpContext context, object? resource)
    {
        PolicyAuthorizationResult result = PolicyAuthorizationResult.Success();
        return Task.FromResult(result);
    }
}
