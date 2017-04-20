using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Project_Unite.Models;

namespace Project_Unite.Controllers
{
    public class ForumController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();

        // GET: Forum
        public ActionResult Index()
        {
            var toplevels = (db.ForumCategories.FirstOrDefault(x => x.Id == "root").Children);


            return View(toplevels);
        }

        //GET: ViewForum
        public ActionResult ViewForum(string id)
        {
            try
            {
                var cat = db.ForumCategories.FirstOrDefault(m => m.Id == id);
                if (!ACL.CanSee(User.Identity.Name, id))
                    return new HttpStatusCodeResult(403);

                return View(cat);
            }
            catch
            {
                return new HttpStatusCodeResult(404);
            }
        }

        public const string BadIdChars = "~`!@#$%^&*()-+={}[]|\\:;'\"<,>.?/";

        public ActionResult PostReply(string id, string fid)
        {
            if (!ACL.CanReply(User.Identity.Name, fid))
                return new HttpStatusCodeResult(403);

            var model = new CreateTopicViewModel();
            model.Category = id;
            return View(model);
        }

        [Authorize]
        public ActionResult Dislike(string id)
        {
            var db = new ApplicationDbContext();
            var topic = db.ForumTopics.FirstOrDefault(x => x.Discriminator == id);
            var uid = User.Identity.GetUserId();
            if (topic == null)
                return new HttpStatusCodeResult(404);
            if (topic.AuthorId == User.Identity.GetUserId())
                return RedirectToAction("ViewTopic", new { id = id, triedtolikeowntopic = true });
            var like = db.Likes.Where(x => x.Topic == topic.Id).FirstOrDefault(x => x.User == uid);
            if (like != null)
            {
                if (like.IsDislike == false)
                {
                    like.IsDislike = true;
                }
                else
                {
                    db.Likes.Remove(like);
                }
            }
            else
            {
                like = new Models.Like();
                like.Id = Guid.NewGuid().ToString();
                like.User = User.Identity.GetUserId();
                like.Topic = topic.Id;
                like.LikedAt = DateTime.Now;
                like.IsDislike = true;
                db.Likes.Add(like);
            }
            db.SaveChanges();
            return RedirectToAction("ViewTopic", new { id = id });
        }

        [Authorize]
        public ActionResult Like(string id)
        {
            var db = new ApplicationDbContext();
            var topic = db.ForumTopics.FirstOrDefault(x => x.Discriminator == id);
            var uid = User.Identity.GetUserId();
            if (topic == null)
                return new HttpStatusCodeResult(404);
            if (topic.AuthorId == User.Identity.GetUserId())
                return RedirectToAction("ViewTopic", new { id = id, triedtolikeowntopic = true });
            var like = db.Likes.Where(x=>x.Topic==topic.Id).FirstOrDefault(x => x.User == uid);
            if (like != null)
            {
                if (like.IsDislike == true)
                {
                    like.IsDislike = false;
                }
                else
                {
                    db.Likes.Remove(like);
                }
            }
            else
            {
                like = new Models.Like();
                like.Id = Guid.NewGuid().ToString();
                like.User = User.Identity.GetUserId();
                like.Topic = topic.Id;
                like.LikedAt = DateTime.Now;
                like.IsDislike = false;
                db.Likes.Add(like);
            }
            db.SaveChanges();
            return RedirectToAction("ViewTopic", new { id = id });
        }

        [Authorize]
        public ActionResult EditPost(string id)
        {
            var db = new ApplicationDbContext();
            var topic = db.ForumPosts.FirstOrDefault(x => x.Id == id);
            var uid = User.Identity.GetUserId();
            string acl_perm = "CanEditPosts";
            if (topic == null)
                return new HttpStatusCodeResult(404);
            if (topic.AuthorId == User.Identity.GetUserId())
                acl_perm = "CanEditOwnPosts";
            if (!ACL.Granted(User.Identity.Name, acl_perm))
                return new HttpStatusCodeResult(403);
            var model = new EditPostViewModel();
            model.Id = topic.Id;
            model.Contents = topic.Body;
            return View(model);
        }

        [Authorize]
        public ActionResult DeletePost(string id)
        {
            var db = new ApplicationDbContext();
            var topic = db.ForumPosts.FirstOrDefault(x => x.Id == id);
            var uid = User.Identity.GetUserId();
            string acl_perm = "CanDeletePosts";
            if (topic == null)
                return new HttpStatusCodeResult(404);
            if (topic.AuthorId == User.Identity.GetUserId())
                acl_perm = "CanDeleteOwnPosts";
            if (!ACL.Granted(User.Identity.Name, acl_perm))
                return new HttpStatusCodeResult(403);
            var parent = db.ForumTopics.FirstOrDefault(x => x.Id == topic.Parent);
            bool redirectToParent = false;
            string cat = "";
            if (parent.Posts.Length - 1 == 0)
            {
                cat = parent.Parent;
                db.ForumTopics.Remove(parent);
                db.SaveChanges();
                redirectToParent = true;
            }
            db.ForumPosts.Remove(topic);
            db.SaveChanges();
            if(redirectToParent)
                return RedirectToAction("ViewForum", new { id = cat });

            return RedirectToAction("ViewTopic", new { id = db.ForumTopics.FirstOrDefault(x => x.Id == topic.Parent).Discriminator});
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult EditPost(EditPostViewModel model)
        {
            var db = new ApplicationDbContext();
            var topic = db.ForumPosts.FirstOrDefault(x => x.Id == model.Id);
            var uid = User.Identity.GetUserId();
            string acl_perm = "CanEditPosts";
            if (topic == null)
                return new HttpStatusCodeResult(404);
            if (topic.AuthorId == User.Identity.GetUserId())
                acl_perm = "CanEditOwnPosts";
            if (!ACL.Granted(User.Identity.Name, acl_perm))
                return new HttpStatusCodeResult(403);
            var edit = new ForumPostEdit();
            edit.EditedAt = DateTime.Now;
            edit.EditReason = model.EditReason;
            edit.Id = Guid.NewGuid().ToString();
            edit.Parent = topic.Id;
            edit.PreviousState = topic.Body;
            edit.UserId = uid;
            db.ForumPostEdits.Add(edit);
            topic.Body = model.Contents;
            db.SaveChanges();
            return RedirectToAction("ViewTopic", new { id = db.ForumTopics.FirstOrDefault(x => x.Id == topic.Parent).Discriminator });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult PostReply(CreateTopicViewModel model)
        {


            var db = new ApplicationDbContext();
            var topic = db.ForumTopics.FirstOrDefault(x => x.Discriminator == model.Category);
            if (topic == null)
                return new HttpStatusCodeResult(404);
            if (!ACL.CanReply(User.Identity.Name, topic.Parent))
                return new HttpStatusCodeResult(403);

            var post = new ForumPost();
            post.AuthorId = User.Identity.GetUserId();
            post.Body = model.Body;
            post.Id = Guid.NewGuid().ToString();
            post.Parent = topic.Id;
            post.PostedAt = DateTime.Now;
            db.ForumPosts.Add(post);
            db.SaveChanges();
            NotificationDaemon.NotifyFollowers(User.Identity.GetUserId(), "New post from " + ACL.UserNameRaw(User.Identity.GetUserId()) + "!", ACL.UserNameRaw(User.Identity.GetUserId()) + " just posted a reply to " + topic.Subject + ".", Url.Action("ViewTopic", new { id = topic.Discriminator }));
            NotificationDaemon.NotifyUser(User.Identity.GetUserId(), topic.AuthorId, "New post from " + ACL.UserNameRaw(User.Identity.GetUserId()) + "!", ACL.UserNameRaw(User.Identity.GetUserId()) + " just posted a reply to " + topic.Subject + ".", Url.Action("ViewTopic", new { id = topic.Discriminator }));

            return RedirectToAction("ViewTopic", new { id = topic.Discriminator });

        }


        public ActionResult CreateTopic(string id)
        {
            if (!ACL.CanPost(User.Identity.Name, id))
                return new HttpStatusCodeResult(403);

            var model = new CreateTopicViewModel();
            model.Category = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult CreateTopic(CreateTopicViewModel model)
        {
            if (!ACL.CanPost(User.Identity.Name, model.Category))
                return new HttpStatusCodeResult(403);


            var db = new ApplicationDbContext();
            var forum = db.ForumCategories.FirstOrDefault(x => x.Id == model.Category);
            if (forum == null)
                return new HttpStatusCodeResult(404);

            string subjectId = model.Subject;

            if (String.IsNullOrWhiteSpace(model.Subject) == true)
            {
                ViewBag.Error = "hey no null subjects";
                return new HttpStatusCodeResult(404);
            }

            char[] badChars = subjectId.Where(x => !AllowedChars.Contains(x)).ToArray();

            foreach(var c in badChars)
            {
                subjectId = subjectId.Replace(c, '_');
            }

            while (subjectId.Contains("__"))
            {
                subjectId = subjectId.Replace("__", "_");
            }

            var topic = new ForumTopic();
            topic.AuthorId = User.Identity.GetUserId();
            topic.Id = Guid.NewGuid().ToString();
            topic.Discriminator = subjectId + "_" + db.ForumTopics.Count().ToString();
            topic.StartedAt = DateTime.Now;
            topic.Parent = forum.Id;
            topic.IsAnnounce = model.IsAnnounce;
            topic.IsSticky = model.IsSticky;
            topic.IsGlobal = model.IsGlobal;
            topic.Subject = model.Subject;
            var post = new ForumPost();
            post.AuthorId = topic.AuthorId;
            post.Body = model.Body;
            post.Id = Guid.NewGuid().ToString();
            post.Parent = topic.Id;
            post.PostedAt = topic.StartedAt;
            db.ForumTopics.Add(topic);
            db.ForumPosts.Add(post);
            db.SaveChanges();
            NotificationDaemon.NotifyFollowers(User.Identity.GetUserId(), "New topic started by " + ACL.UserNameRaw(User.Identity.GetUserId()) + "!", ACL.UserNameRaw(User.Identity.GetUserId()) + " just started a topic: " + topic.Subject + ".", Url.Action("ViewTopic", new { id = topic.Discriminator }));

            return RedirectToAction("ViewTopic", new { id = topic.Discriminator });

        }

        const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890abcdefghijklmnopqrstuvwxyz";

        [Authorize]
        public ActionResult ViewUnread()
        {
            var db = new ApplicationDbContext();
            var posts = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name).UnreadPosts;

            return View(posts);
        }

        [Authorize]
        public ActionResult MarkRead(string id)
        {
            ACL.MarkRead(User.Identity.GetUserId(), id);
            return RedirectToAction("ViewUnread");
        }



        [Authorize]
        public ActionResult ViewTopic(string id, bool triedtolikeowntopic = false, int page = 1)
        {
            int realpage = page - 1;
            int pageSize = 10;
            if (triedtolikeowntopic)
                ViewBag.Error = "You cannot like or dislike your own topic!";
            var db = new ApplicationDbContext();
            var topic = db.ForumTopics.FirstOrDefault(x => x.Discriminator == id);
            if (topic == null)
                return new HttpStatusCodeResult(404);
            if (!ACL.CanSee(User.Identity.Name, topic.Parent))
                return new HttpStatusCodeResult(403);
            int pages = topic.Posts.GetPageCount(pageSize);

            ViewBag.Page = realpage;
            ViewBag.PageCount = pages;
            ViewBag.PageSize = pageSize;
            return View(topic);
        }
    }

    public static class PaginationExtensions
    {
        public static int GetPageCount<T>(this IEnumerable<T> collection, int pageSize)
        {
            return (collection.Count() + pageSize - 1) / pageSize;
        }

        public static T[] GetItemsOnPage<T>(this T[] collection, int page, int pageSize)
        {
            List<T> obj = new List<T>();
            
            for(int i = pageSize * page; i < pageSize + (pageSize * page) && i < collection.Count(); i++)
            {
                obj.Add(collection[i]);
            }
            return obj.ToArray();
        }
    }
}