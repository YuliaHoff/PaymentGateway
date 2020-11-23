using PaymentGateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.Services
{
    public interface IUserService
    {        
        Task<string> Authenticate(string username, string password);
        Task<string> ValidateUserToken(string token);
        string GetUserIdFromToken(string token);
    }
}
