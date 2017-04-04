using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Project_Unite.Models;

namespace Project_Unite
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            try
            {
                var siteConfig = new ApplicationDbContext().Configs.FirstOrDefault();

                var smtp = new SmtpClient(siteConfig.SMTPServer, siteConfig.SMTPPort);
                if (siteConfig.UseTLSEncryption)
                    smtp.EnableSsl = true; //This is misleading... We want TLS but all we have is SSL. Oh well.
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(siteConfig.SMTPUsername, siteConfig.SMTPPassword);
                var sMsg = new MailMessage(siteConfig.SMTPReturnAddress, message.Destination);

                sMsg.Body = @"<img src=""https://cdn.discordapp.com/attachments/241613675545231360/280020406528901131/unknown.png""/>

<h1>Message from the ShiftOS staff</h1>

<p>" + CommonMark.CommonMarkConverter.Convert(message.Body) + "</p>";
                sMsg.Subject =  $"[{siteConfig.SiteName}] " + message.Subject;
                sMsg.IsBodyHtml = true;
                var db = new ApplicationDbContext();
                db.AuditLogs.Add(new AuditLog("system", AuditLogLevel.Admin, $"Email sending...<br/><br/><strong>To:</strong> {sMsg.To}<br/><strong>Subject:</strong><br/>{sMsg.Subject}"));
                db.SaveChanges();
                smtp.SendCompleted += (o, a) =>
                {
                    var alog = new AuditLog("system", AuditLogLevel.Admin, "");
                    if (a.Cancelled == true)
                        alog.Description += "Send cancelled.<br/>";
                    if (a.Error == null)
                        alog.Description += "No errors detected.</br>";
                    else
                        alog.Description += $"Error:<br/><pre><code class=\"language-csharp\">{a.Error}</code></pre>";
                    var ndb = new ApplicationDbContext();
                    ndb.AuditLogs.Add(alog);
                    ndb.SaveChanges();
                };
                smtp.SendAsync(sMsg, null);
            }
            catch (Exception ex)
            {
                var db = new ApplicationDbContext();
                db.AuditLogs.Add(new AuditLog("system", AuditLogLevel.Admin, $@"Failed to send email:

{ex}"));
                db.SaveChanges();
            }
            return Task.FromResult(0);
        }

        
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
