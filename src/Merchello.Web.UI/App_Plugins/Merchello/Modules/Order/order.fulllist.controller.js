(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Order.ListController
     * @function
     * 
     * @description
     * The controller for the orders list page
     */
    controllers.OrderFullListController = function ($scope, $element, angularHelper, assetsService, notificationsService, merchelloReportService) {

        $scope.init = function () {
            $scope.loadInvoices();
        }


        //--------------------------------------------------------------------------------------
        // Declare and initialize key scope properties
        //--------------------------------------------------------------------------------------

        /**
          * @ngdoc method
          * @name loadInvoices
          * @function
          * 
          * @description
          * Load the invoices, either filtered or not, depending on the current page, and status of the filterText variable.
          */
        $scope.loadInvoices = function () {       
            $scope.salesLoaded = false;
            var promiseInvoices = merchelloReportService.getAllInvoices();
            promiseInvoices.then(function (response) {
                var queryResult = new merchello.Models.QueryResult(response);
                $scope.invoices = _.map(queryResult.items, function (invoice) {
                    return new merchello.Models.Invoice(invoice);
                });
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
                $scope.salesLoaded = true;

            }, function (reason) {
                notificationsService.error("Failed To Load Invoices", reason.message);
            });
        };

        
        $scope.downloadOrderCSV = function () {
            $scope.exportTableToCSV.apply(this, [$('#orderListTable'), 'export.csv']);
        }

        $scope.exportTableToCSV = function ($table, filename) {
            var $rows = $table.find('tr:has(td)'),

            // Temporary delimiter characters unlikely to be typed by keyboard
            // This is to avoid accidentally splitting the actual contents
            tmpColDelim = String.fromCharCode(11), // vertical tab character
            tmpRowDelim = String.fromCharCode(0), // null character

            // actual delimiter characters for CSV format
            colDelim = '","',
            rowDelim = '"\r\n"',

            // Grab text from table into CSV formatted string
            csv = '"' + $rows.map(function (i, row) {
                var $row = $(row),
                    $cols = $row.find('td');

                return $cols.map(function (j, col) {
                    var $col = $(col),
                        text = $col.text();

                    return text.replace('"', '""'); // escape double quotes

                }).get().join(tmpColDelim);

            }).get().join(tmpRowDelim)
                .split(tmpRowDelim).join(rowDelim)
                .split(tmpColDelim).join(colDelim) + '"',

            // Data URI
            csvData = 'data:application/csv;charset=utf-8,' + encodeURIComponent(csv);

            $('#orderDownloadCSV')
                .attr({
                    'download': filename,
                    'href': csvData,
                    'target': '_blank'
                });

        }

        $scope.init();

    };

    angular.module("umbraco").controller("Merchello.Dashboards.Order.FullListController", ['$scope', '$element', 'angularHelper', 'assetsService', 'notificationsService', 'merchelloReportService', merchello.Controllers.OrderFullListController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));
