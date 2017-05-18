using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Project_Unite.Models;
using Reachmail.Easysmtp.Post.Request;
using Reachmail.Easysmtp.Post.Response;

namespace Project_Unite
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage msg)
        {
            var siteConfig = new ApplicationDbContext().Configs.First();
            var reachmail = Reachmail.Api.Create(siteConfig.SMTPUsername);

            var request = new DeliveryRequest
            {
                FromAddress = "sys@michaeltheshifter.me",
                Recipients = new Recipients {
            new Recipient {
                Address = msg.Destination
            }
        },
                Subject = "[ShiftOS] " + msg.Subject,
                BodyText = msg.Body,
                BodyHtml = "html",
                Tracking = true,
                FooterAddress = "sys@michaeltheshifter.me",
                SignatureDomain = "getshiftos.ml"
            };

            var result = reachmail.Easysmtp.Post(request);
            if (result.Failures)
            {
                var sb = new StringBuilder();
                sb.AppendLine("A fatal exception has occurred in the Unite mail backend.");
                sb.AppendLine();
                sb.AppendLine("Basically, we couldn't send messages to the following addresses: ");

                foreach(var addr in result.FailedAddresses)
                {
                    sb.AppendLine(" - " + addr.Email);
                    sb.AppendLine(" - Reason: " + addr.Reason);
                }
                sb.AppendLine("FUCKING CONTACT MICHAEL. NOW.");
                string exc = sb.ToString();
                var db = new ApplicationDbContext();
                db.AuditLogs.Add(new AuditLog("system", AuditLogLevel.Admin, exc));
                db.SaveChanges();
            }
            return Task.FromResult<DeliveryResponse>(result);
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
