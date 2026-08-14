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
    }
}
