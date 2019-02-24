﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityPricing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;



namespace CommunityPricing.Areas.Authorization
{
    public class AdministratorsAuthorizationHandler_Vendor
        : AuthorizationHandler<OperationAuthorizationRequirement, Vendor>
        {
            protected override Task HandleRequirementAsync(
                                                  AuthorizationHandlerContext context,
                                        OperationAuthorizationRequirement requirement,
                                         Vendor resource)
            {
                if (context.User == null)
                {
                    return Task.CompletedTask;
                }

                // Administrators can do anything.
                if (context.User.IsInRole(Constants.AdministratorsRole))
                {
                    context.Succeed(requirement);
                }

                return Task.CompletedTask;
            }
        }
}


