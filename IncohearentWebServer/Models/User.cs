using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IncohearentWebServer.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PrivateAddress { get; set; }
        public string PublicAddress { get; set; }
        public bool LoggedIn { get; set; }

        public User() { }
        public User(string username, string publAddr, string privAddr, bool log)
        {
            this.Username = username;
            this.PrivateAddress = privAddr;
            this.PublicAddress = publAddr;
            this.LoggedIn = log;
        }
    }
}
