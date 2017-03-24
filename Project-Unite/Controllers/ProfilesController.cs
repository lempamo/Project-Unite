using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Project_Unite.Models;

namespace Project_Unite.Controllers
{
    [Authorize]
    public class ProfilesController : Controller
    {
        // GET: Profiles
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewProfile(string id)
        {
            var db = new ApplicationDbContext();
            var user = db.Users.FirstOrDefault(x => x.DisplayName == id);
            if (user == null)
                return new HttpStatusCodeResult(404);

            return View(user);
        }

        public ActionResult UnfollowUser(string id)
        {
            var db = new ApplicationDbContext();
            if (db.Users.FirstOrDefault(x => x.Id == id) == null)
                return new HttpStatusCodeResult(404);
            if (!ACL.IsFollowed(User.Identity.Name, id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var uid = User.Identity.GetUserId();
            var userToFollow = db.Users.FirstOrDefault(x => x.Id == id);
            var follow = db.Follows.FirstOrDefault(x=>x.Followed==userToFollow.Id&&x.Follower==uid);
            if(follow != null)
                db.Follows.Remove(follow);
            db.SaveChanges();
            return RedirectToAction("ViewProfile", new { id = userToFollow.DisplayName });


        }


        public ActionResult FollowUser(string id)
        {
            var db = new ApplicationDbContext();
            if (db.Users.FirstOrDefault(x => x.Id == id) == null)
                return new HttpStatusCodeResult(404);
            if (ACL.IsFollowed(User.Identity.Name, id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var uid = User.Identity.GetUserId();
            var userToFollow = db.Users.FirstOrDefault(x => x.Id == id);
            var follow = new UserFollow();
            follow.Id = Guid.NewGuid().ToString();
            follow.Followed = id;
            follow.Follower = uid;
            db.Follows.Add(follow);
            db.SaveChanges();
            return RedirectToAction("ViewProfile", new { id = userToFollow.DisplayName });

            
        }

        [Authorize]
        public ActionResult DislikePost(string id)
        {
            var db = new ApplicationDbContext();
            var topic = db.UserPosts.FirstOrDefault(x => x.Id == id);
            var uid = User.Identity.GetUserId();
            if (topic == null)
                return new HttpStatusCodeResult(404);
            if (topic.UserId == User.Identity.GetUserId())
                return RedirectToAction("ViewProfile", new { id = ACL.UserNameRaw(uid) });
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
            string tid = topic.UserId;
            db.SaveChanges();
            return RedirectToAction("ViewProfile", new { id = ACL.UserNameRaw(tid) });

        }

        [Authorize]
        public ActionResult LikePost(string id)
        {
            var db = new ApplicationDbContext();
            var topic = db.UserPosts.FirstOrDefault(x => x.Id == id);
            var uid = User.Identity.GetUserId();
            if (topic == null)
                return new HttpStatusCodeResult(404);
            if (topic.UserId == User.Identity.GetUserId())
                return RedirectToAction("ViewProfile", new { id = ACL.UserNameRaw(uid) });
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
            string tid = topic.UserId;
            db.SaveChanges();
            return RedirectToAction("ViewProfile", new { id = ACL.UserNameRaw(tid) });
        }


        public ActionResult PostContent(UserPost model)
        {
            var db = new ApplicationDbContext();
            model.Id = Guid.NewGuid().ToString();
            model.PostedAt = DateTime.Now;
            model.UserId = User.Identity.GetUserId();
            db.UserPosts.Add(model);
            db.SaveChanges();
            return RedirectToAction("ViewProfile", "Profiles", new { id = ACL.UserNameRaw(User.Identity.GetUserId()) });
            
        }
    }
}