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

            List<Member> members = new List<Member>();
            if (group.Members != null)
            {
                foreach (Guid userId in group.Members)
                {
                    members.Add(
                        new Member()
                        {
                            // currently only users allowed as members
                            TypeName = "User",
                            Value = userId.ToString()
                        });
                }
            }
            result.Members = members;

            return result;
        }

        // From Core2Group
        public static explicit operator TargetGroup(Core2Group group)
        {
            TargetGroup result = new TargetGroup();
            
            if (group.Identifier != null)
                result.Identifier = new Guid(group.Identifier);
            result.DisplayName = group.DisplayName;
            result.ObjectId = group.ExternalIdentifier;


            List<Guid> members = new List<Guid>();
            if (group.Members != null)
            {
                foreach (Member member in group.Members)
                {
                    //// currently only users allowed as members
                    //if (member.TypeName != "User")
                    //    throw new Exception("InvalidMemberType");
                    members.Add(new Guid(member.Value));
                }
            }
            result.Members = members;

            return result;
        }

        #endregion

    }
}
