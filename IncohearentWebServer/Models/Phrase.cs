using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IncohearentWebServer.Models
{
    public class Phrase
    {
        public int PhraseId { get; set; }
        public string PhraseGenerated { get; set; }
        public List<string> PhraseDissect { get; set; }
        public List<string> PhrasePhonetic { get; set; }

        public Phrase()
        {

        }
    }
}
