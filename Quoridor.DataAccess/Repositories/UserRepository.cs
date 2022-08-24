using Microsoft.Extensions.Logging;
using Quoridor.Shared.Abstractions.Repositories;
using Quoridor.Shared.Abstractions.Services;
using Quoridor.Shared.DTO.Configuration;
using Quoridor.Shared.DTO.DatabaseEntities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quoridor.DataAccess.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly ILogger<UserRepository> logger;

        public UserRepository(
            ConnectionStringConfiguration connectionStringConfiguration,
            ILogger<UserRepository> logger)
            : base(connectionStringConfiguration.Main)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public User GetById(Guid id)
        {
            var sql = @"
                        SELECT 
                            [PK_User] as Id, 
                            [DisplayName],
                            [UserName],
                            [Email],
                            [Password],
                            [CreatedDateUTC]
                        FROM [dbo].[T_User]
                        WHERE [PK_User] = @UserId";
            var user = this.QueryFirstOrDefault<User>(sql, new { UserId = id });
            return user;
        }

        public async Task<User> GetByEmailAndPasswordAsync(string email, string password)
        {
            var sql = @"SELECT 
                            [PK_User] as Id, 
                            [DisplayName],
                            [UserName],
                            [Email],
                            [Password],
                            [CreatedDateUTC]
                        FROM [dbo].[T_User]
                        WHERE [Email] = @Email AND [Password] = @Password";
            var user = await this.QueryFirstOrDefaultAsync<User>(sql, new { Email = email, Password = password });
            return user;
        }
    }
}
