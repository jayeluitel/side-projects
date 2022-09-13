using System;
using System.Globalization;

namespace AzureADAudit
{
    public class AuthenticationConfig
    {
        public string? Instance { get; set; }
        public string? ApiUrl { get; set; }
        public string? Tenant { get; set; }
        public string? ClientID { get; set; }
        public string? Authority
        {
            get
            {
                return String.Format(CultureInfo.InvariantCulture, Instance, Tenant);
            }
        }

        public string? ClientSecret { get; set; }
        public string? ClientName { get; set; }
    }
}

