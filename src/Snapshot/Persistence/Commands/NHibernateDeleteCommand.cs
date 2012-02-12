using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;
using NHibernate.Exceptions;
using NHibernate;
using Core.Persistence;

namespace Persistence.Commands
{
	public class NHibernateDeleteCommand<ENTITY> : IDeleteCommand<ENTITY> where ENTITY : DomainEntity
	{
		private ISession session;

		public NHibernateDeleteCommand(ISession session)
		{
			this.session = session;
		}

		public void Execute(ENTITY entity)
		{
			ITransaction transaction = session.BeginTransaction();

			try
			{
				this.session.Delete(entity);
				transaction.Commit();
			}
			catch (GenericADOException ex)
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
