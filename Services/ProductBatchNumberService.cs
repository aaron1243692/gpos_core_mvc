using gpos.Data;
using Microsoft.EntityFrameworkCore;

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
            var lastBatchNo = await _db.ProductBatches
                .AsNoTracking()
                .Where(batch => batch.BatchNo.StartsWith("BATCH-"))
                .OrderByDescending(batch => batch.Id)
                .Select(batch => batch.BatchNo)
                .FirstOrDefaultAsync();

            var nextNumber = 1;

            if (!string.IsNullOrWhiteSpace(lastBatchNo)
                && int.TryParse(lastBatchNo.Replace("BATCH-", string.Empty), out var currentNumber))
            {
                nextNumber = currentNumber + 1;
            }

            return $"BATCH-{nextNumber:000000}";
        }
    }
}
