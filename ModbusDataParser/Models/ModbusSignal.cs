namespace ModbusDataParser.Models
{
    public class ModbusSignal
    {
        public int Number { get; set; }
        public string? ProjectFunctionalDesignation { get; set; }
        public string? PlcTag { get; set; }
        public string? Description { get; set; }
        public string? Unit { get; set; }
        public string? Scale { get; set; }
        public double? LL { get; set; }
        public double? LA { get; set; }
        public double? HA { get; set; }
        public double? HH { get; set; }
        public string? ScalingFactors { get; set; }
        public string? SignalType { get; set; }
        public int? RegisterType { get; set; }
        public string? AddressBit { get; set; }
        public string? AccessType { get; set; }
        public string? DataType { get; set; }
        public int? FunctionCode { get; set; }
        public string? DcsChannel { get; set; }
        public string? DcsTag { get; set; }
        public string? DcsFunctions { get; set; }
        public string? Note { get; set; }
        public string? SheetName { get; set; }
        public string? SourceFile { get; set; }
        public string? SlaveStation { get; set; }
        public string? SlaveIdMain { get; set; }
        public string? InterfaceType { get; set; }
        public string? ProtocolType { get; set; }
        public string? Speed { get; set; }
        public string? ParityBit { get; set; }
        public string? StopBit { get; set; }
        public string? DataBit { get; set; }
        public string? Reverse { get; set; }
        public string? Timeout { get; set; }
        public string? MaximumLength { get; set; }
    }
}
