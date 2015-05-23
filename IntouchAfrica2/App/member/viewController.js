angular.module('intouch').controller('memberViewController', ['$scope', '$routeParams', 'memberService', 'toaster', '$window', '$location', function ($scope, $routeParams, memberService, toaster, $window, $location) {
    memberService.getMemberById($routeParams.id).then(function (member) {
        $scope.memberProperties = []

        for (var prop in member) {
            if (!member.hasOwnProperty(prop)) { continue };

            $scope.memberProperties.push({ name: prop, value: member[prop] });
        }
        $scope.member = member;
    }, handleError);

    function handleError(err) {
        toaster.pop('error', 'Error', err);
    }
}]);