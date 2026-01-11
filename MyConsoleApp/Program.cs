//Using System Globalization to format date and time
using System.Globalization;

Console.WriteLine("Hello, World!");
DateTime now = DateTime.Now;
string formattedDate = now.ToString("hh:mm tt", new CultureInfo("en-US"));
Console.WriteLine($"The current time is {formattedDate}");
var daysUntilChristmas = (new DateTime(DateTime.Now.Year, 12, 25) - DateTime.Now).Days;
Console.WriteLine($"There are {daysUntilChristmas} days until the next Christmas");