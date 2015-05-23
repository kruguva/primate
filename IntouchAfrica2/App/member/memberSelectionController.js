angular.module('intouch').controller('memberSelectionController', ['$scope', '$modalInstance', 'memberService', 'type', '$modal', function ($scope, $modalInstance, memberService, type, $modal) {

    $scope.type = type;

    memberService.getMembersOfType(type).then(function (members) {
        for (var i = 0; i < members.length; i++) {
            processMember(members[i], false);
        }
        $scope.members = members;
    });

    function processMember(member, selected) {
        member.isSelected = selected;
        member.select = (function (m) {
            return function () {
                m.isSelected = !m.isSelected;
            }
        })(member);
    }

    $scope.ok = function () {
        var selectedMembers = _.filter($scope.members, function (m) { return m.isSelected; });
        $modalInstance.close(selectedMembers);
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

    $scope.createNew = function () {
        var modalInstance = $modal.open({
            templateUrl: '/App/templates/createModalTemplate.html',
            controller: 'memberCreateModalController',
            size: 'lg',
            resolve: {
                type: function () {
                    return type;
                }
            }
        });

        modalInstance.result.then(function (model) {
            processMember(model, true);
            $scope.members.push(model);
            $scope.ok();
        }, function () {
            console.log('Modal dismissed at: ' + new Date());
        });
    };
}]);