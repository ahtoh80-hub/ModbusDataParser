using System;
using System.Collections.Generic;
using System.Linq;

namespace ModbusDataParser.Models
{
    /// <summary>
    /// Контейнер для всех данных Modbus
    /// </summary>
    public class ModbusSystemData
    {
        public string SystemName { get; set; } = string.Empty;
        public List<InterfaceParameter> Interfaces { get; set; } = new();
        public List<DiscreteSignal> DiscreteSignals { get; set; } = new();
        public List<AnalogSignal> AnalogSignals { get; set; } = new();
        public List<ControlSignal> ControlSignals { get; set; } = new();

        // Статистика
        public int TotalSignals => GetAllSignals().Count();
        public int TotalInterfaces => Interfaces.Count;

        /// <summary>
        /// Общий список всех сигналов
        /// </summary>
        public IEnumerable<ModbusSignal> GetAllSignals()
        {
            foreach (var s in DiscreteSignals)
                yield return s;
            foreach (var s in AnalogSignals)
                yield return s;
            foreach (var s in ControlSignals)
                yield return s;
        }

        /// <summary>
        /// Получение сигналов по функциональному обозначению
        /// </summary>
        public IEnumerable<ModbusSignal> GetSignalsByDesignation(string designation)
        {
            return GetAllSignals().Where(s => s.ProjectDesignation == designation);
        }

        /// <summary>
        /// Получение сигналов по тегу ПЛК
        /// </summary>
        public IEnumerable<ModbusSignal> GetSignalsByPlcTag(string tag)
        {
            return GetAllSignals().Where(s => s.PlcTag == tag);
        }

        /// <summary>
        /// Получение сигналов по коду функции
        /// </summary>
        public IEnumerable<ModbusSignal> GetSignalsByFunction(byte functionCode)
        {
            return GetAllSignals().Where(s => s.FunctionCode == functionCode);
        }

        /// <summary>
        /// Получение интерфейса по функциональному обозначению
        /// </summary>
        public InterfaceParameter? GetInterfaceByDesignation(string designation)
        {
            return Interfaces.FirstOrDefault(i => i.SlaveStation == designation);
        }

        /// <summary>
        /// Группировка сигналов по интерфейсам
        /// </summary>
        public Dictionary<string, List<ModbusSignal>> GroupByInterface()
        {
            var result = new Dictionary<string, List<ModbusSignal>>();
            foreach (var signal in GetAllSignals())
            {
                var key = signal.InterfaceDesignation ?? signal.ProjectDesignation;
                if (!result.ContainsKey(key))
                    result[key] = new List<ModbusSignal>();
                result[key].Add(signal);
            }
            return result;
        }

        /// <summary>
        /// Группировка сигналов по коду функции
        /// </summary>
        public Dictionary<byte, List<ModbusSignal>> GroupByFunction()
        {
            return GetAllSignals()
                .GroupBy(s => s.FunctionCode)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        /// <summary>
        /// Получение статистики по типам сигналов
        /// </summary>
        public Dictionary<string, int> GetSignalTypeStatistics()
        {
            return new Dictionary<string, int>
            {
                ["Discrete (FC 02)"] = DiscreteSignals.Count,
                ["Analog (FC 03)"] = AnalogSignals.Count,
                ["Control (FC 06)"] = ControlSignals.Count,
                ["Total"] = TotalSignals
            };
        }

        /// <summary>
        /// Поиск дубликатов адресов
        /// </summary>
        public IEnumerable<IGrouping<(int Address, byte FunctionCode), ModbusSignal>> FindDuplicateAddresses()
        {
            return GetAllSignals()
                .GroupBy(s => (s.Address, s.FunctionCode))
                .Where(g => g.Count() > 1);
        }

        /// <summary>
        /// Экспорт в DataTable для отображения
        /// </summary>
        public System.Data.DataTable ToDataTable()
        {
            var table = new System.Data.DataTable();
            table.Columns.Add("Number", typeof(int));
            table.Columns.Add("ProjectDesignation", typeof(string));
            table.Columns.Add("PlcTag", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("Unit", typeof(string));
            table.Columns.Add("FunctionCode", typeof(byte));
            table.Columns.Add("Address", typeof(int));
            table.Columns.Add("BitNumber", typeof(int));
            table.Columns.Add("AddressBit", typeof(string));
            table.Columns.Add("DataType", typeof(string));
            table.Columns.Add("AccessType", typeof(string));
            table.Columns.Add("SignalType", typeof(string));
            table.Columns.Add("DcsChannel", typeof(string));
            table.Columns.Add("DcsTag", typeof(string));
            table.Columns.Add("Note", typeof(string));

            foreach (var signal in GetAllSignals().OrderBy(s => s.FunctionCode).ThenBy(s => s.Address))
            {
                var row = table.NewRow();
                row["Number"] = signal.Number;
                row["ProjectDesignation"] = signal.ProjectDesignation;
                row["PlcTag"] = signal.PlcTag;
                row["Description"] = signal.Description;
                row["Unit"] = signal.Unit;
                row["FunctionCode"] = signal.FunctionCode;
                row["Address"] = signal.Address;
                row["BitNumber"] = signal.BitNumber ?? 0;
                row["AddressBit"] = signal.GetModbusAddress();
                row["DataType"] = signal.DataType;
                row["AccessType"] = signal.AccessType;
                row["SignalType"] = signal.SignalType;
                row["DcsChannel"] = signal.DcsChannel;
                row["DcsTag"] = signal.DcsTag;
                row["Note"] = signal.Note;
                table.Rows.Add(row);
            }

            return table;
        }
    }
}