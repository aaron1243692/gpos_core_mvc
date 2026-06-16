using System.Data.Common;
using gpos.Data;
using gpos.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace gpos.Services
{
    public class UserAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserAuthService> _logger;

        public UserAuthService(ApplicationDbContext context, ILogger<UserAuthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<SignInResult> ValidateUserAsync(string login, string password)
        {
            User? user;
            var normalizedLogin = login.Trim();

            try
            {
                user = await _context.Users
                    .AsNoTracking()
                    .Include(account => account.UserRoles)
                        .ThenInclude(userRole => userRole.Role)
                    .FirstOrDefaultAsync(account =>
                        account.Username == normalizedLogin || account.Email == normalizedLogin);
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "Database connection error while signing in user with login {Login}.", normalizedLogin);
                return SignInResult.Failed("Unable to connect to the user database. Please try again later.");
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "Database error while signing in user with login {Login}.", normalizedLogin);
                return SignInResult.Failed("Unable to connect to the user database. Please try again later.");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Database query error while signing in user with login {Login}.", normalizedLogin);
                return SignInResult.Failed("Unable to connect to the user database. Please try again later.");
            }

            if (user is null)
            {
                return SignInResult.Failed("Invalid username/email or password.");
            }

            if (!PasswordMatches(user.PasswordHash, password))
            {
                return SignInResult.Failed("Invalid username/email or password.");
            }

            var roleCode = user.UserRoles
                .Select(userRole => userRole.Role?.Code)
                .FirstOrDefault(code => !string.IsNullOrWhiteSpace(code))
                ?? string.Empty;

            return SignInResult.Success(user, roleCode);
        }

        private static bool PasswordMatches(string storedPassword, string suppliedPassword)
        {
            if (string.IsNullOrWhiteSpace(storedPassword))
            {
                return false;
            }

            if (storedPassword.StartsWith("$2", StringComparison.Ordinal))
            {
                try
                {
                    var bcryptHash = storedPassword.StartsWith("$2y$", StringComparison.Ordinal)
                        ? "$2a$" + storedPassword[4..]
                        : storedPassword;

                    return BCrypt.Net.BCrypt.Verify(suppliedPassword, bcryptHash);
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }
    }

    public sealed class SignInResult
    {
        private SignInResult(bool succeeded, string message, User? user, string roleCode)
        {
            Succeeded = succeeded;
            Message = message;
            User = user;
            RoleCode = roleCode;
        }

        public bool Succeeded { get; }
        public string Message { get; }
        public User? User { get; }
        public string RoleCode { get; }

        public static SignInResult Success(User user, string roleCode) => new(true, string.Empty, user, roleCode);
        public static SignInResult Failed(string message) => new(false, message, null, string.Empty);
    }
}
