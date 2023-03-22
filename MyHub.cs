using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace TotalFireSafety.Hubs
{
    public class MyHub : Hub
    {
        public async Task AddToGroup(string groupName)
        {
            await Groups.Add(Context.ConnectionId, groupName);
        }
        public async Task Send(string message)
        {
            await Clients.All.SendAsync(message);
        }
        public async Task SendToGroup(string groupName,string message)
        {
            await Clients.Group(groupName).SendAsyncGroup("SendMessage",groupName,message);
        }
        public void SendMessage(string message)
        {
            Clients.All.receiveMessage(message);
        }
    }
}