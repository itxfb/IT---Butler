using LoginFinal.BL;
using LoginFinal.HelpingClasses;
using LoginFinal.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using static LoginFinal.HelpingClasses.ProjectVariables;
using Microsoft.AspNetCore.Identity;

namespace LoginFinal.Controllers
{
    public class AuthController : Controller
    {   
        private readonly AppDbContext de;
        private readonly GeneralPurpose gp;
        private readonly IHttpContextAccessor _haccess;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthController(AppDbContext de, IHttpContextAccessor haccess,UserManager<IdentityUser> _userManager, SignInManager<IdentityUser> _signInManager)
        {
            this.de = de;
            this.gp = new GeneralPurpose(de, haccess);
            _haccess = haccess;
            var request = haccess.HttpContext.Request;
            baseUrl = $"{request.Scheme}://{request.Host}";
            this._userManager = _userManager;
            this._signInManager = _signInManager;
        }

        #region Login
        public IActionResult Login(string msg = "", string color = "black")
        {
            User loggedinUser = gp.ValidateLoggedinUser();

            if(loggedinUser != null)
            {
                if (loggedinUser.Role == 1)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Message = msg;
            ViewBag.Color = color;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PostLogin(string Email = "", string Password = "")
        {
            User user = new UserBL().GetAllUsersList(de).Where(x => x.IsActive != 0 && (x.Email.Trim().ToLower() == Email.Trim().ToLower() || x.Username.Trim().ToLower() == Email.Trim().ToLower()) && StringCipher.Decrypt(x.Password).Equals(Password)).FirstOrDefault();

            if (user == null)
            {
                return RedirectToAction("Login", new { msg = "Incorrect Email/Password!", color = "red" });
            }

            //Generating a list of cliams to store them into Cookiest
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim("UserName", user.Username),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim("Role", user.Role.ToString()),
            };


            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            //Initialize a new instance of the ClaimsPrincipal with ClaimsIdentity    
            var principal = new ClaimsPrincipal(identity);
            //SignInAsync is a Extension method for Sign in a principal for the specified scheme.
            //in order to work the following code we need to register it into Startup.cs class
            //so paste the following code ini that
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(x => x.LoginPath = "/Auth/Login");

            //also add the following functions in startup.cs class to store claims in cookies (else wise system will not store cookies)
            //app.UseAuthentication();
            //app.UseAuthorization();

            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties()
            //{
            //   IsPersistent=true,// for remember me check box while logging in
            //   ExpiresUtc = DateTime.UtcNow.AddDays(99999),
            //});

            var result = await _signInManager.PasswordSignInAsync(Email, Password, true, lockoutOnFailure: false);


            if (user.Role == 1)
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                user.Status = 1;
                var update = new UserBL().UpdateUser(user, de);
                if(user.Role == 3)
                {
                    return RedirectToAction("Account", "Seller");
                }
                return RedirectToAction("Account", "Buyer");
            }
        }


        public async Task<bool> PostLogins(string Email = "", string Password = "")
        {
            var pa = StringCipher.Decrypt(Password);
            User user = new UserBL().GetAllUsersList(de).Where(x => x.IsActive != 0 && (x.Email.Trim().ToLower() == Email.Trim().ToLower() || x.Username.Trim().ToLower() == Email.Trim().ToLower()) && StringCipher.Decrypt(x.Password).Equals(pa)).FirstOrDefault();

            if (user == null)
            {
                //return RedirectToAction("Login", new { msg = "Incorrect Email/Password!", color = "red" });
                return false;
            }

            //Generating a list of cliams to store them into Cookiest
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim("UserName", user.Username),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim("Role", user.Role.ToString()),
            };


            //var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ////Initialize a new instance of the ClaimsPrincipal with ClaimsIdentity    
            //var principal = new ClaimsPrincipal(identity);
            //SignInAsync is a Extension method for Sign in a principal for the specified scheme.
            //in order to work the following code we need to register it into Startup.cs class
            //so paste the following code ini that
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(x => x.LoginPath = "/Auth/Login");

            var result = await _signInManager.PasswordSignInAsync(Email, Password, true, lockoutOnFailure: false);
            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties()
            //{
            //    IsPersistent = true// for remember me check box while logging in
            //});


            //also add the following functions in startup.cs class to store claims in cookies (else wise system will not store cookies)
            //app.UseAuthentication();
            //app.UseAuthorization();


            if (user.Role == 1)
            {
                //return RedirectToAction("Index", "Admin");
                return true;
            }
            else
            {
                user.Status = 1;
                var update = new UserBL().UpdateUser(user, de);
                if (user.Role == 3)
                {
                   
                    //return RedirectToAction("Account", "Home");
                    return true;
                }

                //return RedirectToAction("Index", "Home");
                return true;
            }
        }

        #endregion

        #region Forgot Password

        public IActionResult ForgotPassword(string msg = "", string color = "black")
        {
            ViewBag.Color = color;
            ViewBag.Message = msg;

            return View();
        }

        public IActionResult PostForgotPassword(string Email = "")
        {
            bool checkEmail = gp.ValidateEmail(Email);

            if (checkEmail == false)
            {
                int id = new UserBL().GetActiveUserList(de).Where(x => x.Email.ToLower() == Email.ToLower()).Select(x => x.Id).FirstOrDefault();

                string BaseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}"+"/";

                bool checkMail = MailSender.SendForgotPasswordEmail(Email, id.ToString(), BaseUrl);

                if (checkMail == true)
                {
                    return RedirectToAction("ForgotPassword", "Auth", new { msg = "Please check your mail's inbox/spam", color = "green" });
                }
                else
                {
                    return RedirectToAction("ForgotPassword", "Auth", new { msg = "Mail sending fail!", color = "red" });
                }
            }
            else
            {
                return RedirectToAction("ForgotPassword", "Auth", new { msg = "Email does not belong to our record!!", color = "red" });
            }
        }


        public IActionResult ResetPassword(string encId = "", string t = "", string msg = "", string color = "black")
        {
            DateTime PassDate = new DateTime(Convert.ToInt64(t)).Date;
            DateTime CurrentDate = GeneralPurpose.DateTimeNow().Date;

            if (CurrentDate != PassDate)
            {
                return RedirectToAction("Login", "Auth", new { msg = "Link expired, Please try again!", color = "red" });
            }


            ViewBag.Time = t;
            ViewBag.EncId = encId;
            ViewBag.Message = msg;
            ViewBag.Color = color;

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> PostResetPassword(string encId = "", string t = "", string NewPassword = "", string ConfirmPassword = "")
        {
            if (NewPassword != ConfirmPassword)
            {
                return RedirectToAction("ResetPassword", "Auth", new { encId = encId, t = t, msg = "Password and confirm password did not match", color = "red" });
            }

            User user = new UserBL().GetActiveUserById(Convert.ToInt32(StringCipher.Base64Decode(encId)), de);
            user.Password = StringCipher.Encrypt(NewPassword);

            bool check = await new UserBL().UpdateUser(user, de);

            if (check == true)
            {
                return RedirectToAction("Login", "Auth", new { msg = "Password reset successful, Try login", color = "green" });
            }
            else
            {
                return RedirectToAction("ResetPassword", "Auth", new { encId = encId, t = t, msg = "Somethings' wrong!", color = "red" });
            }
        }

        #endregion

        #region Signup

        [HttpPost]
        public async Task<IActionResult> PostRegister(User _user, string _confirmPassword = "", string val="")
        {
            if (_user.Password != _confirmPassword)
            {
                return RedirectToAction("Register", "Home", new { msg = "Password and confirm password didn't match", color = "red" });
            }

            bool checkEmail = gp.ValidateEmail(_user.Email);
            if (checkEmail == false)
            {
                return RedirectToAction("Register", "Home", new { msg = "Email already exists. Try sign in!", color = "red" });
            }
            // 1 For Buyer
            // 2 For Seller
            if(val=="Buyer")
            {
                _user.Role = 4;
            }
            if (val == "Seller")
            {
                _user.Role = 3;
            }
            User u = new User()
            {
                Username = _user.Username.Trim(),
                FirstName = _user.FirstName,
                LastName = _user.LastName,
                Country = _user.Country,
                Contact = _user.Contact,
                Email = _user.Email.Trim(),
                Password = StringCipher.Encrypt(_user.Password),
                Role = _user.Role,
                IsActive = 3,
                Refferal_Code = _user.Username.Trim()+_user.Id,
                CreatedAt = GeneralPurpose.DateTimeNow()
            };

            var usr = new IdentityUser()
            {
                Email = u.Email,
                UserName = u.Username
            };
          
            bool chkUser = await new UserBL().AddUser(u, de);

            
            if (_user.Reffered_By != null)
            {
                var refr = new Refferals
                {
                    RefferedId = u.Id,
                    RefferalCode = _user.Reffered_By.Trim(),
                    RefferalType = 2,
                    IsActive = 1,
                    CreatedAt = DateTime.Now,
                };

                var addRef = new UserBL().AddRefferal(refr, de);
            }
            if (chkUser)
            {

                await _userManager.CreateAsync(usr, u.Password);
                MailSender.ActivationEmail(u.Email);
                var getUser = new UserBL().GetAllUsersList(de).Where(x => x.Email == u.Email).LastOrDefault();
                bool check = await PostLogins(getUser.Email, getUser.Password);
                if (u.Role == 3)
                {
                    if (check == true)
                    {
                        return RedirectToAction("Account", "Seller", new { color = "green" });
                    }
                    else
                    {
                        return RedirectToAction("Register", "Home", new { msg = "Somethings' wrong", color = "red" });

                    }
                }
                return RedirectToAction("Account", "Buyer", new { msg = "Account Activation Email has been sent to your Email", color = "green" });

            }
            else
            {
                return RedirectToAction("Register", "Home", new { msg = "Somethings' wrong", color = "red" });
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> AccountAcctivate(string e = "", int id = -1, int ac=-1)
        {
            if (e != "" || id != -1)
            {
                var getEmail = StringCipher.Base64Decode(e);
                var getUser = new User();
                if (getEmail != "")
                {
                    getUser = new UserBL().GetAllUsersList(de).Where(x => x.IsActive == 3 && x.Email == getEmail).FirstOrDefault();

                }
                else
                {
                    getUser = new UserBL().GetAllUsersList(de).Where(x => x.IsActive == 3 && x.Id == id).FirstOrDefault();
                }
                if (getUser != null)
                {
                    if (getUser.Role == 4)
                    {
                        getUser.IsActive = 1;
                    }
                    else if (getUser.Role == 3)
                    {
                        getUser.IsActive = 2;
                    }
                    bool check = await new UserBL().UpdateUser(getUser, de);
                    if (check == true)
                    {
                        User loggedinUser = gp.ValidateLoggedinUser();
                        if (loggedinUser != null)
                        {
                            if (loggedinUser.Role != 1)
                            {
                                if (getUser.Role == 4)
                                {
                                    return RedirectToAction("AccountActivated", new { msg = "Your Account is Activated", color = "green" });
                                }
                                else
                                {
                                    return RedirectToAction("AccountActivated", new { msg = "Your Account is Activated. Now Wait For Admin Approval", color = "green" });
                                }
                            }
                            else if(ac!=-1)
                            {
                                return RedirectToAction("ViewUser","Admin", new { category= getUser.Role, msg = "Account Activated Successfully!", color = "green" });
                            }
                            else
                            {
                                return RedirectToAction("AccountActivated", new { msg = "Account Activated Successfully!", color = "green" });
                            }
                        }
                        else
                        {
                            return RedirectToAction("AccountActivated", new { msg = "Your Account is Activated", color = "green" });
                        }
                    }
                }
            }
            return RedirectToAction("Login", new { msg = "Something went wrong", color = "red" });
        }


        [AllowAnonymous]
        public IActionResult AccountActivated(string msg = "", string color = "")
        {
            return View();
        }


        #endregion

        #region Manage Profile

       
        public IActionResult UpdateProfile(string msg = "", string color = "black")
        {
            User u = gp.ValidateLoggedinUser();

            ViewBag.User = u;
            ViewBag.Message = msg;
            ViewBag.Color = color;

            return View();
        }


       
        public async Task<IActionResult> PostUpdateProfile(string EncId, User _user, IFormFile ImagePath = null, string[] SkillName = null, string[] TagName = null,IFormCollection formCollection=null, int pcount=0)
        {
            int UserId = Convert.ToInt32(StringCipher.DecryptId(EncId));

            bool checkEmail = gp.ValidateEmail(_user.Email, UserId);

            if (checkEmail == false)
            {
                return RedirectToAction("UpdateProfile", "Auth", new { msg = "Email used by someone else, Please try another", color = "red" });
            }
            User u = new UserBL().GetActiveUserById(UserId, de);
            if (pcount != 0)
            {
                for (int i = 1; i < pcount; i++)
                {
                    var degree = formCollection["degname" + i];
                    var institute = formCollection["insname" + i];
                    var startdate = formCollection["sd" + i];
                    var enddate = formCollection["ed" + i];
                    if (degree.Count()!=0 && institute.Count()!= 0 && startdate.Count() != 0 && enddate.Count() != 0)
                    {
                        Education edu = new Education
                        {
                            DegreeName = degree,
                            InstituteName = institute,
                            UserId = UserId,
                            IsActive = 1,
                            CreatedAt = DateTime.Now,
                            StartDate = Convert.ToDateTime(startdate).Date,
                            EndDate = Convert.ToDateTime(enddate).Date,
                        };
                        de.Education.Add(edu);
                        de.SaveChanges();
                    }
                }
            }
                Skills skills = new UserBL().GetSkillsById(UserId, de);
                Tags UserTags = new UserBL().GetTagsById(UserId, de);
                int count = 0, count2 = 0;

                foreach (var sk in SkillName)
                {
                    if (sk != null)
                    {
                        if (skills != null)
                        {
                            if (skills.CreatedAt == null)
                            {

                                skills.SkillName = sk.ToLower();
                                skills.CreatedAt = DateTime.Now;
                                skills.IsActive = 1;
                                u.Skills.Add(skills);
                                await new UserBL().UpdateUser(u, de);
                            }
                            else
                            {
                                skills.SkillName = sk.ToLower();
                                skills.IsActive = 1;
                                skills.UpdatedAt = DateTime.Now;
                                de.Skills.Update(skills);
                                de.SaveChanges();
                            }

                        }
                        else
                        {
                            var new_skills = new Skills
                            {
                                SkillName = sk.ToLower(),
                                CreatedAt = DateTime.Now,
                                IsActive = 1,
                                UserId = UserId,
                            };
                            u.Skills.Add(new_skills);
                            await new UserBL().UpdateUser(u, de);
                        }

                    }
                    else
                    {
                        count = count + 1;
                    }

                }
                if (skills != null)
                {
                    if (count == SkillName.Count())
                    {
                        skills.IsActive = 0;
                        skills.DeletedAt = DateTime.Now;
                        de.Skills.Update(skills);
                        de.SaveChanges();
                    }
                }

                foreach (var sk in TagName)
                {
                    if (sk != null)
                    {
                        if (UserTags != null)
                        {
                            if (UserTags.CreatedAt == null)
                            {

                                UserTags.TagName = sk.ToLower();
                                UserTags.CreatedAt = DateTime.Now;
                                UserTags.IsActive = 1;
                                u.Tags.Add(UserTags);
                                await new UserBL().UpdateUser(u, de);
                            }
                            else
                            {
                                UserTags.TagName = sk.ToLower();
                                UserTags.UpdatedAt = DateTime.Now;
                                UserTags.IsActive = 1;
                                de.Tags.Update(UserTags);
                                de.SaveChanges();
                            }

                        }
                        else
                        {
                            var new_skills = new Tags
                            {
                                TagName = sk.ToLower(),
                                CreatedAt = DateTime.Now,
                                IsActive = 1,
                                UserId = UserId,
                            };
                            u.Tags.Add(new_skills);
                            await new UserBL().UpdateUser(u, de);
                        }

                    }
                    else
                    {
                        count2 = count2 + 1;
                    }

                }

                if (UserTags != null)
                {
                    if (count2 == TagName.Count())
                    {
                        UserTags.IsActive = 0;
                        UserTags.DeletedAt = DateTime.Now;
                        de.Tags.Update(UserTags);
                        de.SaveChanges();
                    }
                }
            //u.FirstName = u.FirstName;
            //u.LastName = u.LastName;
            u.Username = _user.Username.Trim();
            u.Email = _user.Email.Trim();
            u.Contact = _user.Contact;
            u.Description = _user.Description;
            u.Company = _user.Company;
            u.Country = _user.Country;
            u.DOB = _user.DOB;
            u.Experience_From = _user.Experience_From;
            u.Experience_To = _user.Experience_To;
            u.FacebookLink = _user.FacebookLink;
            u.Gender = _user.Gender;
            u.DribbleLink = _user.DribbleLink;
            u.GoogleLink = _user.GoogleLink;
            u.GitHubLink = _user.GitHubLink;
            u.StackOverFlowLink = _user.StackOverFlowLink;
            u.VimeoLink = _user.VimeoLink;
            u.Language = _user.Language;
            u.City= _user.City;
            u.Organization = _user.Organization;
            u.Position = _user.Position;
            u.Status = _user.Status;
            u.TwitterLink = _user.TwitterLink;
            u.Website = _user.Website;
            u.ZipCode = _user.ZipCode;
            u.Contact = _user.Contact;
            u.UpdatedAt = _user.UpdatedAt;
            

            string DirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwRoot/Images/User");

            if(ImagePath != null)
            {
                if (!Directory.Exists(DirectoryPath))
                {
                    Directory.CreateDirectory(DirectoryPath);
                }

                var FileExtention = Path.GetExtension(ImagePath.FileName);

                string FileName = "Invoice" + "_" + DateTime.Now.Ticks.ToString() + "" + FileExtention;
                var path = Path.Combine(
                    Directory.GetCurrentDirectory(), "wwwRoot/Images/User/",
                    FileName);

                var setPath = "Images/User/" + FileName;

                using var image = Image.Load(ImagePath.OpenReadStream());
                image.Mutate(x => x.Resize(256, 256));
                image.Save(path, new JpegEncoder { Quality = 100 });
                //using (var stream = new FileStream(path, FileMode.Create))
                //{

                //    stream.Position = 0;
                //    ImagePath.CopyTo(stream);
                //    stream.Flush();
                //}
                u.ImagePath = setPath;
            }

            bool chk = await new UserBL().UpdateUser(u, de);

            if (chk)
            {
                if(u.Role == 3)
                {
                    return RedirectToAction("Account", "Seller", new { msg = "Profile updated successfully!", color = "green" });
                }
                if (u.Role == 4)
                {
                    return RedirectToAction("Account", "Seller", new { msg = "Profile updated successfully!", color = "green" });
                }
                return RedirectToAction("UpdateProfile", "Auth", new { msg = "Profile updated successfully!", color = "green" });
            }
            else
            {
                if (u.Role == 3)
                {
                    return RedirectToAction("Account", "Seller", new { msg = "Somthing is Wrong!", color = "red" });
                }
                if (u.Role == 4)
                {
                    return RedirectToAction("Account", "Seller", new { msg = "Somthing is wrong!", color = "red" });
                }
                return RedirectToAction("UpdateProfile", "Auth", new { msg = "Somthing is Wrong!", color = "red" });
            }
        }


       
        public IActionResult UpdatePassword(string msg = "", string color = "black")
        {
            ViewBag.Message = msg;
            ViewBag.Color = color;

            return View();
        }


        
        public IActionResult UpdatePasswordUser(string msg = "", string color = "black")
        {
            ViewBag.Message = msg;
            ViewBag.Color = color;

            return View();
        }

      
        public async Task<IActionResult> PostUpdatePassword(string oldPassword = "", string newPassword = "", string confirmPassword = "")
        {
            if (newPassword != confirmPassword)
            {
                return RedirectToAction("UpdatePassword", "Auth", new { msg = "New password and Confirm password did not match!", color = "red" });
            }

            User u = gp.ValidateLoggedinUser();

            if (StringCipher.Decrypt(u.Password) != oldPassword)
            {
                return RedirectToAction("UpdatePassword", "Auth", new { msg = "Old password did not match!", color = "red" });
            }

            u.Password = StringCipher.Encrypt(newPassword);

            bool chk = await new UserBL().UpdateUser(u, de);

            if (chk)
            {
                return RedirectToAction("UpdatePassword", "Auth", new { msg = "Password updated successfully!", color = "green" });
            }
            else
            {
                return RedirectToAction("UpdatePassword", "Auth", new { msg = "Something's wrong!", color = "red" });
            }
        }

        #endregion
        public async Task<IActionResult> LogOut()
        {
            bool x = new AjaxController(de, _haccess).StatusUpdate(gp.ValidateLoggedinUser().Id, 0);
            if (x==true)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }
            return Unauthorized();
      }




        }
}
