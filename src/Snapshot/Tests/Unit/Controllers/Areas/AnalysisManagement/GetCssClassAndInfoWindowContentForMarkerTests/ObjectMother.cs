using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;
using Core.Persistence;
using Domain;
using Web.Areas.AnalysisManagement.Controllers;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement.GetCssClassAndInfoWindowContentForMarkerTests
{
    public class ObjectMother
    {
        public LocationReportController controller;
              
        public IQueryService<OutpostStockLevel> queryStockLevel;

        public IQueryService<Client> queryClient;
        public IQueryService<User> queryUsers;

        public Client client;
        public Guid clientId;
        public User user;
        public Guid userId;

        public Guid countryId;
        public Country country;
        public Guid regionId;
        public Region region;
        public Guid districtId;
        public District district;
        public Guid outpostId;
        public Outpost outpost;
        public Guid stockLevelId;
        public OutpostStockLevel stockLevel;
        public Guid productId;
        public Product product;

        private const string CLIENT_NAME = "Ion";
        private const string USER_NAME = "IonPopescu";

    }
}
