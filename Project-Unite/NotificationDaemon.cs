using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Project_Unite.Models;

namespace Project_Unite
{
    public static class NotificationDaemon
    {
        public static void NotifyFollowers(string uid, string title, string desc, string url)
        {
            var db = new ApplicationDbContext();
            var user = db.Users.FirstOrDefault(x => x.Id == uid);
            if (user == null)
                throw new Exception("Cannot find user with ID " + uid + ".");
            foreach(var follower in user.Followers)
            {
                NotifyUser(uid, follower.Follower, title, desc, url);
            }
        }

        public static void NotifyEveryone(string uid, string title, string desc, string url)
        {
            var db = new ApplicationDbContext();
            var user = db.Users.FirstOrDefault(x => x.Id == uid);
            if (user == null)
                throw new Exception("Cannot find user with ID " + uid + ".");
            foreach (var usr in db.Users.Where(x=>x.Id!=uid).ToArray())
            {
                NotifyUser(uid, usr.Id, title, desc, url);
            }
        }

        public static void NotifyUser(string uid, string target, string title, string desc, string url)
        {
            var db = new ApplicationDbContext();
            var user = db.Users.FirstOrDefault(x => x.Id == uid);
            if (user == null)
                throw new Exception("Cannot find user with ID " + target + ".");
            var note = new Notification
            {
                Id = Guid.NewGuid().ToString(),
                UserId = target,
                Title = title,
                ActionUrl = url,
                Description = desc,
                AvatarUrl = user.AvatarUrl
            };
            db.Notifications.Add(note);

            db.SaveChanges();
        }


    }
}