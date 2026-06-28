using System;

namespace task01
{
    public static class StringExtensions
    {
        public static bool IsPalindrome(this string input)
        {
            if (input == null || input.Length == 0)
            {
                return false;
            }
            string lowerInput = input.ToLower();

            string cleaned = "";
            foreach (char c in lowerInput)
            {
                if (!char.IsPunctuation(c) && !char.IsWhiteSpace(c))
                {
                    cleaned += c;
                }
            }
            if (cleaned == "")
            {
                return false;
            }
            for (int i = 0; i < cleaned.Length / 2; i++)
            {
                if (cleaned[i] != cleaned[cleaned.Length - 1 - i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}

