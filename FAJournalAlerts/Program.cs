using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace FAJournalAlerts
{
    class Program
    {
        static void Main(string[] args)
        {
            WebClient myWebClient = new WebClient();
            User artist = new User();
            int minuteDelay = 1; //default checks every 1 mins

            string hostUri = "http://faexport.boothale.net";
            string uriSuffix = "/user/";

            Console.WriteLine("Please enter the username to watch for: ");
            artist.username = Console.ReadLine();

            Console.WriteLine("Enter update frequency (in minutes, 3 or greater): ");
            while (minuteDelay < 3)
            {
                try
                {
                    minuteDelay = Int32.Parse(Console.ReadLine());
                    if (minuteDelay < 3)
                        Console.WriteLine("Enter an update frequency of 3 or more mins.");
                }
                catch (System.FormatException)
                {
                    Console.WriteLine("Error: must enter a number.");
                }
            }
    
            uriSuffix += artist.username;
            uriSuffix += "/journals.json";

            myWebClient.BaseAddress = hostUri;
            while (true)
            {
                Console.WriteLine("Checking latest journal for " + artist.username);
                artist.setLatestJournal(myWebClient, uriSuffix);
                System.Threading.Thread.Sleep(minuteDelay * 60 * 1000); // (desired minutes * 60 * 1000)
            }
        }
    }
}
