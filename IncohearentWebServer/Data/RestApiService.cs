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
        public RestApiService() { }

        /*
         
             REST API SERVIS
             -----------------------------------------------------------------------
             Služi za dohvat i pretvorbu fraza sa serverske strane. Tijek izvođenja
             je slijedeći: 

                > Dohvat liste engleskih fraza preko HTTP requesta sa RandomWordGeneratora 
                  (vraća cijelu JSON listu) te odabir nasumične fraze s tog popisa
                > Generiranje svih fonetskih ekvivalenata preko Datamuse API-ja za svaku
                  riječ u generiranoj frazi
                > S obzirom da API vraća mjeru podudarnosti dvije riječi, onda se evaluira
                  score za svaki dobiveni ekvivalent, te u listu mogućih kombinacija za 
                  pojedinu riječ idu samo oni ekvivalenti sa scoreom između 92 i 99. Može
                  se dogoditi da se fraza ne generira jer nijedan ekvivalent ne odgovara
                  prethodnom uvjetu, pa tome služi zastavica, pomoću koje se onda šalje
                  prazna fraza GameHubu, te on sa svoje strane šalje novi zahtjev REST API-ju
                > Ako je sve u redu, za svaku riječ i njene fonetske ekvivalente se formira
                  nasumična fraza od tih ekvivalenata
        */

        public PhoneticPhrases GeneratePhoneticEquivalents()
        {
            Phrases phrases = GetPhrase();
            return GetPhoneticEquivalents(phrases.Phrase);
        }

        private PhoneticPhrases GetPhoneticEquivalents(string phrase)
        {
            // Dohvaća i generira fonetske ekvivalente
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
            // Formira ekvivalentne fraze
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
            // Dohvaća listu frazi s API-ja
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
