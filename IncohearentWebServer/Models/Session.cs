using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IncohearentWebServer.Models
{
    public class Session
    {
        public int SessionId { get; set; }
        public string GameType { get; set; }
        public int UserId { get; set; }
        public int RoundNum { get; set; }
        public int PlayerNum { get; set; }

        public Session()
        {

        }
    }
}
