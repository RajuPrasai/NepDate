using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NepDate
{
    /// <summary>
    /// Provides intelligent date parsing for Nepali dates with support for
    /// various formats, fuzzy matching, and ambiguity resolution.
    /// </summary>
    public static class SmartDateParser
    {
        // Common date separators (same as original)
        private static readonly char[] DateSeparators = { '/', '-', '.', ' ', ',', '_', '|', '।' };

        // Nepali unicode digit mappings (same as original)
        private static readonly Dictionary<char, char> NepaliToEnglishDigits = new Dictionary<char, char>
        {
            { '०', '0' }, { '१', '1' }, { '२', '2' }, { '३', '3' }, { '४', '4' },
            { '५', '5' }, { '६', '6' }, { '७', '7' }, { '८', '8' }, { '९', '9' }
        };

        /// <summary>
        /// Parses a string representation of a Nepali date in various formats and returns a NepaliDate.
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <returns>A NepaliDate representing the parsed date.</returns>
        /// <exception cref="FormatException">Thrown when the input string cannot be parsed as a Nepali date.</exception>
        public static NepaliDate Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentNullException(nameof(input), "Input string cannot be null or empty");

            // Try standard parse first for performance
            if (NepaliDate.TryParse(input, out var result))
                return result;

            // Normalize the input
            string normalizedInput = NormalizeInput(input);

            // Try various parsing strategies
            if (TryParseStandardFormat(normalizedInput, out result))
                return result;

            if (TryParseNepaliUnicodeFormat(normalizedInput, out result))
                return result;

            if (TryParseMonthNameFormat(normalizedInput, out result))
                return result;

            if (TryParseAmbiguousFormat(normalizedInput, out result))
                return result;

            // If all parsing strategies fail, throw an exception
            throw new FormatException($"Could not parse '{input}' as a Nepali date");
        }

        /// <summary>
        /// Tries to parse a string representation of a Nepali date in various formats.
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <param name="result">When this method returns, contains the parsed NepaliDate if successful, or default if not.</param>
        /// <returns>true if the parsing was successful; otherwise, false.</returns>
        public static bool TryParse(string input, out NepaliDate result)
        {
            result = default;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            try
            {
                result = Parse(input);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Normalizes the input string by trimming, replacing separators, and handling common aliases.
        /// </summary>
        private static string NormalizeInput(string input)
        {
            string result = input.Trim();

            // Remove 'BS', 'B.S.', 'VS', 'V.S.' indicators
            result = Regex.Replace(result, @"\b(?:B\.?S\.?|V\.?S\.?)\b", string.Empty, RegexOptions.IgnoreCase);

            // Replace multiple spaces with a single space
            result = Regex.Replace(result, @"\s+", " ");

            // Replace 'gate', 'miti', etc.
            result = Regex.Replace(result, @"\b(?:गते|मिति)\b", string.Empty, RegexOptions.IgnoreCase);

            return result.Trim();
        }

        /// <summary>
        /// Tries to parse a string in standard numeric formats like YYYY/MM/DD, DD/MM/YYYY, etc.
        /// </summary>
        private static bool TryParseStandardFormat(string input, out NepaliDate result)
        {
            result = default;

            // Try different separator-based formats
            foreach (char separator in DateSeparators)
            {
                string[] parts = input.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 3)
                {
                    // Try YYYY/MM/DD
                    if (TryParseYearMonthDay(parts[0], parts[1], parts[2], out result))
                        return true;

                    // Try DD/MM/YYYY
                    if (TryParseYearMonthDay(parts[2], parts[1], parts[0], out result))
                        return true;

                    // Try MM/DD/YYYY
                    if (TryParseYearMonthDay(parts[2], parts[0], parts[1], out result))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tries to parse a string containing month names like "15 Jestha 2080" or "Jestha 15, 2080".
        /// Now uses sequence alignment for fuzzy month matching.
        /// </summary>
        private static bool TryParseMonthNameFormat(string input, out NepaliDate result)
        {
            result = default;

            // Extract potential month names (words with at least 3 characters)
            var words = input.Split(new[] { ' ', ',', '-', '.', '/', '\\' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length >= 3 && !IsNumeric(w))
                .ToList();

            foreach (string word in words)
            {
                // Use fuzzy matching instead of dictionary lookup
                int? monthNumber = NepaliMonthMatcher.FindBestMatch(word, 0.4);

                if (monthNumber.HasValue)
                {
                    // Extract numbers from the input for year and day
                    var numbers = System.Text.RegularExpressions.Regex.Matches(input, @"\d+")
                        .Cast<System.Text.RegularExpressions.Match>()
                        .Select(m => int.Parse(m.Value))
                        .ToList();

                    if (numbers.Count >= 2)
                    {
                        // Determine which number is the year based on magnitude
                        int year, day;
                        if (numbers[0] > 1900) // Likely a year
                        {
                            year = numbers[0];
                            day = numbers.Count > 1 ? numbers[1] : 1;
                        }
                        else if (numbers.Count > 1 && numbers[1] > 1900) // Second number is a year
                        {
                            year = numbers[1];
                            day = numbers[0];
                        }
                        else // No obvious year, try heuristic
                        {
                            // Sort numbers by size, largest is likely year
                            var sortedNumbers = numbers.OrderByDescending(n => n).ToList();
                            year = sortedNumbers[0];
                            day = sortedNumbers.Count > 1 ? sortedNumbers[1] : 1;

                            // If largest number is too small to be a BS year, fallback
                            if (year < 1900)
                            {
                                // Add 2000 to years likely expressed in 2-digit short form (e.g., '80 for 2080)
                                if (year >= 0 && year < 100)
                                    year += 2000;
                                else if (year >= 100 && year < 999)
                                    year += 1000; // Convert 3-digit year like 080 to 1080 or 080 to 2080
                            }
                        }

                        // Validate and sanitize day
                        if (day < 1 || day > 32)
                            continue;

                        // Try to create valid date
                        try
                        {
                            result = new NepaliDate(year, monthNumber.Value, day);
                            return true;
                        }
                        catch
                        {
                            // Continue to next attempt
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Tries to parse a string containing Nepali unicode digits and month names.
        /// </summary>
        private static bool TryParseNepaliUnicodeFormat(string input, out NepaliDate result)
        {
            result = default;

            // Convert Nepali digits to English digits
            string convertedInput = ConvertNepaliDigitsToEnglish(input);

            // If the conversion made a difference, try parsing again
            if (convertedInput != input && TryParse(convertedInput, out result))
                return true;

            return false;
        }

        /// <summary>
        /// Tries to parse ambiguous date formats by making educated guesses about the intended date.
        /// </summary>
        private static bool TryParseAmbiguousFormat(string input, out NepaliDate result)
        {
            result = default;

            // Extract all numbers from the input
            var numbers = Regex.Matches(input, @"\d+")
                .Cast<Match>()
                .Select(m => int.Parse(m.Value))
                .ToList();

            if (numbers.Count >= 3)
            {
                // Try different permutations of year, month, day
                int[][] permutations = {
                    new[] { 0, 1, 2 },  // YMD
                    new[] { 2, 1, 0 },  // DMY
                    new[] { 2, 0, 1 },  // MDY
                    new[] { 1, 0, 2 },  // MYD (uncommon but possible)
                    new[] { 0, 2, 1 },  // YDM (uncommon but possible)
                    new[] { 1, 2, 0 }   // DYM (uncommon but possible)
                };

                foreach (var perm in permutations)
                {
                    // Extract values based on permutation
                    int yearCandidate = numbers[perm[0]];
                    int monthCandidate = numbers[perm[1]];
                    int dayCandidate = numbers[perm[2]];

                    // Adjust 2-digit years
                    if (yearCandidate < 100)
                        yearCandidate += 2000;
                    else if (yearCandidate > 100 && yearCandidate < 1000)
                        yearCandidate += 1000;

                    // Validate month and day ranges
                    if (monthCandidate < 1 || monthCandidate > 12)
                        continue;

                    if (dayCandidate < 1 || dayCandidate > 32)
                        continue;

                    // Try to create valid date
                    try
                    {
                        result = new NepaliDate(yearCandidate, monthCandidate, dayCandidate);
                        return true;
                    }
                    catch
                    {
                        // Continue to next permutation
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to parse year, month, and day components and create a valid NepaliDate.
        /// </summary>
        private static bool TryParseYearMonthDay(string yearStr, string monthStr, string dayStr, out NepaliDate result)
        {
            result = default;

            if (int.TryParse(yearStr, out int year) &&
                int.TryParse(monthStr, out int month) &&
                int.TryParse(dayStr, out int day))
            {
                // Adjust 2-digit years
                if (year < 100)
                    year += 2000;
                else if (year > 100 && year < 1000)
                    year += 1000;

                // Validate month and day ranges
                if (month < 1 || month > 12 || day < 1 || day > 32)
                    return false;

                // Try to create valid date
                try
                {
                    result = new NepaliDate(year, month, day);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Converts Nepali unicode digits to English digits.
        /// </summary>
        private static string ConvertNepaliDigitsToEnglish(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            char[] result = input.ToCharArray();

            for (int i = 0; i < result.Length; i++)
            {
                if (NepaliToEnglishDigits.TryGetValue(result[i], out char englishDigit))
                {
                    result[i] = englishDigit;
                }
            }

            return new string(result);
        }

        /// <summary>
        /// Performs a case-insensitive replacement of a substring within a string.
        /// </summary>
        /// <param name="input">The original string.</param>
        /// <param name="oldValue">The string to be replaced.</param>
        /// <param name="newValue">The string to replace all occurrences of oldValue.</param>
        /// <returns>A new string with all occurrences of oldValue replaced by newValue.</returns>
        private static string ReplaceStringIgnoreCase(string input, string oldValue, string newValue)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(oldValue))
                return input;

            int index = 0;
            var result = new System.Text.StringBuilder();
            int oldValueLength = oldValue.Length;

            // Find all occurrences of oldValue in input, ignoring case
            while (index < input.Length)
            {
                int matchIndex = input.IndexOf(oldValue, index, StringComparison.OrdinalIgnoreCase);
                if (matchIndex < 0)
                {
                    // No more matches, add the rest of the input
                    result.Append(input, index, input.Length - index);
                    break;
                }

                // Add the part before the match
                result.Append(input, index, matchIndex - index);

                // Add the replacement
                result.Append(newValue);

                // Move to the position after the match
                index = matchIndex + oldValueLength;
            }

            return result.ToString();
        }
        
        /// <summary>
        /// Checks if a string represents a numeric value
        /// </summary>
        private static bool IsNumeric(string input)
        {
            return int.TryParse(input, out _);
        }
    }
    
    
    
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
                { "पौष", 9 },
                { "माघ", 10 },
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

            string normalizedInput = input;

            // Try exact match first for performance
            if (CanonicalMonthNames.TryGetValue(normalizedInput, out int exactMatch))
                return exactMatch;

            double bestScore = 0;
            int? bestMatch = null;

            // Compare against all canonical forms using sequence alignment
            foreach (var kvp in CanonicalMonthNames)
            {
                string candidate = kvp.Key;
                double similarity = CalculateSequenceAlignment(normalizedInput, candidate);

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