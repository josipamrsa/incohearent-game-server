using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IncohearentWebServer.Models;
using Microsoft.AspNetCore.SignalR;

namespace IncohearentWebServer
{
    public static class ConnectedUser
    {
        public static List<string> Id = new List<string>();        
    }
    public class GameHub : Hub
    {       
        // Test push
        public async Task JoinLobby(User user)
        {          
            await Groups.AddToGroupAsync(Context.ConnectionId, user.PublicAddress);
            Lobby lobby = new Lobby(user.PublicAddress, user.PrivateAddress, user.UserId, true);
            await Clients.Groups(user.PublicAddress).SendAsync("JoinLobby", user, lobby);         
        }

        public async Task LeaveLobby(User user)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, user.PublicAddress);  
            await Clients.Groups(user.PublicAddress).SendAsync("LeaveLobby", user);
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
