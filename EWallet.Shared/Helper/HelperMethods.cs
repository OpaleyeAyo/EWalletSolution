using System;

namespace EWallet.Utility.Helper
{
    public static class HelperMethods
    {
        public static string GenerateRandom(int size)
        {
            Random rand = new Random();

            String str = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            String randomstring = "";

            for (int i = 0; i < size; i++)
            {
                int x = rand.Next(str.Length);

                randomstring = randomstring + str[x];
            }

            return randomstring;
        }
    }
}
