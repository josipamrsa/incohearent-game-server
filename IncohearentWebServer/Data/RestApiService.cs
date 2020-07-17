using IncohearentWebServer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace IncohearentWebServer.Data
{
    public class RestApiService
    {
        public PhoneticPhrases PhoneticPhrase { get; set; }
        public RestApiService() { }
        public PhoneticPhrases GeneratePhoneticEquivalents()
        {
            Phrases phrases = GetPhrase();
            return GetPhoneticEquivalents(phrases.Phrase);
        }

        private PhoneticPhrases GetPhoneticEquivalents(string phrase)
        {
            List<PhoneticEquivalentPair> phoneticPairs = new List<PhoneticEquivalentPair>();
            bool flag = false;
            string[] dissected = phrase.Split();
            foreach (string word in dissected)
            {
                var request = (HttpWebRequest)WebRequest.Create("https://api.datamuse.com/words?sl=" + word);
                request.UserAgent = "curl";
                request.Method = "GET";
                var responseData = "";
                using (WebResponse response = request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseData = reader.ReadToEnd();
                    }
                }

                
                PhoneticEquivalent[] phonEquiv = JsonConvert.DeserializeObject<List<PhoneticEquivalent>>(responseData).ToArray();
                List<PhoneticEquivalent> highestScorePhonetics = new List<PhoneticEquivalent>();

                foreach (PhoneticEquivalent pe in phonEquiv)
                    if (pe.Score > 92 && pe.Score <= 99) { highestScorePhonetics.Add(pe); }

                if (highestScorePhonetics.Count == 0)
                    flag = true;

                phoneticPairs.Add(new PhoneticEquivalentPair(highestScorePhonetics, word));
            }

            if (flag)
                return new PhoneticPhrases("", "");
            else
            {
                PhoneticPhrases phoneticPhrase = GenerateRandomPhonetic(phoneticPairs, phrase);             
                return phoneticPhrase;
            }          
        }

        private PhoneticPhrases GenerateRandomPhonetic(List<PhoneticEquivalentPair> pep, string original)
        {
            Random rnd = new Random();
            List<string> generated = new List<string>();

            foreach (PhoneticEquivalentPair p in pep)
            {
                List<string> words = new List<string>();
                foreach (PhoneticEquivalent e in p.Equivalent) words.Add(e.Word);
                generated.Add(words[rnd.Next(0, words.ToArray().Length)]);
            }

            return new PhoneticPhrases(original, string.Join(" ", generated.ToArray()));
        }

        private Phrases GetPhrase()
        {
            var request = (HttpWebRequest)WebRequest.Create("https://randomwordgenerator.com/json/phrases.json");
            request.UserAgent = "curl";
            request.Method = "GET";
            var responseData = "";
            using (WebResponse response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    responseData = reader.ReadToEnd();
                }
            }

            Phrases[] phraseList = JsonConvert.DeserializeObject<List<Phrases>>(responseData.Substring(8, responseData.Length - 9)).ToArray();
            Random rnd = new Random();
            Phrases randomPhrases = phraseList[rnd.Next(0, phraseList.Length)];

            return randomPhrases;

        }
    }
}
