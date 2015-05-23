angular.module('intouch').controller('captureAttendanceController', ['$scope', 'structureService', 'attendanceService', '$location', 'toaster', function ($scope, structureService, attendanceService, $location, toaster) {
    $scope.groups = [];
    $scope.members = [];
    $scope.canSubmit = true;
    $scope.attendanceCaptured = false;
    $scope.capturedBy = '';
    $scope.captureTime = new Date();

    $scope.date = new Date();

    structureService.getLeaves().then(function (groups) {
        $scope.groups = groups;
    });

    function statusChanged() {
        $scope.canSubmit = !! $scope.selectedGroup && _.every($scope.members, function (member) { return !!member.status });
    }

    $scope.groupSelected = function () {
        getAttendance();
    };

    $scope.$watch('date', function () {
        getAttendance();
    });

    function getAttendance() {
        attendanceService.getAttendance($scope.selectedGroup.Id, $scope.date).then(function (attendance) {
            if (attendance) {
                setAttendace(attendance);
            } else {
                $scope.attendanceCaptured = false;
            }
            structureService.getMembers($scope.selectedGroup.Id, "Student").then(function (members) {
                for (var i = 0; i < members.length; i++) {
                    var member = members[i];
                    
                    if (attendance) {
                        member.status = _.find(attendance.AttendanceRecords, function (record) { return record.MemberId == member.Id; }).Attendance == 1 ? 'present' : 'absent';
                    } else {
                        member.notifyStatusChanged = statusChanged;
                        member.setStatus = function (status) {
                            this.status = status;
                            this.notifyStatusChanged();
                        };
                    }
                }
                $scope.members = members;
            });
        });
    }

    function setAttendace(attendance) {
        $scope.attendanceCaptured = true;
        $scope.capturedBy = attendance.CapturedByMemberName;
        $scope.captureTime = attendance.CaptureTime;
    }

    $scope.submit = function () {
        var attendanceSet = {
            date: $scope.date,
            structuralGroupId: $scope.selectedGroup.Id,
            attendanceRecords: _.map($scope.members, function (member) {
                return {
                    memberId: member.Id,
                    attendance: member.status === 'present' ? 1 : 2
                };
            })
        };

        attendanceService.postAttendanceSet(attendanceSet).then(function (result) {
            toaster.pop('success', 'Saved', 'Saved attendance for ' + $scope.date);
            setAttendace(attendanceSet);
        }, function (error) {
            toaster.pop('error', 'Error', error);
        });
    };

}]);

