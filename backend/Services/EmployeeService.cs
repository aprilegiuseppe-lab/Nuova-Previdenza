using Microsoft.EntityFrameworkCore;
using NuovaPrevidenza.API.Data;
using NuovaPrevidenza.API.DTOs;
using NuovaPrevidenza.API.Models;

namespace NuovaPrevidenza.API.Services
{
    public interface IEmployeeService
    {
        Task<List<EmployeeDto>> GetAllAsync();
        Task<EmployeeDto?> GetByIdAsync(int id);
        Task<List<EmployeeDto>> SearchAsync(string term);
        Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto);
        Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeDto dto);
        Task<bool> DeleteAsync(int id);
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ApplicationDbContext context, ILogger<EmployeeService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<EmployeeDto>> GetAllAsync()
        {
            var employees = await _context.Employees.ToListAsync();
            return employees.Select(MapToDto).ToList();
        }

        public async Task<EmployeeDto?> GetByIdAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            return employee != null ? MapToDto(employee) : null;
        }

        public async Task<List<EmployeeDto>> SearchAsync(string term)
        {
            var employees = await _context.Employees
                .Where(e => e.Name.Contains(term) || e.Code.Contains(term))
                .ToListAsync();
            return employees.Select(MapToDto).ToList();
        }

        public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
        {
            var employee = new Employee
            {
                Name = dto.Name,
                Matricola = dto.Matricola,
                Code = dto.Code,
                TotalAmount = dto.TotalAmount,
                InstalmentAmount = dto.InstalmentAmount,
                NumberOfInstalments = dto.NumberOfInstalments,
                DecreeDate = dto.DecreeDate,
                DecreeProtocol = dto.DecreeProtocol,
                NotificationDate = dto.NotificationDate,
                DueDate = dto.DueDate,
                Status = dto.Status,
                Result = dto.Result,
                ResultDate = dto.ResultDate,
                PlannedDecree = dto.PlannedDecree,
                TrafficLight = dto.TrafficLight,
                Priority = dto.Priority,
                WithholdingProtocol = dto.WithholdingProtocol,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return MapToDto(employee);
        }

        public async Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeDto dto)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                throw new KeyNotFoundException($"Employee with id {id} not found");

            employee.Name = dto.Name;
            employee.Matricola = dto.Matricola;
            employee.Code = dto.Code;
            employee.TotalAmount = dto.TotalAmount;
            employee.InstalmentAmount = dto.InstalmentAmount;
            employee.NumberOfInstalments = dto.NumberOfInstalments;
            employee.DecreeDate = dto.DecreeDate;
            employee.DecreeProtocol = dto.DecreeProtocol;
            employee.NotificationDate = dto.NotificationDate;
            employee.DueDate = dto.DueDate;
            employee.Status = dto.Status;
            employee.Result = dto.Result;
            employee.ResultDate = dto.ResultDate;
            employee.PlannedDecree = dto.PlannedDecree;
            employee.TrafficLight = dto.TrafficLight;
            employee.Priority = dto.Priority;
            employee.IsActive = dto.IsActive;
            employee.WithholdingProtocol = dto.WithholdingProtocol;
            employee.UpdatedAt = DateTime.UtcNow;

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
            return MapToDto(employee);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return false;

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return true;
        }

        private static EmployeeDto MapToDto(Employee employee) =>
            new EmployeeDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Matricola = employee.Matricola,
                Code = employee.Code,
                TotalAmount = employee.TotalAmount,
                InstalmentAmount = employee.InstalmentAmount,
                NumberOfInstalments = employee.NumberOfInstalments,
                DecreeDate = employee.DecreeDate,
                DecreeProtocol = employee.DecreeProtocol,
                NotificationDate = employee.NotificationDate,
                DueDate = employee.DueDate,
                Status = employee.Status,
                Result = employee.Result,
                ResultDate = employee.ResultDate,
                PlannedDecree = employee.PlannedDecree,
                TrafficLight = employee.TrafficLight,
                Priority = employee.Priority,
                IsActive = employee.IsActive,
                WithholdingProtocol = employee.WithholdingProtocol
            };
    }
}
