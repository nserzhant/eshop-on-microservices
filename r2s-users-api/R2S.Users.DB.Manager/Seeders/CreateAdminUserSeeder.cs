using R2S.Users.Core.Enums;
using R2S.Users.Core.Read;
using R2S.Users.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R2S.Users.DB.Manager.Seeders
{
    internal class CreateAdminUserSeeder
    {
        private readonly IUserService _userService = null;
        private readonly IUserQueryService _userQueryService = null;

        public CreateAdminUserSeeder(IUserService userService, IUserQueryService userQueryService)
        {
            _userService = userService;
            _userQueryService = userQueryService;
        }

        public async Task Seed(string adminUserEmail, string adminUserPassword)
        {
            try
            {
                var result = await _userService.Register(adminUserEmail, adminUserPassword);

                if (!result.Succeeded)
                {
                    Console.WriteLine("Failed to create user");
                    foreach (var error in result.Errors)
                        Console.WriteLine($"{error.Code}    {error.Description}");

                    return;
                }

                var user = await _userQueryService.GetByEmail(adminUserEmail);
                await _userService.SaveUserRoles(user.Id, Roles.Administrator);
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }
    }
}
