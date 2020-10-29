using System;

namespace GirafRest.Utilities
{
    public static class PictogramUtil
    {
        /// <summary>
        /// The wagner-fisher implementation of the levenshtein distance named funny by my peers (long story)
        /// </summary>
        /// <returns>The edit distance between the strings a and b.</returns>
        /// <param name="a">Search string.</param>
        /// <param name="b">string to be compared against the search string</param>
        public static int IbsenDistance(string a, string b)
        {
            const int insertionCost = 1;
            const int deletionCost = 100;
            const int substitutionCost = 100;
            int[,] d = new int[a.Length + 1, b.Length + 1];
            for (int i = 0; i <= a.Length; i++)
            for (int j = 0; j <= b.Length; j++)
                d[i, j] = 0;

            for (int i = 1; i <= a.Length; i++)
                d[i, 0] = i * deletionCost;

            for (int j = 1; j <= b.Length; j++)
                d[0, j] = j * insertionCost;

            for (int j = 1; j <= b.Length; j++)
            {
                for (int i = 1; i <= a.Length; i++)
                {
                    int _substitutionCost = 0;
                    if (a[i - 1] != b[j - 1])
                        _substitutionCost = substitutionCost;

                    d[i, j] = Math.Min(d[i - 1, j] + deletionCost,
                        Math.Min(d[i, j - 1] + insertionCost,
                            d[i - 1, j - 1] + _substitutionCost));
                }
            }
            return d[a.Length, b.Length];
        }
    }
}