using System.Data.Common;
using gpos.Data;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace gpos.Services
{
    public class EmployeeAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmployeeAuthService> _logger;

        public EmployeeAuthService(ApplicationDbContext context, ILogger<EmployeeAuthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<EmployeeSignInResult> ValidateSalesmanAsync(string username, string password)
        {
            var normalizedUsername = username.Trim();

            try
            {
                var connection = _context.Database.GetDbConnection();

                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                await using var command = connection.CreateCommand();
                command.CommandText = """
                    SELECT id, name, username, password, role, status
                    FROM employees
                    WHERE username = @username
                    LIMIT 1;
                    """;

                var usernameParameter = command.CreateParameter();
                usernameParameter.ParameterName = "@username";
                usernameParameter.Value = normalizedUsername;
                command.Parameters.Add(usernameParameter);

                await using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    return EmployeeSignInResult.Failed("Invalid username or password.");
                }

                var employee = new EmployeeAccount(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetString(4),
                    reader.GetString(5));

                if (!employee.Status.Equals("active", StringComparison.OrdinalIgnoreCase))
                {
                    return EmployeeSignInResult.Failed("This account is inactive.");
                }

                if (!employee.Role.Equals("salesman", StringComparison.OrdinalIgnoreCase))
                {
                    return EmployeeSignInResult.Failed("This account is not allowed to access Salesman POS.");
                }

                if (!PasswordMatches(employee.PasswordHash, password))
                {
                    return EmployeeSignInResult.Failed("Invalid username or password.");
                }

                return EmployeeSignInResult.Success(employee);
            }
            catch (MySqlException ex)
            {
                _logger.LogError(ex, "Database connection error while signing in salesman {Username}.", normalizedUsername);
                return EmployeeSignInResult.Failed("Unable to connect to the employee database. Please try again later.");
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "Database error while signing in salesman {Username}.", normalizedUsername);
                return EmployeeSignInResult.Failed("Unable to connect to the employee database. Please try again later.");
            }
        }

        private static bool PasswordMatches(string storedPassword, string suppliedPassword)
        {
            if (string.IsNullOrWhiteSpace(storedPassword) || !storedPassword.StartsWith("$2", StringComparison.Ordinal))
            {
                return false;
            }

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
    }

    public sealed record EmployeeAccount(int Id, string Name, string Username, string PasswordHash, string Role, string Status);

    public sealed class EmployeeSignInResult
    {
        private EmployeeSignInResult(bool succeeded, string message, EmployeeAccount? employee)
        {
            Succeeded = succeeded;
            Message = message;
            Employee = employee;
        }

        public bool Succeeded { get; }
        public string Message { get; }
        public EmployeeAccount? Employee { get; }

        public static EmployeeSignInResult Success(EmployeeAccount employee) => new(true, string.Empty, employee);
        public static EmployeeSignInResult Failed(string message) => new(false, message, null);
    }
}
