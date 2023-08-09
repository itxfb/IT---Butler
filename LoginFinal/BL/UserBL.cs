using LoginFinal.DAL;
using LoginFinal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginFinal.BL
{
    public class UserBL
    {
        public List<User> GetAllUsersList(AppDbContext de)
        {
            return new UserDAL().GetAllUsersList(de);
        }

        public Education GetEducationById(int id, AppDbContext de)
        {
            return new UserDAL().GetEducationById(id, de);
        }
            public bool DeleteEducationById(int id, AppDbContext de)
        {
            return new UserDAL().DeleteEducationById(id, de);
        }
        public List<Logging> GetAllLogs(AppDbContext de)
        {
            return new UserDAL().GetAllLogs(de);
        }

        public List<Tags> GetAllTags(AppDbContext de)
        {
            return new UserDAL().GetAllTags(de);
        }
        public List<Skills> GetAllSkills(AppDbContext de)
        {
            return new UserDAL().GetAllSkills(de);

        }
        public List<User> GetActiveUserList(AppDbContext de)
        {
            return new UserDAL().GetActiveUserList(de);
        }


        public User GetUserById(int id, AppDbContext de)
        {
            return new UserDAL().GetUserById(id, de);
        }

        public User GetActiveUserById(int id, AppDbContext de)
        {
            return new UserDAL().GetActiveUserById(id, de);
        }


        public async Task<bool> AddUser(User _user, AppDbContext de)
        {
            if (String.IsNullOrEmpty(_user.Username) || String.IsNullOrEmpty(_user.Email) || String.IsNullOrEmpty(_user.Password))
                return false;

            return await new UserDAL().AddUser(_user, de);
        }

        public Skills GetSkillsById(int Id , AppDbContext de)
        {
            return new UserDAL().GetSkillsById(Id, de);
        }

        public Tags GetTagsById(int Id, AppDbContext de)
        {
            return new UserDAL().GetTagsById(Id, de);
        }
        public async Task<int> AddUser2(User _user, AppDbContext de)
        {
            if (String.IsNullOrEmpty(_user.Username) || String.IsNullOrEmpty(_user.Email) || String.IsNullOrEmpty(_user.Password))
                return -1;

            return await new UserDAL().AddUser2(_user, de);
        }

        public async Task<bool> UpdateUser(User _user, AppDbContext de)
        {
            return await new UserDAL().UpdateUser(_user, de);
        }

        public async Task<bool> DeleteUser(int id, AppDbContext de)
        {
            return await new UserDAL().DeleteUser(id, de);
        }


        public bool AddLog(Logging l,AppDbContext de)
        {
            return new UserDAL().AddLog(l,de);
        }

        public bool AddRefferal(Refferals l, AppDbContext de)
        {
            return new UserDAL().AddRefferal(l, de);
        }
    }
}
