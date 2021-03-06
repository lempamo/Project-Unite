﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_Unite.Models
{
    public class CreateUserModel
    {
        
        public string Email { get; set; }
        public string Username { get; set; }
        
    }

    public class AddForumCategoryViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Parent { get; set; }

        public string StealPermissionsFrom { get; set; }
        
        public IEnumerable<SelectListItem> PossibleParents { get; set; }
        public string Id { get; internal set; }
    }

    public enum PermissionPreset
    {
        None = 0,
        CanRead = 1,
        CanReply = 2,
        CanPost = 3
    }

    public class ModeratorBanListViewModel
    {
        public IEnumerable<ApplicationUser> UserBans { get; set; }
        public IEnumerable<BannedIP> IPBans { get; set; }
    }

    public class AuditLog
    {
        public AuditLog()
        {

        }

        public AuditLog(string uid, AuditLogLevel lvl, string desc)
        {
            Id = Guid.NewGuid().ToString();
            Level = lvl;
            UserId = uid;
            Description = desc;
            Timestamp = DateTime.Now;
        }

        public string Id { get; set; }
        public AuditLogLevel Level { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
    }

    [Flags]
    public enum AuditLogLevel
    {
        Admin,
        Moderator,
        User
    }

    public class Configuration
    {
        public string Id { get; set; }
        public string WebhookUrl { get; set; }
        public string FeedbackEmail { get; set; }
        public string SiteName { get; set; }
        public string ReturnEmail { get; set; }
        public string UniteBotToken { get; set; }
        public string DiscordChannelId { get; set; }

        public string SMTPServer { get; set; }
        public int SMTPPort { get; set; }
        public string SMTPUsername { get; set; }
        public string SMTPPassword { get; set; }
        public bool UseTLSEncryption { get; set; }
        public string SMTPReturnAddress { get; set; }
    }
}