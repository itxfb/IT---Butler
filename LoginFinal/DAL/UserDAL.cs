using LoginFinal.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginFinal.DAL
{
    public class UserDAL
    {
        public List<User> GetAllUsersList(AppDbContext de)
        {
            return de.Users.ToList();
        }

        public bool DeleteEducationById(int id, AppDbContext de)
        {
            try
            {
                if(id!=-1)
                {
                    var useredu = GetEducationById(id, de);
                    if(useredu != null)
                    {
                        useredu.IsActive = 0;
                        useredu.DeletedAt = DateTime.Now;
                        de.Education.Update(useredu);
                        de.SaveChanges();
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public Education GetEducationById(int id,AppDbContext de)
        {
            return de.Education.Where(a => a.Id == id && a.IsActive == 1).FirstOrDefault();
        }
        public List<User> GetActiveUserList(AppDbContext de)
        {
            return de.Users.Where(x => x.IsActive == 1).ToList();
        }

        public List<Skills> GetAllSkills(AppDbContext de)
        {
            return de.Skills.Where(x=>x.IsActive==1).ToList();
        }

        public List<Tags> GetAllTags(AppDbContext de)
        {
            return de.Tags.Where(x => x.IsActive == 1).ToList();
        }

        public List<Logging> GetAllLogs(AppDbContext de)
        {
            return de.Logging.Where(x=>x.IsActive == 1).ToList();
        }
        public User GetUserById(int Id, AppDbContext de)
        {
            return de.Users.Where(x => x.Id == Id).FirstOrDefault(x=>x.IsActive==1);
        }

        public User GetActiveUserById(int Id, AppDbContext de)
        {
            return de.Users.Where(x => x.Id == Id).FirstOrDefault(x => x.IsActive != 0);
        }

        public Skills GetSkillsById(int Id, AppDbContext de)
        {
            return de.Skills.Where(a => a.UserId == Id && a.IsActive != 0).FirstOrDefault();
        }

        public Tags GetTagsById(int Id , AppDbContext de)
        {
            return de.Tags.Where(a => a.UserId == Id && a.IsActive != 0).FirstOrDefault();
        }
        public async Task<bool> AddUser(User _user, AppDbContext de)
        {
            try
            {
                de.Users.Add(_user);
                await de.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AddRefferal(Refferals r , AppDbContext de)
        {
            try
            {
                de.Refferals.Add(r);
                de.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<int> AddUser2(User _user, AppDbContext de)
        {
            try
            {
                de.Users.Add(_user);
                await de.SaveChangesAsync();

                return _user.Id;
            }
            catch
            {
                return -1;
            }
        }

        public async Task<bool> UpdateUser(User _user, AppDbContext de)
        {
            try
            {
                de.Users.Update(_user);
                await de.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<bool> DeleteUser(int id, AppDbContext de)
        {
            try
            {
                User u = de.Users.Find(id);
                var s = de.Skills.Where(a => a.UserId == id).ToList();
                var t = de.Tags.Where(a => a.UserId == id).ToList();
                if (s.Count() != 0)
                {
                    foreach (var x in s)
                    {
                        x.IsActive = 0;
                        x.DeletedAt = DateTime.Now;
                        de.Skills.Update(x);
                        de.SaveChanges();
                    }
                   
                }
                if (t.Count() != 0)
                {
                    foreach (var x in t)
                    {
                        x.IsActive = 0;
                        x.DeletedAt = DateTime.Now;
                        de.Tags.Update(x);
                        de.SaveChanges();
                    }
                   
                }
                u.IsActive = 0;
                u.DeletedAt = DateTime.Now;
                de.Entry(u).State = EntityState.Modified;
                await de.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }


        public bool AddLog(Logging l, AppDbContext de)
        {
            try
            {
                de.Logging.Add(l);
                de.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
