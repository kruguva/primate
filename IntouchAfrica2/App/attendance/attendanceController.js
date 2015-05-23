angular.module('intouch').controller('attendanceController', ['$scope', 'attendanceService', 'structureService', 'toaster', function ($scope, attendanceService, structureService, toaster) {

    $scope.date = new Date();
    attendanceService.getAttendanceForMember($scope.date).then(function (data) {
        $scope.presentCount = 0;
        $scope.absentCount = 0;
    }, function (error) {
        toaster.pop('error', 'Error', error);
    });

}]);