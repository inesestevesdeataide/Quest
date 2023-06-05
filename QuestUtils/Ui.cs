using System;
namespace ProjQuest_InesAtaide.UserInterface;

public class Ui
{
    // user input
    public static bool ReadBoolYesNo(string questionLine, bool clearOn)
    {
        bool boolean = false;
        bool booleanIsValid = false;

        while (!booleanIsValid)
        {
            Console.WriteLine($"{questionLine}\nType 'Yes' or 'No':");
            string strBoolean = Console.ReadLine().ToLower().Trim();

            if (strBoolean == "yes")
            {
                booleanIsValid = true;
                boolean = true;
            }
            else if (strBoolean == "no")
            {
                booleanIsValid = true;
                boolean = false;
            }
            else
            {
                if (clearOn)
                {
                    Console.Clear();
                }
                Console.WriteLine("-> Invalid answer.\n");
            }
        }

        return boolean;
    }

    public static DateTime ReadDate()
    {
        try
        {
            Console.WriteLine("Insert date and time.\nFollow the format (yyyy/mm/dd hh:mm):\n");
            string strDateTime = Console.ReadLine().Trim();

            var split = strDateTime.Split(' ');

            string strDate = split[0];
            string strTime = split[1];

            var timeSplit = strTime.Split(':');

            int hour = Convert.ToInt16(timeSplit[0]);
            int minutes = Convert.ToInt16(timeSplit[1]);

            var dateSplit = strDate.Split('/');

            int year = Convert.ToInt16(dateSplit[0]);
            int month = Convert.ToInt16(dateSplit[1]);
            int day = Convert.ToInt16(dateSplit[2]);

            DateTime date = new DateTime(year, month, day, hour, minutes, 0);

            if (DateTime.Now.CompareTo(date) < 0)
            {
                return date;
            }
            else
            {
                Ui.ClearAndWriteLine("-> Date can not be past.\nTry again:\n");
                return ReadDate();
            }
        }
        catch (Exception ex)
        {
            Console.Clear();
            Console.WriteLine("-> Try again:\n");
            return ReadDate();
        }
    }

    public static (int hours, int minutes) ReadDuration()
    {
        int hours = 0;
        int minutes = 0;
        bool isValid = false;

        while (!isValid)
        {
            Console.WriteLine("Insert duration.\nFollow the format (hh:mm):\n");
            string strDuration = Console.ReadLine().Trim();

            if (strDuration.Contains(':'))
            {
                var split = strDuration.Split(':');

                bool hoursOk = int.TryParse(split[0], out hours);
                bool minutesOk = int.TryParse(split[1], out minutes);

                if (hoursOk && hours >= 0 && minutesOk && minutes >= 0 && minutes <= 59 && (hours != 0 || minutes != 0))
                {
                    isValid = true;
                }
            }
            else
            {
                Console.Clear();
                Console.WriteLine("-> Try again.\n");
            }
        }

        return (hours, minutes);
    }

    public static int ReadInt(string message)
    {
        Console.WriteLine(message);

        int.TryParse(Console.ReadLine().Trim(), out int result);

        return result;
    }

    public static int ReadIntInRange(string variable, int first, int last, string options, bool clearOn)
    {
        int value = 0;
        bool isValid = false;

        while (!isValid)
        {
            Console.WriteLine($"Select your {variable}:");
            Console.WriteLine(options);

            bool isOk = int.TryParse(Console.ReadLine(), out value);

            if (value >= first && value <= last)
            {
                isValid = true;
            }
            else
            {
                if (clearOn)
                {
                    Console.Clear();
                }
                else
                {
                    Console.WriteLine();
                }

                Console.WriteLine($"-> The {variable} must be between {first} and {last}.\n");
            }
        }

        return value;
    }

    // user output
    public static void ClearAndWriteLine(string message)
    {
        Console.Clear();
        Console.WriteLine(message);
    }

    public static void PressEnterToMoveOn()
    {
        Console.WriteLine("\nPress enter to move on.");
        Console.Read();
        Console.Clear();
    }
}