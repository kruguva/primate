using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntouchAfrica2.ViewModels
{
    public class GroupMemberViewModel
    {
        public GroupMemberViewModel()
        {

        }

        public GroupMemberViewModel(MemberViewModel member, StructuralGroupViewModel structuralGroup)
        {
            Member = member;
            StructuralGroup = structuralGroup;
        }

        public MemberViewModel Member { get; set; }
        public StructuralGroupViewModel StructuralGroup { get; set; }

        public int MemberId { get; set; }
        public int StructuralGroupId { get; set; }
    }
}