using LoginFinal.BL;
using LoginFinal.HelpingClasses;
using LoginFinal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static LoginFinal.HelpingClasses.ProjectVariables;

namespace LoginFinal.Controllers
{
    public class AjaxController : Controller
    {
        private readonly AppDbContext de;
        private readonly GeneralPurpose gp;

        public AjaxController(AppDbContext de, IHttpContextAccessor haccess)
        {
            this.de = de;
            this.gp = new GeneralPurpose(de, haccess);
            var request = haccess.HttpContext.Request;
            baseUrl = $"{request.Scheme}://{request.Host}";
        }


        #region User
        [HttpPost]
        public IActionResult GetUserDataTableList(int category = -1, string Name = "", string email = "")
        {
            List<User> ulist = new UserBL().GetAllUsersList(de).Where(x => x.IsActive != 0 && x.Role != 1).ToList();
            if (category == 4)
            {
                ulist = ulist.Where(x => x.Role == 4).ToList();
            }
            if (category == 3)
            {
                ulist = ulist.Where(x => x.Role == 3).ToList();
            }
            if (!String.IsNullOrEmpty(Name))
            {
                ulist = ulist.Where(x => x.Username.ToLower().Contains(Name.ToLower())).ToList();
            }

            if (!String.IsNullOrEmpty(email))
            {
                ulist = ulist.Where(x => x.Email.ToLower().Contains(email.ToLower())).ToList();
            }

            int start = Convert.ToInt32(Request.Form["start"].FirstOrDefault());
            int length = Convert.ToInt32(Request.Form["length"].FirstOrDefault());
            string searchValue = Request.Form["search[value]"].FirstOrDefault();
            string sortColumnName = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"];
            string sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();

            if (sortColumnName != "" && sortColumnName != null)
            {
                if (sortColumnName != "0")
                {
                    if (sortDirection == "asc")
                    {
                        ulist = ulist.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                    }
                    else
                    {
                        ulist = ulist.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                    }
                }
            }

            int totalrows = ulist.Count();

            //filter
            if (!string.IsNullOrEmpty(searchValue))
            {
                ulist = ulist.Where(x => x.Email.ToLower().Contains(searchValue.ToLower()) ||
                                    x.Username != null && x.Username.ToLower().Contains(searchValue.ToLower())
                                    ).ToList();
            }

            int totalrowsafterfilterinig = ulist.Count();


            // pagination
            ulist = ulist.Skip(start).Take(length).ToList();

            List<UserDto> udto = new List<UserDto>();

            foreach (User u in ulist)
            {
                UserDto obj = new UserDto()
                {
                    Id = u.Id,
                    EncId = StringCipher.EncryptId(u.Id),
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    UserName = u.Username,
                    Email = u.Email,
                    Contact = u.Contact,
                    Encemail = StringCipher.Base64Encode(u.Email),
                    IsActive = (int)u.IsActive
                };

                udto.Add(obj);
            }

            return Json(new { data = udto, draw = Request.Form["draw"].FirstOrDefault(), recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig });
        }

        public IActionResult GetUserRefferal(string Name = "", string email = "")
        {
            List<Refferals> ulist = de.Refferals.Where(x=>x.RefferalCode==gp.ValidateLoggedinUser().Refferal_Code).Include(a=>a.RefferedUser).ToList();
           
            if (!String.IsNullOrEmpty(Name))
            {
                ulist = ulist.Where(x => x.RefferedUser.Username.ToLower().Contains(Name.ToLower())).ToList();
            }

            if (!String.IsNullOrEmpty(email))
            {
                ulist = ulist.Where(x => x.RefferedUser.Email.ToLower().Contains(email.ToLower())).ToList();
            }

            int start = Convert.ToInt32(Request.Form["start"].FirstOrDefault());
            int length = Convert.ToInt32(Request.Form["length"].FirstOrDefault());
            string searchValue = Request.Form["search[value]"].FirstOrDefault();
            string sortColumnName = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"];
            string sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();

            if (sortColumnName != "" && sortColumnName != null)
            {
                if (sortColumnName != "0")
                {
                    if (sortDirection == "asc")
                    {
                        ulist = ulist.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                    }
                    else
                    {
                        ulist = ulist.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                    }
                }
            }

            int totalrows = ulist.Count();

            //filter
            if (!string.IsNullOrEmpty(searchValue))
            {
                ulist = ulist.Where(x => x.RefferedUser.Email.ToLower().Contains(searchValue.ToLower()) ||
                                    x.RefferedUser.Username != null && x.RefferedUser.Username.ToLower().Contains(searchValue.ToLower())
                                    ).ToList();
            }

            int totalrowsafterfilterinig = ulist.Count();


            // pagination
            ulist = ulist.Skip(start).Take(length).ToList();

            List<UserDto> udto = new List<UserDto>();

            foreach (var u in ulist)
            {
                UserDto obj = new UserDto()
                {
                    Id = u.RefferedUser.Id,
                    FirstName = u.RefferedUser.FirstName,
                    LastName = u.RefferedUser.LastName,
                    UserName = u.RefferedUser.Username,
                    Email = u.RefferedUser.Email,
                    Role = u.RefferedUser.Role,
                    Refferal_Code= u.RefferalCode,
                    IsActive = u.RefferalType,
                };

                udto.Add(obj);
            }

            return Json(new { data = udto, draw = Request.Form["draw"].FirstOrDefault(), recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig });
        }
        public bool StatusUpdate(int id, int val)
        {
            var user = new UserBL().GetActiveUserById(id, de);
            user.Status = val;
            var update = new UserBL().UpdateUser(user, de);
            if (update.Result == true)
                return true;
            else
                return false;
        }

        [HttpPost]
        public IActionResult GetUserById(int id)
        {
            User u = new UserBL().GetActiveUserById(id, de);
            if (u == null)
            {
                return Json(0);
            }

            UserDto obj = new UserDto()
            {
                Id = u.Id,
                EncId = StringCipher.EncryptId(u.Id),
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Contact = u.Contact,
                Password = StringCipher.Decrypt(u.Password),
                Country = u.Country,
                Description = u.Description,
                UserName = u.Username,
                Gender = u.Gender,
                Website = u.Website,
                ZipCode = u.ZipCode,
                City = u.City,
                Organization = u.Organization,

            };

            return Json(obj);
        }
        #endregion

        #region ValidateUser
        [HttpPost]
        public IActionResult ValidateEmail(string email, int id = -1)
        {
            return Json(gp.ValidateEmail(email, id));
        }

        [HttpPost]
        public IActionResult ValidateUsername(string username, int id = -1)
        {
            return Json(gp.ValidateUsername(username, id));
        }
        #endregion


        #region Messages

        [HttpPost]
        public string GetChatList(string Name="")
        {
            try
            {
                var userr = gp.ValidateLoggedinUser();
                var login = userr.Id;

                var frstrec = new List<Message>();

                frstrec = de.Messages.Where(a => a.RecieverId == login || a.SenderId == login && a.IsActive == 1).Include(p => p.Users).Include(p => p.RecieverEnd).ToList();

                if (!String.IsNullOrEmpty(Name))
                {
                    var usern= new UserBL().GetActiveUserList(de).Where(x=>x.FirstName.ToLower().Contains(Name.ToLower()) || x.LastName.ToLower().Contains(Name.ToLower())).ToList();
                    frstrec = frstrec.Where(a => a.RecieverId == usern[0].Id || a.SenderId==usern[0].Id).ToList();
                }
                    if (frstrec.Count() != 0)
                {
                    frstrec = frstrec.OrderByDescending(a => a.CreatedAt).ToList();
                    return JsonConvert.SerializeObject(frstrec, Formatting.Indented, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                }
                else
                {
                    return "";
                }

            }
            catch
            {
                return "";
            }

        }
        
        [HttpPost]
        public string GetAllChats(int recieverid = -1)
        {
            var login = gp.ValidateLoggedinUser().Id;
            if (recieverid != -1)
            {
                var msgz = de.Messages.Where(a => a.SenderId == login || a.RecieverId == login).Include(a => a.Users).Include(a => a.RecieverEnd).ToList();
                msgz = msgz.OrderBy(a => a.CreatedAt).ToList();


                return JsonConvert.SerializeObject(msgz, Formatting.Indented, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            }
            return "";
        }
        #endregion

        #region Education
        public bool DeleteEducation(int id=-1)
        {
            try
            {
                if(id!=-1)
                {
                    var check= new UserBL().DeleteEducationById(id, de);
                    return check;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

      
        public bool UpdateEducation(int id=-1,string ins="", string deg="", string sd="", string ed="")
        {


            try
            {



                if (id != -1)
                {
                    var n = new UserBL().GetEducationById(id, de);
                    if (n != null)
                    {
                        n.StartDate = Convert.ToDateTime(sd);
                        n.EndDate = Convert.ToDateTime(ed);
                        n.InstituteName = ins;
                        n.DegreeName = deg;
                        n.UpdatedAt = DateTime.Now;
                        de.Education.Update(n);
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
        #endregion

    }
}

