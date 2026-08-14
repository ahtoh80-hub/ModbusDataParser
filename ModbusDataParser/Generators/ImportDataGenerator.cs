using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ModbusDataParser.Models;

namespace ModbusDataParser.Generators
{
    public enum ExportFormat
    {
        Csv,
        Json,
        Sql,
        Xml
    }

    /// <summary>
    /// Генератор данных для импорта
    /// </summary>
    public class ImportDataGenerator
    {
        private readonly ModbusSystemData data;

        public ImportDataGenerator(ModbusSystemData data)
        {
            this.data = data;
        }

        /// <summary>
        /// Генерация CSV для импорта сигналов
        /// </summary>
        public string GenerateCsv(char separator = ';', bool includeHeader = true)
        {
            var sb = new StringBuilder();

            if (includeHeader)
            {
                sb.AppendLine(string.Join(separator, new[]
                {
                    "№п/п", "Функц.обозначение", "ТегПЛК", "Описание", "Ед.изм.",
                    "Шкала", "LL", "LA", "HA", "HH", "Коэф.", "ТипСигнала",
                    "ТипРегистра", "Адрес/бит", "ТипДоступа", "ТипДанных",
                    "КодФункции", "КаналРСУ", "ТегРСУ", "ФункцииРСУ", "Примечание"
                }));
            }

            foreach (var signal in data.GetAllSignals().OrderBy(s => s.FunctionCode).ThenBy(s => s.Address))
            {
                sb.AppendLine(string.Join(separator, new[]
                {
                    signal.Number.ToString(),
                    EscapeCsv(signal.ProjectDesignation),
                    EscapeCsv(signal.PlcTag),
                    EscapeCsv(signal.Description),
                    EscapeCsv(signal.Unit),
                    EscapeCsv(signal.Scale),
                    EscapeCsv(signal.LL),
                    EscapeCsv(signal.LA),
                    EscapeCsv(signal.HA),
                    EscapeCsv(signal.HH),
                    signal.ScalingFactor.ToString(),
                    EscapeCsv(signal.SignalType),
                    EscapeCsv(signal.RegisterType),
                    signal.GetModbusAddress(),
                    EscapeCsv(signal.AccessType),
                    EscapeCsv(signal.DataType),
                    signal.FunctionCode.ToString("X2"),
                    EscapeCsv(signal.DcsChannel),
                    EscapeCsv(signal.DcsTag),
                    EscapeCsv(signal.DcsFunctions),
                    EscapeCsv(signal.Note)
                }));
            }

            return sb.ToString();
        }

        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (value.Contains(';') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
                return $"\"{value.Replace("\"", "\"\"")}\"";
            return value;
        }

        /// <summary>
        /// Генерация JSON для импорта
        /// </summary>
        public string GenerateJson(bool pretty = true)
        {
            var signals = data.GetAllSignals().Select(s => new
            {
                number = s.Number,
                projectDesignation = s.ProjectDesignation,
                plcTag = s.PlcTag,
                description = s.Description,
                unit = s.Unit,
                scale = s.Scale,
                ll = s.LL,
                la = s.LA,
                ha = s.HA,
                hh = s.HH,
                scalingFactor = s.ScalingFactor,
                signalType = s.SignalType,
                registerType = s.RegisterType,
                address = s.Address,
                bit = s.BitNumber,
                addressBit = s.GetModbusAddress(),
                accessType = s.AccessType,
                dataType = s.DataType,
                functionCode = s.FunctionCode,
                functionName = s.GetFunctionName(),
                dcsChannel = s.DcsChannel,
                dcsTag = s.DcsTag,
                dcsFunctions = s.DcsFunctions,
                note = s.Note
            });

            var result = new
            {
                system = data.SystemName,
                generated = DateTime.Now,
                statistics = data.GetSignalTypeStatistics(),
                interfaces = data.Interfaces.Select(i => new
                {
                    number = i.Number,
                    type = i.InterfaceType,
                    protocol = i.ProtocolType,
                    slaveStation = i.SlaveStation,
                    speed = i.Speed,
                    parity = i.ParityBit,
                    timeout = i.Timeout
                }),
                signals = signals
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(result, 
                pretty ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None);

            return json;
        }

        /// <summary>
        /// Генерация SQL скрипта для импорта
        /// </summary>
        public string GenerateSql(string tableName = "ModbusSignals", bool useInsert = true)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"-- SQL скрипт для импорта сигналов Modbus");
            sb.AppendLine($"-- Система: {data.SystemName}");
            sb.AppendLine($"-- Дата: {DateTime.Now}");
            sb.AppendLine($"-- Всего сигналов: {data.TotalSignals}");
            sb.AppendLine();

            if (useInsert)
            {
                sb.AppendLine($"INSERT INTO {tableName} (");
                sb.AppendLine("    Number, ProjectDesignation, PlcTag, Description, Unit,");
                sb.AppendLine("    Scale, LL, LA, HA, HH, ScalingFactor, SignalType,");
                sb.AppendLine("    RegisterType, Address, BitNumber, AddressBit, AccessType,");
                sb.AppendLine("    DataType, FunctionCode, DcsChannel, DcsTag, DcsFunctions, Note");
                sb.AppendLine(") VALUES");

                var signals = data.GetAllSignals().OrderBy(s => s.FunctionCode).ThenBy(s => s.Address).ToList();
                for (int i = 0; i < signals.Count; i++)
                {
                    var s = signals[i];
                    var comma = i < signals.Count - 1 ? "," : ";";
                    
                    sb.AppendLine($"    ({s.Number}, {SqlString(s.ProjectDesignation)}, {SqlString(s.PlcTag)},");
                    sb.AppendLine($"     {SqlString(s.Description)}, {SqlString(s.Unit)},");
                    sb.AppendLine($"     {SqlString(s.Scale)}, {SqlString(s.LL)}, {SqlString(s.LA)},");
                    sb.AppendLine($"     {SqlString(s.HA)}, {SqlString(s.HH)}, {s.ScalingFactor}, {SqlString(s.SignalType)},");
                    sb.AppendLine($"     {SqlString(s.RegisterType)}, {s.Address}, {(s.BitNumber.HasValue ? s.BitNumber.Value.ToString() : "NULL")},");
                    sb.AppendLine($"     {SqlString(s.GetModbusAddress())}, {SqlString(s.AccessType)},");
                    sb.AppendLine($"     {SqlString(s.DataType)}, {s.FunctionCode}, {SqlString(s.DcsChannel)},");
                    sb.AppendLine($"     {SqlString(s.DcsTag)}, {SqlString(s.DcsFunctions)}, {SqlString(s.Note)}){comma}");
                }
            }
            else
            {
                // Создание таблицы
                sb.AppendLine($"CREATE TABLE {tableName} (");
                sb.AppendLine("    Id INT IDENTITY(1,1) PRIMARY KEY,");
                sb.AppendLine("    Number INT,");
                sb.AppendLine("    ProjectDesignation NVARCHAR(100),");
                sb.AppendLine("    PlcTag NVARCHAR(150),");
                sb.AppendLine("    Description NVARCHAR(500),");
                sb.AppendLine("    Unit NVARCHAR(50),");
                sb.AppendLine("    Scale NVARCHAR(50),");
                sb.AppendLine("    LL NVARCHAR(50),");
                sb.AppendLine("    LA NVARCHAR(50),");
                sb.AppendLine("    HA NVARCHAR(50),");
                sb.AppendLine("    HH NVARCHAR(50),");
                sb.AppendLine("    ScalingFactor INT,");
                sb.AppendLine("    SignalType NVARCHAR(50),");
                sb.AppendLine("    RegisterType NVARCHAR(50),");
                sb.AppendLine("    Address INT,");
                sb.AppendLine("    BitNumber INT,");
                sb.AppendLine("    AddressBit NVARCHAR(20),");
                sb.AppendLine("    AccessType NVARCHAR(20),");
                sb.AppendLine("    DataType NVARCHAR(50),");
                sb.AppendLine("    FunctionCode TINYINT,");
                sb.AppendLine("    DcsChannel NVARCHAR(50),");
                sb.AppendLine("    DcsTag NVARCHAR(150),");
                sb.AppendLine("    DcsFunctions NVARCHAR(200),");
                sb.AppendLine("    Note NVARCHAR(500)");
                sb.AppendLine(");");

                sb.AppendLine();
                sb.AppendLine($"-- Данные для вставки");
                foreach (var s in data.GetAllSignals().OrderBy(s => s.FunctionCode).ThenBy(s => s.Address))
                {
                    sb.AppendLine($"INSERT INTO {tableName} (Number, ProjectDesignation, PlcTag, Description, Unit, Scale, LL, LA, HA, HH, ScalingFactor, SignalType, RegisterType, Address, BitNumber, AddressBit, AccessType, DataType, FunctionCode, DcsChannel, DcsTag, DcsFunctions, Note) VALUES");
                    sb.AppendLine($"    ({s.Number}, {SqlString(s.ProjectDesignation)}, {SqlString(s.PlcTag)}, {SqlString(s.Description)}, {SqlString(s.Unit)}, {SqlString(s.Scale)}, {SqlString(s.LL)}, {SqlString(s.LA)}, {SqlString(s.HA)}, {SqlString(s.HH)}, {s.ScalingFactor}, {SqlString(s.SignalType)}, {SqlString(s.RegisterType)}, {s.Address}, {(s.BitNumber.HasValue ? s.BitNumber.Value.ToString() : "NULL")}, {SqlString(s.GetModbusAddress())}, {SqlString(s.AccessType)}, {SqlString(s.DataType)}, {s.FunctionCode}, {SqlString(s.DcsChannel)}, {SqlString(s.DcsTag)}, {SqlString(s.DcsFunctions)}, {SqlString(s.Note)});");
                }
            }

            return sb.ToString();
        }

        private string SqlString(string value)
        {
            if (string.IsNullOrEmpty(value)) return "NULL";
            return $"'{value.Replace("'", "''")}'";
        }

        /// <summary>
        /// Генерация XML для импорта
        /// </summary>
        public string GenerateXml()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine($"<ModbusData System=\"{System.Security.SecurityElement.Escape(data.SystemName)}\" Generated=\"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\">");
            sb.AppendLine($"  <Statistics TotalSignals=\"{data.TotalSignals}\" Interfaces=\"{data.Interfaces.Count}\" />");
            
            sb.AppendLine("  <Interfaces>");
            foreach (var i in data.Interfaces)
            {
                sb.AppendLine($"    <Interface Number=\"{i.Number}\" Type=\"{i.InterfaceType}\" Protocol=\"{i.ProtocolType}\" SlaveStation=\"{i.SlaveStation}\" Speed=\"{i.Speed}\" />");
            }
            sb.AppendLine("  </Interfaces>");

            sb.AppendLine("  <Signals>");
            foreach (var s in data.GetAllSignals().OrderBy(s => s.FunctionCode).ThenBy(s => s.Address))
            {
                sb.AppendLine($"    <Signal Number=\"{s.Number}\" " +
                            $"ProjectDesignation=\"{System.Security.SecurityElement.Escape(s.ProjectDesignation)}\" " +
                            $"PlcTag=\"{System.Security.SecurityElement.Escape(s.PlcTag)}\" " +
                            $"Description=\"{System.Security.SecurityElement.Escape(s.Description)}\" " +
                            $"Unit=\"{s.Unit}\" " +
                            $"FunctionCode=\"{s.FunctionCode:X2}\" " +
                            $"Address=\"{s.Address}\" " +
                            $"Bit=\"{s.BitNumber}\" " +
                            $"AddressBit=\"{s.GetModbusAddress()}\" " +
                            $"DataType=\"{s.DataType}\" " +
                            $"AccessType=\"{s.AccessType}\" " +
                            $"SignalType=\"{s.SignalType}\" " +
                            $"DcsChannel=\"{s.DcsChannel}\" " +
                            $"DcsTag=\"{System.Security.SecurityElement.Escape(s.DcsTag)}\" />");
            }
            sb.AppendLine("  </Signals>");
            sb.AppendLine("</ModbusData>");

            return sb.ToString();
        }

        /// <summary>
        /// Сохранение в файл
        /// </summary>
        public void SaveToFile(string filePath, ExportFormat format = ExportFormat.Csv)
        {
            string content = format switch
            {
                ExportFormat.Csv => GenerateCsv(),
                ExportFormat.Json => GenerateJson(),
                ExportFormat.Sql => GenerateSql(),
                ExportFormat.Xml => GenerateXml(),
                _ => GenerateCsv()
            };

            File.WriteAllText(filePath, content, Encoding.UTF8);
        }

        /// <summary>
        /// Генерация скрипта для чтения Modbus
        /// </summary>
        public string GenerateModbusReadScript()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"// Скрипт для чтения данных Modbus");
            sb.AppendLine($"// Система: {data.SystemName}");
            sb.AppendLine($"// Дата: {DateTime.Now}");
            sb.AppendLine();

            // Группировка по интерфейсам
            var interfaces = data.Interfaces;
            foreach (var iface in interfaces)
            {
                sb.AppendLine($"// Интерфейс: {iface.SlaveStation}");
                sb.AppendLine($"// Тип: {iface.InterfaceType}, Протокол: {iface.ProtocolType}");
                
                var signals = data.GetAllSignals()
                    .Where(s => s.InterfaceDesignation == iface.SlaveStation)
                    .GroupBy(s => s.FunctionCode)
                    .OrderBy(g => g.Key);

                foreach (var group in signals)
                {
                    sb.AppendLine($"//   Функция {group.Key:X2}: {group.Count()} сигналов");
                    var addresses = group.Select(s => s.Address).Distinct().OrderBy(a => a);
                    sb.AppendLine($"//   Адреса: {string.Join(", ", addresses)}");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}