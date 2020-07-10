using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IncohearentWebServer.Models
{
    public class Lobby
    {
        public int LobbyId { get; set; }
        public string GatewayIp { get; set; }
        public string UserIp { get; set; }
        public int UserId { get; set; }
        public bool IsActive { get; set; }

        public Lobby() { }
        public Lobby(string gip, string uip, int user, bool active)
        {
            this.GatewayIp = gip;
            this.UserIp = uip;
            this.UserId = user;
            this.IsActive = active;
        }
    }
}
