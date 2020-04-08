// Copyright (c) Microsoft Corporation.// Licensed under the MIT license.

namespace Microsoft.SCIM.WebHostSample.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Microsoft.SCIM;
    using Microsoft.SCIM.WebHostSample.Models;
    using Microsoft.SCIM.WebHostSample.Resources;
    using Microsoft.SCIM.WebHostSample.Services;

    public class ScimUserProvider : ProviderBase
    {
        private readonly IStorageService _storageService;

        public ScimUserProvider(IStorageService storageService)
        {
            this._storageService = storageService;
        }

        public override Task<Resource> CreateAsync(Resource resource, string correlationIdentifier)
        {
            if (resource.Identifier != null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            Core2EnterpriseUser user = resource as Core2EnterpriseUser;
            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            // call service
            TargetUser target;
            try
            {
                target = _storageService.CreateUser((TargetUser)user);
            }
            catch (Exception err)
            {
                switch (err.Message)
                {
                    case "Conflict":
                        throw new HttpResponseException(HttpStatusCode.Conflict);
                    default:
                        throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }

            return Task.FromResult((Core2EnterpriseUser)target as Resource);
        }

        public override Task DeleteAsync(IResourceIdentifier resourceIdentifier, string correlationIdentifier)
        {
            if (string.IsNullOrWhiteSpace(resourceIdentifier?.Identifier))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            // call service
            try
            {
                Guid identifier = new Guid(resourceIdentifier.Identifier);
                _storageService.DeleteUser(identifier);
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            return Task.CompletedTask;
        }

        public override Task<Resource[]> QueryAsync(IQueryParameters parameters, string correlationIdentifier)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            if (null == parameters.AlternateFilters)
            {
                throw new ArgumentException(SampleServiceResources.ExceptionInvalidParameters);
            }

            if (string.IsNullOrWhiteSpace(parameters.SchemaIdentifier))
            {
                throw new ArgumentException(SampleServiceResources.ExceptionInvalidParameters);
            }

            Resource[] results;
            IFilter queryFilter = parameters.AlternateFilters.SingleOrDefault();
            
            // get all users if no filter
            if (queryFilter == null)
            {
                IEnumerable<TargetUser> allUsers = this._storageService.QueryUsers();
                results =
                    allUsers.Select((TargetUser user) => (Core2EnterpriseUser)user as Resource).ToArray();

                return Task.FromResult(results);
            }

            if (string.IsNullOrWhiteSpace(queryFilter.AttributePath))
            {
                throw new ArgumentException(SampleServiceResources.ExceptionInvalidParameters);
            }

            if (string.IsNullOrWhiteSpace(queryFilter.ComparisonValue))
            {
                throw new ArgumentException(SampleServiceResources.ExceptionInvalidParameters);
            }

            if (queryFilter.FilterOperator != ComparisonOperator.Equals)
            {
                throw new NotSupportedException(SampleServiceResources.UnsupportedComparisonOperator);
            }

            if (queryFilter.AttributePath.Equals(AttributeNames.UserName))
            {
                IEnumerable<TargetUser> filteredUsers = this._storageService.QueryUsers(userName: parameters.AlternateFilters.Single().ComparisonValue);
                results = filteredUsers.Select((TargetUser user) => (Core2EnterpriseUser)user as Resource).ToArray();

                return Task.FromResult(results);
            }

            if (queryFilter.AttributePath.Equals(AttributeNames.ExternalIdentifier))
            {
                IEnumerable<TargetUser> filteredUsers = this._storageService.QueryUsers(externalId: parameters.AlternateFilters.Single().ComparisonValue);
                results = filteredUsers.Select((TargetUser user) => (Core2EnterpriseUser)user as Resource).ToArray();

                return Task.FromResult(results);
            }

            throw new NotSupportedException(SampleServiceResources.UnsupportedFilterAttributeUser);
        }

        public override Task<Resource> ReplaceAsync(Resource resource, string correlationIdentifier)
        {
            if (resource.Identifier == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            Core2EnterpriseUser user = resource as Core2EnterpriseUser;

            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            // call service
            TargetUser target;
            try
            {
                target = _storageService.UpdateUser((TargetUser)resource);
            }
            catch (Exception err)
            {
                switch (err.Message)
                {
                    case "Conflict":
                        throw new HttpResponseException(HttpStatusCode.Conflict);
                    case "NotFound":
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    default:
                        throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }

            return Task.FromResult((Core2EnterpriseUser)target as Resource);
        }

        public override Task<Resource> RetrieveAsync(IResourceRetrievalParameters parameters, string correlationIdentifier)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            if (string.IsNullOrEmpty(parameters?.ResourceIdentifier?.Identifier))
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            // call service
            TargetUser result = null;
            try
            {
                result = _storageService.RetrieveUser(new Guid(parameters.ResourceIdentifier.Identifier));
            }
            catch (Exception err)
            {
                switch (err.Message)
                {
                    case "NotFound":
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    default:
                        throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }

            return Task.FromResult((Core2EnterpriseUser)result as Resource);
        }

        public override Task UpdateAsync(IPatch patch, string correlationIdentifier)
        {
            if (null == patch)
            {
                throw new ArgumentNullException(nameof(patch));
            }

            if (null == patch.ResourceIdentifier)
            {
                throw new ArgumentException(SampleServiceResources.ExceptionInvalidPatch);
            }

            if (string.IsNullOrWhiteSpace(patch.ResourceIdentifier.Identifier))
            {
                throw new ArgumentException(SampleServiceResources.ExceptionInvalidPatch);
            }

            if (null == patch.PatchRequest)
            {
                throw new ArgumentException(SampleServiceResources.ExceptionInvalidPatch);
            }

            PatchRequest2 patchRequest = patch.PatchRequest as PatchRequest2;

            if (null == patchRequest)
            {
                string unsupportedPatchTypeName = patch.GetType().FullName;
                throw new NotSupportedException(unsupportedPatchTypeName);
            }

            // call service
            TargetUser target;
            try
            {
                // get user
                target = _storageService.RetrieveUser(new Guid(patch.ResourceIdentifier.Identifier));

                // patch user
                Core2EnterpriseUser patched = (Core2EnterpriseUser)target;
                patched.Apply(patchRequest);

                // update user
                _storageService.UpdateUser((TargetUser)patched);
            }
            catch (Exception err)
            {
                switch (err.Message)
                {
                    case "Conflict":
                        throw new HttpResponseException(HttpStatusCode.Conflict);
                    case "NotFound":
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    default:
                        throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }

            return Task.CompletedTask;
        }
    }
}
