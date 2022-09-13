using System;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Group = Microsoft.Graph.Group;

namespace AzureADAudit
{
    public class GraphClient
    {
        public static GraphServiceClient GetAuthenticatedGraphClient(IConfidentialClientApplication app, string[] scopes)
        {
            try
            {
                GraphServiceClient graphServiceClient =
                    new($"https://graph.microsoft.com/V1.0", new DelegateAuthenticationProvider(async (requestMessage) =>
                    {
                        AuthenticationResult result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                    }));
                return graphServiceClient;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"GraphClient.GetAuthenticatedGraphClient method error: {ex.Message}");
                throw;
            }
        }

        public static async Task<List<User>> GetAllUsersAsync(IConfidentialClientApplication app, string[] scopes)
        {
            try
            {
                List<User> users = new();
                GraphServiceClient graphServiceClient = GetAuthenticatedGraphClient(app, scopes);
                var azureUsers = await graphServiceClient.Users
                    .Request()
                    .Select("accountEnabled,userPrincipalName,displayName,id,city")
                    .GetAsync();

                users.AddRange(azureUsers.CurrentPage);
                while(azureUsers.NextPageRequest != null)
                {
                    azureUsers = await azureUsers.NextPageRequest.GetAsync();
                    users.AddRange(azureUsers.CurrentPage);
                }
                return users;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"GraphClient.GetAllUsersAsync method error: {ex.Message}");
                throw;
            }
        }


        //These methods were specific to a project for work and have been ommitted from my public repo

        //public static async Task FixCorruptedUserAsync(List<User> corruptedUsers, IConfidentialClientApplication app, string[] scopes)
        //{
        //    try
        //    {
        //        

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"GraphClient.FixCorruptedUserAsync method error: {ex.Message}");
        //        throw;
        //    }
        //}

        //

        //public static async Task AddFieldToUserAsync(IConfidentialClientApplication app, string[] scopes, User user, string field)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"GraphClient.AddFieldToUserAsync method error: {ex.Message}");
        //        throw;
        //    }
        //}

        public static async Task<Dictionary<string, string>> GetAllSecurityGroupsAsync(IConfidentialClientApplication app, string[] scopes)
        {
            try
            {
                GraphServiceClient graphServiceClient = GetAuthenticatedGraphClient(app, scopes);
                List<Group> allGroups = new();
                Dictionary<string, string> secGroupIds = new();

                var groups = await graphServiceClient.Groups
                    .Request()
                    .GetAsync();

                allGroups.AddRange(groups.CurrentPage);
                while(groups.NextPageRequest != null)
                {
                    groups = await groups.NextPageRequest.GetAsync();
                    allGroups.AddRange(groups.CurrentPage);
                }

                foreach(var group in allGroups)
                {
                    secGroupIds.Add(group.DisplayName, group.Id);
                }
                return secGroupIds;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GraphClient.GetAllSecurityGroupsAsync method error: {ex.Message}");
                throw;
            }
        }

        public static async Task AddUserToSecGroupAsync(IConfidentialClientApplication app, string[] scopes, User user, string secGroup)
        {
            try
            {
                GraphServiceClient graphServiceClient = GetAuthenticatedGraphClient(app, scopes);
                await graphServiceClient.Groups[secGroup].Members.References.Request().AddAsync(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GraphClient.AddUserToSecGroupAsync method error: {ex.Message}");
                throw;
            }
        }


        public static async Task<IUserMemberOfCollectionWithReferencesPage> GetUserSecGroupsAsync(IConfidentialClientApplication app, string[] scopes, User user)
        {
            try
            {
                GraphServiceClient graphServiceClient = GetAuthenticatedGraphClient(app, scopes);
                var secGroups = await graphServiceClient.Users[user.Id].MemberOf.Request().GetAsync();
                return secGroups;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GraphClient.GetUserSecGroupsAsync method error: {ex.Message}");
                throw;
            }
        }

        public static async Task<User> CreateUser(IConfidentialClientApplication app, string[] scopes, string user, string tenant)
        {
            try
            {
                string userPrincipalName = user[..user.IndexOf('@')];
                string password = GeneratePassword();

                if (password == "") password = GeneratePassword();

                GraphServiceClient graphServiceClient = GetAuthenticatedGraphClient(app, scopes);
                var newUser = new User
                {
                    AccountEnabled = true,
                    DisplayName = user,
                    MailNickname = userPrincipalName,
                    UserPrincipalName = $"{userPrincipalName}@{tenant}.com",
                    PasswordProfile = new PasswordProfile
                    {
                        ForceChangePasswordNextSignIn = true,
                        Password = password
                    }
                };

                return await graphServiceClient.Users.Request().AddAsync(newUser);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"GraphClient.CreateUser method error: {ex.Message}");
                throw;
            }
        }

        public static string GeneratePassword()
        {
            try
            {
                string lowercase = $"abcdefghijklmnopqrstuvwxyz";
                string uppercase = $"ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                string numeric = $"0123456789";
                string special = $"!@#$%^&*()";

                string characterSet = lowercase + uppercase + numeric + special;
                char[] password = new char[8];
                Random random = new();

                for(int i = 0; i < password.Length; i++)
                {
                    password[i] = characterSet[random.Next(characterSet.Length - 1)];
                    bool identicalChars = i > 2 && password[i] == password[i - 1] && password[i - 1] == password[i - 2];
                    if (identicalChars) i--;
                }
                string validPass = new(password);
                return ValidatePassword(validPass) ? validPass : "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GraphClient.GeneratePassword method error: {ex.Message}");
                throw;
            }
        }

        public static bool ValidatePassword(string password)
        {
            try
            {
                const string REGEX_LOWERCASE = @"[a-z]";
                const string REGEX_UPPERCASE = @"[A-Z]";
                const string REGEX_NUMERIC = @"[\d]";
                const string REGEX_SPECIAL = @"!@#$%^&*()";

                bool lowercase = Regex.IsMatch(password, REGEX_LOWERCASE);
                bool uppercase = Regex.IsMatch(password, REGEX_UPPERCASE);
                bool numeric = Regex.IsMatch(password, REGEX_NUMERIC);
                bool special = Regex.IsMatch(password, REGEX_SPECIAL);

                return lowercase && uppercase && numeric && special;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GraphClient.ValidatePassword method error: {ex.Message}");
                throw;
            }
        }
    }
}

