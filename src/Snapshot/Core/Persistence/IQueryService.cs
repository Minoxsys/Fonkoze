using Core.Domain;
using System;
using System.Linq;

namespace Core.Persistence
{
    public interface IQueryService<TEntity> where TEntity : DomainEntity
    {
        TEntity Load(Guid id);

        IQueryable<TEntity> Query();

        IQueryable<TEntity> Query(IDomainQuery<TEntity> whereQuery);

        IQueryable<TEntity> QueryWithCacheRefresh();

        IQueryable<TEntity> QueryWithCacheRefresh(IDomainQuery<TEntity> whereQuery);
    }
}
