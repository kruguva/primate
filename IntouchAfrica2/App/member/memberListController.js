angular.module('intouch').controller('memberListController', ['$scope', '$routeParams', 'memberService', 'toaster', '$window', '$location', function ($scope, $routeParams, memberService, toaster, $window, $location) {

    $scope.type = $routeParams.type;

    memberService.getMembersOfType($routeParams.type).then(function (members) {
        $scope.members = members;
    });

}]);