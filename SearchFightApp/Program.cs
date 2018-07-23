using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Web;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Globalization;

namespace SearFightApp
{
   
    public class SearchEngine
    {
        private const string googleUrl = "https://www.google.com/search?hl=en&lr=lang_en&q=";
        private const string yahooUrl = "https://search.yahoo.com/search?p=";
        private const string bingUrl = "https://www.bing.com/search?mkt=en-US&q=";
        private string searchKeyword;
        
        WebClient web = new WebClient();

        //set user input
        public void setUserInput(string langToSearch)
        {
            searchKeyword = WebUtility.UrlEncode(langToSearch);
            //searchKeyword = langToSearch;
        }

        //returns the complete url with keyword to search
        public string getUrl(string searchEngine)
        {
            switch (searchEngine)
            {
                case "Google":
                    return googleUrl + searchKeyword;
                case "Yahoo":
                    return yahooUrl + searchKeyword;
                case "Bing":
                    return bingUrl + searchKeyword;
                default:
                    return "URL NOT FOUND";
            }
        }

        //returns the number of results in Google search
        public KeyValuePair<string, long> googleSearchResult()
        {
            long numberOfHits = 0;

            string htmlPage = web.DownloadString(getUrl("Google"));

            Regex r = new Regex(@"(<div*?id=resultStats>*)*[0-9,]+(?=\sresults)");
            Match m = r.Match(htmlPage);


            string totalResultsString = m.Value;
            numberOfHits = Int64.Parse(totalResultsString.Replace(",", ""));

            return (new KeyValuePair<string, long>("Google", numberOfHits));
        }

        public KeyValuePair<string, long> yahooSearchResult()
        {
            long numberOfHits = 0;

            string htmlPage = web.DownloadString(getUrl("Yahoo"));

            Regex r = new Regex(@"(<span*?id=resultStats>*)*[0-9,]+(?=\sresults)");
            Match m = r.Match(htmlPage);

            string totalResultsString = m.Value;
            numberOfHits = Int64.Parse(totalResultsString.Replace(",", ""));

            return (new KeyValuePair<string, long>("Yahoo", numberOfHits));
        }

        public KeyValuePair<string, long> bingSearchResult()
        {
            long numberOfHits = 0;

           string htmlPage = web.DownloadString(getUrl("Bing"));

            Regex r = new Regex(@"(<div*?id=resultStats>*)*[0-9,]+(?=\sresults)");
            Match m = r.Match(htmlPage);

            string totalResultsString = m.Value;
            numberOfHits = Int64.Parse(totalResultsString.Replace(",", ""));

            return (new KeyValuePair<string, long>("Bing", numberOfHits));
        }


        //for testing html view on external file.
        static async void WriteTextAsync(string text)
        {
            // Set a variable to the My Documents path.
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Write the text asynchronously to a new file named "WriteTextAsync.txt".
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(mydocpath, "WriteTextAsync.txt")))
            {
                await outputFile.WriteAsync(text);
            }
        }

    }

    class LanguageData
    {
        private List<KeyValuePair<string, long>> results = new List<KeyValuePair<string, long>>();
        public string langName;

        SearchEngine s = new SearchEngine();
        

        public LanguageData(string name)
        {
            langName = name;
            s.setUserInput(name);
            results.Add(s.googleSearchResult());
            results.Add(s.yahooSearchResult());
            results.Add(s.bingSearchResult());
        }

        public List<KeyValuePair<string, long>> getResultsList()
        {
            return results;
        }
    }

    
    class Searchfight
    {
        
        static void Main(string[] args)
        {

            string[] langsToSearch;
            string userInput;
       
            var winners = new Dictionary<string, long>();
            var winnersT = new Dictionary<string, Dictionary<string, long>>();

            long winnerHighest=0;

          //bool empty = true;
            List<LanguageData> data = new List<LanguageData>();

            if (args.Length == 0) //Checking for input Error
            {
                Console.WriteLine("Please search for at least one language");
                Environment.Exit(0);
            }
            
            //do
            //{
            //    Console.WriteLine("Enter the languages to search");
            //    userInput = Console.ReadLine();


            //    if (userInput == String.Empty || userInput == null)
            //    {
            //        Console.WriteLine("Please search for at least one language", "Name");
            //    }
            //    else
            //        empty = false;
            //} while (empty == true);


            SearchEngine s = new SearchEngine();


            //langsToSearch = Regex.Split(userInput, "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            
            for (int i = 0; i< args.Length; i++)
            {
                args[i] = args[i].Trim('"');
                LanguageData lang = new LanguageData(args[i]);
                data.Add(lang);
            }
            
            foreach (LanguageData language in data)
            {
                Console.Write($"{language.langName}: ");
               
                foreach(KeyValuePair<string, long> result in language.getResultsList())
                {
                    if (winners == null || winners.Count == 0)
                    {
                        
                        winners[result.Key] = result.Value;
                        var dic = new Dictionary<string, long>();
                        dic[language.langName] = result.Value;
                        winnersT[result.Key + " winner"] = dic;
                    }
                    else
                    {
                        if(winners.ContainsKey(result.Key))
                        {
                            if (winners[result.Key] < result.Value)
                            {
                                winners[result.Key] = result.Value;
                                var dica = new Dictionary<string, long>();
                                dica[language.langName] = result.Value;
                                winnersT[result.Key + " winner"] = dica;
                            }
                        }
                        else
                        {
                            winners[result.Key] = result.Value;
                            var dic = new Dictionary<string, long>();
                            dic[language.langName] = result.Value;
                            winnersT[result.Key + " winner"] = dic;
                        }

                    }


                    Console.Write($" {result.Key} = {result.Value}");
                }
                Console.WriteLine("");
            }

            string totalWinner ="";
            foreach(KeyValuePair<string, Dictionary<string, long>> wins in winnersT)
            {
                foreach(KeyValuePair<string, long> win in wins.Value)
                {
                    Console.WriteLine($"{wins.Key}: {win.Key}");
                    if(win.Value> winnerHighest)
                    {
                        winnerHighest = win.Value;
                        totalWinner = win.Key;
                    }
                }
            }

            Console.WriteLine($"Total Winner : {totalWinner}");

            Console.ReadKey();

           

        }

       


    }
}

