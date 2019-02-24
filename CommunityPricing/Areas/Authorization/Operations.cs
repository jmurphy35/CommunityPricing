using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace CommunityPricing.Areas.Authorization
{
    public static class Operations
    {
        public static OperationAuthorizationRequirement Create =
          new OperationAuthorizationRequirement { Name = Constants.CreateOperationName };
        public static OperationAuthorizationRequirement Read =
            new OperationAuthorizationRequirement { Name = Constants.ReadOperationsName };
        public static OperationAuthorizationRequirement Update =
            new OperationAuthorizationRequirement { Name = Constants.UpdateOperationsName };
        public static OperationAuthorizationRequirement Delete =
            new OperationAuthorizationRequirement { Name = Constants.DeleteOperationsName };
        public static OperationAuthorizationRequirement Detail =
            new OperationAuthorizationRequirement { Name = Constants.DetailOperationsName };
    }

    public class Constants
    {
        public static readonly string CreateOperationName = "Create";
        public static readonly string ReadOperationsName = "Read";
        public static readonly string UpdateOperationsName = "Update";
        public static readonly string DeleteOperationsName = "Delete";
        public static readonly string DetailOperationsName = "Detail";

        public static readonly string AdministratorsRole = "Administrator";
        public static readonly string PermittedMembersRole = "PermittedMember";
    }
}
