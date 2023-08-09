using System;

using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using LoginFinal.BL;
using LoginFinal.HelpingClasses;
using LoginFinal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace LoginFinal.DataHub
{
    
    public class ChatHub : Hub
    {

        private readonly AppDbContext de;
        private readonly GeneralPurpose gp;
        

        public ChatHub(AppDbContext de, IHttpContextAccessor haccess)
        {
            this.de = de;
            this.gp = new GeneralPurpose(de, haccess);
        }
        public async Task SendMessage(string username, string message2, string sndid, string recid)
        {
            Message message = new Message();
            //var dt = DateTime.Now.ToString("t");

            message.CreatedAt = DateTime.Now;
            message.Message_Description = message2.TrimEnd();
            message.SenderId = Convert.ToInt32(sndid);
            message.RecieverId = Convert.ToInt32(recid);
            var imgt = new UserBL().GetActiveUserById(message.SenderId, de).ImagePath;
            message.IsActive = 1;
            de.Messages.Add(message);
            await de.SaveChangesAsync();
            //await Clients.User(recid).SendAsync(message2);
           
                await Clients.All.SendAsync("newMessage", username, message2, message.RecieverId, imgt);
        }

    }
}