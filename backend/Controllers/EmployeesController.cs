using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuovaPrevidenza.API.DTOs;
using NuovaPrevidenza.API.Services;

namespace NuovaPrevidenza.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IExcelService _excelService;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(
            IEmployeeService employeeService,
            IExcelService excelService,
            ILogger<EmployeesController> logger)
        {
            _employeeService = employeeService;
            _excelService = excelService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var employees = await _employeeService.GetAllAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting employees: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var employee = await _employeeService.GetByIdAsync(id);
                if (employee == null)
                    return NotFound(new { message = "Employee not found" });
                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting employee: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("search/{term}")]
        public async Task<IActionResult> Search(string term)
        {
            try
            {
                var employees = await _employeeService.SearchAsync(term);
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error searching employees: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var employee = await _employeeService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating employee: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var employee = await _employeeService.UpdateAsync(id, dto);
                return Ok(employee);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Employee not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating employee: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _employeeService.DeleteAsync(id);
                if (!result)
                    return NotFound(new { message = "Employee not found" });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting employee: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("export")]
        public async Task<IActionResult> Export()
        {
            try
            {
                var employees = await _employeeService.GetAllAsync();
                var excelContent = _excelService.ExportEmployeesToExcel(employees);
                
                return File(
                    excelContent,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Dipendenti_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error exporting employees: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("import")]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file provided" });

            if (!file.FileName.EndsWith(".xlsx"))
                return BadRequest(new { message = "Invalid file format. Please upload an Excel file" });

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var employees = _excelService.ImportEmployeesFromExcel(stream);
                    
                    var results = new List<EmployeeDto>();
                    foreach (var employee in employees)
                    {
                        var created = await _employeeService.CreateAsync(employee);
                        results.Add(created);
                    }

                    return Ok(new { message = $"{results.Count} employees imported successfully", data = results });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error importing employees: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
