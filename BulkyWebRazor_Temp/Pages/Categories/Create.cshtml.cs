using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class CreateModel : PageModel
    {
        private readonly ApplicationRazorDbContext _db;
        public Category category { get; set; }
        public CreateModel(ApplicationRazorDbContext db)
        {
            _db = db;
        }
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            _db.Category.Add(category);
            _db.SaveChanges();
            TempData["success"] = "Category created successfully!";
            return RedirectToPage("Index");
        }
    }
}
