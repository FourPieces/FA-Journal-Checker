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
        public string username = "";
        public int latestJournal = -42;

        public User(string u)
        {
            username = u;
        }

        //Parses the ID of the latest journal and compares it to the last ID parsed, alerting the user if the new ID is larger
        public void setLatestJournal(WebClient w, string suffix)
        {
            Byte[] myDataBuffer = null;
            try
            {
                myDataBuffer = w.DownloadData(suffix); //reads the .json file specified by the suffix
            }
            catch (System.Net.WebException)
            {
                Console.WriteLine("404 error: Could not access webpage. Exiting.");
                Console.ReadLine();
                Environment.Exit(-1);
            }

            string download = Encoding.ASCII.GetString(myDataBuffer); //encode it as ASCII in a string file
            if (download.Length < 7) //IDs are 7 characters long. If the download is less than that, there are no journals to compare against.
            {
                download = "0";
                Console.WriteLine("User currently has no journals.");
            }
            else
            {
                download = download.Remove(14);
                download = download.Trim(new Char[] { ' ', '\n', '[', '\"', ',', ']', '\n' }); //trims the .json file to only include the most recent journal ID
            }

            //If the program hasn't been run before, initialize the journal as the latest journal
            if (latestJournal == -42)
            {
                latestJournal = Int32.Parse(download);
                Console.WriteLine("Initialized with latest journal (or 0 if no journals exist).\n");
            }

            //If the last latest journal has a smaller ID than the current latest journal, that means that the current journal is more recent than the last check
            //Alert the user if so
            else if (latestJournal < Int32.Parse(download))
            {
                latestJournal = Int32.Parse(download);
                MessageBox.Show(username + " posted a new journal!\n" + getJournalName(w), "New Journal!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Console.WriteLine("No new journals!");
            }
        }

        //Takes the .rss file of the journal and parses out the latest name
        private string getJournalName(WebClient w)
        {
            //takes the .rss file of all the journals and sets it as a string
            string suffix = "/user/" + username + "/journals.rss";
            byte[] myDataBuffer = w.DownloadData(suffix);
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
    }
}
