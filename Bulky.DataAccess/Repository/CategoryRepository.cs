using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    // CategoryRepository inherits from Repository<Category> 
    // and also implements ICategoryRepository (specialized for Category).
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        // Constructor of CategoryRepository
        // When you create a new CategoryRepository, you must pass ApplicationDbContext.
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            // Explanation:
            // : base(db) → This calls the constructor of the base class (Repository<Category>)
            // and passes the same ApplicationDbContext instance to it.
            // Without this, the base class wouldn't be initialized correctly.
            _db = db;
        }

        //public void Save()
        //{
        //    _db.SaveChanges();
        //}

        public void Update(Category category)
        {
            _db.Categories.Update(category);
        }
    }
}
