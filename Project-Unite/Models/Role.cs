using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Project_Unite.Models
{
    public class Role : IdentityRole
    {
        public string Description { get; set; }
        public string ColorHex { get; set; }

        public int Priority { get; set; }

        public bool CanViewProfiles { get; set; }
        public bool CanEditOwnProfile { get; set; }
        public bool CanEditProfiles { get; set; }
        public bool CanEditUsername { get; set; }
        public bool CanEditUsernames { get; set; }
        public bool CanIssueBan { get; set; }
        public bool CanIssueIPBan { get; set; }
        public bool CanIssueEmailBan { get; set; }
        public bool CanIssueMute { get; set; }
        public bool CanReleaseBuild { get; set; }
        public bool CanBlog { get; set; }
        public bool CanAccessModCP { get; set; }
        public bool CanAccessAdminCP { get; set; }
        public bool CanAccessDevCP { get; set; }
        public bool CanEditForumCategories { get; set; }
        public bool CanDeleteForumCategories { get; set; }
        public bool CanPostTopics { get; set; }
        public bool CanPostPolls { get; set; }
        public bool CanPostReplies { get; set; }
        public bool CanPostStatuses { get; set; }
        public bool CanEditRoles { get; set; }
        public bool CanDeleteRoles { get; set; }
        public bool CanDeleteOwnTopics { get; set; }
        public bool CanDeleteTopics { get; set; }
        public bool CanDeleteOwnPosts { get; set; }
        public bool CanDeletePosts { get; set; }
        public bool CanDeleteOwnStatuses { get; set; }
        public bool CanDeleteStatuses { get; set; }
        public bool CanEditOwnTopics { get; set; }
        public bool CanEditTopics { get; set; }
        public bool CanEditOwnPosts { get; set; }
        public bool CanEditPosts { get; set; }
        public bool CanEditOwnStatuses { get; set; }
        public bool CanVoteInPolls { get; set; }
        public bool CanDeleteUsers { get; set; }
        public bool CanAnonymizeUsers { get; set; }
        public bool CanPostSkins { get; set; }
        public bool CanEditOwnSkins { get; set; }
        public bool CanEditSkins { get; set; }
        public bool CanDeleteOwnSkins { get; set; }
        public bool CanDeleteSkins { get; set; }
        public bool CanUpvoteSkins { get; set; }
        public bool CanDownvoteSkins { get; set; }
        public bool CanStickyOwnTopics { get; set; }
        public bool CanStickyTopics { get; set; }
        public bool CanAnnounceOwnTopics { get; set; }
        public bool CanAnnounceTopics { get; set; }
        public bool CanGlobalOwnTopics { get; set; }
        public bool CanGlobalTopics { get; set; }
        public bool CanMoveOwnTopics { get; set; }
        public bool CanMoveTopics { get; set; }
        public bool CanUnlistOwnTopics { get; set; }
        public bool CanUnlistTopics { get; set; }
        public bool CanLockOwnTopics { get; set; }
        public bool CanUnlockOwnTopics { get; set; }
        public bool CanLockTopics { get; set; }
        public bool CanUnlockTopics { get; set; }

        public bool CanViewUserInfo { get; set; }

        public bool CanSeeUnlistedTopics { get; set; }

    }
}