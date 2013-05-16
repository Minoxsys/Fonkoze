using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;
using Domain;
using NUnit.Framework;
using Rhino.Mocks;
using Web.Areas.AnalysisManagement.Controllers;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.GetCssClassAndInfoWindowContentForMarkerTests
{
    [TestFixture]
    public class GetCssClassAndInfoWindowContentForMarkerMethod
    {
        ObjectMother objectMother = new ObjectMother();

        [SetUp]
        public void BeforeAll()
        {
            objectMother.Init();
        }

        [Test] //1
        public void WhenOutpostIsNull_AndThereAreNoRecordsInTheOutpostStockLevel_ReturnString_badStock()
        {
            //Arrange
            objectMother.queryStockLevel.Expect(call => call.Query()).Return(objectMother.oslNoStockLevelRecordsList.AsQueryable());

            //Act
            LocationReportController.CssClassAndInfoWinContent s = objectMother.controller.GetCssClassAndInfoWindowContentForMarker(objectMother.country, null, null, null,objectMother.client);

            //Assert
            objectMother.queryStockLevel.VerifyAllExpectations();
            Assert.AreEqual("badStock", s.CssClass);


        }

        [Test]//2
        public void WhenOutpostIsNull_AndAtLeastOneProductStockIsUnderTheLowerLimit_ReturnString_badStock()
        {
            //Arrange
            objectMother.queryStockLevel.Expect(call => call.Query()).Return(objectMother.oslRedList.AsQueryable());

            //Act
            LocationReportController.CssClassAndInfoWinContent s = objectMother.controller.GetCssClassAndInfoWindowContentForMarker(objectMother.country, null, null, null,objectMother.client);

            //Assert
            objectMother.queryStockLevel.VerifyAllExpectations();
            Assert.AreEqual("badStock", s.CssClass);

        }

        [Test]//3
        public void WhenOutpostIsNull_AndAtLeastOneProductStockIsUnderOrEqualTo120PercentOfTheLowerLimit_ReturnString_closeToBadStock()
        {
            //Arrange
            objectMother.queryStockLevel.Expect(call => call.Query()).Return(objectMother.oslAmberList.AsQueryable());

            //Act
            LocationReportController.CssClassAndInfoWinContent s = objectMother.controller.GetCssClassAndInfoWindowContentForMarker(objectMother.country, null, null, null, objectMother.client);

            //Assert
            objectMother.queryStockLevel.VerifyAllExpectations();
            Assert.AreEqual("closeToBadStock", s.CssClass);

        }

        [Test]//4
        public void WhenOutpostIsNull_AndAllProductStocksAreOver120PercentOfTheLowerLimit_ReturnString_goodStock()
        {
            //Arrange
            objectMother.queryStockLevel.Expect(call => call.Query()).Return(objectMother.oslGreenList.AsQueryable());

            //Act
            LocationReportController.CssClassAndInfoWinContent s = objectMother.controller.GetCssClassAndInfoWindowContentForMarker(objectMother.country, null, null, null, objectMother.client);

            //Assert
            objectMother.queryStockLevel.VerifyAllExpectations();
            Assert.AreEqual("goodStock", s.CssClass);


        

        }

        [Test] //1 No stock levels in the table
        public void WhenOutpostIsNOTNull_AndThereAreNoRecordsInTheOutpostStockLevel_ReturnString_badStock()
        {
            //Arrange
            objectMother.queryStockLevel.Expect(call => call.Query()).Return(objectMother.oslNoStockLevelRecordsList.AsQueryable());

            //Act
            LocationReportController.CssClassAndInfoWinContent s = objectMother.controller.GetCssClassAndInfoWindowContentForMarker(objectMother.country, null, null, objectMother.outpost, objectMother.client);

            //Assert
            objectMother.queryStockLevel.VerifyAllExpectations();
            Assert.AreEqual("badStock", s.CssClass);
            Assert.AreEqual("Seller has no products assigned.", s.InfoWinContent);


        }

        [Test] //2 Red
        public void WhenOutpostIsNOTNull_AndAtLeastOneProductStockIsUnderTheLowerLimit_Return_badStockAndInfoWinContent()
        {
            //Arrange
            objectMother.queryStockLevel.Expect(call => call.Query()).Return(objectMother.oslNoStockLevelRecordsList.AsQueryable());

            //Act
            LocationReportController.CssClassAndInfoWinContent s = objectMother.controller.GetCssClassAndInfoWindowContentForMarker(objectMother.country, null, null, objectMother.outpost, objectMother.client);

            //Assert
            objectMother.queryStockLevel.VerifyAllExpectations();
            Assert.AreEqual("badStock", s.CssClass);
            Assert.That(s.InfoWinContent, Is.Not.Empty);
            

   

        }

        [Test] //3 Amber
        public void WhenOutpostIsNOTNull_AndAtLeastOneProductStockIsUnderOrEqualTo120PercentOfTheLowerLimit_Return_closeToBadStockAndInfoWinContent()
        {
            //Arrange
            objectMother.queryStockLevel.Expect(call => call.Query()).Return(objectMother.oslNoStockLevelRecordsList.AsQueryable());

            //Act
            LocationReportController.CssClassAndInfoWinContent s = objectMother.controller.GetCssClassAndInfoWindowContentForMarker(objectMother.country, null, null, objectMother.outpost, objectMother.client);

            //Assert
            objectMother.queryStockLevel.VerifyAllExpectations();
            Assert.AreEqual("badStock", s.CssClass);
            Assert.That(s.InfoWinContent, Is.Not.Empty);

            

        }

        [Test] //4 Green
        public void WhenOutpostIsNOTNull_AndAllProductStocksAreOver120PercentOfTheLowerLimit_Return_goodStockAndInfoWinContent()
        {
            //Arrange
            objectMother.queryStockLevel.Expect(call => call.Query()).Return(objectMother.oslGreenList.AsQueryable());

            //Act
            LocationReportController.CssClassAndInfoWinContent s = objectMother.controller.GetCssClassAndInfoWindowContentForMarker(objectMother.country, null, null, objectMother.outpost, objectMother.client);

            //Assert
            objectMother.queryStockLevel.VerifyAllExpectations();
            Assert.AreEqual("goodStock", s.CssClass);
            Assert.AreEqual("All Stock Levels Are Good.<br/><br/>Last SMS: 1-Jan-2013",s.InfoWinContent);
            

           

        }

    }
}
