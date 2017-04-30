using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Project_Unite.Models;
using System.Text;
using System.Net;

namespace Project_Unite.Controllers
{
    public class AuthController : Controller
    {
        private ApplicationSignInManager _signInManager = null;
        private ApplicationUserManager _userManager = null;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        [AllowAnonymous]
        public async Task<ActionResult> Login(string appname, string appdesc, string version)
        {
            try
            {
                string authHeader = Request.Headers["Authentication"];
                string b64_auth = authHeader.Remove(0, 6); //get rid of the "Basic " text.
                byte[] data = Convert.FromBase64String(b64_auth);
                string plaintext = Encoding.UTF8.GetString(data);
                string[] split = plaintext.Split(':');
                string username = split[0];
                string password = split[1];
                var result = await SignInManager.PasswordSignInAsync(username, password, false, false);
                if(result == Microsoft.AspNet.Identity.Owin.SignInStatus.Success)
                {
                    var db = new ApplicationDbContext();
                    var user = db.Users.FirstOrDefault(x => x.UserName == username);
                    var auth_token = db.OAuthTokens.Where(x => x.UserId == user.Id).FirstOrDefault(x => x.AppName == appname && x.AppDescription == appdesc && x.Version == version);
                    if(auth_token == null)
                    {
                        auth_token = new Models.OAuthToken
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = user.Id,
                            AppName = appname,
                            AppDescription = appdesc,
                            Version = version
                        };
                        db.OAuthTokens.Add(auth_token);
                        db.SaveChanges();
                        return Content(auth_token.Id);
                    }
                    else
                    {
                        return Content(auth_token.Id);
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(403);
                }
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

        }
    }
}