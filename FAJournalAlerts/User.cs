using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Forms;

namespace FAJournalAlerts
{
    class User
    {
        private string username = "";
        private string journalSuffix = "/user/";
        private int latestJournal = -42;
        private WebClient individualWebClient = new WebClient();

        public User(string u, WebClient w)
        {
            username = u;
            individualWebClient = w; //copies over the webclient to be used with this user
            appendSuffix(); //sets the suffix for that user's username
            initializeLatestJournal(); //initializes the latest journal to the current journal ID or 0 if no journals exist
        }

        //Downloads the journal ID for the user and returns it as an int
        private int downloadJournalID()
        {
            Byte[] myDataBuffer = null;
            try
            {
                myDataBuffer = individualWebClient.DownloadData(journalSuffix); //reads the .json file specified by the suffix
            }
            catch (System.Net.WebException)
            {
                Console.WriteLine("404 error: Could not access webpage. Exiting.");
                Console.ReadLine();
                Environment.Exit(-1);
            }

            return trimJournals(Encoding.ASCII.GetString(myDataBuffer)); //encode it as ASCII in a string file
        }
        
        //trims formatting from .json file to get only the most recent journal
        //If Length is too small, there is no journal, so return 0 as the ID
        private int trimJournals(string s)
        {
            if (s.Length < 7)
                return 0;
            try
            {
                s = s.Remove(14);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                Console.WriteLine("Unexpected journal length.\nPlease make sure the user you are checking has journal IDs of 7+ digits in length.");
                Environment.Exit(-1);
            }
            s = s.Trim(new Char[] { ' ', '\n', '[', '\"', ',', ']', '\n' });
            return Int32.Parse(s);
        }

        //sets lastestJournal for the user to the most recent journal ID
        private void initializeLatestJournal()
        {
            latestJournal = downloadJournalID();
            if (latestJournal > 0)
                Console.WriteLine("Initialized with latest journal for " + username + ". ID: " + latestJournal + " Title: " + getJournalName());
            else
                Console.WriteLine(username + " currently has no journals. Initialized with ID 0.");
        }

        //Takes the ID of the latest journal and compares it to the last ID stored, alerting the user if the new ID is larger
        public void setLatestJournal()
        {
            //If the last latest journal has a smaller ID than the current latest journal, that means that the current journal is more recent than the last check
            //Alert the user if so
            int currentJournalID = downloadJournalID();
            if (latestJournal < currentJournalID)
            {
                latestJournal = currentJournalID;
                MessageBox.Show(username + " posted a new journal!\n" + getJournalName(), "New Journal!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Console.WriteLine("No new journals!");
            }
        }

        //Takes the .rss file of the journal and parses out the latest name
        private string getJournalName()
        {
            //takes the .rss file of all the journals and sets it as a string
            string rssSuffix = "/user/" + username + "/journals.rss";
            byte[] myDataBuffer = individualWebClient.DownloadData(rssSuffix);
            string rssFile = Encoding.ASCII.GetString(myDataBuffer);

            //looks for the first instance of <item> <title> </title>, which is where the title of the most recent journal is
            int start, end;
            string strStart = "<item>\n  <title>";
            string strEnd = "</title>";

            //make a substring between <item>\n  <title> and </title>
            if (rssFile.Contains(strStart) && rssFile.Contains(strEnd))
            {
                start = rssFile.IndexOf(strStart, 0) + strStart.Length;
                end = rssFile.IndexOf(strEnd, start);
                return rssFile.Substring(start, end - start);
            }
            else
            {
                return "";
            }
        }

        private void appendSuffix()
        {
            journalSuffix += username;
            journalSuffix += "/journals.json";
        }

        public string getUsername()
        {
            return username;
        }
    }
}
