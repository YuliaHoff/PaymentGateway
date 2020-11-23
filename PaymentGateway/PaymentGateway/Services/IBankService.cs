using PaymentGateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Services
{
    public interface IBankService
    {
        Task<BankResponse> MakePaymentInTheBank(Payment payment);
    }
}
