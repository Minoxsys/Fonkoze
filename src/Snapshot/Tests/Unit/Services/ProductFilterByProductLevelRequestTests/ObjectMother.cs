using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using Web.Helpers;
using Web.Areas.CampaignManagement.Models.ProductLevelRequest;
using Web.Services;

namespace Tests.Unit.Services.ProductFilterByProductLevelRequestTests
{
    public class ObjectMother
    {
        public ProductLevelRequest productLevelRequest;
        public ProductFilterByProductLevelRequest filter;

        public Product selectedProduct;
        public Product secondProduct;
        public Product unselectedProduct;

        public void Setup_ProductLevelRequest_And_InputData()
        {
            productLevelRequest = new ProductLevelRequest();
            productLevelRequest.StoreProducts<ProductModel[]>(
                new ProductModel[] 
                { 
                    new ProductModel { ProductItem = "Selected Product", Selected = true },
                    new ProductModel { ProductItem = "Unselected Product", Selected = false },
                    new ProductModel { ProductItem = "Second Selected Product", Selected = true }
                });
            selectedProduct = new Product { Name = "Selected Product" };
            secondProduct = new Product { Name = "Second Product" };
            unselectedProduct = new Product { Name = "Unselected Product" };

            filter = new ProductFilterByProductLevelRequest(productLevelRequest);
        }
    }
}
