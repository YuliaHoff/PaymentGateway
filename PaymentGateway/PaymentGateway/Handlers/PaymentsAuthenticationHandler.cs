using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentGateway.Models;
using PaymentGateway.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace PaymentGateway.Handlers
{
    public class PaymentsAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUserService _userService;

        public PaymentsAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserService userService)
            : base(options, logger, encoder, clock)
        {
            _userService = userService;
        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // skip authentication if endpoint has [AllowAnonymous] attribute
            var endpoint = Context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
                return AuthenticateResult.NoResult();

            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");

            string token;
            string userID;
            try
            {
                token = Request.Headers["Authorization"].ToString();               
                userID = await _userService.ValidateUserToken(token);
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            if (userID == null)
                return AuthenticateResult.Fail("Invalid token. Please authenticate again");

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, userID),
                new Claim(ClaimTypes.Name, token),
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}