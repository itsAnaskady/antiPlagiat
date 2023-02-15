using Microsoft.EntityFrameworkCore;

namespace AppAntiPlagiat.Models.Repositories
{
    public class SQLUserRepository : IApplicationRepository<Utilisateur>
    {
        private readonly ApplicationDbContext dbContext;

        public SQLUserRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public void Add(Utilisateur entity)
        {
            dbContext.Utilisateurs.Add(entity);
            dbContext.SaveChanges();
        }

        public Utilisateur Delete(Utilisateur entity)
        {
            Utilisateur u = Get(entity.Id);
            if(u != null)
            {
                dbContext.Utilisateurs.Remove(u);
                dbContext.SaveChanges();
            }
            
            return u;
           
        }

        public Utilisateur Get(string id)
        {
            Utilisateur u = dbContext.Utilisateurs.SingleOrDefault(x => x.Id == id);
            return u;
        }

        public Utilisateur Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Utilisateur> GetAll()
        {
            return dbContext.Utilisateurs;
        }

        public Utilisateur Update(Utilisateur entity)
        {
            var u = dbContext.Utilisateurs.Attach(entity);
            u.State = EntityState.Modified;
            dbContext.SaveChanges();
            return entity;
                
        }
    }
}
