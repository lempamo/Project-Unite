using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Project_Unite.Models;

namespace Project_Unite.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ActionResult SetAvatar(string id)
        {
            var db = new ApplicationDbContext();
            var usr = db.Users.FirstOrDefault(x => x.Id == User.Identity.GetUserId());
            var avtr = db.UserAvatars.FirstOrDefault(x => x.Id == id);
            usr.AvatarUrl = avtr.AvatarUrl;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UploadAvatar()
        {
            var model = new UploadImageViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadAvatar(UploadImageViewModel model)
        {
            string[] allowedTypes = new[] { ".png", ".jpg", ".bmp", ".jpeg", ".gif" };

            bool containsAllowedType = false;
            if (model.Image != null)
            {
                foreach (var ending in allowedTypes)
                {
                    if (model.Image.FileName.EndsWith(ending))
                    {
                        containsAllowedType = true;
                        break;
                    }
                }
            }
            if (containsAllowedType == false)
                ModelState.AddModelError("UploadImageViewModel", new Exception("File type not allowed."));

            if (ModelState.IsValid == true)
            {
                var db = new ApplicationDbContext();
                var usr = db.Users.FirstOrDefault(x => x.UserName==User.Identity.Name);
                string avatarRoot = $"~/Uploads/{usr.DisplayName}/Avatars";
                string serverPath = Server.MapPath(avatarRoot);
                if (!System.IO.Directory.Exists(serverPath))
                    System.IO.Directory.CreateDirectory(serverPath);

                avatarRoot += "/" + model.Image.FileName;
                serverPath += "\\" + model.Image.FileName;

                model.Image.SaveAs(serverPath);

                var avatar = new Avatar();
                avatar.Id = Guid.NewGuid().ToString();
                avatar.AvatarUrl = avatarRoot.Remove(0, 1);
                avatar.UserId = usr.Id;
                avatar.UploadedAt = DateTime.Now;
                usr.AvatarUrl = avatar.AvatarUrl;
                db.UserAvatars.Add(avatar);
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            return View(model);
        }

        public ActionResult ListAvatars()
        {
            string id = User.Identity.GetUserId();
            var avatars = new ApplicationDbContext().UserAvatars.Where(x => x.UserId == id);
            return View(avatars);
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

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

        public ActionResult MarkAllRead()
        {
            var db = new ApplicationDbContext();
            string uid = User.Identity.GetUserId();
            var unread = db.Notifications.Where(x => x.IsRead == false && x.UserId == uid);
            foreach(var u in unread.ToArray())
            {
                u.IsRead = true;
            }
            db.SaveChanges();
            return Redirect(Url.Action("Index", "Manage") + "#t_notifications");
        }

        public ActionResult Notification(string id, string url)
        {
            var db = new ApplicationDbContext();
            var note = db.Notifications.FirstOrDefault(x => x.Id == id);
            if (note == null)
                return new HttpStatusCodeResult(404);
            note.IsRead = false;
            db.SaveChanges();
            return Redirect(url);
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

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var userId = User.Identity.GetUserId();
            var usr = UserManager.FindById(userId);
            return View(usr);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ApplicationUser model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var db = new ApplicationDbContext();
            var usr = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            usr.DisplayName = model.DisplayName;
            usr.FullName = model.FullName;
            usr.Hobbies = model.Hobbies;
            usr.Bio = model.Bio;
            usr.Interests = model.Interests;
            usr.ShowFollowers = model.ShowFollowers;
            usr.ShowFollowed = model.ShowFollowed;
            usr.ShowEmail = model.ShowEmail;
            usr.EmailOnNotifications = model.EmailOnNotifications;
            usr.ShowPostAndTopicCounts = model.ShowPostAndTopicCounts;
            usr.ShowJoinDate = model.ShowJoinDate;
            if (usr.Email != model.Email)
                usr.EmailConfirmed = false;
            usr.Email = model.Email;
            usr.Website = model.Website;
            usr.YoutubeUrl = model.YoutubeUrl;
            db.SaveChanges();
            return View(model);
        }

        public ActionResult RevokeAPIKey(string id)
        {
            var db = new ApplicationDbContext();
            var usr = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            var key = db.OAuthTokens.FirstOrDefault(x => x.Id == id);
            if (usr == null || key == null)
                return new HttpStatusCodeResult(404);
            db.OAuthTokens.Remove(key);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeFullName(ApplicationUser model)
        {
            var db = new ApplicationDbContext();
            var usr = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            usr.FullName = model.FullName;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeBio(ApplicationUser model)
        {
            var db = new ApplicationDbContext();
            var usr = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            usr.Bio = model.Bio;
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeInterests(ApplicationUser model)
        {
            var db = new ApplicationDbContext();
            var usr = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            usr.Interests = model.Interests;
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeHobbies(ApplicationUser model)
        {
            var db = new ApplicationDbContext();
            var usr = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            usr.Hobbies = model.Hobbies;
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeYouTube(ApplicationUser model)
        {
            var db = new ApplicationDbContext();
            var usr = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            usr.YoutubeUrl = model.YoutubeUrl;
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeWebsite(ApplicationUser model)
        {
            var db = new ApplicationDbContext();
            var usr = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);
            usr.Website = model.Website;
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

#endregion
    }
}