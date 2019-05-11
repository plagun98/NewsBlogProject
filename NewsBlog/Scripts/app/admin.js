var app = angular.module('adminApp',[]);
(function (app) {
    'use strict';

    app.controller('adminCtrl', function ($scope, $timeout, $sce, $http, AdminService) {
        $scope.search = '';
        $scope.term = '';
        $scope.filteredItems = 0;
       
        getArticles();
        getCategories();
        setSelectPosition();

        


        function getArticles() {
            var getData = AdminService.getArticles();
            getData.then(function (b) {
                
                $scope.articles = b.data;
                //$scope.filteredItems = $scope.articles.length;
                $scope.totalItems = $scope.articles.length;
                $scope.PerPageItems = 6;
                $scope.currentPage = 1;
            }, function (b) {
                console.log(b);
            });
        }

        $scope.update = function getArticlesByFilter(filter) {
            this.filter = filter;
            var getData = AdminService.getArticlesByFilter($scope.filter);
            getData.then(function (b) {
                $scope.articles = b.data;
                $scope.filteredItems = $scope.articles.length;
            }, function (b) {
                console.log(b);
            });
        }

        function setSelectPosition() {
            window.document.onload = function () {
                alert('+');
                document.getElementById("select").selectedIndex = -1;
            }
        }

        function getCategories() {
            var getData = AdminService.getCategories();
            getData.then(function (b) {
                $scope.categories = b.data;
            }, function (b) {
                console.log(b);
            });
        }

        $scope.getArticle = function getArticle(articleId) {
            this.articleId = articleId;
            var getData = AdminService.getArticle($scope.articleId);
            getData.then(function (b) {
                $scope.article = b.data;
            }, function (b) {
                console.log(b);
            });
        }

        $scope.removeArticle = function (index) {
            AdminService.deleteArticle(index);
            getArticles();
            location.reload();
        };

        $scope.removeCategory = function (index) {
            AdminService.deleteCategory(index);
            getCategories();
            location.reload();
        };


        $scope.toBase64 = function _arrayBufferToBase64(buffer) {
            var binary = '';
            var bytes = new Uint8Array(buffer);
            var len = bytes.byteLength;
            for (var i = 0; i < len; i++) {
                binary += String.fromCharCode(bytes[i]);
            }
            return window.btoa(binary);
        }

        
        $scope.numberOfPages = function () {
            return Math.ceil($scope.totalItems / $scope.PerPageItems);
        } 
    });
})(app);

app.filter('Pagestart', function () {
    return function (input, start) {
        if (!input || !input.length) { return; }
        start = +start; //parse to int
        return input.slice(start);
    }
}); 

app.directive('ngConfirmClick', [
    function () {
        return {
            link: function (scope, element, attr) {
                var msg = attr.ngConfirmClick || "Are you sure?";
                var clickAction = attr.confirmedClick;
                element.bind('click', function (event) {
                    if (window.confirm(msg)) {
                        scope.$eval(clickAction)
                    }
                });
            }
        };
    }])


app.service('AdminService', function ($http) {
    this.getArticles = function () {
        return $http.get('/Admin/GetArticles/');
    };
    this.getArticle = function (id) {
        return $http.get('/Admin/GetArticle/?'+id);
    };
    this.deleteArticle = function (index) {
        return $http.post('Articles/Delete/' + index);
    };
    this.deleteCategory = function (index) {
        return $http.post('Categories/Delete/' + index);
    };

    this.getCategories = function () {
        return $http.get('/Admin/GetCategories/');
    }
    this.getArticlesByFilter = function (filter) {
        return $http.get('/Admin/GetArticlesByFilter/?filter=' + filter);
    }
});