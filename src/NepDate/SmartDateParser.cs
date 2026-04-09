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
        // Month name mappings (English, Nepali transliteration, and Unicode)
        private static readonly Dictionary<string, int> MonthNameMappings = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            // Month 1 - Baisakh (वैशाख)
            { "baisakh", 1 }, { "baishakh", 1 }, { "baisak", 1 }, { "vaisakh", 1 }, { "vaisakha", 1 },
            { "vaishak", 1 }, { "vaisakhi", 1 }, { "beshak", 1 }, { "baishak", 1 },
            { "baisaga", 1 }, { "baishaga", 1 }, { "vesak", 1 },
            
            // Month 2 - Jestha (जेष्ठ)
            { "jestha", 2 }, { "jeth", 2 }, { "jeshtha", 2 }, { "jyeshtha", 2 }, { "jyestha", 2 },
            { "jesth", 2 }, { "jeshth", 2 }, { "jetha", 2 }, { "jeshta", 2 }, { "jayshtha", 2 },
            { "jayestha", 2 }, { "jesta", 2 }, { "jyesth", 2 }, { "jyaistha", 2 }, { "jaistha", 2 },
            
            // Month 3 - Asar (असार)
            { "asar", 3 }, { "asadh", 3 }, { "ashar", 3 }, { "ashad", 3 }, { "asad", 3 },
            { "aasad", 3 }, { "asada", 3 }, { "ashadh", 3 }, { "asadha", 3 }, { "ashadha", 3 },
            { "ashara", 3 }, { "asara", 3 }, { "ashada", 3 }, { "asaad", 3 }, { "aashar", 3 },
            
            // Month 4 - Shrawan (श्रावण)
            { "shrawan", 4 }, { "sawan", 4 }, { "saun", 4 }, { "srawan", 4 }, { "shraawan", 4 },
            { "shravan", 4 }, { "shravana", 4 }, { "sawun", 4 }, { "savan", 4 }, { "shrawana", 4 },
            { "sravana", 4 }, { "sawon", 4 }, { "sravan", 4 }, { "saawan", 4 }, { "sharwan", 4 },
            { "sarwan", 4 }, { "sraawan", 4 }, { "shaun", 4 }, { "shawan", 4 },
            
            // Month 5 - Bhadra (भाद्र)
            { "bhadra", 5 }, { "bhadau", 5 }, { "bhado", 5 }, { "bhaadra", 5 },
            { "bhadow", 5 }, { "bhadava", 5 }, { "bhadaw", 5 }, { "bhada", 5 },
            { "bhadoo", 5 }, { "bhadon", 5 }, { "bhadrapad", 5 }, { "bhadrapada", 5 }, { "bhaado", 5 },
            
            // Month 6 - Ashwin (आश्विन)
            { "ashwin", 6 }, { "asoj", 6 }, { "ashoj", 6 }, { "aswin", 6 }, { "ashvin", 6 },
            { "aaswin", 6 }, { "ashwini", 6 }, { "aswini", 6 }, { "ashvini", 6 }, { "aasoj", 6 },
            { "aashoj", 6 }, { "asoja", 6 }, { "asojh", 6 }, { "ashoja", 6 },
            { "asvin", 6 }, { "aashwin", 6 }, { "ashvina", 6 }, { "ashwina", 6 }, { "asvaayuja", 6 },

            // Month 7 - Kartik (कार्तिक)
            { "kartik", 7 }, { "kattik", 7 }, { "kaartik", 7 }, { "kartika", 7 }, { "katik", 7 },
            { "kartike", 7 }, { "karttik", 7 }, { "kartiki", 7 }, { "karthik", 7 }, { "karthika", 7 },
            { "kathik", 7 }, { "kaatik", 7 }, { "katak", 7 }, { "karttic", 7 }, { "kartic", 7 },
            
            // Month 8 - Mangsir (मंसिर)
            { "mangsir", 8 }, { "mangshir", 8 }, { "manshir", 8 }, { "marg", 8 }, { "margashirsha", 8 },
            { "mangasir", 8 }, { "mangsheer", 8 }, { "mangseer", 8 }, { "margshirsha", 8 },
            { "mansheer", 8 }, { "margsir", 8 }, { "managsir", 8 }, { "mangaseer", 8 }, { "mangsheersh", 8 },
            { "mangsira", 8 }, { "mansir", 8 }, { "magshir", 8 }, { "mangir", 8 }, { "magsir", 8 },
            
            // Month 9 - Poush (पौष)
            { "poush", 9 }, { "push", 9 }, { "pus", 9 }, { "paush", 9 },
            { "pausha", 9 }, { "pousha", 9 }, { "pos", 9 }, { "pausa", 9 }, { "pousa", 9 },
            { "posh", 9 }, { "posma", 9 }, { "paus", 9 }, { "poos", 9 },
            
            // Month 10 - Magh (माघ)
            { "magh", 10 }, { "mag", 10 }, { "maagh", 10 }, { "magha", 10 }, { "maagha", 10 },
            { "maga", 10 }, { "magah", 10 }, { "maag", 10 }, { "maaha", 10 }, { "maghu", 10 },
            { "maghaa", 10 }, { "magg", 10 }, { "mahi", 10 }, { "mahag", 10 },
            
            // Month 11 - Falgun (फाल्गुन)
            { "falgun", 11 }, { "phagun", 11 }, { "phalgun", 11 }, { "fagan", 11 }, { "fagun", 11 },
            { "phalguna", 11 }, { "falguna", 11 }, { "phalgoon", 11 }, { "falgunn", 11 }, { "phalguni", 11 },
            { "phalagan", 11 }, { "phalagun", 11 }, { "phalag", 11 },
            { "fagoon", 11 }, { "phaguna", 11 }, { "falgoona", 11 }, { "phagoon", 11 },
            
            // Month 12 - Chaitra (चैत्र)
            { "chaitra", 12 }, { "chait", 12 }, { "chaita", 12 }, { "chet", 12 }, { "chetra", 12 },
            { "chaitr", 12 }, { "chaity", 12 }, { "cheta", 12 }, { "chaitya", 12 },
            { "chaitri", 12 }, { "chaito", 12 }, { "chythro", 12 }, { "chaithra", 12 },
            
            // Nepali unicode month names
            // Month 1 - Baisakh
            { "बैशाख", 1 }, { "वैशाख", 1 }, { "बैसाख", 1 }, { "बैशाक", 1 }, { "वैसाख", 1 }, { "वैशाक", 1 },
            
            // Month 2 - Jestha
            { "जेष्ठ", 2 }, { "जेठ", 2 }, { "जेस्थ", 2 }, { "ज्येष्ठ", 2 }, { "जेस्ठ", 2 }, { "जेष्ट", 2 },
            
            // Month 3 - Asar
            { "आषाढ", 3 }, { "असार", 3 }, { "अषाढ", 3 }, { "आशाढ", 3 }, { "आषाढ़", 3 }, { "असाढ", 3 }, { "अषाड", 3 },
            
            // Month 4 - Shrawan
            { "श्रावण", 4 }, { "सावन", 4 }, { "साउन", 4 }, { "श्रावन", 4 }, { "सावण", 4 }, { "श्रवण", 4 },
            
            // Month 5 - Bhadra
            { "भाद्र", 5 }, { "भदौ", 5 }, { "भादौ", 5 }, { "भाद्रपद", 5 }, { "भदो", 5 }, { "भादोै", 5 }, { "भाद्रा", 5 },
            
            // Month 6 - Ashwin
            { "आश्विन", 6 }, { "असोज", 6 }, { "अश्विन", 6 }, { "आसोज", 6 }, { "अस्विन", 6 }, { "अश्वीन", 6 }, { "अश्वीना", 6 },
            
            // Month 7 - Kartik
            { "कार्तिक", 7 }, { "कात्तिक", 7 }, { "कार्तीक", 7 }, { "कार्तिका", 7 }, { "कातिक", 7 }, { "कर्तिक", 7 }, { "कार्तिक्", 7 },
            
            // Month 8 - Mangsir
            { "मंसिर", 8 }, { "मङ्सिर", 8 }, { "मार्ग", 8 }, { "मंग्सिर", 8 }, { "मंशिर", 8 }, { "मागशिर", 8 }, { "मार्गशीर्ष", 8 },
            
            // Month 9 - Poush
            { "पौष", 9 }, { "पुष", 9 }, { "पुस", 9 }, { "पौश", 9 }, { "पौष्य", 9 }, { "पौस", 9 },
            
            // Month 10 - Magh
            { "माघ", 10 }, { "माग", 10 }, { "माह", 10 }, { "माघा", 10 }, { "माग्ह", 10 }, { "मा्घ", 10 },
            
            // Month 11 - Falgun
            { "फाल्गुन", 11 }, { "फागुन", 11 }, { "फाल्गुण", 11 }, { "फल्गुन", 11 }, { "फाल्गुना", 11 },
            
            // Month 12 - Chaitra
            { "चैत्र", 12 }, { "चैत", 12 }, { "चैता", 12 }, { "चॆत्र", 12 }, { "चेत्र", 12 }, { "चैत्रा", 12 }
        };

        // Nepali unicode digit mappings
        private static readonly Dictionary<char, char> NepaliToEnglishDigits = new Dictionary<char, char>
        {
            { '०', '0' }, { '१', '1' }, { '२', '2' }, { '३', '3' }, { '४', '4' },
            { '५', '5' }, { '६', '6' }, { '७', '7' }, { '८', '8' }, { '९', '9' }
        };

        // Common date separators
        private static readonly char[] DateSeparators = { '/', '-', '.', ' ', ',', '_', '|', '।' };

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
            catch (Exception)
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
                string pattern = $"{Regex.Escape(separator.ToString())}";
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
        /// </summary>
        private static bool TryParseMonthNameFormat(string input, out NepaliDate result)
        {
            result = default;

            // Pattern for: [day] [month name] [year] or [month name] [day], [year]
            var monthNameMatches = MonthNameMappings.Keys
                .Where(monthName => input.IndexOf(monthName, StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderByDescending(m => m.Length)  // Prefer longer matches to avoid partial matches
                .ToList();

            foreach (var monthName in monthNameMatches)
            {
                int monthValue = MonthNameMappings[monthName];

                // Find year and day in the input
                string remaining = ReplaceStringIgnoreCase(input, monthName, " ").Trim();

                // Extract year and day
                var numbers = Regex.Matches(remaining, @"\d+")
                    .Cast<Match>()
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
                        result = new NepaliDate(year, monthValue, day);
                        return true;
                    }
                    catch
                    {
                        // Continue to next attempt
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
    }
}