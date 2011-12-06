using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Domain;
using Core.Persistence;
using Web.Models.Shared;

namespace Web.Controllers
{
    public class CrudController<TENTITY> : Controller where TENTITY : DomainEntity, new()
    {

        const string INDEX_VIEW = "Index";

        const string EDIT_VIEW = "Edit";


        protected IQueryService<TENTITY> _query;
        protected ISaveOrUpdateCommand<TENTITY> _saveOrUpdate;
        protected IDeleteCommand<TENTITY> _delete;


        protected Action<dynamic> OnIndex = (it) => { };
        protected Action<TENTITY> OnEdit = (it) => { };
        protected Action OnCreate = () => { };
        protected Action OnDelete = () => { };

        public CrudController(
        IQueryService<TENTITY> query,
        ISaveOrUpdateCommand<TENTITY> saveOrUpdate,
        IDeleteCommand<TENTITY> delete

            )
        {
            _query = query;
            _saveOrUpdate = saveOrUpdate;
            _delete = delete;
        }

        public ActionResult Index<T>() where T : ListQuery<TENTITY>, new()
        {

            var model = new T();
            model.Items = _query.Query().ToList();

            OnIndex(model);

            return View(model);
        }

        public ActionResult Create()
        {

            OnCreate();

            return View(EDIT_VIEW, new TENTITY());
        }

        protected ActionResult Edit<TINPUT>(TINPUT input, Action<TENTITY> assignProperties) where TINPUT : InputModel
        {

            var domainEntity = _query.Load(input.Id);
            if (input.Id == Guid.Empty)
                domainEntity = new TENTITY();

            assignProperties(domainEntity);

            _saveOrUpdate.Execute(domainEntity);

            return RedirectToAction(INDEX_VIEW);
        }


        public ActionResult Edit(Guid id)
        {

            var domainEntity = _query.Load(id);

            OnEdit(domainEntity);

            return View(domainEntity);
        }

        [HttpPost]
        public ActionResult Delete(Guid id)
        {

            var domainEntity = _query.Load(id);

            _delete.Execute(domainEntity);

            OnDelete();

            return RedirectToAction(INDEX_VIEW);
        }

    }
}
