﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Project_Unite.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Mvc;
using System.Diagnostics;
using System.Web.Mvc.Html;
using System.Data.Entity;
using System.Text;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Principal;

namespace Project_Unite
{
    public static class ACL
    {
        public static IHtmlString NotificationCount(this HtmlHelper hpr, string uid)
        {
            var db = new ApplicationDbContext();
            var usr = db.Users.FirstOrDefault(x => x.Id == uid||x.UserName==uid);
            if (usr == null)
                return hpr.Raw("N/A");
            return hpr.Raw(usr.UnreadNotifications.ToString());
        }

        internal static ApplicationUser GetUserFromToken(string token)
        {
            var db = new ApplicationDbContext();
            var t = db.OAuthTokens.FirstOrDefault(x => x.Id == token);
            if (t == null)
                return null;
            return db.Users.FirstOrDefault(x => x.Id == t.UserId);
        }

        public static IHtmlString NewestUser(this HtmlHelper hpr)
        {
            var db = new ApplicationDbContext();
            var user = db.Users.OrderByDescending(x => x.JoinedAt).FirstOrDefault();
            if (user == null)
                return hpr.Raw(@"<a href=""#"">No new users</a>");
            else
                return hpr.Raw("<a href=\"/Profiles/ViewProfile/" + user.DisplayName + "\"><span class=\"glyphicon glyphicon-star\"></span> Our newest user, <strong>" + user.DisplayName + "</strong></a>");
        }

        internal static string MarkdownRaw(string md)
        {
            if (md == null)
                return "";
            md = ResolveUserLinksInMarkdown(md);
            return CommonMark.CommonMarkConverter.Convert(md);

        }

        public static IHtmlString GetLatestUnread(this HtmlHelper hpr, string userName)
        {
            var db = new ApplicationDbContext();
            var user = db.Users.FirstOrDefault(x => x.UserName == userName);
            if (user == null)
                return hpr.Raw("");
            var notesOrdered = user.Notifications.Where(x => x.IsRead == false).OrderByDescending(x => x.Timestamp).ToArray();
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < notesOrdered.Length && i < 5; i++)
            {
                builder.AppendLine("<li><a href=\"" + notesOrdered[i].ActionUrl + "\">");
                //Avatar holder start:
                builder.AppendLine("<div style=\"width:64px;height:64px;display:inline-block;\">");
                //Avatar
                builder.AppendLine("<img src=\"" + notesOrdered[i].AvatarUrl + "\" width=\"64\" height=\"64\"/>");
                //Avatar holder end:
                builder.AppendLine("</div>");

                //Notification title.
                builder.AppendLine("<p><strong>" + notesOrdered[i].Title + "</strong><br/><br/>");
                //Contents.
                builder.AppendLine(notesOrdered[i].Description + "</p>");

                builder.AppendLine("</a></li>");
            }
            return hpr.Raw(builder.ToString());
        }

        internal static object NotificationCountRaw(string uid)
        {
            var db = new ApplicationDbContext();
            var usr = db.Users.FirstOrDefault(x => x.Id == uid || x.UserName == uid);
            if (usr == null)
                return 0;
            return usr.UnreadNotifications;
        }

        public static bool IsUnlisted(string topicId)
        {
            return new ApplicationDbContext().ForumTopics.FirstOrDefault(x => x.Id == topicId).IsUnlisted;
        }

        public static bool IsUnread(string user, string postId)
        {
            var db = new ApplicationDbContext();
            var u = db.Users.FirstOrDefault(x => x.Id == user);
            return u.UnreadPosts.FirstOrDefault(x => x.Id == postId) != null;
        }

        public static void MarkRead(string userId, string postId)
        {
            var mark = new ReadPost();
            mark.Id = Guid.NewGuid().ToString();
            mark.PostId = postId;
            mark.UserId = userId;
            var db = new ApplicationDbContext();
            db.ReadPosts.Add(mark);
            db.SaveChanges();
        }

        public static IHtmlString TopicLinkFor(this HtmlHelper hpr, string topicId)
        {
            var topic = new ApplicationDbContext().ForumTopics.FirstOrDefault(x => x.Id == topicId);
            return hpr.ActionLink(topic.Subject, "ViewTopic", "Forum", new { id = topic.Discriminator }, null);
        }

        public static string GetUserGroup(string uid)
        {
            var db = new ApplicationDbContext();
            return db.Users.FirstOrDefault(x => x.Id == uid).GroupId;
        }

        public static string ResolveUserLinksInMarkdown(string mkdn)
        {
            string[] words = mkdn.Split(' ');
            foreach(var word in words)
            {
                if (word.StartsWith("@"))
                {
                    string uname = word.Remove(0, 1);
                    var db = new ApplicationDbContext();
                    var user = db.Users.FirstOrDefault(x => x.DisplayName == uname);
                    if(user != null)
                    {
                        mkdn = mkdn.Replace(word, $"[{word}](/Profiles/ViewProfile/{uname})");
                    }
                }
            }
            return mkdn;
        }

        public static IHtmlString Markdown(this HtmlHelper hpr, string md)
        {
            if (md == null)
                return hpr.Raw("");
            md = ResolveUserLinksInMarkdown(md);
            return hpr.Raw(CommonMark.CommonMarkConverter.Convert(hpr.Encode(md)));
        }

        public static int UnreadPostsCount(string username)
        {
            return new ApplicationDbContext().Users.FirstOrDefault(x => x.UserName == username).UnreadPosts.Length;
        }

        public static bool IsFollowed(string you, string fId)
        {
            var db = new ApplicationDbContext();
            var uid = db.Users.FirstOrDefault(x => x.UserName == you).Id;
            return db.Follows.FirstOrDefault(x => x.Follower == uid && x.Followed == fId) != null;
        }

        public static IHtmlString UserLink(this HtmlHelper hpr, string userId)
        {
            using(var db = new ApplicationDbContext())
            {
                var usr = db.Users.Include(x=>x.Roles).FirstOrDefault(x => x.Id == userId);
                if (usr == null)
                    return hpr.Raw("<a href=\"#\">system</a>");
                var userRoles = new List<Role>();
                foreach (var usrRole in usr.Roles)
                {
                    userRoles.Add(db.Roles.FirstOrDefault(r => r.Id == usrRole.RoleId) as Role);
                }
                var userRole = userRoles.OrderByDescending(m => m.Priority).FirstOrDefault();
                return hpr.ActionLink(usr.DisplayName, "ViewProfile", "Profiles", new { id = usr.DisplayName }, new { id="uname_" + usr.Id, style = userRole == null ? "color:white;" : @"color: " + userRole.ColorHex });

            }
        }

        public static IHtmlString UserName(this HtmlHelper hpr, string userId)
        {
            using (var db = new ApplicationDbContext())
            {
                var usr = db.Users.Include(x => x.Roles).FirstOrDefault(x => x.Id == userId);

                var userRoles = new List<Role>();
                foreach (var usrRole in usr.Roles)
                {
                    userRoles.Add(db.Roles.FirstOrDefault(r => r.Id == usrRole.RoleId) as Role);
                }
                var userRole = userRoles.OrderByDescending(m => m.Priority).FirstOrDefault();
                if (userRole == null)
                {
                    return hpr.Raw($@"<strong>{hpr.Encode(usr.DisplayName)}</strong>");
                }
                else
                {
                    return hpr.Raw($@"<strong style=""color:{userRole.ColorHex}"">{hpr.Encode(usr.DisplayName)}</strong>");
                }
            }
        }


        public static string UserNameRaw(string userId)
        {
            using (var db = new ApplicationDbContext())
            {
                var usr = db.Users.Include(x => x.Roles).FirstOrDefault(x => x.Id == userId);


                return usr.DisplayName;

            }
        }

        public static string UserNameFromEmailRaw(string userId)
        {
            using (var db = new ApplicationDbContext())
            {
                var usr = db.Users.Include(x => x.Roles).FirstOrDefault(x => x.UserName == userId);


                return usr.DisplayName;

            }
        }

        public static bool CanSee(string userName, string fId)
        {
            var db = new ApplicationDbContext();
            var user = db.Users.FirstOrDefault(x => x.UserName == userName);
            var frm = db.ForumCategories.FirstOrDefault(x => x.Id == fId);
            if (frm == null) return false;
            if (user == null) {
                return frm.VisibleToGuests;
            }
            else
            {
                if (user.HighestRole.IsAdmin)
                    return frm.AdminPermission > 0;
                if (user.HighestRole.IsDeveloper)
                    return frm.DeveloperPermission > 0;
                if (user.HighestRole.IsModerator)
                    return frm.ModeratorPermission > 0;
                if (user.HighestRole.IsMember)
                    return frm.MemberPermission > 0;

            }
            return false;
        }

        public static bool UserEmailConfirmed(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return true;
            return new ApplicationDbContext().Users.FirstOrDefault(x => x.UserName == username).EmailConfirmed;
        }

        public static Role LowestPriorityRole()
        {
            var db = new ApplicationDbContext();
            var roles = db.Roles;
            List<Role> actualRoles = new List<Role>();
            foreach (Role r in roles)
            {
                actualRoles.Add(r);
            }
            return actualRoles.OrderBy(x => x.Priority).First();
        }

        public static bool CanReply(string userName, string fId)
        {
            var db = new ApplicationDbContext();
            var user = db.Users.FirstOrDefault(x => x.UserName == userName);
            var frm = db.ForumCategories.FirstOrDefault(x => x.Id == fId);
            if (frm == null) return false;
            if (user == null)
            {
                return false;
            }
            else
            {
                if (user.HighestRole.IsAdmin)
                    return frm.AdminPermission > 1;
                if (user.HighestRole.IsDeveloper)
                    return frm.DeveloperPermission > 1;
                if (user.HighestRole.IsModerator)
                    return frm.ModeratorPermission > 1;
                if (user.HighestRole.IsMember)
                    return frm.MemberPermission > 1;
            }
            return false;
        }

        public static ApplicationUser GetUserInfo(string id)
        {
            return new ApplicationDbContext().Users.FirstOrDefault(x => x.Id == id);
        }

        public static bool CanPost(string userName, string fId)
        {
            var db = new ApplicationDbContext();
            var user = db.Users.FirstOrDefault(x => x.UserName == userName);
            var frm = db.ForumCategories.FirstOrDefault(x => x.Id == fId);
            if (frm == null) return false;
            if (user == null)
            {
                return false;
            }
            else
            {
                if (user.HighestRole.IsAdmin)
                    return frm.AdminPermission > 2;
                if (user.HighestRole.IsDeveloper)
                    return frm.DeveloperPermission > 2;
                if (user.HighestRole.IsModerator)
                    return frm.ModeratorPermission > 2;
                if (user.HighestRole.IsMember)
                    return frm.MemberPermission > 2;
            }
            return false;
        }

        public static bool CanManageRole(string userId, string roleId)
        {
            try
            {
                if (!HttpContext.Current.User.Identity.IsAdmin())
                    return false;

                var db = new ApplicationDbContext();

                var usr = db.Users.FirstOrDefault(u => u.UserName == userId);

                var userRoles = new List<Role>();
                foreach (var usrRole in usr.Roles)
                {
                    userRoles.Add(db.Roles.FirstOrDefault(r => r.Id == usrRole.RoleId) as Role);
                }
                var manageRole = (Role)db.Roles.FirstOrDefault(x => x.Id == roleId);

                db.Dispose();
                var userRole = (Role)userRoles.OrderByDescending(m => m.Priority).First();
                if (manageRole.Priority > userRole.Priority)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }


        public static ForumCategory GetForumById(string id)
        {
            var db = new ApplicationDbContext();
            
                return db.ForumCategories.FirstOrDefault(x => x.Id == id);
            
        }

        public static bool IsGuest(this IIdentity id)
        {
            if (HttpContext.Current.Request.IsAuthenticated)
                return false;
            return true;
        }


        public static bool IsModerator(this IIdentity id)
        {
            if (!HttpContext.Current.Request.IsAuthenticated)
                return false;
            var db = new ApplicationDbContext();
            return db.Users.FirstOrDefault(x => x.UserName == id.Name).HighestRole.IsModerator;
        }

        public static bool IsDeveloper(this IIdentity id)
        {
            if (!HttpContext.Current.Request.IsAuthenticated)
                return false;
            var db = new ApplicationDbContext();
            return db.Users.FirstOrDefault(x => x.UserName == id.Name).HighestRole.IsDeveloper;
        }

        public static bool IsMember(this IIdentity id)
        {
            if (!HttpContext.Current.Request.IsAuthenticated)
                return false;
            var db = new ApplicationDbContext();
            return db.Users.FirstOrDefault(x => x.UserName == id.Name).HighestRole.IsMember;
        }


        public static bool IsAdmin(this IIdentity id)
        {
            if (!HttpContext.Current.Request.IsAuthenticated)
                return false;
            var db = new ApplicationDbContext();
            return db.Users.FirstOrDefault(x => x.UserName == id.Name).HighestRole.IsAdmin;
        }
    }
}