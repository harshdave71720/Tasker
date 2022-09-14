using System;
using System.Collections.Generic;
using System.Text;

namespace Tasker.Identity.Infrastructure.Configuration
{
    internal class JwtConfigurationOptions
    {
        public const string JwtConfiguration = "JwtConfiguration";

        public string Key { get; set; }

        public string Issuer { get; set; }

        public int ExpireMinutes { get; set; }

        public int ClockSkew { get; set; }
    }
}
