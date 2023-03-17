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
        public void Hello()
        {
            Clients.All.hello();
        }
        public async Task AddToGroup(string groupName)
        {
            await Groups.Add(HttpContext.Current.Session.SessionID, groupName);
        }
        public async Task Send(string message)
        {
            await Clients.All.SendAsync(message);
        }
        public async Task SendToGroup(string grpName, string message)
        {
            await Clients.Group(grpName).SendAsyncGroup(message);
        }
    }
}