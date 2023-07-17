﻿using Skoruba.IdentityServer4.Shared.Configuration.Configuration.Identity;
using UserController.STS.Identity.Configuration.Interfaces;

namespace UserController.STS.Identity.Configuration
{
    public class RootConfiguration : IRootConfiguration
    {
        public AdminConfiguration AdminConfiguration { get; } = new AdminConfiguration();
        public RegisterConfiguration RegisterConfiguration { get; } = new RegisterConfiguration();
    }
}







