using Core.Domain;
using Core.Persistence;
using NHibernate;
using System;

namespace Persistence.Commands
{
    public class NHibernateSaveOrUpdateCommand<TEntity> : ISaveOrUpdateCommand<TEntity> where TEntity : DomainEntity
    {
        private readonly ISession _session;

        public NHibernateSaveOrUpdateCommand(ISession session)
        {
            _session = session;
        }

        public void Execute(TEntity entity)
        {
            ITransaction transaction = _session.BeginTransaction();

            entity.Updated = DateTime.UtcNow;
            try
            {
                if (entity.Created == DateTime.MinValue)
                    entity.Created = DateTime.UtcNow;

                _session.SaveOrUpdate(entity);
                transaction.Commit();
            }
            catch (NHibernate.Exceptions.GenericADOException)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                transaction.Dispose();
                transaction = null;
            }
        }
    }
}