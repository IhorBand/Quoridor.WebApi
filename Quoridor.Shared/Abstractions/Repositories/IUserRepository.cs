using System;
using System.Threading.Tasks;
using Quoridor.Shared.DTO.DatabaseEntities;

namespace Quoridor.Shared.Abstractions.Repositories
{
    public interface IUserRepository
    {
        public User GetById(Guid id);
        public Task<User> GetByEmailAndPasswordAsync(string email, string password);
    }
}
