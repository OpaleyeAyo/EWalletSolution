using System;

namespace EWallet.Utility.HelperMethods
{
    public static class GenerateAccountNumber
    {
        public static string GenerateNumber( )
        {
            Random random = new Random();

            return Convert.ToString((long)Math.Floor(random.NextDouble() * 9_000_000_000L + 1_000_000_000L));
        }
    }
}
