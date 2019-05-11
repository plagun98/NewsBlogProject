var app = angular.module('manageApp', []);
(function (app) {
    'use strict';
app.controller('manageCtrl', function ($scope, $timeout, $sce, $http, PagerService) {
            var vm = this;
            vm.pager = {};
            vm.setPage = setPage;
            vm.search = '';

            initController();

            function initController() {

                $http.get("/Articles/GetUnpublishedArticles/")
                    .then(function (res) {
                        vm.articles = res.data;
                    });

                $http.get("/Articles/GetArticlesByUser/")
                    .then(function (res) {
                        vm.allItems = res.data;
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

    });
})(app);

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
