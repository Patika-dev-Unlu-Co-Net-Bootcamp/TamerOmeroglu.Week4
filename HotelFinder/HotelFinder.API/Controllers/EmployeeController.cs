﻿using HotelFinder.API.Exceptions;
using HotelFinder.DataAccess;
using HotelFinder.Entity;
using HotelFinder.Entity.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HotelFinder.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly HotelFinderDbContext _context;

        public EmployeeController(HotelFinderDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAllEmployee()
        {
            var employees = _context.Employees.Select(x => ItemToDTO(x)).ToList();

            if (employees.Count == 0)
            {
                throw new NotFoundException("No registered employee");
            }
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public IActionResult GetEmployee(int id)
        {
            var employee = _context.Employees.FirstOrDefault(x => x.Id == id);

            if (employee == null)
            {
                throw new NotFoundException("Employee not found");
            }
            return Ok(ItemToDTO(employee));
        }


        [HttpPost("create")]
        public IActionResult CreateEmployee([FromBody] EmployeeCreateDto employeeDto)
        {

            var hotelExits = HotelIdExists(employeeDto.HotelId);

            if (!hotelExits)
            {
                throw new BadRequestException("There is no hotel with the id in your request");
            }

            var employee = new Employee()
            {
                Name = employeeDto.Name,
                Surname = employeeDto.Surname,
                HotelId = employeeDto.HotelId
            };

            _context.Employees.Add(employee);
            _context.SaveChanges();


            return CreatedAtAction(
                    nameof(GetEmployee),
                    new { id = employee.Id },
                    ItemToDTO(employee));
        }


        [HttpPut("{id}")]
        public IActionResult UpdateEmployee(int id, [FromBody] EmployeeCreateDto employeeDto)
        {
            var employee = _context.Employees.FirstOrDefault(x => x.Id == id);

            if (employee == null)
            {
                throw new NotFoundException("Employee Not Found");
            }

            employee.Name = employeeDto.Name;
            employee.Surname = employeeDto.Surname;
            employee.HotelId = employeeDto.HotelId;

            _context.SaveChanges();

            return Ok();
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            var employee = _context.Employees.FirstOrDefault(x => x.Id == id);

            if (employee == null)
            {
                throw new NotFoundException("Employee Not Found");
            }

            _context.Remove(employee);

            return Ok();
        }

        private bool HotelIdExists(int id) => _context.Hotels.Any(e => e.Id == id);

        private static EmployeeDto ItemToDTO(Employee todoItem) =>
        new EmployeeDto
        {
            Id = todoItem.Id,
            Name = todoItem.Name,
            Surname = todoItem.Surname,
            HotelId = todoItem.HotelId
        };

    }
}
