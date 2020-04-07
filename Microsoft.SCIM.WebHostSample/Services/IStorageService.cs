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
        TargetUser CreateUser(TargetUser user);
        void DeleteUser(Guid identifier);
        TargetUser UpdateUser(TargetUser user);
        TargetUser RetrieveUser(Guid identifier);
    }
}
