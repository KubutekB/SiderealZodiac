using System;
using System.Reflection.Metadata;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Numerics;

class Program
{
    static void Main()
    {

        /* Debug dictionary
        foreach (var kvp in zodiacBoundaries(boundaries))
        {
            Console.Write($"Key: {kvp.Key}, Values: ");

            // Iterate through the array values and print
            foreach (var value in kvp.Value)
            {
                Console.Write($"{value} ");
            }

            Console.WriteLine(); // Move to the next line for the next key-value pair
        }*/

        /*Debug ecliptic string
        for (int i=360; i>=0; i--)
        {
            testBuildEcliptic(i);
        }
        
        Environment.Exit(1);
        
        */

        string version = "1.0.0";

        printMessage(ConsoleColor.Green, "Welcome to the sidereal zodiac calculator version: " + version);

        //Console.WriteLine("Welcome to the sidereal zodiac calculator version: " + version);



        var zb = zodiacBoundaries(boundaries, smallImages);
        

        while (true) 
        {
            Console.WriteLine("");
            Console.WriteLine("1-Input a date (ex. birthday)");
            Console.WriteLine("2-Get help");
            Console.WriteLine("3-Quit");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Green;
            string option = Console.ReadLine();
            Console.ResetColor();
            
            switch (option)
            {
                case "1":
                    string givenDate = GetDate();
                    double position = eclipticDegrees(springEquinox_1900, givenDate); //TODO check if dates before equinox work as they should

                    int minVal = 0;
                    int maxVal = 360;
                    string between1 = zodiacNames[0];
                    string between2 = zodiacNames[zodiacNames.Length - 1];
                    bool found = false;

                    foreach (var key in zb.Keys) // loop for if date falls in a sign 
                    {

                        

                        if ((position <= key[1] && position >= key[0]) || (key[0] == boundaries.Max() && (position >= boundaries.Max() || position <= boundaries.Min()))) //Convoluted because of pisces
                        {
                            printMessage(ConsoleColor.Yellow, $"Your sidereal Sun zodiac sign is: {zb[key]}");
                            printMessage(ConsoleColor.White, testBuildEcliptic(position, givenDate));
                            //Console.WriteLine($"Your sidereal Sun zodiac sign is: {zb[key]}");
                            //Console.WriteLine(testBuildEcliptic(position, givenDate));
                            found = true;
                            break;
                        }
                       
                        
                        


                        if (position >= key[0] && minVal <= key[0])
                        {
                            minVal = key[0];
                            between1 = zb[key];
                        }
                        if (position <= key[1] && maxVal >= key[1])
                        {
                            maxVal = key[1];
                            between2 = zb[key];
                        }

                    }

                    //Just do it like this, redo later
                    if (between1 == "aries")
                    {
                        between1 = smallPisces;
                    }

                    if (between2 == "pisces")
                    {
                        between2 = smallPisces;
                    }


                    if (!found)
                    {

                        printMessage(ConsoleColor.Yellow, $"Your sidereal Sun zodiac sign falls between: {between1} and {between2}");
                        printMessage(ConsoleColor.White, testBuildEcliptic(position, givenDate));
                        //Console.WriteLine($"Your sidereal Sun zodiac sign falls between: {between1} and {between2}");
                        //Console.WriteLine(testBuildEcliptic(position, givenDate));
                    }
                    
                    continue;
                case "2":
                    printMessage(ConsoleColor.White, wikipediaInfo);
                    printMessage(ConsoleColor.Cyan, isz);
                    continue;
                case "3":
                    break;
                default:
                    Console.WriteLine("Invalid Input");
                    continue;
            }
            break;
            
        }





    }

    static string wikipediaInfo = "In astrology, sidereal and tropical are terms that refer to two different systems of ecliptic coordinates used to divide the ecliptic into twelve \"signs\". " +
        "Each sign is divided into 30 degrees, making a total of 360 degrees.The terms sidereal and tropical may also refer to two different definitions of a year, applied in sidereal solar calendars or tropical solar calendars." +
        "\r\n\r\nWhile sidereal systems of astrology calculate twelve zodiac signs based on the observable sky and thus account for the apparent backwards movement of fixed stars of about 1 degree every 72 years from the perspective of" +
        " the Earth due to the Earth's axial precession, tropical systems consider 0 degrees of Aries as always coinciding with the March equinox (known as the spring equinox in the Northern Hemisphere) " +
        "and define twelve zodiac signs from this starting point, basing their definitions upon the seasons and not upon the observable sky wherein the March equinox currently falls in Pisces due to the Earth's axial precession." +
        "These differences have caused sidereal and tropical zodiac systems, which were aligned around 2,000 years ago when the March equinox coincided with Aries in the observable sky, to drift apart over the centuries.";

    static void printMessage(ConsoleColor color, string s)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(s);
        Console.ResetColor();

    }

    //Cut out zodiacs from ascii string
    static string GetSubimage(string s, int startRow, int endRow, int startColumn, int endColumn)
    {
        
        string[] rows = s.Split('\n');

        // Validate input bounds
        startRow = Math.Max(0, Math.Min(rows.Length - 1, startRow));
        endRow = Math.Max(0, Math.Min(rows.Length - 1, endRow));

        startColumn = Math.Max(0, Math.Min(rows[0].Length - 1, startColumn));
        endColumn = Math.Max(0, Math.Min(rows[0].Length - 1, endColumn));

        // Extract the subimage
        StringBuilder subimage = new StringBuilder();
        for (int i = startRow; i <= endRow; i++)
        {
            subimage.AppendLine(rows[i].Substring(startColumn, endColumn - startColumn + 1));
        }

        return subimage.ToString();
    }

    //Get date from user input
    static string GetDate()
    {
        string year;
        string month;
        string day;


        string blanketLoop(string ymd, int startRange, int endRange)
        {
            string input;
            while (true)
            {
                Console.WriteLine("Enter " + ymd);
                Console.ForegroundColor = ConsoleColor.Green;
                input = Console.ReadLine();
                Console.WriteLine("");
                Console.ResetColor();
                if (int.TryParse(input, out int result))
                {
                    switch (result)
                    {
                        case int i when i >= startRange && i <= endRange:
                            break;
                        default:
                            Console.WriteLine("Out of range! (range: " + startRange.ToString() + "-" + endRange.ToString() + ")");
                            continue;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Input!");
                    continue;
                }
                break;
            }
            if (input.Length < 2)
            {
                input = "0" + input;
            }
            return input;
        }

        year = blanketLoop("year", 1900, 2050);
        month = blanketLoop("month", 1, 12);

        string[] longMonths = { "1", "3", "5", "7", "8", "10", "12" };
        string[] shortMonths = { "4", "6", "9", "11" };

        switch (month)
        {
            
            case string s when longMonths.Any(s.Contains):
                day = blanketLoop("day", 1, 31);
                break;
            case string s when shortMonths.Any(s.Contains):
                day = blanketLoop("day", 1, 30);
                break;
            case "02":
                int tempYear = Int32.Parse(year);
                if (tempYear % 4 == 0 && tempYear != 1900)
                {
                    day = blanketLoop("day", 1, 29);
                }
                else
                {
                    day = blanketLoop("day", 1, 28);
                }
                break;
            default:
                Console.WriteLine("This should not ever be printed otherwise something went really wrong, setting day = '01'");
                day = "01";
                break;
        }
 

        return year + "-" + month + "-" + day + " 00:00"; //Returns midnight for now; TODO - implement asking for time
    }

    static string springEquinox_1900 = "1900-03-21 01:39";
    static string[] zodiacNames = { "aries", "taurus", "gemini", "cancer", "leo", "virgo", "libra", "scorpio", "sagittarius", "capricorn", "aquarius", "pisces" };
    static int[] boundaries = { 33, 48, 51, 84, 93, 113, 123, 133, 140, 171, 173, 218, 224, 239, 242, 267, 268, 296, 303, 323, 324, 345, 351, 29}; //360+29
    //Celestial Coordinates:
    /*
     aries: 33 - 48
     taurus: 51 - 84
     gemini: 93 - 113
     cancer: 123 - 133
     leo: 140 - 171
     virgo: 173 - 218
     libra: 224 - 239
     scorpio: 242 - 267
     sagittarius: 268 - 296
     capricorn: 303 - 323
     aquaius: 324 - 345
     pisces: 351 -  29

    */
    static asciiImages imgs = new asciiImages();
    static string isz = imgs.smallZodiacs;
    static string smallAries = GetSubimage(isz, 0, 17, 5, 40);
    static string smallTaurus = GetSubimage(isz, 0, 17, 41, 70);
    static string smallGemini = GetSubimage(isz, 0, 17, 71, 105);
    static string smallCancer = GetSubimage(isz, 0, 17, 106, 140);   
    static string smallLeo = GetSubimage(isz, 18, 34, 5, 40);
    static string smallVirgo = GetSubimage(isz, 18, 34, 41, 70);
    static string smallLibra = GetSubimage(isz, 18, 34, 71, 108);
    static string smallScorpio = GetSubimage(isz, 18, 34, 109, 135);
    static string smallSagittarius = GetSubimage(isz, 34, 51, 5, 40);
    static string smallCapricorn = GetSubimage(isz, 34, 51, 41, 70);
    static string smallAquarius = GetSubimage(isz, 34, 51, 71, 106);
    static string smallPisces = GetSubimage(isz, 34, 51, 107, 150);
    static string[] smallImages = { smallAries, smallTaurus, smallGemini, smallCancer, smallLeo, smallVirgo, smallLibra, smallScorpio, smallSagittarius, smallCapricorn, smallAquarius, smallPisces };
    static string ecliptic = @"
          0      30     60     90    120    150    180    210    240    270    300    330    360
          |------|------|------|------|------|------|------|------|------|------|------|------|
                                                                                                ";

    //Returns the difference between two dates in form of degrees on the ecliptic
    static double eclipticDegrees(string start, string end) 
    {
        double yearLength = 365.2425;

        DateTime startDate = DateTime.ParseExact(start, "yyyy-MM-dd HH:mm", null);
        DateTime endDate = DateTime.ParseExact(end, "yyyy-MM-dd HH:mm", null);

        TimeSpan timeDifference = endDate - startDate;

        double addedTime = timeDifference.TotalDays;
        double eclipticDeg = (addedTime / yearLength);
        eclipticDeg -= Math.Floor(addedTime / yearLength);
        eclipticDeg *= 360;
        

        return Math.Round(eclipticDeg, 2); 
    }

    static Dictionary<int[], string> zodiacBoundaries(int[] values, string[] display)
    {
        Dictionary<int[], string> result = new Dictionary<int[], string>();
        if (values.Length != 24)
        {
            Console.WriteLine("Array length is not valid. Expected length: 24");
            Environment.Exit(1); 
        }

        
        for (int i = 1; i < 24; i+=2)
        {
            result.Add(new int[] { values[i - 1], values[i] }, display[(i - 1) / 2]);
        }
        
        return result;


    }

    static string testBuildEcliptic(double pos, string date) //Was a pain to code - TODO - make it easier on the eyes and simpler if possible
    {
        int n = 6; //rows
        int m = 150; //columns - long so everything will fit

        //making array
        string emptyString = new string(' ', n * m);
        char[] charArray = emptyString.ToCharArray();
        for (int i = 0; i < n; i++)
        {
            charArray[i * m] = '\n';
        }


        string numbers = "0      30     60     90    120    150    180    210    240    270    300    330   360";
        string numberLine = "|------|------|------|------|------|------|------|------|------|------|------|------|"; 

        int desiredPos = (int)Math.Ceiling((double)pos / 360 * numbers.Length); //Place on the number line
        if (desiredPos == 0)
        {
            desiredPos++;
        }

        string dPosString = pos.ToString(); //Text to display in the correct position 

        int startColumn = 1; 

        for (int i = 0; i < numbers.Length && startColumn + i < m; i++)
        {
            charArray[0 * m + i + 1] = numbers[i];
            charArray[1 * m + i + 1] = numberLine[i];
        }
        charArray[4 * m + desiredPos] = '|';
        charArray[3 * m + desiredPos] = '|';
        charArray[2 * m + desiredPos] = '^';

        char[] msg = ("You are here (" +date+ " = " + dPosString + " degrees on the ecliptic)").ToCharArray();

        for (int i = 0; i < msg.Length; i++)
        {
            charArray[5 * m + desiredPos + i] = msg[i];
            
        }

        // Convert the character array back to a string
        string modifiedString = new string(charArray);


        return modifiedString;
    }
}


