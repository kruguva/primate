var app = angular.module('intouch', ['ngRoute', 'ngResource', 'ui.bootstrap', 'angularBootstrapNavTree', 'toaster']);

app.config(['$routeProvider', function ($routeProvider) {
    $routeProvider
        .when('/', { templateUrl: '/App/index.html', controller: 'mainController' })
        .when('/structure', { templateUrl: '/App/structure/structure.html', controller: 'structureController' })
        .when('/attendance/capture', { templateUrl: '/App/attendance/captureAttendance.html', controller: 'captureAttendanceController' })
        .when('/attendance/studentParent', { templateUrl: '/App/attendance/studentParentAttendance.html', controller: 'studentParentAttendanceController' })
        .when('/attendance/teacher', { templateUrl: '/App/attendance/teacherAttendance.html', controller: 'teacherAttendanceController' })
        .when('/member/create/:type', { templateUrl: '/App/member/create.html', controller: 'memberCreateController' })
        .when('/members/:type', { templateUrl: '/App/member/memberList.html', controller: 'memberListController' })
        .when('/member/:id', { templateUrl: '/App/member/view.html', controller: 'memberViewController' })
        .otherwise({ redirectTo: '/' });
}]);

