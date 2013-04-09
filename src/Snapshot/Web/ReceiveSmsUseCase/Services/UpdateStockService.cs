using System;
using Web.ReceiveSmsUseCase.Models;

namespace Web.ReceiveSmsUseCase.Services
{
    public class UpdateStockService : IUpdateStockService
    {
        public void UpdateProductStocks(ISmsParseResult parseResult)
        {
            if (!parseResult.Success)
            {
                throw new ArgumentException("Can't update stock with incorrect parse results!");
            }
        }

              //public void UpdateOutpostStockLevelsWithValuesReceivedBySms(SmsReceived smsReceived)
        //{
        //    var stockLevels = GetOutpostStockLevelsByProductGroupReferenceAndPhoneNumber(smsReceived.ProductGroupReferenceCode, smsReceived.Number);

        //    foreach (OutpostStockLevel outpostStockLevel in stockLevels)
        //    {
        //        UpdateSingleOutpostStockLevelWithValuesReceivedBySms(outpostStockLevel, smsReceived);
        //    }
        //}

        //private List<OutpostStockLevel> GetOutpostStockLevelsByProductGroupReferenceAndPhoneNumber(string productGroupReferenceCode, string phoneNumber)
        //{
        //    List<OutpostStockLevel> stockLevels = new List<OutpostStockLevel>();
        //    SmsRequest smsRequest = queryServiceSmsRequest.Query().Where(r => r.ProductGroupReferenceCode == productGroupReferenceCode && r.Number.Contains(phoneNumber)).OrderByDescending(r => r.Created).FirstOrDefault();

        //    if (smsRequest != null)
        //    {
        //        stockLevels.AddRange(GetOutpostStockLevelsByOutpostAndProductGroup(smsRequest.OutpostId, smsRequest.ProductGroupId));
        //    }

        //    return stockLevels;
        //}

        //private List<OutpostStockLevel> GetOutpostStockLevelsByOutpostAndProductGroup(Guid outpostId, Guid productGroupId)
        //{
        //    return queryServiceStockLevel.Query().Where(s => s.Outpost.Id == outpostId && s.ProductGroup.Id == productGroupId).ToList();
        //}

        //private void UpdateSingleOutpostStockLevelWithValuesReceivedBySms(OutpostStockLevel outpostStockLevel, SmsReceived smsReceived)
        //{
        //    outpostStockLevelService.SaveHistoricalOutpostStockLevelToPreviousOutpostStockLevelOfCurrent(outpostStockLevel);

        //    foreach (ReceivedStockLevel receivedStockLevel in smsReceived.ReceivedStockLevels)
        //    {
        //        if (outpostStockLevel.Product.SMSReferenceCode.Equals(receivedStockLevel.ProductSmsReference))
        //        {
        //            outpostStockLevel.PrevStockLevel = outpostStockLevel.StockLevel;
        //            outpostStockLevel.StockLevel = receivedStockLevel.StockLevel;
        //            outpostStockLevel.UpdateMethod = SMS_UPDATED_METHOD;
        //            saveCommandOutpostStockLevel.Execute(outpostStockLevel);
        //        }
        //    }
        //}

       // #endregion
    }
}