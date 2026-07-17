namespace gpos.Services
{
    // POS policy: round every persisted monetary result to cents, away from zero,
    // then build header totals by summing those rounded persisted line values.
    public static class MoneyRounding
    {
        public static decimal Round(decimal value) => decimal.Round(value, 2, MidpointRounding.AwayFromZero);
        public static decimal Multiply(decimal quantity, decimal unitAmount) => Round(quantity * unitAmount);
    }
}
