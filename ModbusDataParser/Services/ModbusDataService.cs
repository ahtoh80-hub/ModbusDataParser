using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModbusDataParser.Models;
using ModbusDataParser.Parsers;

namespace ModbusDataParser.Services
{
    public class ModbusDataService
    {
        private readonly ExcelModbusParser _parser = new();
        private RegisterTableData? _currentData;
        private readonly Dictionary<string, RegisterTableData> _allFilesData = new();

        public event EventHandler? DataChanged;

        public RegisterTableData? CurrentData => _currentData;
        public List<string> LoadedFiles => _allFilesData.Keys.ToList();

        public void LoadFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            var data = _parser.ParseFile(filePath);
            _allFilesData[Path.GetFileName(filePath)] = data;
            _currentData = data;
            OnDataChanged();
        }

        public void LoadFiles(IEnumerable<string> filePaths)
        {
            foreach (var filePath in filePaths)
            {
                if (File.Exists(filePath))
                {
                    var data = _parser.ParseFile(filePath);
                    _allFilesData[Path.GetFileName(filePath)] = data;
                }
            }

            if (_allFilesData.Count > 0)
            {
                _currentData = _allFilesData.Values.First();
                OnDataChanged();
            }
        }

        public RegisterTableData? GetFileData(string fileName)
        {
            _allFilesData.TryGetValue(fileName, out var data);
            return data;
        }

        public void SetCurrentFile(string fileName)
        {
            if (_allFilesData.TryGetValue(fileName, out var data))
            {
                _currentData = data;
                OnDataChanged();
            }
        }

        public List<ModbusSignal> GetAllSignals()
        {
            return _allFilesData.Values.SelectMany(d => d.Signals).ToList();
        }

        public List<InterfaceParameter> GetAllInterfaces()
        {
            return _allFilesData.Values.SelectMany(d => d.InterfaceParameters).ToList();
        }

        public string GenerateImportData(IEnumerable<ModbusSignal> signals)
        {
            var lines = new List<string>
            {
                "PLC_Tag,Address,RegisterType,DataType,FunctionCode,AccessType,Description,Unit,SignalType,DCS_Tag,DCS_Functions"
            };

            foreach (var signal in signals)
            {
                if (string.IsNullOrEmpty(signal.PlcTag) && string.IsNullOrEmpty(signal.Description))
                    continue;

                var line = $"{EscapeCsv(signal.PlcTag)}," +
                          $"{EscapeCsv(signal.AddressBit)}," +
                          $"{signal.RegisterType}," +
                          $"{EscapeCsv(signal.DataType)}," +
                          $"{signal.FunctionCode}," +
                          $"{EscapeCsv(signal.AccessType)}," +
                          $"{EscapeCsv(signal.Description)}," +
                          $"{EscapeCsv(signal.Unit)}," +
                          $"{EscapeCsv(signal.SignalType)}," +
                          $"{EscapeCsv(signal.DcsTag)}," +
                          $"{EscapeCsv(signal.DcsFunctions)}";

                lines.Add(line);
            }

            return string.Join(Environment.NewLine, lines);
        }

        public string GenerateAddressMap(IEnumerable<ModbusSignal> signals)
        {
            var lines = new List<string>
            {
                "Address,Bit,PLC_Tag,Description,DataType,FunctionCode,RegisterType"
            };

            foreach (var signal in signals)
            {
                if (string.IsNullOrEmpty(signal.AddressBit)) continue;

                var addressParts = signal.AddressBit.Split('.');
                var address = addressParts[0];
                var bit = addressParts.Length > 1 ? addressParts[1] : "";

                var line = $"{address}," +
                          $"{bit}," +
                          $"{EscapeCsv(signal.PlcTag)}," +
                          $"{EscapeCsv(signal.Description)}," +
                          $"{EscapeCsv(signal.DataType)}," +
                          $"{signal.FunctionCode}," +
                          $"{signal.RegisterType}";

                lines.Add(line);
            }

            return string.Join(Environment.NewLine, lines);
        }

        private string EscapeCsv(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "\"\"";
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
                return $"\"{value.Replace("\"", "\"\"")}\"";
            return value;
        }

        protected virtual void OnDataChanged()
        {
            DataChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
