using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq.Expressions;

namespace Nat.Core.BaseRepository
{

    public abstract class BaseRepository<TEntity> where TEntity : class
    {
        internal DbContext Venue;
        internal DbSet<TEntity> dbSet;
        //private VenueEntities context = new VenueEntities();

        public BaseRepository(DbContext Venue)
        {
            this.Venue = Venue;
            this.dbSet = Venue.Set<TEntity>();
        }

        //Get All Method --
        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        //GetAllExpressions --
        public virtual IEnumerable<TEntity> GetAllExpressions(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, object>>[] naProperties)
        {
            IQueryable<TEntity> dbQuery = dbSet;

            if (filter != null)
            {
                dbQuery = dbQuery.Where(filter);
            }

            foreach (Expression<Func<TEntity, object>> nProperty in naProperties)
                dbQuery = dbQuery.Include<TEntity, object>(nProperty);

            if (orderBy != null)
            {
                dbQuery = orderBy(dbQuery);
            }

            return dbQuery.ToList();
        }


        //Get By ID--
        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(Convert.ToInt64(id));
        }

        //Insert method--
        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        //Delete method--
        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(Convert.ToInt64(id));
            Delete(entityToDelete);
        }

        //Delete Method --
        public virtual void Delete(TEntity entityToDelete)
        {
            if (Venue.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        //Update Method --
        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            Venue.Entry(entityToUpdate).State = EntityState.Modified;
        }

    }
}
