using LoginFinal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginFinal.DAL
{
    public class ChatDAL
    {
        public readonly AppDbContext db;

        public ChatDAL(AppDbContext db)
        {
            this.db = db;
        }
        public List<Message> GetAllChats()
        {
            List<Message> clist = db.Messages.Where(x => x.IsActive == 1).ToList();

            return clist;
        }

        public Message GetChatById(int _id)
        {
            Message chat = new Message();

            chat = db.Messages.Where(x => x.Id == _id).FirstOrDefault();

            return chat;
        }

        public int AddChat(Message _chat)
        {
            try
            {
                db.Messages.Add(_chat);
                db.SaveChanges();

                return _chat.Id;
            }
            catch
            {
                return -1;
            }
        }


        public bool UpdatChart(Message _chat)
        {
            try
            {
                db.Update(_chat);
                db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ClearUnreadChart(int _id)
        {
            try
            {
                using (db)
                {
                    db.Messages.Where(x => x.SenderId == _id && x.IsRead == 0).ToList().ForEach(x => x.IsRead = 1);
                    db.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteChat(int _id)
        {
            try
            {
                db.Messages.RemoveRange(db.Messages.Where(X => X.Id == _id));
                db.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}