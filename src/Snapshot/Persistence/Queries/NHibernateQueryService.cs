using Core.Domain;
using Core.Persistence;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Linq;

namespace Persistence.Queries
{
    public class NHibernateQueryService<TEntity> : IQueryService<TEntity> where TEntity : DomainEntity
    {
        private readonly ISession _unitOfWork;

        public NHibernateQueryService(ISession unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public TEntity Load(Guid id)
        {
            var entity = _unitOfWork.Get<TEntity>(id);
            return entity;
        }

        public IQueryable<TEntity> Query()
        {
            IQueryable<TEntity> query = _unitOfWork.Query<TEntity>();
            return query;
        }

        public IQueryable<TEntity> QueryWithCacheRefresh()
        {
            IQueryable<TEntity> query = _unitOfWork.Query<TEntity>().CacheMode<TEntity>(CacheMode.Refresh);
            return query;
        }

        public IQueryable<TEntity> Query(IDomainQuery<TEntity> whereQuery)
        {
            var query = Query().Where(whereQuery.Expression);
            return query;
        }

        public IQueryable<TEntity> QueryWithCacheRefresh(IDomainQuery<TEntity> whereQuery)
        {
            var query = QueryWithCacheRefresh().Where(whereQuery.Expression);
            return query;
        }
    }
}
