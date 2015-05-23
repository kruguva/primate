using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;
using Umbraco.Web.Security;

namespace IntouchAfrica2.DataAccess
{
    public class StructureRepository : IDisposable
    {
        private UmbracoDatabase _db;
        private IMemberService _memberService;

        private IEnumerable<StructuralGroup> _allGroupCache;

        public StructureRepository(UmbracoDatabase db, IMemberService memberService)
        {
            this._db = db;
            this._memberService = memberService;
        }

        public IEnumerable<Member> GetMembers(int structuralGroupId, string memberType)
        {
            var groupMembers = _db.Fetch<GroupMember>("SELECT * FROM GroupMember WHERE StructuralGroupId = @0", structuralGroupId);

            var members = groupMembers.Select(g => (Member)_memberService.GetById(g.MemberId));
            if (string.IsNullOrEmpty(memberType)) return members;
            else return members.Where(m => m.ContentType.Name == memberType);
        }

        public IEnumerable<StructuralGroup> GetAllGroups()
        {
            if (_allGroupCache == null)
            {
                _allGroupCache = _db.Fetch<StructuralGroup>("SELECT * FROM StructuralGroup ORDER BY Type DESC");
            }
            return _allGroupCache;
        }

        public StructuralGroup GetById(int id)
        {
            if (_allGroupCache != null)
                return _allGroupCache.SingleOrDefault(g => g.Id == id);

            return _db.SingleOrDefault<StructuralGroup>(id);
        }

        public IEnumerable<StructuralGroup> GetLeaves(MembershipHelper membershipHelper, StructuralGroup root)
        {
            PopulateChildren(root);
            var leaves = new List<StructuralGroup>();
            PopulateLeaves(root, leaves);

            foreach (var leaf in leaves)
                leaf.Parent = GetAllGroups().SingleOrDefault(g => g.Id == leaf.ParentId);

            return leaves;
        }

        public StructuralGroup GetRootForMember(IPublishedContent member)
        {
            var allGroups = GetAllGroups();
            if (!allGroups.Any()) return null;

            var groupMembers = _db.Fetch<GroupMember>("SELECT * FROM GroupMember WHERE MemberId = @0", member.Id);
            var root = allGroups.FirstOrDefault(g => groupMembers.Any(m => m.StructuralGroupId == g.Id));
            if (root == null) root = allGroups.First();

            PopulateChildren(root);
            return root;
        }

        public StructuralGroup GetStructureFrom(int structuralGroupId)
        {
            var group = GetAllGroups().FirstOrDefault(g => g.Id == structuralGroupId);
            PopulateChildren(group);
            return group;
        }

        public void PopulateChildren(StructuralGroup group)
        {
            group.ChildGroups = GetAllGroups().Where(g => g.ParentId == group.Id).ToList();
            foreach (var child in group.ChildGroups)
                PopulateChildren(child);
        }

        public void PopulateLeaves(StructuralGroup root, List<StructuralGroup> leaves)
        {
            if (root.Type == (int)StructuralGroupType.Leaf) leaves.Add(root);
            else
                foreach (var child in root.ChildGroups)
                    PopulateLeaves(child, leaves);
        }

        public void Insert(StructuralGroup group)
        {
            _db.Insert(group);
        }

        public void Update(StructuralGroup group)
        {
            _db.Update(group);
        }

        public void Delete(StructuralGroup group)
        {
            _db.Delete(group);
        }

        public void Dispose()
        {
            
        }
    }
}