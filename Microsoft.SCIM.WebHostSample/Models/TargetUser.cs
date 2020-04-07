using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.SCIM.WebHostSample.Models
{
    public class TargetUser
    {
        public Guid Identifier { get; set; }
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

        #region Core2EnterpriseUser conversion

        // To Core2EnterpriseUser
        public static explicit operator Core2EnterpriseUser(TargetUser user)
        {
            Core2EnterpriseUser result = new Core2EnterpriseUser();

            result.Identifier = user.Identifier.ToString();
            result.UserName = user.UserName;
            result.Active = user.Active;
            result.DisplayName = user.DisplayName;
            result.Title = user.JobTitle;

            result.ElectronicMailAddresses = new List<ElectronicMailAddress>()
            {
                new ElectronicMailAddress()
                {
                    ItemType = "work",
                    Primary = true,
                    Value = user.Mail
                }
            };

            result.PreferredLanguage = user.PreferredLanguage;

            result.Name = new Name()
            {
                GivenName = user.GivenName,
                FamilyName = user.Surname,
                Formatted = user.FormattedName
            };

            result.Addresses = new List<Address>()
            {
                new Address()
                {
                    ItemType = "work",
                    Primary = true,
                    Formatted = user.PhysicalDeliveryOfficeName,
                    StreetAddress = user.StreetAddress,
                    Locality = user.City,
                    Region = user.State,
                    PostalCode = user.PostalCode,
                    Country = user.Country,
                }
            };

            result.PhoneNumbers = new List<PhoneNumber>()
            {
                new PhoneNumber()
                {
                    ItemType = "work",
                    Primary = true,
                    Value = user.TelephoneNumber
                },
                new PhoneNumber()
                {
                    ItemType = "mobile",
                    Value = user.Mobile
                },
                new PhoneNumber()
                {
                    ItemType = "fax",
                    Value = user.FacsimileTelephoneNumber
                }
            };

            result.ExternalIdentifier = user.ExternalId;

            result.EnterpriseExtension = new ExtensionAttributeEnterpriseUser2()
            {
                EmployeeNumber = user.EmployeeId,
                Department = user.Department,
                Manager = new Manager() { Value = user.Manager }
            };

            return result;
        }

        // From Core2EnterpriseUser
        public static explicit operator TargetUser(Core2EnterpriseUser user)
        {
            TargetUser result = new TargetUser();

            result.Identifier = new Guid(user.Identifier);
            result.UserName = user.UserName;
            result.Active = user.Active;
            result.DisplayName = user.DisplayName;
            result.JobTitle = user.Title;

            if (user.ElectronicMailAddresses != null)
            {
                result.Mail = user.ElectronicMailAddresses.FirstOrDefault(i => i.ItemType == "work").Value;
            }

            result.PreferredLanguage = user.PreferredLanguage;

            if (user.Name != null)
            {
                result.GivenName = user.Name.GivenName;
                result.Surname = user.Name.FamilyName;
                result.FormattedName = user.Name.Formatted;
            }

            if (user.Addresses != null)
            {
                Address workAddress = user.Addresses.FirstOrDefault(i => i.ItemType == "work");
                if (workAddress != null)
                {
                    result.StreetAddress = workAddress.StreetAddress;
                    result.City = workAddress.Locality;
                    result.State = workAddress.Region;
                    result.PostalCode = workAddress.PostalCode;
                    result.Country = workAddress.Country;
                }
            }

            if (user.PhoneNumbers != null)
            {
                result.TelephoneNumber = user.PhoneNumbers.FirstOrDefault(i => i.ItemType == "work").Value;
                result.Mobile = user.PhoneNumbers.FirstOrDefault(i => i.ItemType == "mobile").Value;
                result.FacsimileTelephoneNumber = user.PhoneNumbers.FirstOrDefault(i => i.ItemType == "fax").Value;
            }

            result.ExternalId = user.ExternalIdentifier;

            if (user.EnterpriseExtension != null)
            {
                result.EmployeeId = user.EnterpriseExtension.EmployeeNumber;
                result.Department = user.EnterpriseExtension.Department;
                result.Manager = user.EnterpriseExtension.Manager.Value;
            }

            return result;
        }

        #endregion

    }
}
