app.controller('APIController', function ($scope, $http) {
    $scope.getStatusById = function (id) {
        $http({
            method: 'GET',
            url: '/Admin/Status/' + id,
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem('token')
            }
        }).then(function (response) {
            $scope.status = response.data;
        }, function (error) {
            console.log(error);
        });
    };
});