var app = angular.module("homeApp",[]);

(function (app) {
    "use strict";

    app.controller("homeCtrl", function ($scope, $http, $timeout, CommentService, ArticleService, PagerService) {
        $scope.search = '';
        $scope.messageString = '';
        getRecentComments();
        getTop3PerWeekArticles();
        //getPublishedArticles();
        getCategories();


        var vm = this;

        vm.pager = {};
        vm.setPage = setPage;

        initController();

        function initController() {
            // get all items from api
            $http.get("/Articles/GetPublishedArticles/").then(function (res) {
                vm.allItems = res.data; // real items from api to be paged

                // initialize to page 1
                vm.setPage(1);
            });
        }

        function setPage(page) {
            if (page < 1 || page > vm.pager.totalPages) {
                return;
            }

            // get pager object from service
            vm.pager = PagerService.GetPager(vm.allItems.length, page);

            // get current page of items
            vm.items = vm.allItems.slice(vm.pager.startIndex, vm.pager.endIndex + 1);
        }
        
        function getRecentComments() {
            var getData = CommentService.getRecentComments();
            getData.then(function (c) {
                $scope.rcomm = c.data;
            }, function (c) {
                console.log(c);
            })
        }

        function getTop3PerWeekArticles() {
            var getData = ArticleService.getTop3PerWeekArticles();
            getData.then(function (c) {
                vm.topArticles = c.data;
            }, function (c) {
                console.log(c);
            })
        }

        $scope.getAllArticles = function getPublishedArticles() {
            var getData = ArticleService.getPublishedArticles();
            getData.then(function (c) {
                vm.allItems = c.data;
                $scope.messageString = '';
                vm.setPage(1);
            }, function (c) {
                console.log(c);
            })
        }

        $scope.getHotArticles = function getPublishedHotArticles() {
            var getData = ArticleService.getPublishedHotArticles();
            getData.then(function (c) {
                vm.allItems = c.data;
                $scope.messageString = '';
                vm.setPage(1);
            }, function (c) {
                console.log(c);
            })
        }

        $scope.getAdminChoosedArticles = function getPublishedAdminChoosedArticles() {
            var getData = ArticleService.getPublishedAdminChoosedArticles();
            getData.then(function (c) {
                vm.allItems = c.data;
                $scope.messageString = '';
                vm.setPage(1);
            }, function (c) {
                console.log(c);
            })
        }

        $scope.getPublishedArticlesByCommentsCount = function GetPublishedArticlesByCommentsCount() {
            var getData = ArticleService.getPublishedArticlesByCommentsCount();
            getData.then(function (c) {
                vm.allItems = c.data;
                $scope.messageString = '';
             
                vm.setPage(1);
            }, function (c) {
                console.log(c);
            })
        }

        function getCategories() {
            var getData = ArticleService.getCategories();
            getData.then(function (c) {
                $scope.categories = c.data;
            }, function (c) {
                console.log(c);
            })
        }

        $scope.searchCategory = function getPublishedArticlesByCategory(cat) {

            var getData = ArticleService.getPublishedArticlesByCategory(cat);
            // get all items from api
            getData.then(function (res) {
                vm.allItems = res.data; // real items from api to be paged
                $scope.messageString = 'По вашому запиту ' + cat + ' знайдено {' + vm.allItems.length + '} статей';
               
                // initialize to page 1
                vm.setPage(1);
            }, function (c) {
                console.log(c);
                })

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

app.filter('unsafe', function ($sce) { return $sce.trustAsHtml; });

app.service("CommentService", function ($http) {
    this.getRecentComments = function () {
        return $http.get("/Comments/GetRecentComments/");
    };
});



app.service("PagerService", function ($http) {
    // service definition
    var service = {};

    service.GetPager = GetPager;

    return service;

    // service implementation
    function GetPager(totalItems, currentPage, pageSize) {
        // default to first page
        currentPage = currentPage || 1;

        // default page size is 10
        pageSize = pageSize || 5;

        // calculate total pages
        var totalPages = Math.ceil(totalItems / pageSize);

        var startPage, endPage;
        if (totalPages == 0) {
            vm.allItems = {};
        }
        if (totalPages <= 10) {
            // less than 10 total pages so show all
            startPage = 1;
            endPage = totalPages;
        } else {
            // more than 10 total pages so calculate start and end pages
            if (currentPage <= 6) {
                startPage = 1;
                endPage = 10;
            } else if (currentPage + 4 >= totalPages) {
                startPage = totalPages - 9;
                endPage = totalPages;
            } else {
                startPage = currentPage - 5;
                endPage = currentPage + 4;
            }
        }

        // calculate start and end item indexes
        var startIndex = (currentPage - 1) * pageSize;
        var endIndex = Math.min(startIndex + pageSize - 1, totalItems - 1);

        // create an array of pages to ng-repeat in the pager control
        var pages = _.range(startPage, endPage + 1);

        // return object with all pager properties required by the view
        return {
            totalItems: totalItems,
            currentPage: currentPage,
            pageSize: pageSize,
            totalPages: totalPages,
            startPage: startPage,
            endPage: endPage,
            startIndex: startIndex,
            endIndex: endIndex,
            pages: pages
        };
    }
});

app.service("ArticleService", function ($http) {
    this.getTop3PerWeekArticles = function () {
        return $http.get("/Articles/GetTop3PerWeekArticles/");
    };
    this.getPublishedArticles = function () {
        return $http.get("/Articles/GetPublishedArticles/");
    };
    this.getPublishedArticles = function () {
        return $http.get("/Articles/GetPublishedArticles/");
    }; 
    this.getPublishedHotArticles = function () {
        return $http.get("/Articles/GetPublishedHotArticles/");
    }; 
    
    this.getPublishedArticlesByCategory = function (category) {
        return $http.get("/Articles/GetPublishedArticlesByCategory/?category=" + category);
    };
    this.getCategories = function () {
        return $http.get("/Articles/GetCategories/");
    };

    this.getPublishedAdminChoosedArticles = function () {
        return $http.get("/Articles/GetPublishedAdminChoosedArticles/");
    }

    this.getPublishedArticlesByCommentsCount = function () {
        return $http.get("/Articles/GetPublishedArticlesByCommentsCount/");
    }
});
