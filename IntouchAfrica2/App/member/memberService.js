angular.module('intouch').service('memberService', ['apiService', function (apiService) {
    return {
        getCreationModel: getCreationModel,
        save: save,
        getMembersOfType: getMembersOfType,
        getMemberById: getMemberById,
        addMemberToGroup: addMemberToGroup,
        removeMemberFromGroup: removeMemberFromGroup
    };

    function getCreationModel(type) {
        return apiService.doGet('/umbraco/api/schoolmembership/creationModel', {type:type})
    }

    function save(model) {
        return apiService.doPost('/umbraco/api/schoolmembership/createFromModel', model)
    }

    function getMembersOfType(type) {
        return apiService.doGet('/umbraco/api/schoolmembership/getMembers', { type: type });
    }

    function getMemberById(id) {
        return apiService.doGet('/umbraco/api/schoolmembership/getMemberInfo', { id: id });
    }

    function addMemberToGroup(memberId, structuralGroupId) {
        return apiService.doPost('/umbraco/api/schoolmembership/addMemberToGroup', { MemberId: memberId, StructuralGroupId: structuralGroupId });
    }

    function removeMemberFromGroup(memberId, structuralGroupId) {
        return apiService.doDelete('/umbraco/api/schoolmembership/removeMemberFromGroup', { MemberId: memberId, StructuralGroupId: structuralGroupId });
    }
}]);