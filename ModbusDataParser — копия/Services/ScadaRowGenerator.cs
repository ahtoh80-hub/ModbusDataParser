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
        /// Формирует полный Modbus адрес с учетом типа адресации
        /// </summary>
        /// <param name="registerType">Тип регистра: 0=CO, 1=DI, 3=IR, 4=HR</param>
        /// <param name="addressBit">Адрес/бит из Excel</param>
        /// <param name="addressType">Тип адресации: Modicon1Based или Pdu0Based</param>
        /// <returns>Полный адрес с ведущими нулями</returns>
        private string FormatModbusAddress(int registerType, string? addressBit, AddressType addressType)
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

            int resultValue;
            
            if (addressType == AddressType.Modicon1Based)
            {
                // 1-based (Modicon): добавляем смещение в зависимости от типа регистра
                switch (registerType)
                {
                    case 0: // Coils - диапазон 00001-09999
                        resultValue = addrValue;
                        break;
                    case 1: // Discrete Inputs - диапазон 10001-19999
                        resultValue = 10000 + addrValue;
                        break;
                    case 3: // Input Registers - диапазон 30001-39999
                        resultValue = 30000 + addrValue;
                        break;
                    case 4: // Holding Registers - диапазон 40001-49999
                        resultValue = 40000 + addrValue;
                        break;
                    default:
                        resultValue = addrValue;
                        break;
                }
            }
            else
            {
                // 0-based (PDU): используем адрес как есть (0-based)
                // Для PDU адресация начинается с 0
                resultValue = addrValue;
            }

            // Форматируем с ведущими нулями до 5 цифр
            return resultValue.ToString("D5");
        }

        /// <summary>
        /// Получает тип регистра для адресации (используется в Марке и Наименовании)
        /// </summary>
        private string GetRegisterTypePrefix(int registerType)
        {
            return registerType switch
            {
                0 => "CO",
                1 => "DI",
                3 => "IR",
                4 => "HR",
                _ => "HR"
            };
        }

        public List<ScadaRow> GenerateRows(IEnumerable<ModbusSignal> signals, ScadaExportSettings settings)
        {
            var rows = new List<ScadaRow>();
            var counter = 1;

            foreach (var signal in signals)
            {
                if (string.IsNullOrEmpty(signal.AddressBit) && string.IsNullOrEmpty(signal.PlcTag))
                    continue;

                var regType = signal.RegisterType ?? 4; // По умолчанию Holding Register
                var scadaType = GetMappedDataType(signal.DataType ?? "32-Bit Floating");
                var regTypePrefix = GetRegisterTypePrefix(regType);

                // Полный Modbus адрес с учетом типа адресации
                var fullModbusAddress = FormatModbusAddress(regType, signal.AddressBit, settings.AddressType);

                // Формируем Марку с полным Modbus адресом
                var brand = $"_{settings.SubsystemName}_MB_{regTypePrefix}_{fullModbusAddress}_{scadaType}";
                
                // Формируем Наименование с полным Modbus адресом
                var name = $"MB_{regTypePrefix}_{fullModbusAddress}_{scadaType}";

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
                    PlcGr = regType.ToString()
                };

                rows.Add(row);
                counter++;
            }

            return rows;
        }
    }
}