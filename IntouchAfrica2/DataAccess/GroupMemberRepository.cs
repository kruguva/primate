using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Persistence;

namespace IntouchAfrica2.DataAccess
{
    public class GroupMemberRepository : IDisposable
    {
        private UmbracoDatabase _db;

        public GroupMemberRepository(UmbracoDatabase db)
        {
            _db = db;
        }

        public IEnumerable<GroupMember> GetAll()
        {
            return _db.Query<GroupMember>("SELECT * FROM GroupMember");
        }

        public GroupMember GetById(int id)
        {
            return _db.Query<GroupMember>("SELECT * FROM GroupMember WHERE Id = " + id).FirstOrDefault();
        }

        public IEnumerable<GroupMember> GetForMember(int memberId)
        {
            return _db.Query<GroupMember>("SELECT * FROM GroupMember WHERE MemberId = " + memberId);
        }

        public IEnumerable<GroupMember> GetForGroup(int structuralGroupId)
        {
            return _db.Query<GroupMember>("SELECT * FROM GroupMember WHERE StructuralGroupId = " + structuralGroupId);
        }

        public GroupMember Get(int memberId, int structuralGroupId)
        {
            var query = string.Format("SELECT * FROM GroupMember WHERE StructuralGroupId = {0} AND MemberID = {1}", structuralGroupId, memberId);
            return _db.Query<GroupMember>(query).FirstOrDefault();
        }

        public GroupMember Insert(GroupMember member)
        {
            _db.Insert(member);
            return member;
        }

        public void Delete(GroupMember member)
        {
            _db.Delete(member);
        }

        public void Dispose()
        {
            
        }
    }
}