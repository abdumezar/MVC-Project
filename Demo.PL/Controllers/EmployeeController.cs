using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.DAL.Models;
using Demo.PL.Helper;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
	[Authorize]
	public class EmployeeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        //private readonly IEmployeeRepository employeeRepository;
        //private readonly IDepartmentRepository departmentRepository;
        private readonly IMapper mapper;

        public EmployeeController(IUnitOfWork unitOfWork_, IMapper mapper_)
        {
            //employeeRepository = employeeRepository_;
            //departmentRepository = departmentRepository_;
            unitOfWork = unitOfWork_;
            mapper = mapper_;
        }

        public async Task<IActionResult> Index(string SearchValue)
        {
            // 1. ViewData [ Dictionary Object - 2 value pair (ASP.NET Framework 3.5)]
            //ViewData["Message"] = "Hello From View Data";

            // 2. ViewBag [ Dynamic Property (ASP.NET Framework 4.0)]
            //ViewBag.Messag = "Hello From View Bag";

            IEnumerable<Employee> employees;

            if (string.IsNullOrEmpty(SearchValue))
                employees = await unitOfWork.employeeRepository.GetAllAsync();
            else
                employees = unitOfWork.employeeRepository.SearchEmployeeByName(SearchValue);

            var mappedEmps = mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeViewModel>>(employees);
            return View(mappedEmps);
        }

        [HttpGet]
        public IActionResult Create()
        {
            //ViewData["Departments"] = departmentRepository.GetAll();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel employeeVM)
        {
            if (ModelState.IsValid) // server side validation
            {
                // Manual Mapping
                //var employee = new Employee()
                //{};

                // Employee employee = (Employee) employeeVM;

                employeeVM.ImageName = await DocumentSettings.UploadFiles(employeeVM.Image, "Images");
                var mappedEmp = mapper.Map<EmployeeViewModel, Employee>(employeeVM);

                await unitOfWork.employeeRepository.AddAsync(mappedEmp);
                int count = await unitOfWork.Complete();
                // 3. Temp Date
                
                if (count > 0)
                    TempData["Message"] = "Employee Added Successfully!";
                
                return RedirectToAction(nameof(Index));
            }
            return View(employeeVM);
        }

        //[HttpGet] // Deafult
        public async Task<IActionResult> Details(int? id, string ViewName = "Details")
        {
            if (id is null)
                return BadRequest();

            var employee = await unitOfWork.employeeRepository.GetAsync(id.Value);

            if (employee is null)
                return NotFound();

            var mappedEmp = mapper.Map<Employee, EmployeeViewModel>(employee);

            return View(ViewName, mappedEmp);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            //if (id is null)
            //    return BadRequest();

            //var employee = employeeRepository.Get(id.Value);

            //if (employee is null)
            //    return NotFound();

            //return View(employee);
            return await Details(id, "Edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, EmployeeViewModel employeeVM)
        {
            if (ModelState.IsValid) // server side validation
            {
                try
                {
                    if (id != employeeVM.Id)
                        return BadRequest();

                    employeeVM.ImageName = await DocumentSettings.UploadFiles(employeeVM.Image, "Images");
                    var mappedEmp = mapper.Map<EmployeeViewModel, Employee>(employeeVM);

                    unitOfWork.employeeRepository.Update(mappedEmp);
                    int count = await unitOfWork.Complete();
                    // 3. Temp Date
                    if (count > 0)
                        TempData["Message"] = "Employee Updated Successfully!";

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
            return View(employeeVM);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            return await Details(id, "Delete");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] int id, EmployeeViewModel employeeVM)
        {
            try
            {
                if (id != employeeVM.Id)
                    return BadRequest();

                var mappedEmp = mapper.Map<EmployeeViewModel, Employee>(employeeVM);
                unitOfWork.employeeRepository.Delete(mappedEmp);
                int count = await unitOfWork.Complete();

                if (count > 0)
                {
                    TempData["Message"] = "Employee Deleted Successfully!";
                    DocumentSettings.DeleteFile(employeeVM.ImageName, "Images");
                }
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // 1. Log Exception
                // 2. Friendly Message

                // OR
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(employeeVM);
            }

        }
    }
}
