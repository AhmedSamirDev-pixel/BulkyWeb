using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;

        // DbSet<T> represents the table in the database for the given entity type (T)
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext db)
        {
            _db = db;

            // _db.Set<T>() dynamically returns the correct DbSet for the entity type T.
            // Example: if T = Category → _db.Categories
            //          if T = Product  → _db.Products
            dbSet = _db.Set<T>();
        }

        // Add a new entity to the corresponding DbSet (table)
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        // Get a single entity that matches a filter condition
        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, 
            bool tracked = false)
        {
            // dbSet is of type DbSet<T>, which implements IQueryable<T>
            IQueryable<T> query = dbSet;
            if (tracked)
                query = dbSet;

            else
                query = dbSet.AsNoTracking();

            // Using IQueryable here means the filter will be translated to SQL
            // and executed in the database (not in memory).
            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.
                    Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            // Calling FirstOrDefault() forces EF Core to run the query against the database
            // and fetch a single matching entity, or null if none exist.
            return query.FirstOrDefault();
        }

        // Get all records from the DbSet (table)
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includeProperties = null) // Optional Parameter
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
                query = query.Where(filter);

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach(var includeProp in includeProperties.
                    Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.ToList();
        }

        // Remove a single entity
        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        // Remove multiple entities in one call
        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }
    }
}
