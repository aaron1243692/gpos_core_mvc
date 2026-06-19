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

                SalesmanEmployeeAccount employee;
                await using (var command = connection.CreateCommand())
                {
                    command.CommandText = """
                        SELECT id, full_name, username, password_hash, status
                        FROM employee_account
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

                    employee = new SalesmanEmployeeAccount(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetString(3),
                        reader.GetInt32(4));
                }

                if (employee.Status != 1)
                {
                    return EmployeeSignInResult.Failed("This account is inactive.");
                }

                if (!PasswordMatches(employee.PasswordHash, password))
                {
                    return EmployeeSignInResult.Failed("Invalid username or password.");
                }

                if (!await IsEmployeeScheduledNowAsync(employee.Id))
                {
                    return EmployeeSignInResult.Failed("Your account is not scheduled for this time.");
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

        private async Task<bool> IsEmployeeScheduledNowAsync(int employeeAccountId)
        {
            var now = DateTime.Now;
            var currentDay = ToScheduleDay(now.DayOfWeek);
            var previousDay = currentDay == 1 ? 7 : currentDay - 1;
            var currentTime = now.TimeOfDay;

            var schedules = await _context.EmployeeShiftSchedules.AsNoTracking()
                .Where(schedule => schedule.EmployeeAccountId == employeeAccountId
                    && schedule.Status == 1
                    && (schedule.DayOfWeek == currentDay || schedule.DayOfWeek == previousDay))
                .ToListAsync();

            return schedules.Any(schedule => IsScheduleActiveNow(schedule.DayOfWeek, schedule.StartTime, schedule.EndTime, currentDay, previousDay, currentTime));
        }

        private static bool IsScheduleActiveNow(int scheduleDay, TimeSpan startTime, TimeSpan endTime, int currentDay, int previousDay, TimeSpan currentTime)
        {
            if (startTime < endTime)
            {
                return scheduleDay == currentDay && currentTime >= startTime && currentTime <= endTime;
            }

            if (startTime > endTime)
            {
                return scheduleDay == currentDay && currentTime >= startTime
                    || scheduleDay == previousDay && currentTime <= endTime;
            }

            return scheduleDay == currentDay;
        }

        private static int ToScheduleDay(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Monday => 1,
                DayOfWeek.Tuesday => 2,
                DayOfWeek.Wednesday => 3,
                DayOfWeek.Thursday => 4,
                DayOfWeek.Friday => 5,
                DayOfWeek.Saturday => 6,
                DayOfWeek.Sunday => 7,
                _ => 0
            };
        }
    }

    public sealed record SalesmanEmployeeAccount(int Id, string Name, string Username, string PasswordHash, int Status);

    public sealed class EmployeeSignInResult
    {
        private EmployeeSignInResult(bool succeeded, string message, SalesmanEmployeeAccount? employee)
        {
            Succeeded = succeeded;
            Message = message;
            Employee = employee;
        }

        public bool Succeeded { get; }
        public string Message { get; }
        public SalesmanEmployeeAccount? Employee { get; }

        public static EmployeeSignInResult Success(SalesmanEmployeeAccount employee) => new(true, string.Empty, employee);
        public static EmployeeSignInResult Failed(string message) => new(false, message, null);
    }
}
