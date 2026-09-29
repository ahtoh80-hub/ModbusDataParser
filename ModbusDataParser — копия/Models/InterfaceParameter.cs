namespace ModbusDataParser.Models
{
    public class InterfaceParameter
    {
        public int Number { get; set; }
        public string? InterfaceType { get; set; }
        public string? ProtocolType { get; set; }
        public string? SlaveStation { get; set; }
        public string? SlaveIdMain { get; set; }
        public string? SlaveIdBackup { get; set; }
        public string? Speed { get; set; }
        public string? ParityBit { get; set; }
        public string? StopBit { get; set; }
        public string? DataBit { get; set; }
        public string? Reverse { get; set; }
        public string? Timeout { get; set; }
        public string? MaximumLength { get; set; }
        public string? Note { get; set; }
        public string? SourceFile { get; set; }
    }
}
