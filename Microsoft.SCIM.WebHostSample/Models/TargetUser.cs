using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.SCIM.WebHostSample.Models
{
    public class TargetUser
    {
        public string UserName { get; set; } //userPrincipalName -> userName

        public bool Active { get; set; } //Switch([IsSoftDeleted], , "False", "True", "True", "False") -> active
        public string DisplayName { get; set; } //displayName
        public string JobTitle { get; set; } //title
        public string Mail { get; set; } //emails[type eq "work"].value
        public string PreferredLanguage { get; set; } //preferredLanguage
        public string GivenName { get; set; } //name.givenName
        public string Surname { get; set; } //name.familyName
        public string FormattedName { get; set; } //Join(" ", [givenName], [surname]) { get; set; } -> name.formatted
        public string PhysicalDeliveryOfficeName { get; set; } //addresses[type eq "work"].formatted
        public string StreetAddress { get; set; } //addresses[type eq "work"].streetAddress
        public string City { get; set; } //addresses[type eq "work"].locality
        public string State { get; set; } //addresses[type eq "work"].region
        public string PostalCode { get; set; } //addresses[type eq "work"].postalCode
        public string Country { get; set; } //addresses[type eq "work"].country
        public string TelephoneNumber { get; set; } //phoneNumbers[type eq "work"].value
        public string Mobile { get; set; } //phoneNumbers[type eq "mobile"].value
        public string FacsimileTelephoneNumber { get; set; } //phoneNumbers[type eq "fax"].value
        public string ExternalId { get; set; } //MailNickname -> externalId
        public string EmployeeId { get; set; } //urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:employeeNumber
        public string Department { get; set; } //urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:department
        public string Manager { get; set; } //urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:manager
    }
}
