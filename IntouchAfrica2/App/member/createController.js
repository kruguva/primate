﻿angular.module('intouch').controller('memberCreateController', ['$scope', '$routeParams', 'memberService', 'toaster', '$window', '$location', function ($scope, $routeParams, memberService, toaster, $window, $location) {
    $scope.type = $routeParams.type;

    memberService.getCreationModel($scope.type).then(function (creationModel) {
        $scope.model = creationModel;
        $scope.properties = _.filter(creationModel.Properties, function(p) { return p.Type != 'Umbraco.NoEdit';});
    }, function (error) {
        toaster.pop('error', 'Error', error);
    })

    $scope.save = function () {
        for (var i = 0; i < $scope.model.Properties.length; i++) {
            if ($scope.model.Properties[i].Type === 'Umbraco.MemberPicker') {
                $scope.model.Properties[i].value = $scope.model.Properties[i].value == null ? 0 : $scope.model.Properties[i].value.Id;
            }
        }

        memberService.save($scope.model)
            .then(function (model) {
                toaster.pop('success', 'Success', 'Successfully created new ' + $scope.type);
                $location.path('/structure');
            },
            function (error) {
                toaster.pop('error', 'Error', error);
            });
    };

    $scope.cancel = function () {
        $window.history.back();
    };

}]);