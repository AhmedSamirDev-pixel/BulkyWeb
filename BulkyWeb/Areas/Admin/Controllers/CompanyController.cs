using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork, ApplicationDbContext db)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return View("Index",objCompanyList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        public IActionResult SaveCreation(Company companyObj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Company.Add(companyObj);
                _unitOfWork.Save();
                TempData["success"] = "Company created successfully!";
                return RedirectToAction("Index");
            }

            return View("Create", companyObj);
        }

        [HttpGet]
        public IActionResult Edit (int? id)
        {
            if (id == 0 || id == null)
                return NotFound();
           Company? companyObj = _unitOfWork.Company.Get(company => company.Id  == id);
            if (companyObj == null)
                return NotFound();
            return View("Edit", companyObj);
        }

        [HttpPost]

        public IActionResult SaveEdit(Company companyObj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Company.Update(companyObj);
                _unitOfWork.Save();
                TempData["success"] = "Company updated successfully!";
                return RedirectToAction("Index");
            }

            return View("Edit", companyObj);
        }

        [HttpGet]
        public IActionResult Delete (int? id)
        {
            if (id == 0 || id == null)
                return NotFound();
            Company companyObj = _unitOfWork.Company.Get(company => company.Id == id);
            if (companyObj == null)
                return NotFound();
            return View("Delete", companyObj);
        }

        [HttpPost]
        public IActionResult SaveDelete(Company company)
        {
            _unitOfWork.Company.Remove(company);
            _unitOfWork.Save();
            TempData["success"] = "Company deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
