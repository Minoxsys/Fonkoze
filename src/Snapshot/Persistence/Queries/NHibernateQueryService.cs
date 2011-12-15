using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;
using NHibernate.Linq;
using NHibernate;
using Core.Persistence;

namespace Persistence.Queries
{
	public class NHibernateQueryService<ENTITY> : IQueryService<ENTITY> where ENTITY: DomainEntity
	{
		INHibernateUnitOfWork unitOfWork;

		public NHibernateQueryService(INHibernateUnitOfWork unitOfWork)
		{

			this.unitOfWork = unitOfWork;

		}
		public ENTITY Load(Guid id)
		{
			var entity = unitOfWork.CurrentSession.Get<ENTITY>(id);

			return entity;
		}

		public IQueryable<ENTITY> Query()
		{
			IQueryable<ENTITY> query = unitOfWork.CurrentSession.Query<ENTITY>();
			
			return query;
		}

		public IQueryable<ENTITY> Query( IDomainQuery<ENTITY> whereQuery )
		{			
			var query = Query().Where(whereQuery.Expression);

			return query;
		}
	}
}
