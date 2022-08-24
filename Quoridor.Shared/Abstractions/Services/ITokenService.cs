using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quoridor.Shared.Abstractions.Services
{
    public interface ITokenService
    {
        public Task<string> GetTokenAsync(string email, string password);
    }
}
