namespace NepDate.Examples;

public class CalendarExamples
{
    public static void RunAllExamples()
    {
        Console.WriteLine("=== Calendar Data Examples (Tithi, Events, Holidays) ===\n");

        TithiLookup();
        HolidayCheck();
        EventsLookup();
        FullCalendarInfo();
        HolidaysInYear();
        EventDaysInMonth();
    }

    public static void TithiLookup()
    {
        Console.WriteLine("--- Tithi (Lunar Day) ---");

        // Baisakh 1, 2081 BS = Panchami
        var date = new NepaliDate(2081, 1, 1);
        Console.WriteLine($"{date}  Tithi: {date.TithiNp} ({date.TithiEn})");

        // Consecutive days to show the Tithi sequence
        Console.WriteLine("\nFirst week of Baisakh 2081:");
        for (int day = 1; day <= 7; day++)
        {
            var d = new NepaliDate(2081, 1, day);
            string tithi = string.IsNullOrEmpty(d.TithiNp) ? "(no data)" : $"{d.TithiNp} / {d.TithiEn}";
            Console.WriteLine($"  {d}  {tithi}");
        }

        // Outside the scraped Tithi range (2078-2082): returns empty string
        var old = new NepaliDate(2070, 6, 15);
        Console.WriteLine($"\n{old}  TithiNp='{old.TithiNp}' (outside Tithi range)");

        Console.WriteLine();
    }

    public static void HolidayCheck()
    {
        Console.WriteLine("--- Public Holidays ---");

        (NepaliDate date, string label)[] samples =
        {
            (new NepaliDate(2081, 1, 1),  "New Year"),
            (new NepaliDate(2081, 1, 5),  "Ram Navami"),
            (new NepaliDate(2081, 2, 15), "Republic Day"),
            (new NepaliDate(2081, 1, 3),  "Ordinary day"),
        };

        foreach (var (d, label) in samples)
        {
            var info = d.GetCalendarInfo();
            string eventsNp = info.EventsNp.Length > 0 ? string.Join(", ", info.EventsNp) : "";
            string eventsEn = info.EventsEn.Length > 0 ? string.Join(", ", info.EventsEn) : "";
            string events = info.EventsNp.Length > 0 ? $"  [{eventsNp}  /  {eventsEn}]" : "";
            Console.WriteLine($"  {d}  IsPublicHoliday={d.IsPublicHoliday,-5}{events}");
        }

        Console.WriteLine();
    }

    public static void EventsLookup()
    {
        Console.WriteLine("--- Events ---");

        var newYear = new NepaliDate(2081, 1, 1);
        var info = newYear.GetCalendarInfo();
        Console.WriteLine($"Events on {newYear} (New Year — {info.EventsNp.Length} events):");
        for (int i = 0; i < info.EventsNp.Length; i++)
            Console.WriteLine($"  {info.EventsNp[i]}  /  {info.EventsEn[i]}");

        var quiet = new NepaliDate(2081, 1, 3);
        Console.WriteLine($"\nEvents on {quiet}: {(quiet.GetCalendarInfo().EventsNp.Length == 0 ? "(none)" : "has events")}");

        Console.WriteLine();
    }

    public static void FullCalendarInfo()
    {
        Console.WriteLine("--- Full CalendarInfo for 2081/02/10 ---");

        var date = new NepaliDate(2081, 2, 10); // Buddha Jayanti / Purnima
        var info = date.GetCalendarInfo();

        Console.WriteLine($"Date:             {date}");
        Console.WriteLine($"Tithi:            {info.TithiNp} ({info.TithiEn})");
        Console.WriteLine($"Is public holiday: {info.IsPublicHoliday}");
        Console.WriteLine($"Events ({info.EventsNp.Length}):");
        for (int i = 0; i < info.EventsNp.Length; i++)
            Console.WriteLine($"  {info.EventsNp[i]}  /  {info.EventsEn[i]}");

        Console.WriteLine();
    }

    public static void HolidaysInYear()
    {
        Console.WriteLine("--- Public Holidays in BS 2081 ---");

        int count = 0;
        for (int month = 1; month <= 12; month++)
        {
            int end = new NepaliDate(2081, month, 1).MonthEndDay;
            for (int day = 1; day <= end; day++)
            {
                var d = new NepaliDate(2081, month, day);
                if (!d.IsPublicHoliday) continue;

                var info = d.GetCalendarInfo();
                string labelNp = info.EventsNp.Length > 0 ? string.Join(", ", info.EventsNp) : "";
                string labelEn = info.EventsEn.Length > 0 ? string.Join(", ", info.EventsEn) : "";
                string label = info.EventsNp.Length > 0 ? $"{labelNp}  /  {labelEn}" : "";
                Console.WriteLine($"  {d}  {label}");
                count++;
            }
        }
        Console.WriteLine($"Total: {count} holidays in 2081 BS");

        Console.WriteLine();
    }

    public static void EventDaysInMonth()
    {
        Console.WriteLine("--- Event Days in Baisakh 2081 ---");

        int end = new NepaliDate(2081, 1, 1).MonthEndDay;
        for (int day = 1; day <= end; day++)
        {
            var d = new NepaliDate(2081, 1, day);
            var info = d.GetCalendarInfo();
            if (info.EventsNp.Length == 0) continue;

            string eventsNp = string.Join(", ", info.EventsNp);
            string eventsEn = string.Join(", ", info.EventsEn);
            string tithi = string.IsNullOrEmpty(info.TithiNp) ? "" : $"[{info.TithiNp} / {info.TithiEn}]  ";
            Console.WriteLine($"  {d}  {tithi}{eventsNp}  /  {eventsEn}");
        }

        Console.WriteLine();
    }
}
