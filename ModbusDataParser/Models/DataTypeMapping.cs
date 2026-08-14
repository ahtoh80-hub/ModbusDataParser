namespace ModbusDataParser.Models
{
    public class DataTypeMapping
    {
        public string SourceDataType { get; set; } = "";
        public string TargetDataType { get; set; } = "";
        public bool IsMapped { get; set; } = false;
    }

    public class DataTypeMappingSettings
    {
        public List<DataTypeMapping> Mappings { get; set; } = new();

        public string GetScadaType(string sourceDataType)
        {
            var mapping = Mappings.FirstOrDefault(m => m.SourceDataType == sourceDataType && m.IsMapped);
            return mapping?.TargetDataType ?? sourceDataType;
        }
    }

    public static class DataTypeMappingDefaults
    {
        public static List<DataTypeMapping> GetDefaultMappings()
        {
            return new List<DataTypeMapping>
            {
                new DataTypeMapping { SourceDataType = "BOOL", TargetDataType = "BOOL", IsMapped = true },
                new DataTypeMapping { SourceDataType = "8-Bit Signed", TargetDataType = "INT", IsMapped = true },
                new DataTypeMapping { SourceDataType = "8-Bit Unsigned", TargetDataType = "WORD", IsMapped = true },
                new DataTypeMapping { SourceDataType = "16-Bit Signed", TargetDataType = "INT", IsMapped = true },
                new DataTypeMapping { SourceDataType = "16-Bit Unsigned", TargetDataType = "WORD", IsMapped = true },
                new DataTypeMapping { SourceDataType = "32-Bit Signed", TargetDataType = "INT", IsMapped = true },
                new DataTypeMapping { SourceDataType = "32-Bit Unsigned", TargetDataType = "UDINT", IsMapped = true },
                new DataTypeMapping { SourceDataType = "32-Bit Floating", TargetDataType = "REAL", IsMapped = true },
                new DataTypeMapping { SourceDataType = "64-Bit Floating", TargetDataType = "REAL", IsMapped = true }
            };
        }
    }
}
