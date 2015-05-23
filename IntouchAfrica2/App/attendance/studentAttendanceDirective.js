angular.module('intouch').directive('studentAttendance', ['structureService', 'attendanceService', '$location', 'toaster', function (structureService, attendanceService, $location, toaster) {
    return {
        restrict: 'E',
        scope: {
            studentInfo: '=studentInfo'
        },
        templateUrl: '/App/attendance/studentAttendanceTemplate.html'
    }
}])