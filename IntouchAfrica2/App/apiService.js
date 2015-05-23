angular.module('intouch').service('apiService', ['$http', '$q', function ($http, $q) {
    return {
        doGet: doGet,
        doPost: doPost,
        doPut: doPut,
        doDelete: doDelete
    };

    function doGet(url, params) {
        var request = $http({
            method: "get",
            url: url,
            params: params
        });

        return doRequestAsync(request);
    }

    function doPost(url, data) {
        return doRequestAsync($http.post(url, data));
    }

    function doPut(url, data) {
        return doRequestAsync($http.put(url, data));
    }

    function doDelete(url, params) {
        return doRequestAsync($http.delete(url, {
            params: params
        }));
    }

    function doRequestAsync(request) {
        var deferred = $q.defer();
        request.then(function (response) {
            deferred.resolve(response.data == "null" ? null : response.data);
        }, function (error) {
            deferred.reject(error);
        });
        return deferred.promise;
    }

}]);