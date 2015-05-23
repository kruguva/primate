angular.module('intouch').service('structureService', ['$http', '$q', function ($http, $q) {
    return {
        getStructure: getStructure,
        getStructureAsTree: getStructureAsTree,
        getMembers: getMembers,
        getLeaves: getLeaves,
        insertGroup: insertGroup,
        updateGroup: updateGroup,
        deleteGroup: deleteGroup
    };

    function getStructure() {
        var request = $http({
            method: "get",
            url: "/umbraco/api/structure/getRoot"
        });

        return (request.then(handleSuccess, handleError));
    }

    function getMembers(structuralGroupId, memberType) {
        if (!memberType) { memberType = ""; }

        var request = $http({
            method: "get",
            url: "/umbraco/api/structure/getMembers",
            params: {
                structuralGroupId: structuralGroupId,
                memberType: memberType
            }
        });

        return (request.then(handleSuccess, handleError));
    }

    function getLeaves() {
        var request = $http({
            method: "get",
            url: "/umbraco/api/structure/getLeaves"
        });

        return (request.then(handleSuccess, handleError));
    }

    function insertGroup(group) {
        var request = $http({
            method: 'post',
            url: '/umbraco/api/structure/insertGroup',
            data: group
        });

        return (request.then(handleSuccess, handleError));
    }

    function updateGroup(group) {
        var request = $http({
            method: 'put',
            url: '/umbraco/api/structure/updateGroup',
            data: group
        });

        return (request.then(handleSuccess, handleError));
    }

    function deleteGroup(id) {
        var request = $http({
            method: 'delete',
            url: '/umbraco/api/structure/deleteGroup',
            params: {
                id: id
            }
        });

        return (request.then(handleSuccess, handleError));
    }

    function getStructureAsTree() {
        var deferred = $q.defer();

        getStructure().then(function (structure) {
            deferred.resolve(convertStructure(structure));
        }, function (err) {
            deferred.reject(err);
        });

        return deferred.promise;
    }

    function convertStructure(structure) {
        var out = [];
        out.push({
            label: structure.Name,
            data: structure,
            children: processStructure(structure)
        });

        return out;
    }

    function processStructure(structure) {
        var out = [];
        for (var i = 0; i < structure.ChildGroups.length; i++) {
            var struct = structure.ChildGroups[i];
            out.push({
                label: struct.Name,
                data: struct,
                children: processStructure(struct)
            });
        }
        return out;
    }

    function handleError(response) {
        if (!angular.isObject(response.data) ||!response.data.message) {
            return ($q.reject("An unknown error occurred."));
        }

        return ($q.reject(response.data.message));
    }

    function handleSuccess(response) {
        return (response.data);
    }
}]);