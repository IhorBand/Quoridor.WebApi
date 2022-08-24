using System;
using System.Collections.Generic;
using System.Text;

namespace Quoridor.Shared.DTO.DatabaseEntities
{
    public class User
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreatedDateUTC { get; set; }
    }
}
