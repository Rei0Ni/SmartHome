using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SmartHome.Application.Enums;
using SmartHome.Application.Interfaces;
using SmartHome.Domain.Entities;

namespace SmartHome.Application.Services
{
    public class SeedDataService : ISeedDataService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public SeedDataService(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration
            )
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            var roles = Enum.GetNames(typeof(Role)).ToList();

            foreach (var role in roles)
            {
                if (!await _roleManager!.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new ApplicationRole() { Name = role } );
                }
            }

            string email = "Administrator@smarthome.com";
            string password = _configuration["DefaultAdminPassword"] ?? "Administrator@2024";

            if (await _userManager!.FindByEmailAsync(email) == null)
            {
                ApplicationUser default_administrator = new()
                {
                    UserName = "Administrator",
                    Email = email,
                    EmailConfirmed = true,
                };
                await _userManager.CreateAsync(default_administrator, password);
                await _userManager.AddToRoleAsync(default_administrator,
                            Enum.GetName(typeof(Role), Role.Admin)!);
            }
        }
    }
}
