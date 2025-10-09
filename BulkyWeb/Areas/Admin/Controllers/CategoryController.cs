using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        // Field to hold the database context (readonly = can only be set in constructor)
        private readonly IUnitOfWork _unitOfWork;

        // Constructor: ASP.NET Core will automatically provide (inject) 
        // an instance of ApplicationDbContext here using Dependency Injection (DI).
        // This saves us from creating a new object manually every time using 'new'.
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork; // store the injected context in the private field for later use
        }

        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            return View("Index", objCategoryList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        public IActionResult SaveCreation(Category category)
        {
            //if (category.Name == category.DisplayOrder.ToString())
            //    ModelState.AddModelError("Name", "The DisplayOrder cannot exactly match the name.");

            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(category);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully!";
                return RedirectToAction("Index");
            }
            return View("Create", category);

        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            //Category? categoryFromDb1 = _db.Categories.Where(category => category.Id == id).FirstOrDefault();
            //Category? categoryFromDb2 = _db.Categories.Find(id);
            Category? categoryFromDb = _unitOfWork.Category.Get(category => category.Id == id);
            if (categoryFromDb == null)
                return NotFound();
            return View("Edit", categoryFromDb);
        }

        [HttpPost]
        public IActionResult SaveEdit(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(category);
                _unitOfWork.Save();
                TempData["success"] = "Category updated successfully!";
                return RedirectToAction("Index");
            }
            return View("Edit", category);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            Category? categoryFromDb = _unitOfWork.Category.Get(category => category.Id == id);
            if (categoryFromDb == null)
                return NotFound();
            return View("Delete", categoryFromDb);
        }

        [HttpPost]
        public IActionResult SaveDelete(Category category)
        {
            _unitOfWork.Category.Remove(category);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully!";
            return RedirectToAction("Index");
        }

        


    }
}
