using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;
using NHibernate;
using Core.Persistence;

namespace Persistence.Commands
{
	public class NHibernateSaveOrUpdateCommand<ENTITY> : ISaveOrUpdateCommand<ENTITY> where ENTITY : DomainEntity
	{
		private ISession session;
		public NHibernateSaveOrUpdateCommand(
			ISession session)
		{
			this.session = session;
		}

		public void Execute(ENTITY entity)
		{
			ITransaction transaction = session.BeginTransaction();

			entity.Updated = DateTime.UtcNow;
			try
			{
				if (entity.Created == DateTime.MinValue)
					entity.Created = DateTime.UtcNow;

				session.SaveOrUpdate(entity);
				transaction.Commit();
			}
			catch (NHibernate.Exceptions.GenericADOException ex)
			{
				transaction.Rollback();
                throw ex;
			}
			finally
			{
				transaction.Dispose();
				transaction = null;
			}
		}
	}
}