using System;
using System.Collections.Generic;

namespace NepDate
{
    /// <summary>
    /// Provides intelligent parsing of Nepali date strings across a wide variety of formats,
    /// month name spellings, scripts, and separator styles.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Unlike <see cref="NepaliDate.Parse(string)"/>, which requires a strict numeric
    /// <c>YYYY/MM/DD</c> input, <see cref="SmartDateParser"/> accepts:
    /// </para>
    /// <list type="bullet">
    ///   <item><description>Over 100 English and Nepali transliteration spellings of month names (e.g. <c>Shrawan</c>, <c>Saun</c>, <c>साउन</c>).</description></item>
    ///   <item><description>Nepali Devanagari digit strings (<c>२०८०/०८/१५</c>).</description></item>
    ///   <item><description>Optional locale suffixes such as <c>B.S.</c>, <c>V.S.</c>, <c>गते</c>, and <c>मिति</c>.</description></item>
    ///   <item><description>2-digit year abbreviations expanded to the current millennium.</description></item>
    ///   <item><description>Ambiguous component ordering resolved by heuristic permutation.</description></item>
    /// </list>
    /// <para>
    /// For strict numeric parsing without heuristics, use <see cref="NepaliDate.Parse(string)"/>
    /// or its <c>autoAdjust</c> overload instead.
    /// </para>
    /// </remarks>
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

        // Common date separators
        private static readonly char[] DateSeparators = { '/', '-', '.', ' ', ',', '_', '|', '।' };

        /// <summary>
        /// Parses a Nepali date string that may use any supported format, script, or month name spelling.
        /// </summary>
        /// <param name="input">
        /// The date string to parse. Accepts numeric formats (<c>YYYY/MM/DD</c>), month-name formats
        /// (<c>"15 Shrawan 2080"</c>, <c>"Shrawan 15, 2080"</c>), Nepali Unicode digits and month names,
        /// and strings with optional locale suffixes (<c>B.S.</c>, <c>गते</c>).
        /// </param>
        /// <returns>A <see cref="NepaliDate"/> representing the parsed date.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <see langword="null"/> or whitespace.</exception>
        /// <exception cref="FormatException">Thrown when the string cannot be interpreted as a valid Nepali date by any parsing strategy.</exception>
        /// <example>
        /// <code>
        /// NepaliDate d1 = SmartDateParser.Parse("15 Shrawan 2080");   // DD Month YYYY
        /// NepaliDate d2 = SmartDateParser.Parse("Shrawan 15, 2080");   // Month DD, YYYY
        /// NepaliDate d3 = SmartDateParser.Parse("15 Saun 2080");       // alternate spelling
        /// NepaliDate d4 = SmartDateParser.Parse("२०८०/०८/१५");        // Nepali digits
        /// NepaliDate d5 = SmartDateParser.Parse("१५ श्रावण २०८०");     // full Nepali
        /// </code>
        /// </example>
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
        /// Attempts to parse a Nepali date string using the same heuristics as <see cref="Parse(string)"/>,
        /// without throwing an exception on failure.
        /// </summary>
        /// <param name="input">The string to parse.</param>
        /// <param name="result">
        /// When this method returns <see langword="true"/>, contains the parsed <see cref="NepaliDate"/>;
        /// otherwise contains the default value.
        /// </param>
        /// <returns><see langword="true"/> if parsing succeeded; otherwise <see langword="false"/>.</returns>
        public static bool TryParse(string input, out NepaliDate result)
        {
            result = default;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            if (NepaliDate.TryParse(input, out result))
                return true;

            try
            {
                string normalizedInput = NormalizeInput(input);

                if (TryParseStandardFormat(normalizedInput, out result))
                    return true;

                if (TryParseNepaliUnicodeFormat(normalizedInput, out result))
                    return true;

                if (TryParseMonthNameFormat(normalizedInput, out result))
                    return true;

                if (TryParseAmbiguousFormat(normalizedInput, out result))
                    return true;
            }
            catch
            {
                result = default;
            }

            return false;
        }

        /// <summary>
        /// Applies pre-processing to the raw input string: trims whitespace, strips locale suffixes
        /// (<c>B.S.</c>, <c>V.S.</c>, <c>गते</c>, <c>मिति</c>), and collapses repeated whitespace
        /// to a single space.
        /// </summary>
        private static string NormalizeInput(string input)
        {
            string result = input.Trim();

            result = RemoveIndicators(result);
            result = CollapseSpaces(result);
            result = RemoveNepaliKeywords(result);

            return result.Trim();
        }

        /// <summary>
        /// Removes <c>B.S.</c>, <c>BS</c>, <c>V.S.</c>, and <c>VS</c> calendar-era markers
        /// (case-insensitive) from <paramref name="input"/>, preserving all other content.
        /// </summary>
        private static string RemoveIndicators(string input)
        {
            // Remove BS, B.S., VS, V.S. (case-insensitive, word boundaries)
            var chars = new char[input.Length];
            int len = 0;
            int i = 0;

            while (i < input.Length)
            {
                if (i < input.Length - 1)
                {
                    char c0 = input[i];
                    char c1 = input[i + 1];

                    // Check for B.S. or V.S. (with optional dots)
                    if ((c0 == 'B' || c0 == 'b' || c0 == 'V' || c0 == 'v') &&
                        (c1 == 'S' || c1 == 's' || c1 == '.'))
                    {
                        bool isAtWordBoundary = (i == 0 || !char.IsLetterOrDigit(input[i - 1]));
                        if (isAtWordBoundary)
                        {
                            // Try to match: B.S. or BS or V.S. or VS
                            int end = i;
                            if (c1 == '.')
                            {
                                // B. or V. - check for S or S.
                                end = i + 2;
                                if (end < input.Length && (input[end] == 'S' || input[end] == 's'))
                                {
                                    end++;
                                    if (end < input.Length && input[end] == '.') end++;
                                }
                            }
                            else if (c1 == 'S' || c1 == 's')
                            {
                                // BS or VS
                                end = i + 2;
                                if (end < input.Length && input[end] == '.') end++;
                            }

                            if (end > i + 1 && (end >= input.Length || !char.IsLetterOrDigit(input[end])))
                            {
                                i = end;
                                continue;
                            }
                        }
                    }
                }
                chars[len++] = input[i];
                i++;
            }

            return new string(chars, 0, len);
        }

        /// <summary>
        /// Normalises all whitespace runs (space, tab, CR, LF) in <paramref name="input"/> to a single space character.
        /// </summary>
        private static string CollapseSpaces(string input)
        {
            var chars = new char[input.Length];
            int len = 0;
            bool lastWasSpace = false;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                bool isSpace = c == ' ' || c == '\t' || c == '\r' || c == '\n';
                if (isSpace)
                {
                    if (!lastWasSpace)
                    {
                        chars[len++] = ' ';
                    }
                    lastWasSpace = true;
                }
                else
                {
                    chars[len++] = c;
                    lastWasSpace = false;
                }
            }

            return new string(chars, 0, len);
        }

        /// <summary>
        /// Removes the Nepali date-suffix keywords <c>गते</c> (gate) and <c>मिति</c> (miti)
        /// from <paramref name="input"/> wherever they appear.
        /// </summary>
        private static string RemoveNepaliKeywords(string input)
        {
            // Remove गते and मिति
            const string gate = "\u0917\u0924\u0947";  // गते
            const string miti = "\u092E\u093F\u0924\u093F";  // मिति

            var result = ReplaceStringIgnoreCase(input, gate, string.Empty);
            result = ReplaceStringIgnoreCase(result, miti, string.Empty);
            return result;
        }

        /// <summary>
        /// Tries to parse a fully numeric date string by splitting on each recognised separator
        /// and testing the three most common component orderings: YMD, DMY, and MDY.
        /// </summary>
        private static bool TryParseStandardFormat(string input, out NepaliDate result)
        {
            result = default;

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
        /// Tries to parse a date string that contains at least one month name from
        /// <see cref="MonthNameMappings"/>. Extracts the remaining numeric tokens and
        /// attempts to identify year and day by magnitude (year &gt; 1900).
        /// </summary>
        private static bool TryParseMonthNameFormat(string input, out NepaliDate result)
        {
            result = default;

            // Find the longest matching month name (avoids partial matches)
            string bestMatch = null;
            int bestLength = 0;

            foreach (var key in MonthNameMappings.Keys)
            {
                if (key.Length > bestLength && input.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    bestMatch = key;
                    bestLength = key.Length;
                }
            }

            if (bestMatch == null)
                return false;

            int monthValue = MonthNameMappings[bestMatch];
            string remaining = ReplaceStringIgnoreCase(input, bestMatch, " ").Trim();

            // Extract numbers manually instead of Regex.Matches + LINQ
            var numbers = ExtractNumbers(remaining);

            if (numbers.Count >= 2)
            {
                int year, day;
                if (numbers[0] > 1900)
                {
                    year = numbers[0];
                    day = numbers.Count > 1 ? numbers[1] : 1;
                }
                else if (numbers.Count > 1 && numbers[1] > 1900)
                {
                    year = numbers[1];
                    day = numbers[0];
                }
                else
                {
                    // Find max without sorting
                    int maxVal = numbers[0], secondVal = 0;
                    for (int i = 1; i < numbers.Count; i++)
                    {
                        if (numbers[i] > maxVal)
                        {
                            secondVal = maxVal;
                            maxVal = numbers[i];
                        }
                        else if (numbers[i] > secondVal)
                        {
                            secondVal = numbers[i];
                        }
                    }
                    if (numbers.Count == 1) { maxVal = numbers[0]; secondVal = 1; }
                    else if (secondVal == 0) { secondVal = numbers[0] == maxVal && numbers.Count > 1 ? numbers[1] : 1; }

                    year = maxVal;
                    day = secondVal;

                    if (year < 100)
                        year += 2000;
                    else if (year >= 100 && year < 999)
                        year += 1000;
                }

                if (day < 1 || day > 32)
                    return false;

                try
                {
                    result = new NepaliDate(year, monthValue, day);
                    return true;
                }
                catch
                {
                    // Fall through
                }
            }

            return false;
        }

        /// <summary>
        /// Converts any Devanagari digits (०–९) in <paramref name="input"/> to their ASCII equivalents
        /// and then delegates to <see cref="TryParse(string, out NepaliDate)"/>.
        /// Returns <see langword="false"/> without further processing when no Devanagari digits are present.
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
        /// Last-resort parser that extracts all numeric tokens from <paramref name="input"/> and
        /// exhaustively tries all six YMD component permutations until one produces a valid date.
        /// </summary>
        private static bool TryParseAmbiguousFormat(string input, out NepaliDate result)
        {
            result = default;

            var numbers = ExtractNumbers(input);

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
        /// Validates and constructs a <see cref="NepaliDate"/> from three string tokens expected to represent
        /// year, month, and day in that order. Accepts 2- or 3-digit year abbreviations and expands them to
        /// the current millennium.
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
        /// Converts Devanagari digits (०–९, Unicode code points U+0966–U+096F) in
        /// <paramref name="input"/> to their ASCII equivalents (0–9). Non-digit characters
        /// are passed through unchanged. Returns the original string reference when no
        /// Devanagari digits are found, avoiding an unnecessary allocation.
        /// </summary>
        private static string ConvertNepaliDigitsToEnglish(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            char[] result = null;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (c >= '\u0966' && c <= '\u096F')
                {
                    if (result == null)
                    {
                        result = input.ToCharArray();
                    }
                    result[i] = (char)('0' + (c - '\u0966'));
                }
            }

            return result != null ? new string(result) : input;
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
        /// Extracts all contiguous sequences of ASCII digit characters from <paramref name="input"/>
        /// and returns their integer values in order of appearance.
        /// Non-digit characters act as delimiters; at most four values are pre-allocated.
        /// </summary>
        private static List<int> ExtractNumbers(string input)
        {
            var numbers = new List<int>(4);
            int current = 0;
            bool inNumber = false;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (c >= '0' && c <= '9')
                {
                    current = current * 10 + (c - '0');
                    inNumber = true;
                }
                else
                {
                    if (inNumber)
                    {
                        numbers.Add(current);
                        current = 0;
                        inNumber = false;
                    }
                }
            }

            if (inNumber)
            {
                numbers.Add(current);
            }

            return numbers;
        }
    }
}