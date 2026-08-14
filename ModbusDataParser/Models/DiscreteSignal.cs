using System;

namespace ModbusDataParser.Models
{
    /// <summary>
    /// Дискретный сигнал (Function 02 - Read Discrete Inputs)
    /// </summary>
    public class DiscreteSignal : ModbusSignal
    {
        public string AddressBit { get; set; } = string.Empty;

        public DiscreteSignal()
        {
            DataType = "BOOL";
            FunctionCode = 0x02;
        }

        public override string GetModbusAddress()
        {
            if (BitNumber.HasValue)
                return $"{Address}.{BitNumber.Value}";
            return Address.ToString();
        }

        /// <summary>
        /// Получение номера регистра и бита
        /// </summary>
        public (int Register, int Bit) GetRegisterAndBit()
        {
            if (BitNumber.HasValue)
            {
                return (Address, BitNumber.Value);
            }
            return (Address, 0);
        }

        /// <summary>
        /// Форматированный адрес для отображения
        /// </summary>
        public string GetFormattedAddress()
        {
            if (BitNumber.HasValue)
                return $"DI {Address}.{BitNumber.Value}";
            return $"DI {Address}";
        }

        /// <summary>
        /// Проверка является ли сигнал статусным (Communication Status)
        /// </summary>
        public bool IsCommunicationStatus()
        {
            return Description?.Contains("Communication Status", StringComparison.OrdinalIgnoreCase) == true ||
                   PlcTag?.Contains("_TX.", StringComparison.OrdinalIgnoreCase) == true;
        }

        /// <summary>
        /// Проверка является ли сигнал статусом разрешения (Permit)
        /// </summary>
        public bool IsPermitStatus()
        {
            return Description?.Contains("Permit", StringComparison.OrdinalIgnoreCase) == true ||
                   PlcTag?.Contains("_PERMIT", StringComparison.OrdinalIgnoreCase) == true;
        }

        /// <summary>
        /// Проверка является ли сигнал статусом ошибки (Fault)
        /// </summary>
        public bool IsFaultStatus()
        {
            return Description?.Contains("Fault", StringComparison.OrdinalIgnoreCase) == true ||
                   PlcTag?.Contains("_FAULT", StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}