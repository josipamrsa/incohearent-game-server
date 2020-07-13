using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IncohearentWebServer.Data;
using IncohearentWebServer.Models;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR;

namespace IncohearentWebServer
{
    public static class ConnectedUser
    {
        public static List<string> Id = new List<string>();        
    }
  
    public class GameHub : Hub
    {
        static RestApiService restApi { get; set; }
        public async Task JoinLobby(User user)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, user.PublicAddress);
            Lobby lobby = new Lobby(user.PublicAddress, user.PrivateAddress, user.UserId, true);
            await Clients.Groups(user.PublicAddress).SendAsync("JoinLobby", user, lobby);

            // Test za bilo koja 2 uredjaja
            //Lobby lobby = new Lobby(user.PublicAddress, user.PrivateAddress, user.UserId, true);
            //await Clients.All.SendAsync("JoinLobby", user, lobby);
        }

        public async Task LeaveLobby(User user)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, user.PublicAddress);
            await Clients.Groups(user.PublicAddress).SendAsync("LeaveLobby", user);

            // Test za bilo koja 2 uredjaja
            //await Clients.All.SendAsync("LeaveLobby", user);
        }

        public async Task GeneratePhraseList(User user, int p, int r)
        {
            List<PhoneticPhrases> generated = restApi.GeneratePhoneticEquivalents(p, r);
            // Slati listu svima odmah pri početku igre pa pratiti s drugom metodom tko je dobio koju frazu
            await Clients.Groups(user.PublicAddress).SendAsync("PhrasesGenerated", generated);
        }
        
        public async Task FollowPlayerAssignments()
        {

        }

        public override Task OnConnectedAsync()
        {
            ConnectedUser.Id.Add(Context.ConnectionId);
            //foreach (var id in ConnectedUser.Id) System.Diagnostics.Debug.WriteLine(id);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            ConnectedUser.Id.Remove(Context.ConnectionId);
            //foreach (var id in ConnectedUser.Id) System.Diagnostics.Debug.WriteLine(id);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
