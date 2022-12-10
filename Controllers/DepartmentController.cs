using HospitalManagement.Data;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Controllers
{
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
            //if(deptObj.DepartmentId == 0)
            //{
            //    ModelState.AddModelError("CustomError", "Please select a value for Department");
            //}
            if (ModelState.IsValid)
            {
                _db.Departments.Add(deptObj);
                _db.SaveChanges();
                TempData["Success"] = "Changes saved successfully!";
                return RedirectToAction("Index");
            }
            return View();
        }

        //GET
        //public IActionResult DropDownPage()
        //{
        //    var deptList = _db.Departments.ToList();
        //    //List<Department> deptList = new List<Department>();
        //    //deptList = (from c in _db.Departments select c).ToList();
        //    //deptList.Insert(0, new Department { DepartmentId = 0, DepartmentName = "--Select Department Name--" });

        //    ViewBag.message = deptList;

        //    return View();
        //}
    }
}
