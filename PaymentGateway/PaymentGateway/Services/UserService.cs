using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using PaymentGateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Services
{
    public class UserService : IUserService
    {
        private readonly PaymentGatewayContext _context;
        private static Dictionary<string, string> _authenticatedUsers = new Dictionary<string, string>();

        public UserService(PaymentGatewayContext context)
        {
            _context = context;
        }

        public async Task<string> Authenticate(string username, string password)
        {
            var user = await Task.Run(() => _context.Users.SingleOrDefault(x => x.UserName == username));
            if (user == null)
                return null;

            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(user.Salt),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // return null if user and password don't match
            if (user.HashedPassword != hashed)
                return null;

            var token = user.ID + " - " + DateTime.Now + " - " + new Random().Next();

            if (!_authenticatedUsers.ContainsKey(user.ID))
                _authenticatedUsers.Add(user.ID, token);

            // authentication successful so return token
            return token;
        }

        public string GetUserIdFromToken(string token)
        {
            return token.Split(" - ")[0];
        }

        public async Task<string> ValidateUserToken(string token) =>
             await Task.Run(() => _authenticatedUsers.Values.Where(t => t == token).SingleOrDefault());
        
    }
}
