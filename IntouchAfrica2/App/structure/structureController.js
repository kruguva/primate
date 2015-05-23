angular.module('intouch').controller('structureController', ['$scope', '$http', 'structureService', 'memberService', 'toaster', '$modal', 'confirmationService', '$route', '$window', function ($scope, $http, structureService, memberService, toaster, $modal, confirmationService, $route, $window) {
    $scope.structure = [];
    $scope.members = [];

    $scope.treehandler = getMembers;
    $scope.addLevel = addLevel;
    $scope.addClass = addClass;
    $scope.submitLevel = submitLevel;
    $scope.enrolMember = enrolMember;
    $scope.addStaff = addStaff;
    $scope.removeMember = removeMember;
    $scope.deleteGroup = deleteGroup;
    $scope.confirmDelete = confirmDelete;
    $scope.cancelDelete = cancelDelete;

    $scope.status = {
        createisopen: false,
        viewisopen: false
    };

    structureService.getStructureAsTree().then(function (structure) {
        $scope.structure = structure;
    });

    function getMembers(branch) {
        console.log(branch);
        $scope.selectedLevelId = branch.data.Id;
        $scope.selectedLevelType = branch.data.TypeDescription;

        structureService.getMembers(branch.data.Id).then(function (members) {
            $scope.members = members;
        });
    }

    function addLevel() {
        $scope.addingLevel = true;
    }

    function deleteGroup() {
        $scope.requestConfirmDelete = true;
    }

    function confirmDelete() {
        structureService.deleteGroup($scope.selectedLevelId)
            .then(function () {
                toaster.pop('success', 'Deleted', 'Deleted Class or Level');
                $window.location.reload();
            }).catch(handleError);

        $scope.requestConfirmDelete = false;
    }

    function cancelDelete() {
        $scope.requestConfirmDelete = false;
    }

    function submitLevel() {
        structureService.insertGroup({
            Name: $scope.levelName,
            TypeDescription: $scope.typeDescription,
            ParentId: $scope.selectedLevelId ? $scope.selectedLevelId : null
        })
            .then(function (level) {
                toaster.pop('success', 'Created Level', 'Successfully created ' + $scope.typeDescription + ' ' + level.Name)

                var parent = findNode($scope.structure[0], level.ParentId);
                if (!parent.children) {
                    parent.children = [];
                }
                parent.children.push({
                    label: level.Name,
                    data: level,
                    children: []
                });
            })
            .catch(handleError);

        $scope.levelName = '';
        $scope.typeDescription = '';
        $scope.addingLevel = false;
    }

    function findNode(element, id) {
        var stack = [], node, ii;
        stack.push(element);

        while (stack.length > 0) {
            node = stack.pop();
            if (node.data.Id == id) {
                break;
            } else if (node.children && node.children.length) {
                for (ii = 0; ii < node.children.length; ii += 1) {
                    stack.push(node.children[ii]);
                }
            }
        }

        return node;
    }

    function addClass() {
        $scope.addingLevel = true;
        $scope.typeDescription = 'Class';
    }

    function removeLevel() {
        confirmationService.confirmAction('Are you sure you want to remove the selected level all all levels beneath it?')
            .then(function (result) {
                if (result) {
                    structureService.removeLevel($scope.selectedLevelId).then(function () {
                        $route.reload();
                    }, handleError);
                }
            });
    }

    function enrolMember() {

        //TODO: Make enrolment a seperate concept to group membership (a temporary group membership that can be hidden behind a group API) 
        //EG: Querying the Group API for members will give all staff that belong to the group via GroupMember objects OR 
        //All members currently enrolled in the group via Enrollment Objects (which have a timespan of validity) 

        addMember('student');
    }

    function addStaff() {
        addMember('teacher');
    }

    function addMember(type) {
        var modalInstance = $modal.open({
            templateUrl: '/App/templates/memberSelectionList.html',
            controller: 'memberSelectionController',
            size: 'lg',
            resolve: {
                type: function () {
                    return type;
                }
            }
        });

        modalInstance.result.then(function (selectedItems) {
            $scope.selected = selectedItems;
            for (var i = 0; i < selectedItems.length; i++) {
                memberService.addMemberToGroup(selectedItems[i].Id, $scope.selectedLevelId).then(function (groupMember) {
                    if ($scope.selectedLevelId === groupMember.StructuralGroup.Id) {
                        $scope.members.push(groupMember.Member);
                    }
                }, handleError);
            }
        }, function () {
            console.log('Modal dismissed at: ' + new Date());
        });
    }

    function removeMember(id) {
        var levelId = $scope.selectedLevelId;
        memberService.removeMemberFromGroup(id, levelId).then(function () {
            if ($scope.selectedLevelId === levelId) {
                for (var i = 0; i < $scope.members.length; i++) {
                    if ($scope.members[i].Id === id) {
                        $scope.members.splice(i, 1);
                    }
                }
            }
        }, handleError);
    }

    function handleError(error) {
        toaster.pop('error', 'Error', error);
    }

}]);