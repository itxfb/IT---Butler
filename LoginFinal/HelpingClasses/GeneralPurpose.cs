using LoginFinal.BL;
using LoginFinal.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using LoginFinal.Controllers;
using static LoginFinal.HelpingClasses.ProjectVariables;

namespace LoginFinal.HelpingClasses
{
    //In order to use this class as service so that we can access it from front end we
    //need to register it as transient service, copy the following code in Startup.cs class
    //services.AddTransient<GeneralPurpose>();
    public class GeneralPurpose
    {
        private AppDbContext de;
        //need to register HttpContextAccessor in startup.cs class
        //copy the following code in startup.cs
        //services.AddHttpContextAccessor();
        private HttpContext hcontext;

        private IHttpContextAccessor haccess;
        public GeneralPurpose(AppDbContext de, IHttpContextAccessor haccess)
        {
            this.de = de;
            this.haccess = haccess;
            hcontext = haccess.HttpContext;
        }

        public User ValidateLoggedinUser()
        {
            string userId = hcontext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value;
            string name = hcontext.User.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value;
            string email = hcontext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            string role = hcontext.User.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;


            User loggedInUser = new UserBL().GetActiveUserById(Convert.ToInt32(userId), de);


            if (loggedInUser != null)
            {
                var wxy = new AjaxController(de, haccess).StatusUpdate(loggedInUser.Id, 1);
            }

            return loggedInUser;
        }

        public static DateTime DateTimeNow()
        {
            return DateTime.Now;
        }

        public bool ValidateEmail(string email = "", int id = -1)
        {
            int emailCount = 0;

            if (id != -1)
            {
                emailCount = new UserBL().GetAllUsersList(de).Where(x => x.IsActive != 0 && x.Email.ToLower() == email.ToLower() && x.Id != id).Count();
            }
            else
            {
                emailCount = new UserBL().GetAllUsersList(de).Where(x => x.IsActive != 0 && x.Email.ToLower() == email.ToLower()).Count();
            }

            if (emailCount > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool ValidateUsername(string username = "", int id = -1)
        {
            int userCount = 0;

            if (id != -1)
            {
                userCount = new UserBL().GetAllUsersList(de).Where(x => x.IsActive != 0 && x.Username.ToLower() == username.ToLower() && x.Id != id).Count();
            }
            else
            {
                userCount = new UserBL().GetAllUsersList(de).Where(x => x.IsActive != 0 && x.Username.ToLower() == username.ToLower()).Count();
            }

            if (userCount > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
