using System;
using System.Collections.Generic;
using System.Text;

namespace Quoridor.Shared.DTO.InputModels.Token
{
    public class AuthorizeUserModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
