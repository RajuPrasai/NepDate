using System;
using System.Collections.Generic;

namespace NepDate
{
    /// <summary>
    /// Provides fuzzy matching for Nepali month names using sequence alignment algorithm
    /// (Needleman-Wunsch algorithm adapted for string matching)
    /// </summary>
    public static class NepaliMonthMatcher
    {
        // Standard month names (canonical forms) - much smaller dictionary
        private static readonly Dictionary<string, int> CanonicalMonthNames =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                { "baisakh", 1 }, 
                { "jestha", 2 }, 
                { "asar", 3 },{ "ashad", 3 }, 
                { "shrawan", 4 },{ "saun", 4 },
                { "bhadra", 5 },
                { "ashwin", 6 },{ "ashoj", 6 },{"aswayuja",6},
                { "kartik", 7 },
                { "mangsir", 8 },
                { "poush", 9 },
                { "magh", 10 },
                { "falgun", 11 },
                { "chaitra", 12 },
                { "बैशाख", 1 },
                { "जेष्ठ", 2 },{ "जेठ", 2 },
                { "असार", 3 },{ "आषाढ", 3 },
                { "श्रावण", 4 },{ "साउन", 4 },
                { "भाद्र", 5 },{ "भदौ", 5 },
                { "आश्विन", 6 },{ "असोज", 6 },
                { "कार्तिक", 7 },
                { "मंसिर", 8 },{ "मङ्गसिर", 8 },
                { "पौष", 9 },{"पुस",9},
                { "माघ", 10 },{"माग्ह",10},
                { "फाल्गुन", 11 },{"फाल्गुण",11},
                { "चैत्र", 12 },{ "चैत", 12 },
            };

        /// <summary>
        /// Finds the best matching Nepali month using sequence alignment algorithm
        /// </summary>
        /// <param name="input">The month name to match</param>
        /// <param name="threshold">Minimum similarity threshold (0.0 to 1.0)</param>
        /// <returns>The month number (1-12) if found, null otherwise</returns>
        public static int? FindBestMatch(string input, double threshold = 0.6)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;
            
            // Try exact match first for performance
            if (CanonicalMonthNames.TryGetValue(input, out int exactMatch))
                return exactMatch;

            double bestScore = 0;
            int? bestMatch = null;

            // Compare against all canonical forms using sequence alignment
            foreach (var kvp in CanonicalMonthNames)
            {
                string candidate = kvp.Key;
                double similarity = CalculateSequenceAlignment(input, candidate);

                if (similarity > bestScore && similarity >= threshold)
                {
                    bestScore = similarity;
                    bestMatch = kvp.Value;
                }
            }

            return bestMatch;
        }

        /// <summary>
        /// Calculates sequence alignment similarity between two strings using modified Needleman-Wunsch algorithm
        /// </summary>
        /// <param name="s1">First string</param>
        /// <param name="s2">Second string</param>
        /// <returns>Similarity score between 0.0 and 1.0</returns>
        private static double CalculateSequenceAlignment(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
                return 0;

            if (s1 == s2)
                return 1.0;

            int m = s1.Length;
            int n = s2.Length;

            int[,] dp = new int[m + 1, n + 1];

            const int MATCH_SCORE = 3;
            const int MISMATCH_PENALTY = -1;
            const int GAP_PENALTY = -1;

            for (int i = 0; i <= m; i++)
                dp[i, 0] = i * GAP_PENALTY;

            for (int j = 0; j <= n; j++)
                dp[0, j] = j * GAP_PENALTY;

            for (int i = 1; i <= m; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    char c1 = char.ToLower(s1[i - 1]);
                    char c2 = char.ToLower(s2[j - 1]);

                    int matchScore;
                    if (c1 == c2)
                    {
                        matchScore = MATCH_SCORE;
                    }
                    else
                    {
                        matchScore = MISMATCH_PENALTY;
                    }

                    int diagonal = dp[i - 1, j - 1] + matchScore; 
                    int deletion = dp[i - 1, j] + GAP_PENALTY; 
                    int insertion = dp[i, j - 1] + GAP_PENALTY;

                    dp[i, j] = Math.Max(diagonal, Math.Max(deletion, insertion));
                }
            }

            int alignmentScore = dp[m, n];
            int maxPossibleScore = Math.Max(m, n) * MATCH_SCORE;
            int minPossibleScore = Math.Max(m, n) * GAP_PENALTY;

            double normalizedScore =
                (double)(alignmentScore - minPossibleScore) / (maxPossibleScore - minPossibleScore);
            return Math.Max(0, Math.Min(1, normalizedScore));
        }
        
    }
}