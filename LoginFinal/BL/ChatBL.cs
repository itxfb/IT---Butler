using LoginFinal.DAL;
using LoginFinal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginFinal.BL
{
    public class ChatBL
    {
        public readonly AppDbContext db;

        public ChatBL(AppDbContext db)
        {
            this.db = db;
        }
        public List<Message> GetAllChats()
        {
            return new ChatDAL(db).GetAllChats();
        }

        public Message GetChatById(int _id)
        {
            return new ChatDAL(db).GetChatById(_id);
        }

        public int AddChat(Message _chat)
        {
            return new ChatDAL(db).AddChat(_chat);
        }

        public bool ClearUnreadChart(int _id)
        {
            return new ChatDAL(db).ClearUnreadChart(_id);
        }

        public bool UpdatChart(Message _chat)
        {
            return new ChatDAL(db).UpdatChart(_chat);
        }


        public bool DeleteChat(int _id)
        {
            return new ChatDAL(db).DeleteChat(_id);
        }
    }
}