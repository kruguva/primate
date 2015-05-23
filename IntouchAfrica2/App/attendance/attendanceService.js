angular.module('intouch').service('attendanceService', ['$http', '$q', function ($http, $q) {
    return {
        postAttendanceSet: postAttendanceSet,
        getAttendance: getAttendance,
        getAttendanceSummary: getAttendanceSummary,
        getAttendanceForStudents: getAttendanceForStudents,
        getAbsenteeism: getAbsenteeism
    };

    function postAttendanceSet(attendanceSet) {
        var deferred = $q.defer();

        var request = $http.post("/umbraco/api/attendance/postAttendanceSet",
            attendanceSet
        ).then(function(response) {
            deferred.resolve(response.data == "null" ? null : response.data)
        }, function(error) {
            deferred.reject(error);
        });

        return deferred.promise;
    }

    function getAttendance(structuralGroupId, date) {
        return doGet("/umbraco/api/attendance/getAttendanceSet", {
                structuralGroupId: structuralGroupId,
                date: date
            })
    }

    function getAttendanceSummary(structuralGroupId, startDate, endDate) {
        return doGet('/umbraco/api/attendance/getAttendanceSummary', {
            start: startDate,
            end: endDate,
            structuralGroupId: structuralGroupId
        });
    }

    function getAttendanceForStudents(startDate, endDate) {
        return doGet('/umbraco/api/attendance/getAttendanceForStudents', {
            start: startDate,
            end: endDate
        });
    }

    function getAbsenteeism(startDate, endDate) {
        return doGet('/umbraco/api/attendance/getAbsenteeism', {
            start: startDate,
            end: endDate
        });
    }

    function getMemberAttendance(date) {
        return doGet("/umbraco/api/attendance/getAttendanceForMember", {date: date});
    }

    function doGet(url, params) {
        var deferred = $q.defer();

        var request = $http({
            method: "get",
            url: url,
            params: params
        }).then(function (response) {
            deferred.resolve(response.data == "null" ? null : response.data);
        }, function (error) {
            deferred.reject(error)
        });

        return deferred.promise;
    }
}]);