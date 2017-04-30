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
using System.Web.Script.Serialization;

namespace Project_Unite.Controllers
{
    public class AuthController : Controller
    {
        public JavaScriptSerializer Serializer
        {
            get; set;
        }
        //I'll put those curly braces on their own line just to piss a certain smiley off.

        public AuthController()
        {
            Serializer = new JavaScriptSerializer();
        }



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
        public async Task<ActionResult> Register(string appname, string appdesc, string version, string displayname, string sysname)
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
                using (var temp = new ApplicationDbContext())
                {
                    if (temp.Users.FirstOrDefault(x => x.DisplayName == displayname) != null)
                    {
                        return Content(Serializer.Serialize(new Exception("That display name has already been taken.")));
                    }
                }


                var user = new ApplicationUser { UserName = username, Email = username, DisplayName = displayname, Codepoints = 0, JoinedAt = DateTime.Now, MutedAt = DateTime.Now, BannedAt = DateTime.Now, LastLogin = DateTime.Now, SystemName = sysname };
                var result = await UserManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    UserManager.AddToRole(user.Id, ACL.LowestPriorityRole().Name);

                    var db = new ApplicationDbContext();


                    var auth_token = db.OAuthTokens.Where(x => x.UserId == user.Id).FirstOrDefault(x => x.AppName == appname && x.AppDescription == appdesc && x.Version == version);
                    if (auth_token == null)
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
                return Content(Serializer.Serialize(new Exception("Registration failed because a similar account already exists.")));
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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
            catch(Exception ex)
            {
                var db = new ApplicationDbContext();
                db.AuditLogs.Add(new Models.AuditLog("system", AuditLogLevel.Admin, "<b>The following error occurred during an OAuth2 authentication.</b><br/>" + ex.ToString()));
                db.SaveChanges();
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

        }
    }
}