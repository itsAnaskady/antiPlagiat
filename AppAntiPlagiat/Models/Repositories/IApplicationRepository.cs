namespace AppAntiPlagiat.Models.Repositories
{
    public interface IApplicationRepository<TEntity>
    {
        TEntity Get(string id);
        TEntity Get(int id);
        void Add(TEntity entity);
        IEnumerable<TEntity> GetAll();

        TEntity Update(TEntity entity);
        TEntity Delete(TEntity entity);
    }
}
