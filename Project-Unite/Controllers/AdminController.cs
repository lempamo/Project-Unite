using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Project_Unite.Models;

namespace Project_Unite.Controllers
{
    //We have a custom ACL implementation so we do not need to use the ASP.NET role system to check if a user has an ACL rule.
    [Authorize]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            ViewBag.Admin = true;
            return View();
        }


        public ActionResult DeleteForum(string id)
        {
            var frm = db.ForumCategories.FirstOrDefault(x => x.Id == id);
            if (frm == null)
                return new HttpStatusCodeResult(404);

            //Purge ALL DATA RELATED TO THIS CATEGORY.
            DeleteCategoryRecursive(frm);
            db.SaveChanges();

            return RedirectToAction("Forums");
        }


        public void DeleteCategoryRecursive(ForumCategory start)
        {
            foreach (var c in start.Children.ToArray())
            {
                DeleteCategoryRecursive(c);
            }

            foreach (var topic in start.Topics.ToArray())
            {
                DeleteTopic(topic);
            }
            db.ForumCategories.Remove(start);

        }

        public ActionResult CreateUser()
        {
            return View(new CreateUserModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(CreateUserModel model)
        {
            var db = new ApplicationDbContext();

            var user = new ApplicationUser();
            user.AccessFailedCount = 0;
            user.BannedAt = DateTime.Now;
            user.Bio = "";
            user.Codepoints = 0;
            user.DisplayName = model.Username;
            user.Email = model.Email;
            user.EmailConfirmed = true;
            user.FullName = "";
            user.Hobbies = "";
            user.Id = Guid.NewGuid().ToString();
            user.IsBanned = false;
            user.IsMuted = false;
            user.IsPatreon = false;
            user.JoinedAt = DateTime.Now;
            user.LastKnownIPAddress = "127.0.0.1";
            user.LastLogin = DateTime.Now;
            user.LastMonthPaid = 0;
            user.LockoutEnabled = false;
            user.MajorVersion = 1;
            user.MinorVersion = 0;
            user.MutedAt = DateTime.Now;
            user.PasswordHash = "ResetYourPassword.";
            user.PhoneNumberConfirmed = false;
            user.Revision = 0;
            user.SecurityStamp = Guid.NewGuid().ToString();
            user.ShiftnetSubscription = 0;
            user.StoryPosition = 0;
            user.TwoFactorEnabled = false;
            user.UserName = model.Email;

            db.Users.Add(user);

            db.SaveChanges();
            var uman = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            uman.AddToRole(user.Id, ACL.LowestPriorityRole().Name);
            uman.SendEmail(user.Id, "Welcome to the Project: Unite open alpha.", $@"**Hey there, {user.DisplayName}!

Michael here - it's finally happening. Project: Unite is coming together and it's time we merge everyone's user accounts.

Unlike previous ShiftOS site revamps, your account got migrated over. However, there's one thing you need to do. You need to [reset your password]({Url.Action("ForgotPassword", "Account")})! Just click that link and follow the instructions and we'll send you another email with details on how to get into your account.");
            return RedirectToAction("Users");
        }

        public ActionResult BackupAssets()
        {
            var db = new ApplicationDbContext();
            string backupDir = "~/Backups/Database";
            string backupServerDir = Server.MapPath(backupDir);
            if (!System.IO.Directory.Exists(backupServerDir))
                System.IO.Directory.CreateDirectory(backupServerDir);

            string backupUrl = backupDir.Remove(0, 1) + "/ShiftOS-" + DateTime.Now.ToString() + ".zip";
            string backupname = Path.Combine(backupServerDir, "ShiftOS-" + DateTime.Now.ToString().Replace("/", "-") + ".zip");

            try
            {
                System.IO.Compression.ZipFile.CreateFromDirectory(Server.MapPath("~/Uploads"), backupname);
            }
            catch
            {
                return Content(backupname);
            }
            var backupData = new AssetBackup();
            backupData.Id = Guid.NewGuid().ToString();
            backupData.UserId = User.Identity.GetUserId();
            backupData.DownloadUrl = backupUrl;
            backupData.Timestamp = DateTime.Now;
            db.AssetBackups.Add(backupData);
            db.SaveChanges();
            return RedirectToAction("Backups");
        }

        public ActionResult BackupDatabase()
        {
            var db = new ApplicationDbContext();
            string backupDir = "~/Backups/Assets";
            string backupServerDir = Server.MapPath(backupDir);
            if (!System.IO.Directory.Exists(backupServerDir))
                System.IO.Directory.CreateDirectory(backupServerDir);

            string backupUrl = backupDir.Remove(0, 1) + "/ShiftOS-" + DateTime.Now.ToString() + ".sql";
            string backupname = backupServerDir + "\\ShiftOS-" + DateTime.Now.ToString() + ".sql";
            const string sqlCommand = @"BACKUP DATABASE [{0}] TO  DISK = N'{1}' WITH NOFORMAT, NOINIT,  NAME = N'ShiftOS Database Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10";
            int path = db.Database.ExecuteSqlCommand(System.Data.Entity.TransactionalBehavior.DoNotEnsureTransaction, string.Format(sqlCommand, db.Database.Connection.Database, backupname));
            var backupData = new DatabaseBackup();
            backupData.Id = Guid.NewGuid().ToString();
            backupData.UserId = User.Identity.GetUserId();
            backupData.DownloadUrl = backupUrl;
            backupData.Timestamp = DateTime.Now;
            db.Backups.Add(backupData);
            db.SaveChanges();
            return RedirectToAction("Backups");
        }




        public ActionResult Backups()
        {
            ViewBag.Admin = true;
            var backups = new BackupViewModel();
            var db = new ApplicationDbContext();
            backups.Databases = db.Backups.OrderByDescending(x => x.Timestamp);
            backups.AssetFolders = db.AssetBackups.OrderByDescending(x => x.Timestamp);

            return View(backups);
        }

        public void DeleteTopic(ForumTopic topic)
        {
            foreach(var post in topic.Posts.ToArray())
            {
                DeletePost(post);
            }
            string id = topic.Id;
            db.Likes.RemoveRange(db.Likes.Where(x => x.Topic == id));
            db.ForumTopics.Remove(db.ForumTopics.FirstOrDefault(x=>x.Id==id));
        }

        public ActionResult AccessControl()
        {
            var model = new Dictionary<string, ForumPermission[]>();
            var db = new ApplicationDbContext();
            var forums = db.ForumCategories.ToArray();
            foreach(var forum in forums)
            {
                ACL.UpdateACLDefinitions(forum.Id);
                if(forum.Id != "root")
                {
                    if (!model.ContainsKey(forum.Id))
                    {
                        model.Add(forum.Id, forum.Permissions);
                    }
                }
            }
            return View(new AdminAccessControlViewModel(model));  
        }

        public ActionResult SetPermission(string id, string role, string permission)
        {
            if (!ACL.Granted(User.Identity.Name, "CanAccessAdminCP"))
                return new HttpStatusCodeResult(403);
            if (!ACL.Granted(User.Identity.Name, "CanEditRoles"))
                return new HttpStatusCodeResult(403);
            if (!ACL.Granted(User.Identity.Name, "CanEditForumCategories"))
                return new HttpStatusCodeResult(403);

            var db = new ApplicationDbContext();
            var frm = db.ForumCategories.FirstOrDefault(x => x.Id == id);
            if (frm == null)
                return new HttpStatusCodeResult(403);
            var rolePerm = db.ForumPermissions.Where(x => x.CategoryId == frm.Id).FirstOrDefault(x => x.RoleId == role);
            if (rolePerm == null)
                return new HttpStatusCodeResult(404);

            rolePerm.Permissions = (PermissionPreset)Enum.Parse(typeof(PermissionPreset), permission);

            db.AuditLogs.Add(new Models.AuditLog(User.Identity.GetUserId(), AuditLogLevel.Admin, "User altered the ACL definition for forum ID " + id + ", role ID " + role + ", to permission \"" + permission + "\"."));
            db.SaveChanges();

            return RedirectToAction("AccessControl");

        }

        public void DeletePost(ForumPost post)
        {
            string id = post.Id;

            db.ForumPosts.Remove(db.ForumPosts.FirstOrDefault(x=>x.Id==id));
        }


        // GET: Admin/Forums
        public ActionResult Forums()
        {
            ViewBag.Admin = true;
            var cats = db.ForumCategories.First(x => x.Id == "root").Children.Where(x=>x.Id != "root");
            return View(cats);
        }



        public ActionResult AddForumCategory(string parentId)
        {
            ViewBag.Admin = true;
            if (parentId == null)
                parentId = "root";

            var model = new AddForumCategoryViewModel();
            model.PossibleParents = GetForumCategories();
            model.Parent = (parentId);
            return View(model);
        }

        public ActionResult RaisePriority(string id)
        {
            var uid = User.Identity.Name;
            if(ACL.Granted(uid, "CanEditRoles"))
            {
                var db = new ApplicationDbContext();
                var role = db.Roles.FirstOrDefault(x => x.Id == id) as Role;
                if (role == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                if (role.Priority == db.Roles.Count() - 1)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                Role higherUp = null;
                foreach(var r in db.Roles)
                {
                    if ((r as Role).Priority == role.Priority + 1)
                        higherUp = r as Role;
                }

                if(higherUp != null)
                    higherUp.Priority--;
                role.Priority++;
                db.SaveChanges();

                return RedirectToAction("Roles");
            }
            else
            {
                return new HttpStatusCodeResult(403);
            }
        }

        public async Task<ActionResult> RemoveUserFromRole(string id, string usr)
        {
            if(ACL.CanManageRole(User.Identity.Name, id))
            {
                var uman = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                await uman.RemoveFromRoleAsync(usr, id);
                return RedirectToAction("RoleDetails", new { @id = id });
            }
            else
            {
                return new HttpStatusCodeResult(403);
            }
        }

        public ActionResult LowerPriority(string id)
        {
            var uid = User.Identity.Name;
            if (ACL.Granted(uid, "CanEditRoles"))
            {
                var db = new ApplicationDbContext();
                var role = db.Roles.FirstOrDefault(x => x.Id == id) as Role;
                if (role == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                if (role.Priority == 0)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                Role higherUp = null;
                foreach (var r in db.Roles)
                {
                    if ((r as Role).Priority == role.Priority - 1)
                        higherUp = r as Role;
                }

                if(higherUp != null)
                    higherUp.Priority++;
                role.Priority--;
                db.SaveChanges();

                return RedirectToAction("Roles");
            }
            else
            {
                return new HttpStatusCodeResult(403);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddForumCategory(AddForumCategoryViewModel model)
        {
            try
            {
                if (model == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    ViewBag.Error = "Please specify a name for this forum category.";
                    return View(model);
                }

                string DisallowedChars = "/.,\\][;':\"|?><!@#$%^&*()_+-=`~}{ ";

                string id = model.Name.ToLower();

                foreach (var c in DisallowedChars)
                {
                    id = id.Replace(c, '_');
                }


                var frm = new ForumCategory();

                frm.Name = model.Name;
                frm.Id = id;

                frm.Description = model.Description;
                frm.LinkUrl = null;
                frm.Parent = db.ForumCategories.FirstOrDefault(x => x.Id == model.Parent).Id;
                db.ForumCategories.Add(frm);

                db.SaveChanges();
                if (model.StealPermissionsFrom != "root")
                {
                    var frmToSteal = db.ForumCategories.FirstOrDefault(x => x.Id == model.StealPermissionsFrom);
                    ACL.UpdateACLDefinitions(frmToSteal.Id); //Just to be sure..
                    foreach(var perm in frmToSteal.Permissions)
                    {
                        var aclEntry = new ForumPermission();
                        aclEntry.CategoryId = frm.Id;
                        aclEntry.RoleId = perm.RoleId;
                        aclEntry.Permissions = perm.Permissions;
                        aclEntry.Id = Guid.NewGuid().ToString();
                        db.ForumPermissions.Add(aclEntry);
                    }
                    db.SaveChanges();
                }
                else
                {
                    ACL.UpdateACLDefinitions(frm.Id);
                    //This sets the permission data to the default values.
                }


                
                return RedirectToAction("Forums");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View(model);
            }
        }

        public ActionResult AnonymizeUser(string id)
        {
            if (!ACL.Granted(User.Identity.Name, "CanAcessAdminCP"))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            if (!ACL.Granted(User.Identity.Name, "CanAnonymizeUser"))
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            var db = new ApplicationDbContext();
            var user = db.Users.FirstOrDefault(x => x.Id == id);
            if (user == null)
                return new HttpStatusCodeResult(404);

            user.UserName = Guid.NewGuid().ToString() + "@system";
            user.Email = Guid.NewGuid().ToString() + "@system";
            user.PasswordHash = Guid.NewGuid().ToString();
            user.EmailConfirmed = false;
            user.Hobbies = "";
            user.Interests = "";
            user.Bio = @"# User anonymized.

This user has been anonymized by an administrator.";
            user.AvatarUrl = "";
            user.BannerUrl = "";
            var uman = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            foreach(var role in user.Roles)
            {
                uman.RemoveFromRole(user.Id, role.RoleId);
            }
            uman.AddToRole(user.Id, ACL.LowestPriorityRole().Id);
            db.SaveChanges();
            return RedirectToAction("Users");
        }


        public ActionResult Logs()
        {
            if (!ACL.Granted(User.Identity.Name, "CanAccessAdminCP"))
                return new HttpStatusCodeResult(403);

            var db = new ApplicationDbContext();

            return View(db.AuditLogs);
        }


        public ActionResult Users()
        {
            return View(new ApplicationDbContext().Users);
        }

        public ActionResult EditForum(string id)
        {
            ViewBag.Admin = true;
            var frm = db.ForumCategories.FirstOrDefault(x => x.Id == id);
            if (frm == null)
                return new HttpStatusCodeResult(404);

            var m = new AddForumCategoryViewModel();
            m.Id = frm.Id;
            m.Parent = frm.Parent;
            m.Description = frm.Description;
            m.Name = frm.Name;
            m.PossibleParents = GetForumCategories();
            return View(m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditForum(string id, AddForumCategoryViewModel m)
        {
            try
            {
                m.Id = id;
                var frm = db.ForumCategories.FirstOrDefault(x => x.Id == id);

                frm.Name = m.Name;
                frm.Description = m.Description;



                frm.Parent = m.Parent;
                db.SaveChanges();
                
                return RedirectToAction("Forums");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.ToString();
                return View(m);
            }
        }






        // GET: Admin
        public ActionResult Roles()
        {
            ViewBag.Admin = true;
            return View(db.IdentityRoles.ToList());
        }

        // GET: Admin/RoleDetails/5
        public ActionResult RoleDetails(string id)
        {
            ViewBag.Admin = true;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Role role = db.IdentityRoles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        // GET: Admin/Create
        public ActionResult CreateRole()
        {
            ViewBag.Admin = true;
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateRole(Role role)
        {
            if (ModelState.IsValid)
            {
                db.IdentityRoles.Add(role);
                db.SaveChanges();
                return RedirectToAction("Roles");
            }

            return View(role);
        }

        // GET: Admin/AddUserToRole
        public ActionResult AddUserToRole(string id = "")
        {
            ViewBag.Admin = true;
            var model = new AddUserToRoleViewModel();

            if (id == "")
                id = "user";
            model.RoleId = id;
            model.Users = new List<SelectListItem>();
            foreach (var u in db.Users.ToArray())
            {
                var user = u as ApplicationUser;
                model.Users.Add(new SelectListItem
                {
                    Value = user.Id,
                    Text = user.DisplayName
                });
            }


            model.Roles = new List<SelectListItem>();
            foreach(var role in db.Roles.ToArray())
            {
                var r = role as Role;
                model.Roles.Add(new SelectListItem
                {
                    Value = r.Id,
                    Text = r.Name
                });
            }

            return View(model);
        }

        // POST: Admin/AddUserToRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddUserToRole(AddUserToRoleViewModel model)
        {
            var r = db.Roles.FirstOrDefault(role => role.Id == model.RoleId);
            var user = db.Users.FirstOrDefault(u => u.Id == model.Username);
            var uMan = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            var isInRole = await uMan.IsInRoleAsync(user.Id, r.Name);
            
            if(isInRole)
            {
                ViewBag.Error = $"{model.Username} is already in the {r.Name} role.";
                return View(model);
            }
            else
            {
                await uMan.AddToRoleAsync(user.Id, r.Name);
                return RedirectToAction("Roles");
            }


        }

        // GET: Admin/Edit/5
        public ActionResult EditRole(string id)
        {
            ViewBag.Admin = true;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Role role = db.IdentityRoles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRole(Role role)
        {
            if (ModelState.IsValid)
            {
                db.Entry(role).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(role);
        }

        // GET: Admin/Delete/5
        public ActionResult DeleteRole(string id)
        {
            ViewBag.Admin = true;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Role role = db.IdentityRoles.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleConfirmed(string id)
        {
            var role = db.Roles.FirstOrDefault(x=>x.Id==id);
            db.Roles.Remove(role);
            db.SaveChanges();
            return RedirectToAction("Roles");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public List<SelectListItem> GetForumCategories()
        {
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Value = "root",
                Text = "Top Level"
            });

            foreach(var cat in getChildren("root", 1))
            {
                items.Add(cat);
            }

            return items;

        }

        private IEnumerable<SelectListItem> getChildren(string id, int dashcount)
        {
            var lst = new List<SelectListItem>();
            db = new ApplicationDbContext();
            foreach (var cat in db.ForumCategories.FirstOrDefault(c => c.Id == id).Children.Where(x=>x.Id != "root"))
            {
                string dashes = "";
                for (int i = 0; i <= dashcount; i++)
                    dashes += "-";
                lst.Add(new SelectListItem
                {
                    Text = dashes + " " + cat.Name,
                    Value = cat.Id,
                });
                lst.AddRange(getChildren(cat.Id, dashcount + 1));
            }
            return lst;
        }
    }
}
