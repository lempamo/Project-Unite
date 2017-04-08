using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Project_Unite.Models;

namespace Project_Unite.Controllers
{
    public class BlogController : Controller
    {
        // GET: Blog
        public ActionResult Index()
        {
            return View(new ApplicationDbContext().BlogPosts);
        }

        public ActionResult ViewBlog(string id)
        {
            var db = new ApplicationDbContext();
            var blog = db.BlogPosts.FirstOrDefault(x => x.Id == id);
            if (blog == null)
                return new HttpStatusCodeResult(404);
            return View(blog);
        }

        [Authorize]
        public ActionResult DislikePost(string id)
        {
            var db = new ApplicationDbContext();
            var topic = db.BlogPosts.FirstOrDefault(x => x.Id == id);
            var uid = User.Identity.GetUserId();
            if (topic == null)
                return new HttpStatusCodeResult(404);
            if (topic.AuthorId == User.Identity.GetUserId())
                return RedirectToAction("Index", new { id = id, triedtolikeowntopic = true });
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
            return RedirectToAction("Index", new { id = id });
        }

        [Authorize]
        public ActionResult LikePost(string id)
        {
            var db = new ApplicationDbContext();
            var topic = db.BlogPosts.FirstOrDefault(x => x.Id == id);
            var uid = User.Identity.GetUserId();
            if (topic == null)
                return new HttpStatusCodeResult(404);
            if (topic.AuthorId == User.Identity.GetUserId())
                return RedirectToAction("Index", new { id = id, triedtolikeowntopic = true });
            var like = db.Likes.Where(x => x.Topic == topic.Id).FirstOrDefault(x => x.User == uid);
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
            return RedirectToAction("Index", new { id = id });
        }


        [ValidateInput(false)]
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewBlog(string id, string comment)
        {
            var db = new ApplicationDbContext();
            var blog = db.BlogPosts.FirstOrDefault(x => x.Id == id);
            if (blog == null)
                return new HttpStatusCodeResult(404);
            if (string.IsNullOrWhiteSpace(comment))
            {
                ViewBag.Error = "You must enter a comment with actual text in it.";
                return View(blog);
            }
            if(comment.Length < 20)
            {
                ViewBag.Error = "Your comment must have at least 20 characters in it.";
                return View(blog);
            }
            var post = new ForumPost();
            post.AuthorId = User.Identity.GetUserId();
            post.Body = comment;
            post.Id = Guid.NewGuid().ToString();
            post.Parent = id;
            post.PostedAt = DateTime.Now;
            db.ForumPosts.Add(post);
            db.SaveChanges();

            return View(blog);
        }

        [Authorize]
        public ActionResult PostBlog()
        {
            if (!ACL.Granted(User.Identity.Name, "CanBlog"))
                return new HttpStatusCodeResult(403);

            var model = new PostBlogViewModel();
            return View(model);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult PostBlog(PostBlogViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var db = new ApplicationDbContext();
            var blog = new BlogPost();
            blog.AuthorId = User.Identity.GetUserId();
            blog.Contents = model.Contents;
            blog.Name = model.Name;
            blog.Id = model.Name.ToLower();
            string allowed = "-_abcdefghijklmnopqrstuvwxyz1234567890";
            foreach(var c in blog.Id.ToCharArray())
            {
                if (!allowed.Contains(c))
                    blog.Id = blog.Id.Replace(c, '_');
            }
            blog.Id += "_" + db.BlogPosts.Count().ToString();
            blog.PostedAt = DateTime.Now;
            db.BlogPosts.Add(blog);
            db.SaveChanges();
            return RedirectToAction("ViewBlog", new { id = blog.Id });
        }
    }
}