using ClosedXML.Excel;
using NuovaPrevidenza.API.DTOs;
using NuovaPrevidenza.API.Models;

namespace NuovaPrevidenza.API.Services
{
    public interface IExcelService
    {
        byte[] ExportEmployeesToExcel(List<EmployeeDto> employees);
        List<CreateEmployeeDto> ImportEmployeesFromExcel(Stream fileStream);
    }

    public class ExcelService : IExcelService
    {
        private readonly ILogger<ExcelService> _logger;

        public ExcelService(ILogger<ExcelService> logger)
        {
            _logger = logger;
        }

        public byte[] ExportEmployeesToExcel(List<EmployeeDto> employees)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Dipendenti");

                // Header
                worksheet.Cell(1, 1).Value = "Dipendente";
                worksheet.Cell(1, 2).Value = "Matricola";
                worksheet.Cell(1, 3).Value = "Codice";
                worksheet.Cell(1, 4).Value = "Importo Tot";
                worksheet.Cell(1, 5).Value = "Importo Rata";
                worksheet.Cell(1, 6).Value = "N. Rate";
                worksheet.Cell(1, 7).Value = "Data Decreto";
                worksheet.Cell(1, 8).Value = "Prot. Decreto";
                worksheet.Cell(1, 9).Value = "Data Notifica";
                worksheet.Cell(1, 10).Value = "Scadenza";
                worksheet.Cell(1, 11).Value = "Stato";
                worksheet.Cell(1, 12).Value = "Esito";
                worksheet.Cell(1, 13).Value = "Data Esito";
                worksheet.Cell(1, 14).Value = "Dec. Prevista";
                worksheet.Cell(1, 15).Value = "Semaforo";
                worksheet.Cell(1, 16).Value = "Priorità";
                worksheet.Cell(1, 17).Value = "SI";
                worksheet.Cell(1, 18).Value = "Prot. Trattenute";

                // Style header
                var headerRange = worksheet.Range(1, 1, 1, 18);
                headerRange.Style.Fill.BackgroundColor = XLColor.DarkBlue;
                headerRange.Style.Font.FontColor = XLColor.White;
                headerRange.Style.Font.Bold = true;

                // Data
                int row = 2;
                foreach (var employee in employees)
                {
                    worksheet.Cell(row, 1).Value = employee.Name;
                    worksheet.Cell(row, 2).Value = employee.Matricola;
                    worksheet.Cell(row, 3).Value = employee.Code;
                    worksheet.Cell(row, 4).Value = employee.TotalAmount;
                    worksheet.Cell(row, 5).Value = employee.InstalmentAmount;
                    worksheet.Cell(row, 6).Value = employee.NumberOfInstalments;
                    worksheet.Cell(row, 7).Value = employee.DecreeDate.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 8).Value = employee.DecreeProtocol;
                    worksheet.Cell(row, 9).Value = employee.NotificationDate.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 10).Value = employee.DueDate.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 11).Value = employee.Status;
                    worksheet.Cell(row, 12).Value = employee.Result;
                    worksheet.Cell(row, 13).Value = employee.ResultDate.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 14).Value = employee.PlannedDecree;
                    worksheet.Cell(row, 15).Value = employee.TrafficLight;
                    worksheet.Cell(row, 16).Value = employee.Priority;
                    worksheet.Cell(row, 17).Value = employee.IsActive ? "SI" : "NO";
                    worksheet.Cell(row, 18).Value = employee.WithholdingProtocol;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public List<CreateEmployeeDto> ImportEmployeesFromExcel(Stream fileStream)
        {
            var employees = new List<CreateEmployeeDto>();

            using (var workbook = new XLWorkbook(fileStream))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                foreach (var row in rows)
                {
                    try
                    {
                        var employee = new CreateEmployeeDto
                        {
                            Name = row.Cell(1).Value.ToString(),
                            Matricola = int.Parse(row.Cell(2).Value.ToString() ?? "0"),
                            Code = row.Cell(3).Value.ToString(),
                            TotalAmount = decimal.Parse(row.Cell(4).Value.ToString() ?? "0"),
                            InstalmentAmount = decimal.Parse(row.Cell(5).Value.ToString() ?? "0"),
                            NumberOfInstalments = int.Parse(row.Cell(6).Value.ToString() ?? "0"),
                            DecreeDate = DateTime.Parse(row.Cell(7).Value.ToString() ?? DateTime.Now.ToString()),
                            DecreeProtocol = row.Cell(8).Value.ToString(),
                            NotificationDate = DateTime.Parse(row.Cell(9).Value.ToString() ?? DateTime.Now.ToString()),
                            DueDate = DateTime.Parse(row.Cell(10).Value.ToString() ?? DateTime.Now.ToString()),
                            Status = row.Cell(11).Value.ToString(),
                            Result = row.Cell(12).Value.ToString(),
                            ResultDate = DateTime.Parse(row.Cell(13).Value.ToString() ?? DateTime.Now.ToString()),
                            PlannedDecree = row.Cell(14).Value.ToString(),
                            TrafficLight = row.Cell(15).Value.ToString(),
                            Priority = row.Cell(16).Value.ToString(),
                            WithholdingProtocol = row.Cell(18).Value.ToString()
                        };

                        employees.Add(employee);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error importing row: {ex.Message}");
                    }
                }
            }

            return employees;
        }
    }
}
