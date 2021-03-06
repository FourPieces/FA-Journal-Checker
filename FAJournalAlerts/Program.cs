﻿using System;
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
            User[] artists = new User[5];
            int minuteDelay = 1; //default checks every 1 mins

            string hostUri = "http://faexport.boothale.net";

            myWebClient.BaseAddress = hostUri;

            Console.WriteLine("Please enter up to 5 usernames to watch for, one line at a time. Type !stop to stop entering names: ");
            int numArtists = 0;
            string answer = "";
            while (numArtists < 5 && answer != "!stop")
            {
                answer = Console.ReadLine();
                if (answer != "!stop")
                {
                    artists[numArtists] = new User(answer, myWebClient);
                    numArtists++;
                }
            }

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

            while (true)
            {
                for (int i = 0; i < numArtists; i++)
                {
                    Console.WriteLine("Checking latest journal for " + artists[i].getUsername());
                    artists[i].checkLatestJournalID();
                }
                System.Threading.Thread.Sleep(minuteDelay * 60 * 1000); // (desired minutes * 60 * 1000)
            }
        }
    }
}
