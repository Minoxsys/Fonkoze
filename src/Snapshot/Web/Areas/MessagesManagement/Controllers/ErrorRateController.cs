using System;
using Core.Persistence;
using Domain;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Web.Areas.MessagesManagement.Models.ErrorRate;
using Web.Models.Shared;
using Web.Security;

namespace Web.Areas.MessagesManagement.Controllers
{
    public class ErrorRateController : Controller
    {
        private readonly IQueryService<RawSmsReceived> _rawSmsQueryService;
        private readonly IQueryService<Outpost> _outpostQueryService;

        public ErrorRateController(IQueryService<RawSmsReceived> rawSmsQueryService, IQueryService<Outpost> outpostQueryService)
        {
            _outpostQueryService = outpostQueryService;
            _rawSmsQueryService = rawSmsQueryService;
        }

        [HttpGet]
        [Requires(Permissions = "Messages.View")]
        public ActionResult Overview()
        {
            return View();
        }

        public ActionResult GetErrorRateItems(IndexTableInputModel inputModel, Guid? districtId)
        {
            Debug.Assert(inputModel.limit != null, "inputModel.limit != null");
            var pageSize = inputModel.limit.Value;

            var rawDataQuery = _rawSmsQueryService.Query().ToList().GroupBy(sms => sms.Sender);
            var errorRates = new List<ErrorRateViewModel>();
            foreach (IGrouping<string, RawSmsReceived> rawSmsGroup in rawDataQuery)
            {
                var outpost = _outpostQueryService.Query().FirstOrDefault(o => o.DetailMethod == rawSmsGroup.Key);
                int incorrectMsg = rawSmsGroup.Count(s => !s.ParseSucceeded);
                int totalMsg = rawSmsGroup.Count();

                if (!OutpostIsInDistrict(outpost, districtId))
                    continue;

                errorRates.Add(new ErrorRateViewModel
                    {
                        SellerName = outpost != null ? outpost.Name : "-",
                        ErrorMessages = incorrectMsg,
                        TotalMessages = totalMsg,
                        ErrorRate = (int) ((double) incorrectMsg/totalMsg*100),
                        Sender = rawSmsGroup.Key
                    });
            }

            var errorRateQuery = errorRates.AsQueryable();

            var orderByColumnDirection = new Dictionary<string, Func<IQueryable<ErrorRateViewModel>>>
                {
                    {"Sender-ASC", () => errorRateQuery.OrderBy(c => c.Sender)},
                    {"Sender-DESC", () => errorRateQuery.OrderByDescending(c => c.Sender)},
                    {"SellerName-ASC", () => errorRateQuery.OrderBy(c => c.SellerName)},
                    {"SellerName-DESC", () => errorRateQuery.OrderByDescending(c => c.SellerName)},
                    {"ErrorMessages-ASC", () => errorRateQuery.OrderBy(c => c.ErrorMessages)},
                    {"ErrorMessages-DESC", () => errorRateQuery.OrderByDescending(c => c.ErrorMessages)},
                    {"TotalMessages-ASC", () => errorRateQuery.OrderBy(c => c.TotalMessages)},
                    {"TotalMessages-DESC", () => errorRateQuery.OrderByDescending(c => c.TotalMessages)},
                    {"ErrorRate-ASC", () => errorRateQuery.OrderBy(c => c.ErrorRate)},
                    {"ErrorRate-DESC", () => errorRateQuery.OrderByDescending(c => c.ErrorRate)},
                };

            errorRateQuery = orderByColumnDirection[String.Format("{0}-{1}", inputModel.sort, inputModel.dir)].Invoke();

            if (!string.IsNullOrEmpty(inputModel.searchValue))
                errorRateQuery = errorRateQuery.Where(it => it.SellerName.Contains(inputModel.searchValue));

            var totalItems = errorRateQuery.Count();

            Debug.Assert(inputModel.start != null, "inputModel.start != null");
            errorRateQuery = errorRateQuery
                .Take(pageSize)
                .Skip(inputModel.start.Value);

            return Json(new
                {
                    Items = errorRateQuery.ToArray(),
                    TotalItems = totalItems
                }, JsonRequestBehavior.AllowGet);
        }

        private bool OutpostIsInDistrict(Outpost outpost, Guid? districtId)
        {
            if (!districtId.HasValue)
                return true;

            if (districtId.Value == Guid.Empty)//all districts
                return true;

            if (outpost == null)
                return false;

            return outpost.District.Id == districtId.Value;
        }
    }
}
