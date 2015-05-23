angular.module('intouch').service('confirmationService', function () {
    return {
        confirmAction: function confirmAction(question) {
            var modalInstance = $modal.open({
                templateUrl: '/App/templates/confirmActionTemplate.html',
                controller: 'confirmActionController',
                size: 'sm',
                resolve: {
                    question: function () {
                        return question;
                    }
                }
            });

            return modalInstance.result;
        }
    };
});

angular.module('intouch').controller('confirmActionController', ['$scope', '$modalInstance', 'question', function ($scope, $modalInstance, memberService, question) {
    $scope.yes = function () {
        $modalInstance.close(true);
    };

    $scope.no = function () {
        $modalInstance.close(false);
    };
}]);

