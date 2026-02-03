using Microsoft.AspNetCore.Identity;

namespace UserManagementApp.Services
{
    public class PasswordService
    {
        private readonly PasswordHasher<string> _hasher = new();

        public string Hash(string password)
            => _hasher.HashPassword(null!, password);

        public bool Verify(string hash, string password)
            => _hasher.VerifyHashedPassword(null!, hash, password)
               == PasswordVerificationResult.Success;
    }
}
