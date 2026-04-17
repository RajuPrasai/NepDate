
namespace NepDate.Examples;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== NepDate Library Examples ===");
        Console.WriteLine("This program demonstrates the functionality of the NepDate library.\n");

        bool exit = false;

        while (!exit)
        {
            DisplayMenu();

            string choice = Console.ReadLine()?.Trim() ?? "";
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    NepaliDateExamples.RunAllExamples();
                    break;
                case "2":
                    FiscalYearExamples.RunAllExamples();
                    break;
                case "3":
                    NepaliDateRangeExamples.RunAllExamples();
                    break;
                case "4":
                    SmartDateParserExamples.RunAllExamples();
                    break;
                case "5":
                    BulkConvertExamples.RunAllExamples();
                    break;
                case "6":
                    RunAllExamples();
                    break;
                case "7":
                    DateCalculationProfiler.RunProfiler();
                    break;
                case "8":
                    CalendarExamples.RunAllExamples();
                    break;
                case "0":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.\n");
                    break;
            }

            if (!exit)
            {
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                Console.Clear();
            }
        }

        Console.WriteLine("Thank you for exploring the NepDate library examples!");
    }

    static void DisplayMenu()
    {
        Console.WriteLine("Choose an example category to run:");
        Console.WriteLine("1. NepaliDate Basic Functionality");
        Console.WriteLine("2. Fiscal Year Operations");
        Console.WriteLine("3. NepaliDateRange Operations");
        Console.WriteLine("4. SmartDateParser Functionality");
        Console.WriteLine("5. BulkConvert Performance");
        Console.WriteLine("6. Run All Examples");
        Console.WriteLine("7. Run DateCalculationProfiler");
        Console.WriteLine("8. Calendar, Tithi & Events");
        Console.WriteLine("0. Exit");
        Console.Write("\nEnter your choice: ");
    }

    static void RunAllExamples()
    {
        Console.WriteLine("=== Running All NepDate Library Examples ===\n");

        NepaliDateExamples.RunAllExamples();
        Console.WriteLine("\n" + new string('-', 80) + "\n");

        FiscalYearExamples.RunAllExamples();
        Console.WriteLine("\n" + new string('-', 80) + "\n");

        NepaliDateRangeExamples.RunAllExamples();
        Console.WriteLine("\n" + new string('-', 80) + "\n");

        SmartDateParserExamples.RunAllExamples();
        Console.WriteLine("\n" + new string('-', 80) + "\n");

        BulkConvertExamples.RunAllExamples();
        Console.WriteLine("\n" + new string('-', 80) + "\n");

        CalendarExamples.RunAllExamples();

        Console.WriteLine("\n=== All Examples Completed ===");
    }
}