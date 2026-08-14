using ModbusDataParser.Models;

namespace ModbusDataParser.Services
{
    public class ScadaRowGenerator
    {
        private DataTypeMappingSettings _mappingSettings;

        public ScadaRowGenerator(DataTypeMappingSettings? mappingSettings = null)
        {
            _mappingSettings = mappingSettings ?? new DataTypeMappingSettings();
            InitializeDefaultMappings();
        }

        private void InitializeDefaultMappings()
        {
            if (_mappingSettings.Mappings.Count == 0)
            {
                _mappingSettings.Mappings = DataTypeMappingDefaults.GetDefaultMappings();
            }
        }

        public void UpdateMappingSettings(DataTypeMappingSettings settings)
        {
            _mappingSettings = settings;
        }

        public string GetMappedDataType(string sourceDataType)
        {
            var mapping = _mappingSettings.Mappings
                .FirstOrDefault(m => m.SourceDataType == sourceDataType && m.IsMapped);
            
            if (mapping != null && !string.IsNullOrEmpty(mapping.TargetDataType))
            {
                return mapping.TargetDataType;
            }

            var defaultMapping = DataTypeMappingDefaults.GetDefaultMappings()
                .FirstOrDefault(m => m.SourceDataType == sourceDataType);
            
            return defaultMapping?.TargetDataType ?? "REAL";
        }

        /// <summary>
        /// Формирует полный Modbus адрес по стандарту
        /// </summary>
        private string FormatModbusAddress(int registerType, string? addressBit)
        {
            if (string.IsNullOrEmpty(addressBit))
                return "";

            // Извлекаем адрес без бита (если есть .бит)
            var address = addressBit.Split('.')[0];
            
            // Удаляем ведущие нули для корректного парсинга
            var cleanAddress = address.TrimStart('0');
            if (string.IsNullOrEmpty(cleanAddress))
                cleanAddress = "0";
            
            // Парсим адрес как число
            if (!int.TryParse(cleanAddress, out int addrValue))
                return address;

            // Формируем адрес в зависимости от типа регистра по стандарту Modbus
            string result;
            switch (registerType)
            {
                case 0: // Coils - диапазон 00001-09999
                    result = addrValue.ToString("D5");
                    break;
                case 1: // Discrete Inputs - диапазон 10001-19999
                    result = (10000 + addrValue).ToString("D5");
                    break;
                case 3: // Input Registers - диапазон 30001-39999
                    result = (30000 + addrValue).ToString("D5");
                    break;
                case 4: // Holding Registers - диапазон 40001-49999
                    result = (40000 + addrValue).ToString("D5");
                    break;
                default:
                    result = address;
                    break;
            }

            return result;
        }

        public List<ScadaRow> GenerateRows(IEnumerable<ModbusSignal> signals, ScadaExportSettings settings)
        {
            var rows = new List<ScadaRow>();
            var counter = 1;

            foreach (var signal in signals)
            {
                if (string.IsNullOrEmpty(signal.AddressBit) && string.IsNullOrEmpty(signal.PlcTag))
                    continue;

                var (regType, _, regNumber) = GetRegisterInfo(signal.AddressBit, signal.RegisterType);
                var scadaType = GetMappedDataType(signal.DataType ?? "32-Bit Floating");

                // Полный Modbus адрес для Марки, Наименования и поля "Адрес"
                var fullModbusAddress = FormatModbusAddress(regNumber, signal.AddressBit);

                // Формируем Марку с полным Modbus адресом
                var brand = $"_{settings.SubsystemName}_MB_{regType}_{fullModbusAddress}_{scadaType}";
                
                // Формируем Наименование с полным Modbus адресом
                var name = $"MB_{regType}_{fullModbusAddress}_{scadaType}";

                var row = new ScadaRow
                {
                    Number = counter.ToString(),
                    Status = "-",
                    Mode = "Mode",
                    Brand = brand,
                    ObjectType = scadaType,
                    Name = name,
                    Description = signal.Description ?? "",
                    ObjSign = "",
                    ObjNumber = settings.ObjectNumber.ToString(),
                    PlcVarName = signal.PlcTag ?? "",
                    ArhPer = settings.ArchivePeriod.ToString(),
                    Kks = signal.DcsTag ?? "",
                    ObjDParam = "",
                    SrezControl = settings.SliceMask.ToString(),
                    UserGroup = settings.Classifier,
                    EvGroup = settings.EventGroup,
                    PlcName = settings.Controller,
                    PlcAdress = fullModbusAddress,  // Поле "Адрес" в SCADA
                    PlcGr = regNumber.ToString()
                };

                rows.Add(row);
                counter++;
            }

            return rows;
        }

        private (string Type, string Prefix, int RegisterType) GetRegisterInfo(string? addressBit, int? registerType)
        {
            if (string.IsNullOrEmpty(addressBit)) 
                return ("HR", "4", 4);

            var address = addressBit.Split('.')[0];
            
            if (int.TryParse(address, out int addr))
            {
                if (addr >= 1 && addr <= 9999 && registerType == 0)
                    return ("CO", "0", 0);
                if (addr >= 10001 && addr <= 19999 && registerType == 1)
                    return ("DI", "1", 1);
                if (addr >= 30001 && addr <= 39999 && registerType == 3)
                    return ("IR", "3", 3);
                if (addr >= 40001 && addr <= 49999 && registerType == 4)
                    return ("HR", "4", 4);
            }

            return registerType switch
            {
                0 => ("CO", "0", 0),
                1 => ("DI", "1", 1),
                3 => ("IR", "3", 3),
                4 => ("HR", "4", 4),
                _ => ("HR", "4", 4)
            };
        }
    }
}
