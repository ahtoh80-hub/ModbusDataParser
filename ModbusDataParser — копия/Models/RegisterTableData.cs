namespace ModbusDataParser.Models
{
    public class RegisterTableData
    {
        public string? FileName { get; set; }
        public List<InterfaceParameter> InterfaceParameters { get; set; } = new();
        public List<ModbusSignal> Signals { get; set; } = new();
        public Dictionary<string, List<ModbusSignal>> SignalsBySheet { get; set; } = new();
    }
}
