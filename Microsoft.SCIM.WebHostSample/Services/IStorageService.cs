using Microsoft.SCIM.WebHostSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.SCIM.WebHostSample.Services
{
    public interface IStorageService
    {
        IEnumerable<TargetUser> QueryUsers(string userName = null, string externalId = null);
        IEnumerable<TargetGroup> QueryGroups(string displayName = null, string externalId = null);
        TargetUser CreateUser(TargetUser user);
        TargetGroup CreateGroup(TargetGroup group);
        void DeleteUser(Guid identifier);
        void DeleteGroup(Guid identifier);
        TargetUser UpdateUser(TargetUser user);
        TargetGroup UpdateGroup(TargetGroup group);
        TargetUser RetrieveUser(Guid identifier);
        TargetGroup RetrieveGroup(Guid identifier);
    }
}
