using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModel;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.
                GetAll(includeProperties: "Category").ToList();
            return View("Index",  objProductList);  
        }



        #region Create Endpoint Before Combine It With Update
        //public IActionResult Create()
        //{
        //    ProductVM productVM = new ProductVM
        //    {
        //        CategoryList = _unitOfWork.Category
        //        .GetAll()
        //        // For each category in the list, you create a new SelectListItem object.
        //        //Each SelectListItem represents one option in a<select>(dropdown).
        //        .Select(category => new SelectListItem
        //        {
        //            Text = category.Name, // shown to the user
        //            Value = category.Id.ToString() // submitted in the form
        //        }),

        //        Product = new Product()
        //    };
        //    return View("Create", productVM);
        //} 
        #endregion


        // Combine Create Endpoint With Update
        public IActionResult UpSert(int? id) // Update + Insert
        {
            ProductVM productVM = new ProductVM
            {
                CategoryList = _unitOfWork.Category
                .GetAll()
                // For each category in the list, you create a new SelectListItem object.
                //Each SelectListItem represents one option in a<select>(dropdown).
                .Select(category => new SelectListItem
                {
                    Text = category.Name, // shown to the user
                    Value = category.Id.ToString() // submitted in the form
                }),

                Product = new Product()
            };
            if(id == 0 || id == null)
                // Create
                return View("UpSert", productVM);
            else
            {
                // Update
                productVM.Product = _unitOfWork.Product.Get(product => product.Id == id);
                return View("UpSert", productVM);
            }
        }

        [HttpPost]
        public IActionResult SaveUpSert(ProductVM productVM, IFormFile? file)
        {
            
            if(ModelState.IsValid)
            {
                // Get the absolute path of the 'wwwroot' folder where static files
                // (images, css, js) are stored.
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                // Check if a file (image) was uploaded from the form.
                if (file != null)
                {
                    // Generate a unique filename using a GUID to avoid name conflicts.
                    // Keep the same file extension as the uploaded file (e.g., .jpg, .png).
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    // Combine the wwwroot path with the subfolder path "Images/Product"
                    // This gives us the full path on the server to save the uploaded file.
                    string productPath = Path.Combine(wwwRootPath, @"Images\Product");

                    if(!string.IsNullOrEmpty(productVM.Product.ImageURL))
                    {
                        // Delete the old image
                        var oldImagePath = 
                            Path.Combine(wwwRootPath, productVM.Product.ImageURL.Trim('/', '\\'));

                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Create a new file stream at the target path 
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream); // Copy the uploaded file's data into the new file.
                    }

                    // Save the relative path (used by the browser) into the Product's ImageURL property.
                    // Example result: "/Images/Product/123abc.png"
                    productVM.Product.ImageURL = @"/Images/Product/" + fileName;
                }

                if(productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                    TempData["success"] = "Product created successfully!";
                }

                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                    TempData["success"] = "Product updated successfully!";
                }


                _unitOfWork.Save();
                return RedirectToAction("Index");

            }

            else
            {
                {
                    productVM.CategoryList = _unitOfWork.Category
                    .GetAll()
                    // For each category in the list, you create a new SelectListItem object.
                    //Each SelectListItem represents one option in a<select>(dropdown).
                    .Select(category => new SelectListItem
                    {
                        Text = category.Name, // shown to the user
                        Value = category.Id.ToString() // submitted in the form
                    });
                };
                return View("UpSert", productVM);
            }
        }

        #region Edit Endpoint (Edit, SaveEdit) Before Combine It With Create
        //[HttpGet]
        //public IActionResult Edit(int? id)
        //{
        //    if (id == 0 || id == null)
        //        return NotFound();
        //    Product? productFromDb = _unitOfWork.Product.Get(product => product.Id == id);
        //    if (productFromDb == null)
        //        return NotFound();

        //    return View("Edit", productFromDb);
        //}

        //[HttpPost]
        //public IActionResult SaveEdit(Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Product.Update(product);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Category updated successfully!";
        //        return RedirectToAction("Index");
        //    }
        //    return View("Edit", product);
        //}

        #endregion


        #region Delete Endpoint Before Using API.
        //[HttpGet]
        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //        return NotFound();
        //    Product? productFromDb = _unitOfWork.Product.Get(product => product.Id == id);
        //    if (productFromDb == null)
        //        return NotFound();
        //    return View("Delete", productFromDb);
        //}

        //[HttpPost]
        //public IActionResult SaveDelete(Product product)
        //{ 
        //     _unitOfWork.Product.Remove(product);
        //     _unitOfWork.Save();
        //     TempData["success"] = "Product deleted successfully!";
        //     return RedirectToAction("Index");
        //} 
        #endregion


        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.
                GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        } 

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(product => product.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new {success = false, message = "Error While Deleting"});
            }
            
                // Delete the old image
                var oldImagePath =
                    Path.Combine(_webHostEnvironment.WebRootPath,
                    productToBeDeleted.ImageURL.Trim('/', '\\'));

                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new {success = true, message = "Delete Success"});

            
        } 
        #endregion

    }
}
