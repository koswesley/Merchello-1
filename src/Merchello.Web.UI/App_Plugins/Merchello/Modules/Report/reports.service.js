(function (merchelloServices, undefined) {


    /**
        * @ngdoc service
        * @name merchello.Services.MerchelloOrderService
        * @description Loads in data and allows modification for orders
        **/
    merchelloServices.MerchelloReportService = function ($http, umbRequestHelper) {

        return {

            getAllInvoices: function (orderKey) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloSalesReportCsv', 'GetDefaultReportData'),
                        method: "GET"
                    }),
                    'Failed to get invoices');
            }
        };
    };

    angular.module('umbraco.resources').factory('merchelloReportService', ['$http', 'umbRequestHelper', merchello.Services.MerchelloReportService]);

}(window.merchello.Services = window.merchello.Services || {}));
