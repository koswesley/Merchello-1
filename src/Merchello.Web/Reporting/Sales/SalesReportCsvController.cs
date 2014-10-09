using System;
using Merchello.Core.Persistence.Querying;

namespace Merchello.Web.Reporting.Sales
{
    using System.Collections.Generic;

    using Merchello.Core;
    using Merchello.Web.Models.Querying;
    using Merchello.Web.Trees;

    /// <summary>
    /// The sales by item report api controller.
    /// </summary>
    [BackOfficeTree("salesReportCSV", "reports", "Sales Report CSV", "icon-barcode", "merchello/merchello/OrderFullList/manage/", 30)]
    public class SalesReportCsvController : ReportController
    {
        private readonly MerchelloHelper _merchello;
                                                 
        /// <summary>
        /// Initializes a new instance of the <see cref="SalesByItemReportApiController"/> class.
        /// </summary>
        public SalesReportCsvController()
            : this(Core.MerchelloContext.Current)
        {               
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SalesByItemReportApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public SalesReportCsvController(IMerchelloContext merchelloContext)
            : base(merchelloContext)
        {
            _merchello = new MerchelloHelper(merchelloContext.Services);
        }

        /// <summary>
        /// Gets the base url.
        /// </summary>
        public override KeyValuePair<string, object> BaseUrl
        {
            get
            {
                return GetBaseUrl<SalesReportCsvController>("merchelloSalesReportCsv");
            }
        }

        /// <summary>
        /// The get default report data.
        /// </summary>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        public override QueryResultDisplay GetDefaultReportData()
        {
            return _merchello.Query.Invoice.Search(1, 100000);
        }
    }
}