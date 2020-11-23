using PaymentGateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Services
{
    public interface IPaymentService
    {
        Task<IEnumerable<Payment>> GetAllPayments();
        Task<PaymentDTO> GetPaymentByID(string paymentID);
        //string MaskCardNumber(string cardNumber);
        PaymentDTO PaymentToDTO(Payment payment);
        List<PaymentDTO> PaymentsToDTOs(IEnumerable<Payment> payments);
        Task<Payment> ProcessPayment(Payment payment);
    }
}
