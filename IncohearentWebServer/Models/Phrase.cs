using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IncohearentWebServer.Models
{
    public class Phrases
    {
        public string Phrase { get; set; }
        public string Description { get; set; }
        public Phrases() { }
        public Phrases(string p, string d)
        {
            Phrase = p;
            Description = d;
        }
    }

    public class PhoneticPhrases
    {
        public string PhraseGenerated { get; set; }
        public string PhrasePhonetic { get; set; }
        public PhoneticPhrases() { }
        public PhoneticPhrases(string gen, string phonetic)
        {
            PhraseGenerated = gen;
            PhrasePhonetic = phonetic;
        }
    }

    public class PhoneticEquivalentPair
    {
        public List<PhoneticEquivalent> Equivalent { get; set; }
        public string OriginalWord { get; set; }
        public PhoneticEquivalentPair() { }
        public PhoneticEquivalentPair(List<PhoneticEquivalent> pe, string ow)
        {
            Equivalent = pe;
            OriginalWord = ow;
        }
    }

    public class PhoneticEquivalent
    {
        public string Word { get; set; }
        public int Score { get; set; }
        public int NumSyllables { get; set; }
        public PhoneticEquivalent() { }
        public PhoneticEquivalent(string w, int s, int n)
        {
            Word = w;
            Score = s;
            NumSyllables = n;
        }
    }
}
