using OfficeOpenXml;
using ModbusDataParser.Models;

namespace ModbusDataParser.Services
{
    public class ScadaExcelExporter
    {
        private readonly string? _templatePath;
        private List<string[]> _headerRows = new();

        public ScadaExcelExporter(string? templatePath = null)
        {
            _templatePath = templatePath;
        }

        public void LoadHeaderFromTemplate()
        {
            if (string.IsNullOrEmpty(_templatePath) || !File.Exists(_templatePath))
                throw new FileNotFoundException($"Template file not found: {_templatePath}");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage(new FileInfo(_templatePath));
            var worksheet = package.Workbook.Worksheets[0];

            if (worksheet.Dimension == null)
                throw new InvalidOperationException("Template worksheet is empty");

            _headerRows.Clear();

            int rowsToRead = Math.Min(3, worksheet.Dimension.Rows);
            for (int row = 1; row <= rowsToRead; row++)
            {
                var rowData = new List<string>();
                int colsToRead = Math.Min(19, worksheet.Dimension.Columns);
                for (int col = 1; col <= colsToRead; col++)
                {
                    var value = worksheet.Cells[row, col].Text ?? "";
                    rowData.Add(value);
                }
                _headerRows.Add(rowData.ToArray());
            }
        }

        public void ExportToExcel(List<ScadaRow> rows, string outputPath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Sheet1");

            int currentRow = 1;

            // Вставляем шапку
            if (_headerRows.Count > 0)
            {
                foreach (var headerRow in _headerRows)
                {
                    for (int col = 0; col < headerRow.Length && col < 19; col++)
                    {
                        worksheet.Cells[currentRow, col + 1].Value = headerRow[col];
                    }
                    currentRow++;
                }
            }
            else
            {
                // Стандартная шапка если шаблон не загружен
                var defaultHeaders = new[] { "№", "Статус", "Раздел", "Марка", "Тип объекта",
                    "Наименование", "Описание", "Подпись", "Номер", "PLC переменная",
                    "Период архив", "KKS", "Доп.параметр", "Маска упр. в срезах",
                    "Классификатор", "Группа событий", "КОНТРОЛЛЕР", "Адрес", "№ ресурса или группа" };
                
                for (int col = 0; col < defaultHeaders.Length; col++)
                {
                    worksheet.Cells[currentRow, col + 1].Value = defaultHeaders[col];
                    worksheet.Cells[currentRow, col + 1].Style.Font.Bold = true;
                }
                currentRow++;
            }

            // Данные
            int dataStartRow = currentRow;
            foreach (var row in rows)
            {
                worksheet.Cells[currentRow, 1].Value = row.Number;
                worksheet.Cells[currentRow, 2].Value = row.Status;
                worksheet.Cells[currentRow, 3].Value = row.Mode;
                worksheet.Cells[currentRow, 4].Value = row.Brand;
                worksheet.Cells[currentRow, 5].Value = row.ObjectType;
                worksheet.Cells[currentRow, 6].Value = row.Name;
                worksheet.Cells[currentRow, 7].Value = row.Description;
                worksheet.Cells[currentRow, 8].Value = row.ObjSign;
                worksheet.Cells[currentRow, 9].Value = row.ObjNumber;
                worksheet.Cells[currentRow, 10].Value = row.PlcVarName;
                worksheet.Cells[currentRow, 11].Value = row.ArhPer;
                worksheet.Cells[currentRow, 12].Value = row.Kks;
                worksheet.Cells[currentRow, 13].Value = row.ObjDParam;
                worksheet.Cells[currentRow, 14].Value = row.SrezControl;
                worksheet.Cells[currentRow, 15].Value = row.UserGroup;
                worksheet.Cells[currentRow, 16].Value = row.EvGroup;
                worksheet.Cells[currentRow, 17].Value = row.PlcName;
                worksheet.Cells[currentRow, 18].Value = row.PlcAdress;
                worksheet.Cells[currentRow, 19].Value = row.PlcGr;
                currentRow++;
            }

            // Форматирование
            ApplyFormatting(worksheet, dataStartRow, currentRow - 1);
            worksheet.Cells.AutoFitColumns();

            package.SaveAs(new FileInfo(outputPath));
        }

        private void ApplyFormatting(ExcelWorksheet worksheet, int startRow, int endRow)
        {
            if (startRow > endRow) return;

            // Заголовки
            using (var range = worksheet.Cells[1, 1, startRow - 1, 19])
            {
                range.Style.Font.Bold = true;
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Границы для данных
            using (var range = worksheet.Cells[startRow, 1, endRow, 19])
            {
                range.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                range.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            }
        }
    }
}
