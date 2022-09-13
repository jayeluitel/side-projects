using System;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;

namespace AzureADAudit
{
    public class Program
    {
        //Most methods for this program were work-specific and have been ommitted from my public repo
        public static async Task Main()
        {
            var (app, scopes) = InitializeApp();
            List<User> allUsers = await GraphClient.GetAllUsersAsync(app, scopes);
            Dictionary<string, string> securityGroups = await GraphClient.GetAllSecurityGroupsAsync(app, scopes);
        }

        public static (IConfidentialClientApplication app, string[] scopes) InitializeApp()
        {
            try
            {
                var authConfig = new AuthenticationConfig
                {
                    //fill in through config settings
                    //Instance
                    //ApiUri
                    //Tenant
                    //ClientID
                    //ClientSecret
                    //ClientName
                };

                string authority = authConfig.Authority;
                IConfidentialClientApplication app =
                    ConfidentialClientApplicationBuilder.Create(authConfig.ClientID)
                    .WithClientName(authConfig.ClientName)
                    .WithClientSecret(authConfig.ClientSecret)
                    .WithAuthority(new Uri(authority))
                    .Build();

                app.AddInMemoryTokenCache();
                string[] scopes = new string[] { $"{authConfig.ApiUrl}.default" };

                return (app, scopes);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Program.InitiliazeApp method error: {ex.Message}");
                throw;
            }
        }
    }
}

