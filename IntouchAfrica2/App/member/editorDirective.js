angular.module('intouch').directive('intouchEditor', ['$modal', function ($modal) {
    function selectMember(propertyInfo) {
        var modalInstance = $modal.open({
            templateUrl: '/App/templates/memberSelectionList.html',
            controller: 'memberSelectionController',
            size: 'lg',
            resolve: {
                type: function () {
                    return 'parent';
                }
            }
        });

        modalInstance.result.then(function (selectedItems) {
            if (selectedItems.length >= 1) {
                var selectedItem = selectedItems[0];
                propertyInfo.value = selectedItem;
            }
        }, function () {
            console.log('Modal dismissed at: ' + new Date());
        });
    }

    return {
        restrict: 'E',
        scope: {
            propertyInfo: '=propertyInfo'
        },
        templateUrl: '/App/member/editor.html',
        link: function (scope, element, attrs) {
            scope.selectMember = selectMember;
        }
    };
}]);
