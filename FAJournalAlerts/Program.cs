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
            int minuteDelay = 5; //default checks every 5 mins

            string hostUri = "http://faexport.boothale.net";
            string uriSuffix = "/user/";

            Console.WriteLine("Please enter the username to watch for: ");
            artist.username = Console.ReadLine();
            Console.WriteLine("Enter update frequency (in minutes, divisible by 5): ");
            try 
            {
                minuteDelay = Int32.Parse(Console.ReadLine());
                if (minuteDelay % 5 != 0 || minuteDelay < 1)
                    throw new System.Exception("Invalid number of minutes exception. Exiting program.");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
                Environment.Exit(1);
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
