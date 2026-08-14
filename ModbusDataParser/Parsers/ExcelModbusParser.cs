using OfficeOpenXml;
using ModbusDataParser.Models;

namespace ModbusDataParser.Parsers
{
    public class ExcelModbusParser
    {
        public RegisterTableData ParseFile(string filePath)
        {
            var result = new RegisterTableData
            {
                FileName = Path.GetFileName(filePath),
                SignalsBySheet = new Dictionary<string, List<ModbusSignal>>()
            };

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage(new FileInfo(filePath));

            foreach (var sheet in package.Workbook.Worksheets)
            {
                var sheetName = sheet.Name;
                
                if (sheetName.StartsWith("Interface") || sheetName.Contains("параметр") || sheetName.Contains("Interface parameters"))
                {
                    var interfaces = ParseInterfaceParameters(sheet, filePath);
                    result.InterfaceParameters.AddRange(interfaces);
                }
                else if (sheetName.Contains("Register Table") || sheetName.Contains("регистр") || sheetName.StartsWith("Register Table_Slave_"))
                {
                    var signals = ParseRegisterTable(sheet, filePath);
                    result.Signals.AddRange(signals);
                    result.SignalsBySheet[sheetName] = signals;
                }
            }

            return result;
        }

        private List<InterfaceParameter> ParseInterfaceParameters(ExcelWorksheet sheet, string filePath)
        {
            var result = new List<InterfaceParameter>();
            
            if (sheet.Dimension == null) return result;

            int startRow = FindHeaderRow(sheet, new[] { "№ п/п", "№", "1", "2" });

            for (int row = startRow + 1; row <= sheet.Dimension.Rows; row++)
            {
                var firstCell = GetCellText(sheet, row, 1);
                if (string.IsNullOrEmpty(firstCell)) continue;
                if (!int.TryParse(firstCell, out _)) continue;

                var param = new InterfaceParameter
                {
                    Number = int.Parse(firstCell),
                    InterfaceType = GetCellText(sheet, row, 2),
                    ProtocolType = GetCellText(sheet, row, 3),
                    SlaveStation = GetCellText(sheet, row, 4),
                    SlaveIdMain = GetCellText(sheet, row, 5),
                    SlaveIdBackup = GetCellText(sheet, row, 6),
                    Speed = GetCellText(sheet, row, 7),
                    ParityBit = GetCellText(sheet, row, 8),
                    StopBit = GetCellText(sheet, row, 9),
                    DataBit = GetCellText(sheet, row, 10),
                    Reverse = GetCellText(sheet, row, 11),
                    Timeout = GetCellText(sheet, row, 12),
                    MaximumLength = GetCellText(sheet, row, 13),
                    Note = GetCellText(sheet, row, 14),
                    SourceFile = Path.GetFileName(filePath)
                };

                if (!string.IsNullOrEmpty(param.InterfaceType) || !string.IsNullOrEmpty(param.SlaveIdMain))
                {
                    result.Add(param);
                }
            }

            return result;
        }

        private List<ModbusSignal> ParseRegisterTable(ExcelWorksheet sheet, string filePath)
        {
            var result = new List<ModbusSignal>();
            
            if (sheet.Dimension == null) return result;

            int startRow = FindHeaderRow(sheet, new[] { "№", "№ п/п", "1", "2" });

            for (int row = startRow + 1; row <= sheet.Dimension.Rows; row++)
            {
                var firstCell = GetCellText(sheet, row, 1);
                if (string.IsNullOrEmpty(firstCell)) continue;
                if (!int.TryParse(firstCell, out _)) continue;

                var signal = new ModbusSignal
                {
                    Number = int.Parse(firstCell),
                    ProjectFunctionalDesignation = GetCellText(sheet, row, 2),
                    PlcTag = GetCellText(sheet, row, 3),
                    Description = GetCellText(sheet, row, 4),
                    Unit = GetCellText(sheet, row, 5),
                    Scale = GetCellText(sheet, row, 6),
                    LL = GetCellDouble(sheet, row, 7),
                    LA = GetCellDouble(sheet, row, 8),
                    HA = GetCellDouble(sheet, row, 9),
                    HH = GetCellDouble(sheet, row, 10),
                    ScalingFactors = GetCellText(sheet, row, 11),
                    SignalType = GetCellText(sheet, row, 12),
                    RegisterType = GetCellInt(sheet, row, 13),
                    AddressBit = GetCellText(sheet, row, 14),
                    AccessType = GetCellText(sheet, row, 15),
                    DataType = GetCellText(sheet, row, 16),
                    FunctionCode = GetCellInt(sheet, row, 17),
                    DcsChannel = GetCellText(sheet, row, 18),
                    DcsTag = GetCellText(sheet, row, 19),
                    DcsFunctions = GetCellText(sheet, row, 20),
                    Note = GetCellText(sheet, row, 21),
                    SheetName = sheet.Name,
                    SourceFile = Path.GetFileName(filePath)
                };

                if (!string.IsNullOrEmpty(signal.PlcTag) || !string.IsNullOrEmpty(signal.Description))
                {
                    result.Add(signal);
                }
            }

            return result;
        }

        private int FindHeaderRow(ExcelWorksheet sheet, string[] possibleHeaders)
        {
            if (sheet.Dimension == null) return 1;

            for (int row = 1; row <= Math.Min(15, sheet.Dimension.Rows); row++)
            {
                var cellValue = GetCellText(sheet, row, 1);
                foreach (var header in possibleHeaders)
                {
                    if (cellValue == header)
                        return row;
                }
            }
            return 1;
        }

        private string GetCellText(ExcelWorksheet sheet, int row, int col)
        {
            try
            {
                var cell = sheet.Cells[row, col];
                if (cell == null) return string.Empty;
                return cell.Text?.Trim() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private int? GetCellInt(ExcelWorksheet sheet, int row, int col)
        {
            try
            {
                var text = GetCellText(sheet, row, col);
                if (string.IsNullOrEmpty(text)) return null;
                if (int.TryParse(text, out int result)) return result;
                return null;
            }
            catch
            {
                return null;
            }
        }

        private double? GetCellDouble(ExcelWorksheet sheet, int row, int col)
        {
            try
            {
                var text = GetCellText(sheet, row, col);
                if (string.IsNullOrEmpty(text)) return null;
                if (double.TryParse(text, System.Globalization.NumberStyles.Any, 
                    System.Globalization.CultureInfo.InvariantCulture, out double result)) return result;
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
