using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SmartHome.Enum;
using SmartHome.Application.Interfaces;
using SmartHome.Domain.Entities;
using SmartHome.Application.Interfaces.Settings;
using SmartHome.Dto.Settings;
using SmartHome.Dto.Email;

namespace SmartHome.Application.Services
{
    public class SeedDataService : ISeedDataService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITotpService _totpService;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IConfiguration _configuration;

        public SeedDataService(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            ITotpService totpService,
            ISettingsRepository settingsRepository)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
            _totpService = totpService;
            _settingsRepository = settingsRepository;
        }

        public async Task SeedDefaultSettingsAsync()
        {
            var currentSettings = await _settingsRepository.GetSettingsAsync(); // adjust to your method  

            var defaultSettings = new List<Setting>
                   {
                       new() { Key = "SystemTimeZone", Value = "UTC +02:00 - Egypt Standard Time" },
                       new() { Key = "GlobalTheme", Value = "Light" }
                   };

            foreach (var setting in defaultSettings)
            {
                if (!currentSettings.Any(s => s.Key == setting.Key))
                {
                    await _settingsRepository.SaveSettingsAsync(setting);
                }
            }
        }

        public async Task SeedDefaultEmailSettingsAsync()
        {
            var currentEmailSettings = await _settingsRepository.GetEmailSettingsAsync();
            if (currentEmailSettings == null)
            {
                var defaultEmailSettings = new EmailSetting(
                    smtpServer: "smtp.example.com",
                    port: 587,
                    senderEmail: "noreply@smarthome.com",
                    senderName: "SmartHome",
                    username: "smtp-username",
                    password: "smtp-password",
                    useSsl: true
                );

                await _settingsRepository.SaveEmailSettingsAsync(defaultEmailSettings);
            }
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            var roles = System.Enum.GetNames(typeof(Role)).ToList();

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
                            System.Enum.GetName(typeof(Role), Role.Admin)!);
                // Generate and store the secret
                string secretKey = _totpService.GenerateSecretKey();
                default_administrator.TOTPSecret = secretKey;
                await _userManager.UpdateAsync(default_administrator);
            }

            await SeedDefaultSettingsAsync();

            await SeedDefaultEmailSettingsAsync();
        }
    }
}
