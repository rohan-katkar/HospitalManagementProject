using HospitalManagement.Data;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DepartmentController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Department> deptList = _db.Departments;
            return View(deptList);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        public IActionResult Create(Department deptObj)
        {
            if (ModelState.IsValid)
            {
                _db.Departments.Add(deptObj);
                _db.SaveChanges();
                TempData["Success"] = "Changes saved successfully!";
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
