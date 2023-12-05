using Microsoft.Graph;
using Microsoft.Graph.Models;
using System;
using System.Threading.Tasks;

namespace Kissarekisteribackend.Graph

{
    class UserService
    {
        public static async Task<UserCollectionResponse> ListUsers(GraphServiceClient graphClient)
        {
            try
            {
                var users = await graphClient.Users.GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Select =
                    new string[] { "givenName", "surname" };
                });

                return users;
            }

            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return null;
            }
        }



        public static async Task<User> GetUserById(GraphServiceClient graphClient, string userId)
        {
            try
            {
                var user = await graphClient.Users[userId].GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Select =
                    new string[] { "givenName", "surname" };
                });

                return user;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return null;
            }
        }
    }

}
