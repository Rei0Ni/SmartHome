using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Application.Interfaces
{
    public interface ITotpService
    {
        string GenerateSecretKey();
        string GenerateQrCodeUrl(string userName, string secretKey);
        bool ValidateTotpCode(string secretKey, string code);
        string GenerateTotpCode(string secretKey, long timeStep);
        long GetCurrentTimeStep();
    }
}
