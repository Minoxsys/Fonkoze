using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;
using Web.Helpers;
using Web.Areas.CampaignManagement.Models.ProductLevelRequest;

namespace Web.Services
{
    public class ProductFilterByProductLevelRequest : IProductFilter
    {
        private List<ProductModel> productModels;
        private ProductLevelRequest productLevelRequest;

        public ProductFilterByProductLevelRequest(ProductLevelRequest productLevelRequest)
        {
            this.productLevelRequest = productLevelRequest;
            productModels = productLevelRequest.RestoreProducts<ProductModel[]>().ToList();
        }

        public bool CheckProduct(Product product)
        {
            var foundObject = (ProductModel)productModels.Where(p => p.Selected && p.ProductItem.Equals(product.Name)).FirstOrDefault();

            return foundObject != null;
        }
    }
}