using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;

namespace IntouchAfrica2.Events
{
    [Serializable]
    public class MemberCreated : DomainEvent
    {
        public MemberCreated(Member member)
        {
            Member = member;
        }

        public Member Member { get; private set; }
    }
}