angular.module('intouch').controller('appController', ['$scope', '$window', '$http', 'toaster', function ($scope, $window, $http, toaster) {
    $scope.logout = function () {
        var request = $http({
            method: "post",
            url: "/umbraco/api/schoolmembership/logout"
        });

        request.then(function () {
            $window.location.href = "/";
        }, function (error) {
            toaster.pop('error', 'Error', error);
        });
    }
}]);