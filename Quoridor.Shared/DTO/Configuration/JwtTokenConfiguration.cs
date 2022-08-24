using System;
using System.Collections.Generic;
using System.Text;

namespace Quoridor.Shared.DTO.Configuration
{
    public class JwtTokenConfiguration
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Subject { get; set; }
    }
}
