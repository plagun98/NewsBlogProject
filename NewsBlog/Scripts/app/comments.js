var app = angular.module('comment', ['angularTrix']);

(function (app) {
    "use strict";

    app.controller('commCtrl', function ($scope, $timeout, $sce, $http, CommentService, ArticleService) {
        $scope.value = 0;
        $scope.category = '';

        getUserInfo(16);
        getTop3PerWeekArticles();
        
        $scope.getCommentWall = function getCommentWall(articleId) {
            this.articleId = articleId;
            var getData = CommentService.getCommentWall($scope.articleId);
            getData.then(function (b) {
                $scope.comments = b.data;
                $scope.value = b.data.length;
            }, function (b) {
                console.log(b);
            });
        }

        function ReloadComments() {
            var getData = CommentService.getCommentWall($scope.articleId);
            getData.then(function (b) {
                $scope.comments = b.data;
                $scope.value = b.data.length;
            }, function (b) {
                console.log(b);
            });
        }

        $scope.getPopularArticlesByCategory = function getPopularArticlesByCategory(category) {
            this.category = category;
            var getData = ArticleService.getPopularArticlesByCategory($scope.category);
            getData.then(function (c) {
                $scope.popArticles = c.data;
            }, function (c) {
                console.log(c);
            })
        }

        function getTop3PerWeekArticles() {
            var getData = ArticleService.getTop3PerWeekArticles();
            getData.then(function (c) {
                $scope.topArticles = c.data;
            }, function (c) {
                console.log(c);
            })
        }

        function getUserInfo (id) {
            var getData = ArticleService.getUserInfo(id);
            getData.then(function (c) {
                $scope.userInfo = c.data;
            }, function (c) {
                console.log(c);
            })
        }

        $scope.toBase64 = function _arrayBufferToBase64(buffer) {
            var binary = '';
            var bytes = new Uint8Array(buffer);
            var len = bytes.byteLength;
            for (var i = 0; i < len; i++) {
                binary += String.fromCharCode(bytes[i]);
            }
            return window.btoa(binary);
        }


        $scope.addComment = function () {
            $http.post('/Comments/AddComment', $scope.model).then(
                function (successResponse) {
                    ReloadComments();
                    $scope.model = {};
                    $scope.Comment.$setPristine();
                    $scope.Comment.$setUntouched();
                    console.log('success');
                },
                function (errorResponse) {
                    console.log(errorResponse);
                });
        }

    });

})(app);

app.filter('unsafe', function ($sce) { return $sce.trustAsHtml; });

app.service('CommentService', function ($http) {
    this.getCommentWall = function(id){
        return $http.get('/Comments/CommentWall/?id='+id);
    };
});
app.service('ArticleService', function ($http) {
    this.getPopularArticlesByCategory = function (category) {
        return $http.get("/Articles/GetPopularArticlesByCategory/?category=" + category);
    };
    this.getTop3PerWeekArticles = function () {
        return $http.get("/Articles/GetTop3PerWeekArticles/");
    };
    this.getUserInfo = function (id) {
        return $http.get("/Articles/GetUserInfoByArticle/?id=" + id);
    };
})
angular.bootstrap(document.getElementById("comment"), ['comment']);