using HospitalManagement.Data;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Controllers
{
    public class DoctorController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DoctorController(ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize(Roles = "Admin, Staff")]
        public IActionResult Index()
        {
            var doctorList = _db.Doctors.ToList();
            var deptList = _db.Departments.ToList();
            foreach(var doctor in doctorList)
            {
                doctor.Department.DepartmentId = deptList[doctor.DeptRefId-1].DepartmentId;
                doctor.Department.DepartmentName = deptList[doctor.DeptRefId-1].DepartmentName;
            }
            return View(doctorList);
        }

        //GET
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var dept = _db.Departments.ToList();
            ViewBag.message = dept;

            return View();
        }

        //POST
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Doctor doctorObj)
        {
            //var deptList = _db.Departments.Find(doctorObj.Department.DepartmentId);
            //doctorObj.Department.DepartmentName = deptList.DepartmentName;

            if (ModelState.IsValid)
            {
                _db.Doctors.Add(doctorObj);
                _db.SaveChanges();
                TempData["Success"] = "Changes saved successfully!";
                return RedirectToAction("Index");
            }
            var dept = _db.Departments.ToList();
            ViewBag.message = dept;
            return View();
        }

        //GET
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) 
                return NotFound();

            var doctorFromDb = _db.Doctors.Find(id);

            if (doctorFromDb == null) 
                return NotFound();

            var dept = _db.Departments.ToList();
            ViewBag.message = dept;

            return View(doctorFromDb);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Doctor doctorObj)
        {
            if (ModelState.IsValid)
            {
                _db.Doctors.Update(doctorObj);
                _db.SaveChanges();
                TempData["success"] = "Update category success!!";
                return RedirectToAction("Index");
            }
            var dept = _db.Departments.ToList();
            ViewBag.message = dept;
            return View(doctorObj);
        }

        //GET
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) 
                return NotFound();

            var doctorFromDb = _db.Doctors.Find(id);

            if (doctorFromDb == null) 
                return NotFound();

            var dept = _db.Departments.ToList();
            ViewBag.message = dept;
            return View(doctorFromDb);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(Doctor doctorObj)
        {
            /*if (catObj.Name == catObj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("CustomError", "The Display Order cannot same as that of the Name");
            }*/
            if (ModelState.IsValid)
            {
                _db.Doctors.Remove(doctorObj);
                _db.SaveChanges();
                TempData["success"] = "Delete category success!!";
                return RedirectToAction("Index");
            }
            var dept = _db.Departments.ToList();
            ViewBag.message = dept;
            return View(doctorObj);
        }
    }
}
