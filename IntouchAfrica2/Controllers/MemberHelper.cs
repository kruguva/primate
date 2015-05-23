using IntouchAfrica2.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;

namespace IntouchAfrica2.Controllers
{
    public class MemberHelper
    {
        public static IEnumerable<Member> GetMembers(UmbracoDatabase db, IMemberService memberService, int structuralGroupId, string memberType)
        {
            var groupMembers = db.Fetch<GroupMember>("SELECT * FROM GroupMember WHERE StructuralGroupId = @0", structuralGroupId);

            var members = groupMembers.Select(g => (Member)memberService.GetById(g.MemberId));
            if (string.IsNullOrEmpty(memberType)) return members;
            else return members.Where(m => m.ContentType.Name == memberType);
        }

        public static GroupMember AddMemberToGroup(UmbracoDatabase db, int memberId, int structuralGroupId)
        {
            var groupMember = new GroupMember()
            {
                MemberId = memberId,
                StructuralGroupId = structuralGroupId
            };

            using (var repository = new GroupMemberRepository(db))
            {
                repository.Insert(groupMember);
                return groupMember;
            }
        }
    }
}