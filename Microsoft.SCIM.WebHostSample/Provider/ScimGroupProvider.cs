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

    public class ScimGroupProvider : ProviderBase
    {
        private readonly IStorageService _storageService;

        public ScimGroupProvider(IStorageService storageService)
        {
            this._storageService = storageService;
        }

        public override Task<Resource> CreateAsync(Resource resource, string correlationIdentifier)
        {
            if (resource.Identifier != null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            Core2Group group = resource as Core2Group;

            if (string.IsNullOrWhiteSpace(group.DisplayName))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            // call service
            TargetGroup target;
            try
            {
                target = _storageService.CreateGroup((TargetGroup)group);
            }
            catch (Exception err)
            {
                switch (err.Message)
                {
                    case "Conflict":
                        throw new HttpResponseException(HttpStatusCode.Conflict);
                    case "InvalidMemberType":
                        throw new HttpResponseException(HttpStatusCode.NotAcceptable);
                    default:
                        throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }

            return Task.FromResult((Core2Group)target as Resource);
        }

        public override Task DeleteAsync(IResourceIdentifier resourceIdentifier, string correlationIdentifier)
        {
            if (string.IsNullOrWhiteSpace(resourceIdentifier?.Identifier))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            // call service
            try
            {
                Guid identifier = new Guid(resourceIdentifier.Identifier);
                _storageService.DeleteGroup(identifier);
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
            IEnumerable<TargetGroup> buffer = Enumerable.Empty<TargetGroup>();

            // get all users if no filter
            if (queryFilter == null)
            {
                buffer = this._storageService.QueryGroups();
            }
            else
            {
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

                if (queryFilter.AttributePath.Equals(AttributeNames.DisplayName))
                {
                    buffer = this._storageService.QueryGroups(displayName: parameters.AlternateFilters.Single().ComparisonValue);
                }
                else
                {
                    throw new NotSupportedException(SampleServiceResources.UnsupportedFilterAttributeGroup);
                }
            }

            results =
                buffer
                .Select((TargetGroup item) =>
                 {
                     Core2Group bufferItem = (Core2Group)item;

                     if (parameters?.ExcludedAttributePaths?.Any(
                             (string excludedAttributes) =>
                                 excludedAttributes.Equals(AttributeNames.Members, StringComparison.OrdinalIgnoreCase))
                         == true)
                     {
                         bufferItem.Members = null;
                     }

                     return bufferItem;
                 })
                .Select((Core2Group item) => item as Resource).ToArray();

            return Task.FromResult(results);
        }

        public override Task<Resource> ReplaceAsync(Resource resource, string correlationIdentifier)
        {
            if (resource.Identifier == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            Core2Group group = resource as Core2Group;

            if (string.IsNullOrWhiteSpace(group.DisplayName))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            // call service
            TargetGroup target;
            try
            {
                target = _storageService.UpdateGroup((TargetGroup)resource);
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

            return Task.FromResult((Core2Group)target as Resource);
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
            TargetGroup result = null;
            try
            {
                result = _storageService.RetrieveGroup(new Guid(parameters.ResourceIdentifier.Identifier));
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

            return Task.FromResult((Core2Group)result as Resource);
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
            TargetGroup target;
            try
            {
                // get group
                target = _storageService.RetrieveGroup(new Guid(patch.ResourceIdentifier.Identifier));

                // patch group
                Core2Group patched = (Core2Group)target;
                patched.Apply(patchRequest);

                // update user
                _storageService.UpdateGroup((TargetGroup)patched);
            }
            catch (Exception err)
            {
                switch (err.Message)
                {
                    case "Conflict":
                        throw new HttpResponseException(HttpStatusCode.Conflict);
                    case "NotFound":
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    case "InvalidMemberType":
                        throw new HttpResponseException(HttpStatusCode.NotAcceptable);
                    default:
                        throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }
            }

            return Task.CompletedTask;
        }
    }
}
