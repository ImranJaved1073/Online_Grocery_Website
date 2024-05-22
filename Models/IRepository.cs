namespace Ecommerce.Models
{
    public interface IRepository<TEntity>
    {
        //TEntity FindById(int id);
        public void Add(TEntity entity);
        public void Update(TEntity entity);

        public void Delete(TEntity entity);
        public TEntity Get(int id);

        public List<TEntity> Get();

        public List<TEntity> Search(string search);
    }

}
