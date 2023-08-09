using LoginFinal.BL;
using LoginFinal.HelpingClasses;
using LoginFinal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using static LoginFinal.HelpingClasses.ProjectVariables;

namespace LoginFinal.Controllers
{
    [Authorize]
    public class BuyerController : Controller
    {
        private readonly AppDbContext de;
        private readonly GeneralPurpose gp;
        public BuyerController(AppDbContext de, IHttpContextAccessor haccess)
        {
            this.de = de;
            this.gp = new GeneralPurpose(de, haccess);
            var request = haccess.HttpContext.Request;
            baseUrl = $"{request.Scheme}://{request.Host}";
        }
        public IActionResult Account(string msg = "")
        {
            if (gp.ValidateLoggedinUser().Role == 4)
            {
                return RedirectToAction("Account","Seller");
            }

            return RedirectToAction("Index", "Home");

        }

         
        public async Task<IActionResult> PostUpdatePassword(string oldPassword = "", string newPassword = "", string confirmPassword = "")
        {
            if (newPassword != confirmPassword)
            {
                return RedirectToAction("UpdatePasswordUser", "Buyer", new { msg = "New password and Confirm password did not match!", color = "red" });
            }

            User u = gp.ValidateLoggedinUser();

            if (StringCipher.Decrypt(u.Password) != oldPassword)
            {
                return RedirectToAction("UpdatePasswordUser", "Auth", new { msg = "Old password did not match the current password!", color = "red" });
            }

            u.Password = StringCipher.Encrypt(newPassword);

            bool chk = await new UserBL().UpdateUser(u, de);

            if (chk)
            {
                return RedirectToAction("UpdatePasswordUser", "Auth", new { msg = "Password updated successfully!", color = "green" });
            }
            else
            {
                return RedirectToAction("UpdatePasswordUser", "Auth", new { msg = "Somthings' wrong!", color = "red" });
            }
        }
    }
}
