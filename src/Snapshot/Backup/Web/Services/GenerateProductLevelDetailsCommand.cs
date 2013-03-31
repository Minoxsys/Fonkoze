using System;
using System.Linq;
using Domain;

namespace Web.Services
{
    public class GenerateProductLevelDetailsCommand
    {
        private readonly StoreProductLevelRequestDetailService storeDetailService;

        private readonly ProcessProductLevelRequestStrategy processStrategy;

        public GenerateProductLevelDetailsCommand(
            ProcessProductLevelRequestStrategy processStrategy,
            StoreProductLevelRequestDetailService storeDetailService)
        {
            this.processStrategy = processStrategy;
            this.storeDetailService = storeDetailService;
        }

        public virtual void Execute(ProductLevelRequest productLevelRequest)
        {
             processStrategy.Process(productLevelRequest, messageInput => storeDetailService.StoreProductLevelRequestDetail(messageInput));
        }
    }
}