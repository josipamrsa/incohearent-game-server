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
    /// <summary>
    /// 
    /// Testovi
    /// 
    /// TEST ZA BILO KOJE UREDJAJE - LOBBY ULAZ
    /// Lobby lobby = new Lobby(user.PublicAddress, user.PrivateAddress, user.UserId, true);
    /// await Clients.All.SendAsync("JoinLobby", user, lobby);
    /// 
    /// TEST ZA BILO KOJE UREDJAJE - LOBBY IZLAZ
    /// await Clients.All.SendAsync("LeaveLobby", user);
    /// 
    /// </summary>
    
    public static class ConnectedUser
    {
        public static List<string> Id = new List<string>();        
    }
  
    public class GameHub : Hub
    {
        static RestApiService restApi { get; set; }

        public GameHub()
        {
            restApi = new RestApiService();
        }

        //----Lobby----//

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

        //----Session----//

        public async Task StartGame(User user)
        {
            await Clients.Groups(user.PublicAddress).SendAsync("StartGame", user);
        }

        public async Task ConnectSession(User user)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, user.PublicAddress);
            await Clients.Groups(user.PublicAddress).SendAsync("ConnectSession", user);
        }

        public async Task DisconnectSession(User user)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, user.PublicAddress);
            await Clients.Groups(user.PublicAddress).SendAsync("DisconnectSession", user);
        }
       
        //----Phrases----//

        public async Task GeneratePhrases(User user, PhoneticPhrases generated)
        {
            generated = restApi.GeneratePhoneticEquivalents();
            if (!string.IsNullOrEmpty(generated.PhraseGenerated) && !string.IsNullOrEmpty(generated.PhrasePhonetic))
                await Clients.Groups(user.PublicAddress).SendAsync("PhrasesGenerated", generated);
            else
                await Clients.Groups(user.PublicAddress).SendAsync("PhrasesNotGenerated", generated);
        }       
       
        //----Events----//

        public override Task OnConnectedAsync()
        {
            ConnectedUser.Id.Add(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            ConnectedUser.Id.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
