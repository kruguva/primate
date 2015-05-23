angular.module('intouch').controller('studentParentAttendanceController', ['$scope', 'structureService', 'attendanceService', '$location', 'toaster', '$q', function ($scope, structureService, attendanceService, $location, toaster, $q) {

    $scope.studentInfos = [];

    $scope.nextWeek = function () {
        setDates(7);
        loadSummary();
    }

    $scope.previousWeek = function () {
        setDates(-7);
        loadSummary();
    }

    var current = new Date();

    setDates(0);
    loadSummary();
    loadAbsenteeism();

    function setDates(offset) {
        current.setDate(current.getDate() + offset);
        $scope.startDate = startOfWeek(current);
        $scope.endDate = endOfWeek(current);

        for (var i = 0; i < $scope.studentInfos.length; i++) {
            $scope.studentInfos[i].startDate = $scope.startDate;
            $scope.studentInfos[i].endDate = $scope.endDate;
        }
    }

    function getStudentInfo(id, name) {
        var sinfo = _.find($scope.studentInfos, function (s) { return s.id === id; });
        if (!sinfo) {
            sinfo = { id: id, studentName: name, startDate: $scope.startDate, endDate: $scope.endDate };
            $scope.studentInfos.push(sinfo);
        }
        return sinfo;
    }

    function loadSummary() {
        attendanceService.getAttendanceForStudents($scope.startDate, $scope.endDate).then(function (summaries) {
            for (var i = 0; i < summaries.length; i++) {
                var sinfo = getStudentInfo(summaries[i].StudentId, summaries[i].Name);
                sinfo.attendanceSummaries = summaries[i].AttendanceSummaries;
            }
        }, handleError);
    }

    function loadAbsenteeism() {
        var beginOfYear = new Date(new Date().getFullYear(), 0, 1);
        var currentDate = new Date();
        attendanceService.getAbsenteeism(beginOfYear, currentDate).then(function (absenteeism) {
            for (var i = 0; i < absenteeism.length; i++) {
                var sinfo = getStudentInfo(absenteeism[i].StudentId, absenteeism[i].Name);
                sinfo.absenteeDates = absenteeism[i].AbsentDates;
            }
        }, handleError);
    }

    function startOfWeek(date) {
        var day = date.getDay(),
            diff = date.getDate() - day + (day == 0 ? -6 : 1); // adjust when day is sunday

        return new Date(date.setDate(diff));
    }

    function endOfWeek(date) {
        var day = date.getDay(),
            diff = date.getDate() + (5 - day) - (day == 0 ? 7 : 0);

        return new Date(date.setDate(diff));
    }

    function handleError(err) {
        toaster.pop('error', 'Error', err);
    }

}]);
