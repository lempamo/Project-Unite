namespace Project_Unite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class acl_rsi_inducing_booleans_jesus_christ : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetRoles", "CanViewProfiles", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanEditOwnProfile", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanEditProfiles", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanEditUsername", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanEditUsernames", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanIssueBan", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanIssueIPBan", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanIssueEmailBan", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanIssueMute", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanReleaseBuild", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanBlog", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanAccessModCP", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanAccessAdminCP", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanAccessDevCP", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanEditForumCategories", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanPostTopics", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanPostPolls", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanPostReplies", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanPostStatuses", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanEditRoles", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanDeleteRoles", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanDeleteOwnTopics", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanDeleteTopics", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanDeleteOwnPosts", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanDeletePosts", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanDeleteOwnStatuses", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanDeleteStatuses", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanEditOwnTopics", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanEditTopics", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanEditOwnPosts", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanEditPosts", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanEditOwnStatuses", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanVoteInPolls", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanDeleteUsers", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanAnonymizeUsers", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanPostSkins", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanEditOwnSkins", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanEditSkins", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanDeleteOwnSkins", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanDeleteSkins", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanUpvoteSkins", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanDownvoteSkins", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanStickyOwnTopics", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanStickyTopics", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanAnnounceOwnTopics", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanAnnounceTopics", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanGlobalOwnTopics", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanGlobalTopics", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanMoveOwnTopics", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanMoveTopics", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanUnlistOwnTopics", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanUnlistTopics", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanLockOwnTopics", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanUnlockOwnTopics", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanLockTopics", c => c.Boolean());
            AddColumn("dbo.AspNetRoles", "CanUnlockTopics", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetRoles", "CanUnlockTopics");
            DropColumn("dbo.AspNetRoles", "CanLockTopics");
            DropColumn("dbo.AspNetRoles", "CanUnlockOwnTopics");
            DropColumn("dbo.AspNetRoles", "CanLockOwnTopics");
            DropColumn("dbo.AspNetRoles", "CanUnlistTopics");
            DropColumn("dbo.AspNetRoles", "CanUnlistOwnTopics");
            DropColumn("dbo.AspNetRoles", "CanMoveTopics");
            DropColumn("dbo.AspNetRoles", "CanMoveOwnTopics");
            DropColumn("dbo.AspNetRoles", "CanGlobalTopics");
            DropColumn("dbo.AspNetRoles", "CanGlobalOwnTopics");
            DropColumn("dbo.AspNetRoles", "CanAnnounceTopics");
            DropColumn("dbo.AspNetRoles", "CanAnnounceOwnTopics");
            DropColumn("dbo.AspNetRoles", "CanStickyTopics");
            DropColumn("dbo.AspNetRoles", "CanStickyOwnTopics");
            DropColumn("dbo.AspNetRoles", "CanDownvoteSkins");
            DropColumn("dbo.AspNetRoles", "CanUpvoteSkins");
            DropColumn("dbo.AspNetRoles", "CanDeleteSkins");
            DropColumn("dbo.AspNetRoles", "CanDeleteOwnSkins");
            DropColumn("dbo.AspNetRoles", "CanEditSkins");
            DropColumn("dbo.AspNetRoles", "CanEditOwnSkins");
            DropColumn("dbo.AspNetRoles", "CanPostSkins");
            DropColumn("dbo.AspNetRoles", "CanAnonymizeUsers");
            DropColumn("dbo.AspNetRoles", "CanDeleteUsers");
            DropColumn("dbo.AspNetRoles", "CanVoteInPolls");
            DropColumn("dbo.AspNetRoles", "CanEditOwnStatuses");
            DropColumn("dbo.AspNetRoles", "CanEditPosts");
            DropColumn("dbo.AspNetRoles", "CanEditOwnPosts");
            DropColumn("dbo.AspNetRoles", "CanEditTopics");
            DropColumn("dbo.AspNetRoles", "CanEditOwnTopics");
            DropColumn("dbo.AspNetRoles", "CanDeleteStatuses");
            DropColumn("dbo.AspNetRoles", "CanDeleteOwnStatuses");
            DropColumn("dbo.AspNetRoles", "CanDeletePosts");
            DropColumn("dbo.AspNetRoles", "CanDeleteOwnPosts");
            DropColumn("dbo.AspNetRoles", "CanDeleteTopics");
            DropColumn("dbo.AspNetRoles", "CanDeleteOwnTopics");
            DropColumn("dbo.AspNetRoles", "CanDeleteRoles");
            DropColumn("dbo.AspNetRoles", "CanEditRoles");
            DropColumn("dbo.AspNetRoles", "CanPostStatuses");
            DropColumn("dbo.AspNetRoles", "CanPostReplies");
            DropColumn("dbo.AspNetRoles", "CanPostPolls");
            DropColumn("dbo.AspNetRoles", "CanPostTopics");
            DropColumn("dbo.AspNetRoles", "CanEditForumCategories");
            DropColumn("dbo.AspNetRoles", "CanAccessDevCP");
            DropColumn("dbo.AspNetRoles", "CanAccessAdminCP");
            DropColumn("dbo.AspNetRoles", "CanAccessModCP");
            DropColumn("dbo.AspNetRoles", "CanBlog");
            DropColumn("dbo.AspNetRoles", "CanReleaseBuild");
            DropColumn("dbo.AspNetRoles", "CanIssueMute");
            DropColumn("dbo.AspNetRoles", "CanIssueEmailBan");
            DropColumn("dbo.AspNetRoles", "CanIssueIPBan");
            DropColumn("dbo.AspNetRoles", "CanIssueBan");
            DropColumn("dbo.AspNetRoles", "CanEditUsernames");
            DropColumn("dbo.AspNetRoles", "CanEditUsername");
            DropColumn("dbo.AspNetRoles", "CanEditProfiles");
            DropColumn("dbo.AspNetRoles", "CanEditOwnProfile");
            DropColumn("dbo.AspNetRoles", "CanViewProfiles");
        }
    }
}
