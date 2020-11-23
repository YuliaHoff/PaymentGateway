using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaymentGateway.Models;
using PaymentGateway.Services;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace PaymentGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private readonly ILogger<PaymentsController> _logger;
        private readonly PaymentGatewayContext _context;

        public UsersController(PaymentGatewayContext context, ILogger<PaymentsController> logger, IUserService userService)
        {
            _context = context;
            _logger = logger;
            _userService = userService;
        }

        // POST: api/Authenticate
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UserAuthentication user)
        {
            try
            {
                var token = await _userService.Authenticate(user.UserName, user.Password);
                if (token == null)
                    return BadRequest(new { message = "Username or password is incorrect" });

                _logger.LogDebug("Authenticate: user sucessfully authenticated");
                return Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Authenticate: exception occured: " + ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
