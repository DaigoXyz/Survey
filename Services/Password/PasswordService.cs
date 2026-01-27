using Microsoft.AspNetCore.Identity;
using UserEntity = Survey.Entities.User;
using Survey.Services.Password;

namespace Survey.Services.Password
{
    public class PasswordService : IPasswordService
    {
        private readonly PasswordHasher<UserEntity> _hasher = new();

        public string Hash(string password, object userContext)
        {
            var user = userContext as UserEntity
                ?? throw new ArgumentException("Invalid user context");

            return _hasher.HashPassword(user, password);
        }

        public bool Verify(string hashedPassword, string password, object userContext)
        {
            var user = userContext as UserEntity
                ?? throw new ArgumentException("Invalid user context");

            var result = _hasher.VerifyHashedPassword(user, hashedPassword, password);

            return result == PasswordVerificationResult.Success
                || result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
