using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ModbusDataParser.Models;
using OfficeOpenXml;

namespace ModbusDataParser.Parsers
{
    /// <summary>
    /// Парсер Excel файла с данными Modbus
    /// </summary>
    public class ModbusExcelParser
    {
        private const string SHEET_INTERFACE = "Interface parameters";
        private const string SHEET_FUNCTION_02 = "Register Table Function 02";
        private const string SHEET_FUNCTION_03 = "Register Table Function 03";
        private const string SHEET_FUNCTION_06 = "Register Table Function 06";

        // Альтернативные названия вкладок
        private static readonly string[] INTERFACE_SHEET_NAMES = { "Interface parameters", "Interface Parameters", "Параметры интерфейса" };
        private static readonly string[] FC02_SHEET_NAMES = { "Register Table Function 02", "Function 02", "FC02", "Функция 02" };
        private static readonly string[] FC03_SHEET_NAMES = { "Register Table Function 03", "Function 03", "FC03", "Функция 03" };
        private static readonly string[] FC06_SHEET_NAMES = { "Register Table Function 06", "Function 06", "FC06", "Функция 06" };

        public ModbusSystemData ParseFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл не найден: {filePath}");

            var data = new ModbusSystemData
            {
                SystemName = Path.GetFileNameWithoutExtension(filePath)
            };

            using var package = new ExcelPackage(new FileInfo(filePath));

            // Парсинг каждой вкладки
            ParseInterfaceSheet(package, data);
            ParseFunctionSheet(package, data, FC02_SHEET_NAMES, 0x02);
            ParseFunctionSheet(package, data, FC03_SHEET_NAMES, 0x03);
            ParseFunctionSheet(package, data, FC06_SHEET_NAMES, 0x06);

            return data;
        }

        private void ParseInterfaceSheet(ExcelPackage package, ModbusSystemData data)
        {
            var worksheet = FindWorksheet(package, INTERFACE_SHEET_NAMES);
            if (worksheet == null) return;

            int row = 2; // Пропускаем заголовок
            while (row <= worksheet.Dimension?.Rows && worksheet.Cells[row, 1].Value != null)
            {
                try
                {
                    var param = new InterfaceParameter
                    {
                        Number = GetIntValue(worksheet.Cells[row, 1].Value),
                        InterfaceType = GetStringValue(worksheet.Cells[row, 2].Value),
                        ProtocolType = GetStringValue(worksheet.Cells[row, 3].Value),
                        SlaveStation = GetStringValue(worksheet.Cells[row, 4].Value),
                        SlaveIdMain = GetStringValue(worksheet.Cells[row, 5].Value),
                        SlaveIdBackup = GetStringValue(worksheet.Cells[row, 6].Value),
                        Speed = GetStringValue(worksheet.Cells[row, 7].Value),
                        ParityBit = GetStringValue(worksheet.Cells[row, 8].Value),
                        StopBit = GetStringValue(worksheet.Cells[row, 9].Value),
                        DataBit = GetStringValue(worksheet.Cells[row, 10].Value),
                        Reverse = GetStringValue(worksheet.Cells[row, 11].Value),
                        Timeout = GetStringValue(worksheet.Cells[row, 12].Value),
                        MaximumLength = GetStringValue(worksheet.Cells[row, 13].Value),
                        Note = GetStringValue(worksheet.Cells[row, 14].Value)
                    };

                    if (!string.IsNullOrEmpty(param.SlaveStation))
                        data.Interfaces.Add(param);
                }
                catch (Exception ex)
                {
                    // Логируем ошибку, но продолжаем
                    System.Diagnostics.Debug.WriteLine($"Ошибка парсинга строки {row}: {ex.Message}");
                }
                row++;
            }
        }

        private void ParseFunctionSheet(ExcelPackage package, ModbusSystemData data, string[] sheetNames, byte functionCode)
        {
            var worksheet = FindWorksheet(package, sheetNames);
            if (worksheet == null) return;

            int row = 2; // Пропускаем заголовок
            while (row <= worksheet.Dimension?.Rows && worksheet.Cells[row, 1].Value != null)
            {
                try
                {
                    string projectDesignation = GetStringValue(worksheet.Cells[row, 2].Value);
                    string addressBit = GetStringValue(worksheet.Cells[row, 14].Value);
                    string dataType = GetStringValue(worksheet.Cells[row, 16].Value);

                    // Пропускаем пустые строки
                    if (string.IsNullOrEmpty(projectDesignation)) 
                    {
                        row++;
                        continue;
                    }

                    switch (functionCode)
                    {
                        case 0x02:
                            ParseDiscreteSignal(worksheet, row, data, addressBit, dataType);
                            break;

                        case 0x03:
                            ParseAnalogSignal(worksheet, row, data, dataType);
                            break;

                        case 0x06:
                            ParseControlSignal(worksheet, row, data);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка парсинга строки {row} (FC {functionCode:X2}): {ex.Message}");
                }
                row++;
            }
        }

        private void ParseDiscreteSignal(ExcelWorksheet worksheet, int row, ModbusSystemData data, string addressBit, string dataType)
        {
            var signal = new DiscreteSignal
            {
                Number = GetIntValue(worksheet.Cells[row, 1].Value),
                ProjectDesignation = GetStringValue(worksheet.Cells[row, 2].Value),
                PlcTag = GetStringValue(worksheet.Cells[row, 3].Value),
                Description = GetStringValue(worksheet.Cells[row, 4].Value),
                Unit = GetStringValue(worksheet.Cells[row, 5].Value),
                Scale = GetStringValue(worksheet.Cells[row, 6].Value),
                LL = GetStringValue(worksheet.Cells[row, 7].Value),
                LA = GetStringValue(worksheet.Cells[row, 8].Value),
                HA = GetStringValue(worksheet.Cells[row, 9].Value),
                HH = GetStringValue(worksheet.Cells[row, 10].Value),
                ScalingFactor = GetIntValue(worksheet.Cells[row, 11].Value),
                SignalType = GetStringValue(worksheet.Cells[row, 12].Value),
                RegisterType = GetStringValue(worksheet.Cells[row, 13].Value),
                Address = GetIntValue(worksheet.Cells[row, 14].Value),
                AccessType = GetStringValue(worksheet.Cells[row, 15].Value),
                DataType = string.IsNullOrEmpty(dataType) ? "BOOL" : dataType,
                FunctionCode = 0x02,
                DcsChannel = GetStringValue(worksheet.Cells[row, 18].Value),
                DcsTag = GetStringValue(worksheet.Cells[row, 19].Value),
                DcsFunctions = GetStringValue(worksheet.Cells[row, 20].Value),
                Note = GetStringValue(worksheet.Cells[row, 21].Value),
                AddressBit = addressBit,
                InterfaceDesignation = GetStringValue(worksheet.Cells[row, 2].Value)
            };

            // Парсим Address/Bit
            if (!string.IsNullOrEmpty(addressBit))
            {
                if (addressBit.Contains('/') || addressBit.Contains('.'))
                {
                    char separator = addressBit.Contains('/') ? '/' : '.';
                    var parts = addressBit.Split(separator);
                    if (parts.Length >= 2)
                    {
                        if (int.TryParse(parts[0], out int addr))
                            signal.Address = addr;
                        if (int.TryParse(parts[1], out int bit))
                            signal.BitNumber = bit;
                    }
                }
                else if (int.TryParse(addressBit, out int addr))
                {
                    signal.Address = addr;
                    signal.BitNumber = 0;
                }
            }

            if (signal.BitNumber == null)
                signal.BitNumber = 0;

            data.DiscreteSignals.Add(signal);
        }

        private void ParseAnalogSignal(ExcelWorksheet worksheet, int row, ModbusSystemData data, string dataType)
        {
            var signal = new AnalogSignal
            {
                Number = GetIntValue(worksheet.Cells[row, 1].Value),
                ProjectDesignation = GetStringValue(worksheet.Cells[row, 2].Value),
                PlcTag = GetStringValue(worksheet.Cells[row, 3].Value),
                Description = GetStringValue(worksheet.Cells[row, 4].Value),
                Unit = GetStringValue(worksheet.Cells[row, 5].Value),
                Scale = GetStringValue(worksheet.Cells[row, 6].Value),
                LL = GetStringValue(worksheet.Cells[row, 7].Value),
                LA = GetStringValue(worksheet.Cells[row, 8].Value),
                HA = GetStringValue(worksheet.Cells[row, 9].Value),
                HH = GetStringValue(worksheet.Cells[row, 10].Value),
                ScalingFactor = GetIntValue(worksheet.Cells[row, 11].Value),
                SignalType = GetStringValue(worksheet.Cells[row, 12].Value),
                RegisterType = GetStringValue(worksheet.Cells[row, 13].Value),
                Address = GetIntValue(worksheet.Cells[row, 14].Value),
                AccessType = GetStringValue(worksheet.Cells[row, 15].Value),
                DataType = string.IsNullOrEmpty(dataType) ? "32-Bit Floating" : dataType,
                FunctionCode = 0x03,
                DcsChannel = GetStringValue(worksheet.Cells[row, 18].Value),
                DcsTag = GetStringValue(worksheet.Cells[row, 19].Value),
                DcsFunctions = GetStringValue(worksheet.Cells[row, 20].Value),
                Note = GetStringValue(worksheet.Cells[row, 21].Value),
                InterfaceDesignation = GetStringValue(worksheet.Cells[row, 2].Value),
                Is32Bit = dataType == "32-Bit Floating" || dataType == "32-Bit Integer"
            };

            data.AnalogSignals.Add(signal);
        }

        private void ParseControlSignal(ExcelWorksheet worksheet, int row, ModbusSystemData data)
        {
            var signal = new ControlSignal
            {
                Number = GetIntValue(worksheet.Cells[row, 1].Value),
                ProjectDesignation = GetStringValue(worksheet.Cells[row, 2].Value),
                PlcTag = GetStringValue(worksheet.Cells[row, 3].Value),
                Description = GetStringValue(worksheet.Cells[row, 4].Value),
                Unit = GetStringValue(worksheet.Cells[row, 5].Value),
                Scale = GetStringValue(worksheet.Cells[row, 6].Value),
                LL = GetStringValue(worksheet.Cells[row, 7].Value),
                LA = GetStringValue(worksheet.Cells[row, 8].Value),
                HA = GetStringValue(worksheet.Cells[row, 9].Value),
                HH = GetStringValue(worksheet.Cells[row, 10].Value),
                ScalingFactor = GetIntValue(worksheet.Cells[row, 11].Value),
                SignalType = GetStringValue(worksheet.Cells[row, 12].Value),
                RegisterType = GetStringValue(worksheet.Cells[row, 13].Value),
                Address = GetIntValue(worksheet.Cells[row, 14].Value),
                AccessType = GetStringValue(worksheet.Cells[row, 15].Value),
                DataType = "16-Bit Unsigned",
                FunctionCode = 0x06,
                DcsChannel = GetStringValue(worksheet.Cells[row, 18].Value),
                DcsTag = GetStringValue(worksheet.Cells[row, 19].Value),
                DcsFunctions = GetStringValue(worksheet.Cells[row, 20].Value),
                Note = GetStringValue(worksheet.Cells[row, 21].Value),
                InterfaceDesignation = GetStringValue(worksheet.Cells[row, 2].Value)
            };

            data.ControlSignals.Add(signal);
        }

        private ExcelWorksheet? FindWorksheet(ExcelPackage package, string[] names)
        {
            foreach (var name in names)
            {
                var ws = package.Workbook.Worksheets[name];
                if (ws != null)
                    return ws;
            }
            return null;
        }

        #region Helper Methods
        private string GetStringValue(object? value)
        {
            return value?.ToString()?.Trim() ?? string.Empty;
        }

        private int GetIntValue(object? value)
        {
            if (value == null) return 0;
            string str = value.ToString()?.Trim() ?? string.Empty;
            if (int.TryParse(str, out int result))
                return result;
            return 0;
        }

        private double GetDoubleValue(object? value)
        {
            if (value == null) return 0;
            string str = value.ToString()?.Trim() ?? string.Empty;
            str = str.Replace(',', '.');
            if (double.TryParse(str, System.Globalization.NumberStyles.Any, 
                System.Globalization.CultureInfo.InvariantCulture, out double result))
                return result;
            return 0;
        }
        #endregion
    }
}