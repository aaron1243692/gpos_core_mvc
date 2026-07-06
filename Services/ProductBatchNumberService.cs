using gpos.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace gpos.Services
{
    public class ProductBatchNumberService
    {
        private readonly ApplicationDbContext _db;

        public ProductBatchNumberService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<string> GenerateNextBatchNoAsync()
        {
            var batchNumbers = await _db.ProductBatches
                .AsNoTracking()
                .Select(batch => batch.BatchNo)
                .ToListAsync();

            var highestNumber = 0;

            for (var i = 0; i < batchNumbers.Count; i += 1)
            {
                if (TryGetNumericBatchNumber(batchNumbers[i], out var batchNumber) && batchNumber > highestNumber)
                {
                    highestNumber = batchNumber;
                }
            }

            if (highestNumber >= 99999999)
            {
                throw new InvalidOperationException("The product batch number sequence has reached the 8-digit limit.");
            }

            return (highestNumber + 1).ToString("D8");
        }

        private static bool TryGetNumericBatchNumber(string? batchNo, out int batchNumber)
        {
            batchNumber = 0;
            var value = (batchNo ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            if (value.Length <= 8 && value.All(char.IsDigit))
            {
                return int.TryParse(value, out batchNumber);
            }

            var legacyMatch = Regex.Match(value, "^BATCH-(\\d+)$", RegexOptions.IgnoreCase);
            return legacyMatch.Success && int.TryParse(legacyMatch.Groups[1].Value, out batchNumber);
        }
    }
}
