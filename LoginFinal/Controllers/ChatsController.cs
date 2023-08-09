using LoginFinal.BL;
using LoginFinal.HelpingClasses;
using LoginFinal.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using LoginFinal.DataHub;

namespace LoginFinal.Controllers
{
    public class ChatsController : Controller
    {
        private AppDbContext de;
        private readonly GeneralPurpose gp;
        public ChatsController(AppDbContext de, IHttpContextAccessor haccess)
            {
            this.de = de;
            this.gp = new GeneralPurpose(de, haccess);
        }

        #region Ajax


        [HttpPost]
        public ActionResult GetUserList() //used in Index.cshtml page
        {
            List<User> ulist = new UserBL().GetActiveUserList(de).Where(x => x.Id != gp.ValidateLoggedinUser().Id).OrderBy(x => x.FirstName).ToList();

            List<ContactDTO> clist = new List<ContactDTO>();

            int count = 0;
            int IsLoggedin = 0;

            foreach (User i in ulist)
            {
                count = new ChatBL(de).GetAllChats().Where(x => x.SenderId == i.Id && x.IsRead == 0).Count();

                if (i.ConnectionId != null)
                {
                    IsLoggedin = 1;
                }
                ContactDTO obj = new ContactDTO()
                {
                    Id = i.Id,
                    Name = i.FirstName,
                    Count = count,
                    IsLoggedin = IsLoggedin
                };

                clist.Add(obj);

                IsLoggedin = 0;
            }


            return Json(clist);
        }


        [HttpPost]
        public ActionResult GetUnreadChatCount(int SenderId) //used in Index.cshtml page
        {
            int count = new ChatBL(de).GetAllChats().Where(x => x.SenderId == SenderId && x.IsRead == 0).Count();

            ContactDTO obj = new ContactDTO()
            {
                Id = SenderId,
                Count = count
            };

            return Json(obj);
        }


        [HttpPost]
        public ActionResult GetClearUnreadChat(int SenderId) //thid function will be called when we open Chat.cshtml page
        {


            bool chk = new ChatBL(de).ClearUnreadChart(SenderId);

            if (chk == true)
            {
                //context.Clients.All.broadcastChatCount("1", SenderId); //this function is used in Index.cshtml, to get updated chat count
                return Json("1");
            }
            else
            {
                //context.Clients.All.broadcastChatCount("0", 0);

                return Json("0");
            }

        }




        [HttpPost]
        public ActionResult GetChat(int SenderId, int ReceiverId) //used to get chat messages in Chat.cshtml page
        {
            List<Message> clict = new ChatBL(de).GetAllChats().Where(x => (x.SenderId == SenderId && x.RecieverId == ReceiverId) || (x.RecieverId == SenderId && x.SenderId == ReceiverId)).ToList();

            List<Message> uclist = clict.Where(x => x.IsRead == 0).ToList();

            List<MessageDTO> mlist = new List<MessageDTO>();

            foreach (Message i in clict)
            {
                User u = new UserBL().GetUserById(Convert.ToInt32(i.SenderId),de);

                MessageDTO obj = new MessageDTO()
                {
                    Id = i.Id,
                    SenderId = u.Id,
                    SenderName = u.FirstName,
                    Message = i.Message_Description,
                };

                mlist.Add(obj);
            }

            return Json(mlist);
        }

        [HttpPost]
        public ActionResult GetChatCount(int SenderId, int ReceiverId) //Used in chat.scshtml page at top to get the count of total messages
        {
            int ccount = 0;

            ccount = new ChatBL(de).GetAllChats().Where(x => (x.SenderId == SenderId && x.RecieverId == ReceiverId) || (x.RecieverId == SenderId && x.SenderId == ReceiverId)).Count();

            return Json(ccount);
        }

        [HttpPost]
        public ActionResult GetUpdateChat(int Id)
        {
            Message c = new ChatBL(de).GetChatById(Id);

            return Json(c);
        }


        #endregion

        public void AddChat(int SenderId, int ReceiverId, string Message) //handling Chathub functions through back end
        {

            //var context = new DataHub.ChatHub(de,haccess);

            Message obj = new Message()
            {
                SenderId = SenderId,
                RecieverId = ReceiverId,
                Message_Description = Message,
                IsActive = 1,
                CreatedAt = DateTime.Now
            };

            int chk = new ChatBL(de).AddChat(obj);

            if (chk != -1)
            {
                User u = new UserBL().GetUserById(1,de);

                //context.Clients.All.broadcastMessage("1", u.Id, u.FirstName, chk, Message);
            }
            else
            {
                //context.Clients.All.broadcastMessage("0", "", "", "", "");
            }

        }


        public ActionResult Test(int ReceiverId = -1)
        {
            if (gp.ValidateLoggedinUser() != null)
            {
                ViewBag.ReceiverId = ReceiverId;
                ViewBag.SenderId = gp.ValidateLoggedinUser().Id;

                return View();
            }
            else
            {
                return RedirectToAction("login", "Auth");
            }

        }

    }
}
