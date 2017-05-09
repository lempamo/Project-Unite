using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_Unite
{
    /// <summary>
    /// Tells the Unite request router that this view/action requires administrative permissions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequiresAdmin : Attribute
    {
    }

    /// <summary>
    /// Tells the Unite request router that this view/action requires developer permissions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequiresDeveloper : Attribute
    {
    }

    /// <summary>
    /// Tells the Unite request router that this view/action requires moderator permissions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequiresModerator : Attribute
    {
    }

}