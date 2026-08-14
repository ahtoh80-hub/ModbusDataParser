using System;

namespace ModbusDataParser.Models
{
    /// <summary>
    /// Базовый класс для сигнала Modbus
    /// </summary>
    public abstract class ModbusSignal
    {
        public int Number { get; set; }
        public string ProjectDesignation { get; set; } = string.Empty;
        public string PlcTag { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string Scale { get; set; } = string.Empty;
        public string LL { get; set; } = string.Empty;
        public string LA { get; set; } = string.Empty;
        public string HA { get; set; } = string.Empty;
        public string HH { get; set; } = string.Empty;
        public int ScalingFactor { get; set; } = 1;
        public string SignalType { get; set; } = string.Empty;
        public string RegisterType { get; set; } = string.Empty;
        public int Address { get; set; }
        public int? BitNumber { get; set; }
        public string AccessType { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public byte FunctionCode { get; set; }
        public string DcsChannel { get; set; } = string.Empty;
        public string DcsTag { get; set; } = string.Empty;
        public string DcsFunctions { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;

        // Связь с интерфейсом
        public string InterfaceDesignation { get; set; } = string.Empty;

        public abstract string GetModbusAddress();

        public virtual string GetFunctionName()
        {
            return FunctionCode switch
            {
                0x01 => "Read Coils",
                0x02 => "Read Discrete Inputs",
                0x03 => "Read Holding Registers",
                0x04 => "Read Input Registers",
                0x05 => "Write Single Coil",
                0x06 => "Write Single Register",
                0x0F => "Write Multiple Coils",
                0x10 => "Write Multiple Registers",
                _ => $"Function {FunctionCode:X2}"
            };
        }

        public string GetDataTypeDisplay()
        {
            return string.IsNullOrEmpty(DataType) ? "Unknown" : DataType;
        }

        public override string ToString()
        {
            return $"[{Number}] {PlcTag} - {Description} (FC: {FunctionCode:X2}, Addr: {GetModbusAddress()})";
        }
    }
}