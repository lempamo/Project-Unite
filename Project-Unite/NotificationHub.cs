﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace Project_Unite
{
    public class NotificationHub : Hub
    {
        public string Activate()
        {
            return "Monitor Activated";
            
        }
    }
}