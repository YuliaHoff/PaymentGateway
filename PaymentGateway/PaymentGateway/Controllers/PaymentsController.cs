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
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;
        private readonly PaymentGatewayContext _context;

        public PaymentsController(PaymentGatewayContext context, ILogger<PaymentsController> logger, IUserService userService, IPaymentService paymentService)
        {
            _context = context;
            _logger = logger;
            _paymentService = paymentService;
        }

        // GET: api/Payments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDTO>>> GetUserPayments()
        {
            try
            {
                _logger.LogDebug("GetUserPayments called from Merchant");

                var payments = await _paymentService.GetAllPayments();
                if (payments == null)
                {
                    _logger.LogWarning("GetUserPayments has no payments for authenticated user");
                    return NotFound();
                }

                return Ok(_paymentService.PaymentsToDTOs(payments));
            }
            catch (Exception ex)
            {
                _logger.LogError("GetUserPayments: exception occured: " + ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Payments
        [HttpGet("{ID}")]
        public async Task<ActionResult<PaymentDTO>> GetPaymentByID(string ID)
        {
            try
            {
                _logger.LogDebug("GetPaymentByID API called");
                var paymentDTO = await _paymentService.GetPaymentByID(ID);
                if (paymentDTO == null)
                {
                    _logger.LogWarning("GetPaymentByID has no payment with ID: " + ID + " for authenticated user");
                    return NotFound();
                }

                return Ok(paymentDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetPaymentByID: exception occured: " + ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/Payments
        [HttpPost]
        public async Task<ActionResult<Payment>> ProcessPayment(Payment payment)
        {
            try
            {
                _logger.LogDebug("ProcessPayment started");
                payment = await _paymentService.ProcessPayment(payment);
                if (payment == null)
                {
                    // Log error and return
                    _logger.LogError("An error occured in ProcessPayment func, payment could not be processed");
                    return StatusCode(503, "Unable to verify payment");
                }

                string isSucess = payment.IsPaymentSuccessful ? "successful" : "unseccessful";
                _logger.LogDebug("ProcessPayment: payment added to DB. Payment was " + isSucess);
                return CreatedAtAction(nameof(GetUserPayments), new { id = payment.ID }, _paymentService.PaymentToDTO(payment));
            }
            catch (Exception ex)
            {
                // Log error and return
                _logger.LogError("An error occured in ProcessPayment func: " + ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
    }

}
