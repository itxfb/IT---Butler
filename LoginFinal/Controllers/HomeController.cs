using LoginFinal.Models;
using Microsoft.AspNetCore.Mvc;
using LoginFinal.HelpingClasses;
using Microsoft.AspNetCore.Http;
using LoginFinal.BL;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Reflection;
using X.PagedList;
using static LoginFinal.HelpingClasses.ProjectVariables;


namespace LoginFinal.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly AppDbContext de;
        private readonly GeneralPurpose gp;
        public HomeController(AppDbContext de, IHttpContextAccessor haccess)
        {
            this.de = de;
            this.gp = new GeneralPurpose(de, haccess);
            var request = haccess.HttpContext.Request;
            baseUrl = $"{request.Scheme}://{request.Host}";
        }
       
        
        #region LandingPage
        //Landing Page
        public IActionResult Index(string msg="")
        {
            ViewBag.msg = msg;
            var getLogs = new List<Logging>();
            var get_all_ids = new List<int>();
            var getTags = new UserBL().GetAllSkills(de);
            var pop = new List<String>();
            var rv = new List<User>();
            var recm = new List<User>();
            if(gp.ValidateLoggedinUser()!=null)
            {
                getLogs= new UserBL().GetAllLogs(de).Where(a=>a.UserId== gp.ValidateLoggedinUser().Id).ToList();
                if (getLogs.Count() != 0)
                {
                    get_all_ids = new UserBL().GetAllLogs(de).Where(a => a.SearchKeyword.Contains(getLogs[0].SearchKeyword)).Select(x => x.UserId).ToList();
                }
            }

           
            foreach (var x in get_all_ids)
            {
                var recmd = new UserBL().GetUserById(x, de);
                if (!recm.Contains(recmd))
                {
                    if (recmd != null)
                    {
                        recm.Add(recmd);
                    }
                }
            }
            foreach (var x in getLogs)
            {
            var recent = new UserBL().GetUserById(x.UserId,de);
                if(!rv.Contains(recent))
                {
                    if (recent != null)
                    {
                        rv.Add(recent);
                    }
                }
            }
            foreach (var x in getTags)
            {
                var y = x.SkillName.Split(",");
                foreach (var z in y)
                {
                    
                        pop.Add(z);
                }
            }
            if (rv.Count() != 0)
            {
                rv = rv.OrderByDescending(a => a.CreatedAt).ToList();
            }
            pop = pop.GroupBy(i => i)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => g.Key).Distinct().ToList();
            ViewBag.Popular = pop;
            ViewBag.rv = rv;
            ViewBag.recm = recm;
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }
        #endregion

        #region Search


        [Route("Search")]
        public IActionResult Search(string search, int currentPage = 1)
        {
            try
            {
                ViewBag.srch = search;
                if (search == null || search == "")
                {
                    return RedirectToAction("Index");
                }

                //Saving Search For Profile Refresh
                ViewBag.search = search;

                if (gp.ValidateLoggedinUser() != null)
                {
                    Logging log = new Logging
                    {
                        SearchKeyword = search.ToLower(),
                        CreatedAt = DateTime.Now,
                        IsActive = 1,
                        UserId = gp.ValidateLoggedinUser().Id,
                    };
                    var logd = new UserBL().AddLog(log, de);
                }
                //List for Getting All Selected Users
                List<User> users = new List<User>();

                //spliting search keywords
                string[] search2 = search.Split(" ");

                for (int i = 0; i < search2.Length; i++)
                {
                    //Matching Results

                    //Getting all skills 
                    var skills = new UserBL().GetAllSkills(de).Where(x => x.SkillName.Contains(search2[i].ToLower())).ToList();

                    //Getting Tags
                    var tags = new UserBL().GetAllTags(de).Where(x => x.TagName.Contains(search2[i].ToLower())).ToList();

                    //Getting Matching UserName or Name
                    var usern = new UserBL().GetActiveUserList(de).Where(x => (x.FirstName.ToLower() == search2[i].ToLower()) || (x.FirstName.ToLower() == search2[0].ToLower() && x.LastName.ToLower().Contains(search2[i].ToLower()) || (x.Username.ToLower() == search2[i].ToLower()))).ToList();
                    usern = usern.Where(x => x.Role == 3).ToList();


                    //If doesn't match skills
                    if (skills.Count == 0)
                    {
                        //if match tags
                        if (tags.Count != 0)
                        {
                            foreach (var tag in tags)
                            {
                                var user = new UserBL().GetUserById(tag.UserId, de);
                                if (!users.Contains(user))
                                {
                                    users.Add(user);
                                }
                            }

                            //Collect all users after last ilteration
                            if (i == search2.Length - 1)
                            {
                                if (users != null)
                                {
                                    users = users.OrderByDescending(a => a.Stars).ThenBy(a => a.Status == 0).ToList();
                                }
                                var pagedlist = users.ToPagedList(currentPage, 12);
                                return View(pagedlist);
                            }

                        }
                    }
                    //If Matches Skills
                    if (skills.Count != 0)
                    {

                        //get User Matching Skills
                        foreach (var skill in skills)
                        {
                            var user = new UserBL().GetUserById(skill.UserId, de);
                            if (!users.Contains(user))
                            {
                                if (user != null && user.Stars == null)
                                {
                                    user.Stars = 0;
                                }
                                if (user != null)
                                {
                                    users.Add(user);
                                }
                            }

                        }

                        //Collect all users after last ilteration
                        if (i == search2.Length - 1)
                        {
                            if (users.Count() != 0)
                            {
                                users = users.OrderByDescending(a => a.Stars).ThenBy(a => a.Status == 0).ToList();
                                var pagedlist = users.ToPagedList(currentPage, 12);
                                return View(pagedlist);
                            }

                        }
                    }



                    //If keywords matches username or name
                    if (usern.Count != 0)
                    {
                        foreach (var w in usern)
                        {
                            if (!users.Contains(w))
                            {
                                if (w != null && w.Stars == null)
                                {
                                    w.Stars = 0;
                                }
                                if (w != null)
                                {
                                    users.Add(w);
                                }
                            }
                        }


                        //Collect all users after last ilteration
                        if (i == search2.Length - 1)
                        {
                            if (users.Count() != 0)
                            {
                                users = users.OrderByDescending(a => a.Stars).ThenBy(a => a.Status == 0).ToList();

                                var pagedlist = users.ToPagedList(currentPage, 12);
                                if (pagedlist.Count! != 0)
                                {
                                    return View(pagedlist);
                                }
                            }
                            return RedirectToAction("NoResultFound", new { srch=search});
                        }
                    }
                }
                return RedirectToAction("NoResultFound", new { srch = search });
            }
            catch
            {
                return RedirectToAction("NoResultFound", new { srch = search });
            }
           
        }

        [AllowAnonymous]
        [Route("Preview")]
        public IActionResult UserProfile(string idx,int pr=-1, string keyword="")
        {

            var id = StringCipher.DecryptId(idx);
            ViewBag.srch = keyword;

            //Logging Opened Profile
            if(keyword!="" && gp.ValidateLoggedinUser() != null)
            {
                Logging log = new Logging
                {
                    IsActive = 1,
                    CreatedAt = DateTime.Now,
                    SearchKeyword= keyword,
                    ProfileId=id,
                    UserId= gp.ValidateLoggedinUser().Id
                };
                var logd = new UserBL().AddLog(log, de);
            }
            //Getting User Details
            var user = de.Users.Where(a=>a.IsActive==1 && a.Id==id && a.Role==3).Include(assets=>assets.Skills).Include(y=>y.Education).Include(z=>z.Tags).ToList();
            
            //If User Preview Own Profile
            if (pr != -1)
            {
                user = de.Users.Where(a=>a.Id == id && a.IsActive!=0).Include(assets => assets.Skills).Include(y => y.Education).Include(z => z.Tags).ToList();
              
            }
            if (user.Count != 0)
            {
                ViewBag.Status = user[0].Status;
                return View(user);
            }
            return RedirectToAction("NoResultFound", new {msg="No Result Found"});
       }

        [AllowAnonymous]
        [Route("NoResultFound")]
        public IActionResult NoResultFound(string srch="")
        {
            ViewBag.srch = srch;
            return View();
        }
        #endregion

        public IActionResult Register(string value = "", string rdx="")
        {
            ViewBag.Category = value;
            ViewBag.rdx = rdx;
            return View();
        }


        #region Message
        //Get Message Page

        [Authorize]
        public IActionResult Messages(string rec = "", string refx="")
        {

            if (rec != "")
            {
                var getUserdet = new UserBL().GetUserById(StringCipher.DecryptId(rec), de);
                var chk = new ChatBL(de).GetAllChats().Where(a => a.SenderId == gp.ValidateLoggedinUser().Id && a.RecieverId == StringCipher.DecryptId(rec)).FirstOrDefault();
                if (chk == null)
                {
                    Message m = new Message();
                    m.CreatedAt = DateTime.Now;
                    m.SenderId = gp.ValidateLoggedinUser().Id;
                    m.RecieverId = StringCipher.DecryptId(rec);
                    m.IsActive = 1;
                    m.Message_Description = "Hi! I am interested in your service! Please reach me back asap. Thanks!";
                    var msg = new ChatBL(de).AddChat(m);
                }
                if(refx!="" && refx!=gp.ValidateLoggedinUser().Refferal_Code && chk==null)
                {
                    var getLoggedinUser = gp.ValidateLoggedinUser();
                    var refferal = new Refferals
                    {
                        RefferalCode = refx,
                        RefferalType=1,
                        RefferedId= getLoggedinUser.Id,
                        CreatedAt = DateTime.Now,
                        IsActive=1,
                    };
                    var addref = new UserBL().AddRefferal(refferal, de);
                }
            }
            ViewBag.SenderId = gp.ValidateLoggedinUser().Id;
            return View();
        }
        #endregion


    }
}
