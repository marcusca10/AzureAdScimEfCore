// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

namespace Microsoft.SCIM.WebHostSample.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Microsoft.SCIM;
    using Microsoft.SCIM.WebHostSample.Models;

    public class InMemoryStorageService : IStorageService
    {
        internal readonly IDictionary<string, Core2Group> Groups;
        internal readonly IDictionary<string, TargetUser> Users;

        public InMemoryStorageService()
        {
            this.Groups = new Dictionary<string, Core2Group>();
            this.Users = new Dictionary<string, TargetUser>();
        }

        private static readonly Lazy<InMemoryStorageService> InstanceValue =
                                new Lazy<InMemoryStorageService>(
                                        () =>
                                            new InMemoryStorageService());

        public InMemoryStorageService Instance
        {
            get
            {
                return InstanceValue.Value;
            }
        }

        public IEnumerable<TargetUser> QueryUsers(string userName = null, string externalId = null)
        {
            // verify what filter was supplied
            if (userName != null)
            {
                return this.Users.Values.Where(
                        (TargetUser item) => string.Equals(item.UserName, userName, StringComparison.OrdinalIgnoreCase));
            }
            else if (externalId != null)
            {
                return this.Users.Values.Where(
                        (TargetUser item) => string.Equals(item.UserName, externalId, StringComparison.OrdinalIgnoreCase));
            }

            // return all users if no filter
            return this.Users.Values;
        }

        public TargetUser CreateUser(TargetUser user)
        {
            // verify if the user alredy exists
            if
            (
                this.Users.Values.Any(
                    (TargetUser exisitingUser) =>
                        string.Equals(exisitingUser.UserName, user.UserName, StringComparison.Ordinal))
            )
            {
                throw new Exception("Conflict");
            }

            // create user
            Guid resourceIdentifier = Guid.NewGuid();
            user.Identifier = resourceIdentifier;
            this.Users.Add(resourceIdentifier.ToString(), user);

            return this.Users[resourceIdentifier.ToString()];
        }

        public void DeleteUser(Guid identifier)
        {
            if (this.Users.ContainsKey(identifier.ToString()))
            {
                this.Users.Remove(identifier.ToString());
            }
            else
                throw new Exception("NotFound");
        }

        public TargetUser UpdateUser(TargetUser user)
        {
            // check if UserName exists under different Identifier
            if
            (
                this.Users.Values.Any(
                    (TargetUser exisitingUser) =>
                        string.Equals(exisitingUser.UserName, user.UserName, StringComparison.Ordinal) &&
                        !(exisitingUser.Identifier == user.Identifier))
            )
            {
                throw new Exception("Conflict");
            }

            // check if Identifier exists
            if (!this.Users.ContainsKey(user.Identifier.ToString()))
            {
                throw new Exception("NotFound");
            }

            // replace user
            this.Users[user.Identifier.ToString()] = user;

            return this.Users[user.Identifier.ToString()];
        }

        public TargetUser RetrieveUser(Guid identifier)
        {
            // check if Identifier exists
            if (!this.Users.ContainsKey(identifier.ToString()))
                throw new Exception("NotFound");

            return this.Users[identifier.ToString()];
        }
    }
}
