﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace UserController.STS.Identity.ViewModels.Manage
{
    public class ExternalLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }

        public IList<AuthenticationScheme> OtherLogins { get; set; }

        public bool ShowRemoveButton { get; set; }

        public string StatusMessage { get; set; }
    }
}








