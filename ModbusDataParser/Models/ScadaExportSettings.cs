namespace ModbusDataParser.Models
{
    public class ScadaExportSettings
    {
        public string SubsystemName { get; set; } = "";
        public int ObjectNumber { get; set; } = 0;
        public int ArchivePeriod { get; set; } = 0;
        public int SliceMask { get; set; } = 1;
        public string Classifier { get; set; } = "[ВСЕ]";
        public string EventGroup { get; set; } = "[ВСЕ]";
        public string Controller { get; set; } = "";
        public bool AddInterfaceParameters { get; set; } = true;
        
        // Новое свойство: тип адресации
        public AddressType AddressType { get; set; } = AddressType.Modicon1Based;
    }

    public enum AddressType
    {
        Modicon1Based,  // 1-based (Modicon): 40001, 30001, и т.д.
        Pdu0Based       // 0-based (PDU): 0, 1, 2, ...
    }
}