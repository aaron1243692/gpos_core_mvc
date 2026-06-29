using System.Security.Cryptography;
using gpos.Data;
using Microsoft.EntityFrameworkCore;

namespace gpos.Services
{
    public class VoucherCodeService
    {
        private const int CodeLength = 6;
        private const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private readonly ApplicationDbContext _db;

        public VoucherCodeService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<string> GenerateUniqueVoucherRuleCodeAsync()
        {
            string code;
            do
            {
                code = GenerateCode();
            }
            while (await _db.VoucherRules.AnyAsync(rule => rule.Code == code));

            return code;
        }

        private static string GenerateCode()
        {
            return new string(Enumerable.Range(0, CodeLength)
                .Select(_ => Characters[RandomNumberGenerator.GetInt32(Characters.Length)])
                .ToArray());
        }
    }
}
