using HospitalManagement.Data;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Controllers
{
    public class PatientController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PatientController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var patientList = _db.Patients.ToList();
            var doctorList = _db.Doctors.ToList();
            foreach(var patient in patientList)
            {
                patient.Doctor.DoctorName = doctorList[patient.DoctorRefId - 1].DoctorName;
            }
            return View(patientList);
        }

        //GET
        public IActionResult Create()
        {
            var doctorList = _db.Doctors.ToList();
            ViewBag.message = doctorList;

            return View();
        }

        //POST
        [HttpPost]
        public IActionResult Create(Patient patientObj)
        {
            if (ModelState.IsValid)
            {
                _db.Patients.Add(patientObj);
                _db.SaveChanges();
                TempData["Success"] = "Changes saved successfully!";
                return RedirectToAction("Index");
            }
            var doctorList = _db.Doctors.ToList();
            ViewBag.message = doctorList;
            return View();
        }

        //GET
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) 
                return NotFound();

            var patientFromDb = _db.Patients.Find(id);

            if (patientFromDb == null) 
                return NotFound();

            var doctorList = _db.Doctors.ToList();
            ViewBag.message = doctorList;

            return View(patientFromDb);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Patient patientObj)
        {
            if (ModelState.IsValid)
            {
                _db.Patients.Update(patientObj);
                _db.SaveChanges();
                TempData["success"] = "Update category success!!";
                return RedirectToAction("Index");
            }
            var doctorList = _db.Doctors.ToList();
            ViewBag.message = doctorList;
            return View(patientObj);
        }

        //GET
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) 
                return NotFound();

            var patientFromDb = _db.Patients.Find(id);

            if (patientFromDb == null) 
                return NotFound();

            var doctorList = _db.Doctors.ToList();
            ViewBag.message = doctorList;
            return View(patientFromDb);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Patient patientObj)
        {
            /*if (catObj.Name == catObj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("CustomError", "The Display Order cannot same as that of the Name");
            }*/
            if (ModelState.IsValid)
            {
                _db.Patients.Remove(patientObj);
                _db.SaveChanges();
                TempData["success"] = "Delete category success!!";
                return RedirectToAction("Index");
            }
            var doctorList = _db.Doctors.ToList();
            ViewBag.message = doctorList;
            return View(patientObj);
        }
    }
}
