using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class EditModel : PageModel
    {
        private readonly ApplicationRazorDbContext _db;
        public Category category { get; set; }
        public EditModel(ApplicationRazorDbContext db)
        {
            _db = db;
        }
        public void OnGet(int? id)
        {
            if (id != null && id != 0)
                category = _db.Category.FirstOrDefault(category => category.Id == id);
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                _db.Category.Update(category);
                _db.SaveChanges();
                TempData["success"] = "Category updated successfully!";
                return RedirectToPage("Index");
            }
            return Page();

        }
    }
}
