using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.DAL.Models;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        //private readonly IDepartmentRepository departmentRepository;
        private readonly IMapper mapper;

        public DepartmentController(IUnitOfWork unitOfWork_, IMapper mapper_)
        {
            unitOfWork = unitOfWork_;
            //departmentRepository = departmentRepository_;

            mapper = mapper_;
        }

        public async Task<IActionResult> Index(string SearchValue)
        {
            IEnumerable<Department> departments;
            
            if (string.IsNullOrEmpty(SearchValue))
                departments = await unitOfWork.departmentRepository.GetAllAsync();
            else
                departments = unitOfWork.departmentRepository.SearchForDepartmentByName(SearchValue);

            var mappedDepts = mapper.Map<IEnumerable<Department>, IEnumerable<DepartmentViewModel>>(departments);
            return View(mappedDepts);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentViewModel departmentVM)
        {
            if(ModelState.IsValid) // server side validation
            {

                var mappedDept = mapper.Map<DepartmentViewModel, Department>(departmentVM);

                await unitOfWork.departmentRepository.AddAsync(mappedDept);
                int count = await unitOfWork.Complete();
                // 3. Temp Date
                if (count > 0)
                    TempData["Message"] = "Department Added Successfully!";

                return RedirectToAction(nameof(Index));
            }
            return View(departmentVM);
        }

        //[HttpGet] // Deafult
        public async Task<IActionResult> Details(int? id, string ViewName = "Details")
        {
            if (id is null)
                return BadRequest();

            var department = await unitOfWork.departmentRepository.GetAsync(id.Value);

            if (department is null) 
                return NotFound();

            var mappedDept = mapper.Map<Department, DepartmentViewModel>(department);

            return View(ViewName ,mappedDept);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            //if (id is null)
            //    return BadRequest();

            //var department = departmentRepository.Get(id.Value);

            //if (department is null)
            //    return NotFound();

            //return View(department);
            return await Details(id, "Edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id ,DepartmentViewModel departmentVM)
        {
            if (ModelState.IsValid) // server side validation
            {
                try
                {
                    if (id != departmentVM.Id)
                        return BadRequest();
                    
                    var mappedDept = mapper.Map<DepartmentViewModel, Department>(departmentVM);

                    unitOfWork.departmentRepository.Update(mappedDept);

                    int count = await unitOfWork.Complete();
                    // 3. Temp Date
                    if (count > 0)
                        TempData["Message"] = "Department Updated Successfully!";

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // 1. Log Exception
                    // 2. Friendly Message

                    // OR
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(departmentVM);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            return await Details(id, "Delete");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete([FromRoute] int id  ,DepartmentViewModel departmentVM)
        {
            try  
            {
                if (id != departmentVM.Id)
                    return BadRequest();
                
                var mappedDept = mapper.Map<DepartmentViewModel, Department>(departmentVM);
                
                unitOfWork.departmentRepository.Delete(mappedDept);
                unitOfWork.Complete();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // 1. Log Exception
                // 2. Friendly Message

                // OR
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(departmentVM);
            }
            
        }

    }
}
