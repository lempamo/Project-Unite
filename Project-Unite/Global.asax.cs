using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Project_Unite.Models;

namespace Project_Unite
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var configuration = new Migrations.Configuration();
            var migrator = new DbMigrator(configuration);
            
            migrator.Update();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var configuration = new Migrations.Configuration();
            var migrator = new DbMigrator(configuration);

            migrator.Update();


            var addr = HttpContext.Current.Request.UserHostAddress;
            var db = new ApplicationDbContext();
            var ip = db.BannedIPs.FirstOrDefault(i => i.Address == addr);
            if (ip != null)
            {
                //The user is banned. Anally rape their ability to get on here.
                this.Response.StatusCode = 403;
                this.CompleteRequest();
                return;
            }
        }

        protected void Application_EndRequest(object s, EventArgs e)
        {
            var db = new ApplicationDbContext();
            if (!string.IsNullOrEmpty(User.Identity.Name))
            {
                //Check for a username ban.
                var usr = db.Users.First(x => x.UserName == User.Identity.Name);
                var banned = usr.IsBanned;
                if (banned == true)
                {
                    //The user is banned. Anally rape their ability to get on here.
                    this.Response.StatusCode = 200;
                    this.Response.ContentType = "plaintext";
                    this.Response.ClearContent();
                    this.Response.Output.Write($@"Greetings from the ShiftOS administration team, {ACL.UserNameRaw(User.Identity.GetUserId())}.

If you are seeing this page, it means two things:

1. I, Michael, am an awesome programmer and managed to get this ban system fully working.
2. You've been banned.

If you would like to appeal the ban, contact me through my email address: michaelshiftos@gmail.com

Ban information:

Bannee: {ACL.UserNameRaw(User.Identity.GetUserId())}
Banner: {ACL.UserNameRaw(usr.BannedBy)}
Timestamp: {usr.BannedAt}

DISCLAIMER:
=============

We reserve the right to disregard ban appeals if we see fit. Users other than myself or another Administrator
may administrate a ban appeal. Do not try to contact the banner. They will most likely not respond, and if they do,
it will most likely not end well for your status.

Do not attempt to multi-account or perform any form of ban-evading. If you are detected ban-evading,
you will be IP-banned and this screen won't be as friendly to you.

As it stands, this ban does not affect your ability to play ShiftOS. You can still connect to the multi-user domain,
however, if you do ban-evade, your IP ban CAN and WILL be extended to the multi-user domain.

We also reserve the right to purge your data after 2 months of a permanent ban. This is to save space on our database,
so do not cry to us when you find out you can't log in because your account got anhilated by the automatic purge system.

 - Michael VanOverbeek, on behalf of the ShiftOS staff. Play fair.");
                    this.CompleteRequest();
                    return;

                }
                if (usr.LastKnownIPAddress != Request.UserHostAddress)
                {
                    usr.LastKnownIPAddress = Request.UserHostAddress;
                    db.SaveChanges();
                }
            }

        }
    }
}
