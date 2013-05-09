namespace Core.Persistence
{
    public interface ISaveOrUpdateCommand<TEntity> where TEntity : Domain.DomainEntity
    {
        void Execute(TEntity entity);
    }
}