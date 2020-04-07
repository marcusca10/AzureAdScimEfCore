using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.SCIM.WebHostSample.Models
{
    public class TargetGroup
    {
        public Guid Identifier { get; set; }
        public string DisplayName { get; set; } //displayName
        public string ObjectId { get; set; } //externalId
        public IEnumerable<Guid> Members { get; set; } //members

        #region Core2Group conversion

        // To Core2Group
        public static explicit operator Core2Group(TargetGroup group)
        {
            Core2Group result = new Core2Group();

            result.Identifier = group.Identifier.ToString();
            result.DisplayName = group.DisplayName;
            result.ExternalIdentifier = group.ObjectId;

            if (group.Members != null)
            {
                result.Members = new List<Member>();
                foreach (Guid userId in group.Members)
                {
                    result.Members.Append(
                        new Member()
                        {
                            // TypeName = "",
                            Value = userId.ToString()
                        });
                }
            }

            return result;
        }

        // From Core2Group
        public static explicit operator TargetGroup(Core2Group group)
        {
            TargetGroup result = new TargetGroup();

            result.Identifier = new Guid(group.Identifier);
            result.DisplayName = group.DisplayName;
            result.ObjectId = group.ExternalIdentifier;


            result.Members = new List<Guid>();
            if (group.Members != null)
            {
                foreach (Member member in group.Members)
                {
                    result.Members.Append(new Guid(member.Value));
                }
            }

            return result;
        }

        #endregion

    }
}
