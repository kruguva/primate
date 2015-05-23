angular.module('intouch').controller('teacherAttendanceController', ['$scope', 'structureService', 'attendanceService', '$location', 'toaster', function ($scope, structureService, attendanceService, $location, toaster) {
    $scope.attendanceCaptured = false;
    $scope.capturedBy = '';
    $scope.detailSelected = false;
    $scope.structure = [];

    var current = new Date();

    $scope.treehandler = function (branch) {
        $scope.selectedStructuralGroupId = branch.data.Id;
        loadSummary(branch.data.Id);
    }

    $scope.selectDetail = function (attendance) {
        loadAttendance($scope.selectedStructuralGroupId, attendance.Date);
    }

    $scope.nextWeek = function () {
        setDates(7);
        loadSummary($scope.selectedStructuralGroupId);
    }

    $scope.previousWeek = function () {
        setDates(-7);
        loadSummary($scope.selectedStructuralGroupId);
    }

    $scope.isToday = function (date) {
        if (typeof (date) === "string") { date = new Date(date);} 
        var d = date.setHours(0, 0, 0, 0);
        var today = new Date().setHours(0, 0, 0, 0);
        return d === today;
    }

    setDates(0);

    structureService.getStructureAsTree().then(function (structure) {
        $scope.structure = structure;
        $scope.selectedStructuralGroupId = structure[0].data.Id;
        loadSummary(structure[0].data.Id);
    });

    function loadAttendance(structuralGroupId, date) {
        attendanceService.getAttendance(structuralGroupId, date).then(function (attendance) {
            if (attendance) {
                setAttendace(attendance);
            } else {
                $scope.attendanceCaptured = false;
            }
            structureService.getMembers($scope.selectedStructuralGroupId, "Student").then(function (members) {
                for (var i = 0; i < members.length; i++) {
                    var member = members[i];
                    
                    if (attendance) {
                        member.status = _.find(attendance.AttendanceRecords, function (record) { return record.MemberId == member.Id; }).Attendance == 1 ? 'present' : 'absent';
                    } 
                }
                $scope.members = members;
            });
        });
    };

    function setDates(offset) {
        current.setDate(current.getDate() + offset);
        $scope.startDate = startOfWeek(current);
        $scope.endDate = endOfWeek(current);
    }

    function loadSummary(structuralGroupId) {
        attendanceService.getAttendanceSummary(structuralGroupId, $scope.startDate, $scope.endDate).then(function(summary) {
            $scope.attendanceSummaries = summary;
        }, handleError);
        
    }

    function setAttendace(attendance) {
        $scope.attendanceCaptured = true;
        $scope.capturedBy = attendance.CapturedByMemberName;
        $scope.captureTime = attendance.CaptureTime;
        $scope.detailSelected = true;
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