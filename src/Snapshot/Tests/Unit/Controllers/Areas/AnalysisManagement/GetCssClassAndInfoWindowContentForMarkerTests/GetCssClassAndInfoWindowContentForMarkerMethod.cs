using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Areas.AnalysisManagement.Controllers;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement
{
    [TestFixture]
    public class GetCssClassAndInfoWindowContentForMarkerMethod
    {
        public void WhenCountryNotNullButThereAreNoRecordsInTheOutpostStockLevel_ReturnString_badStock()
        {
            //LocationReportController controller = new LocationReportController();
            //LocationReportController.CssClassAndInfoWinContent s = controller.GetCssClassAndInfoWindowContentForMarker(null, null, null, null);


        }

        public void WhenCountryNotNullAndAtLeastOneProductStockIsUnderTheLowerLimit_ReturnString_badStock()
        {
            //LocationReportController controller = new LocationReportController();
            //LocationReportController.CssClassAndInfoWinContent s = controller.GetCssClassAndInfoWindowContentForMarker(null, null, null, null);
           
           
        
        }

        public void WhenCountryNotNullAndAtLeastOneProductStockIsUnderOrEqualTo120PercentOfTheLowerLimit_ReturnString_closeToBadStock()
        {
            //LocationReportController controller = new LocationReportController();
            //LocationReportController.CssClassAndInfoWinContent s = controller.GetCssClassAndInfoWindowContentForMarker(null, null, null, null);

            

        }

        public void WhenCountryNotNullAndAllProductStocksAreOver120PercentOfTheLowerLimit_ReturnString_goodStock()
        {
            //LocationReportController controller = new LocationReportController();
            //LocationReportController.CssClassAndInfoWinContent s = controller.GetCssClassAndInfoWindowContentForMarker(null, null, null, null);

        

        }
        public void WhenRegionNotNullAndAtLeastOneProductStockIsUnderTheLowerLimit_ReturnString_badStock()
        {
            //LocationReportController controller = new LocationReportController();
            //LocationReportController.CssClassAndInfoWinContent s = controller.GetCssClassAndInfoWindowContentForMarker(null, null, null, null);

   

        }

        public void WhenRegionNotNullAndAtLeastOneProductStockIsUnderOrEqualTo120PercentOfTheLowerLimit_ReturnString_closeToBadStock()
        {
            //LocationReportController controller = new LocationReportController();
            //LocationReportController.CssClassAndInfoWinContent s = controller.GetCssClassAndInfoWindowContentForMarker(null, null, null, null);

            

        }

        public void WhenRegionNotNullAndAllProductStocksAreOver120PercentOfTheLowerLimit_ReturnString_goodStock()
        {
            //LocationReportController controller = new LocationReportController();
            //LocationReportController.CssClassAndInfoWinContent s = controller.GetCssClassAndInfoWindowContentForMarker(null, null, null, null);

           

        }

    }
}
