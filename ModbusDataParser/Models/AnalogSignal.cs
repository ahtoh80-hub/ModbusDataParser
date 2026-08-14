using System;

namespace ModbusDataParser.Models
{
    /// <summary>
    /// Аналоговый сигнал (Function 03 - Read Holding Registers)
    /// </summary>
    public class AnalogSignal : ModbusSignal
    {
        public bool Is32Bit { get; set; } = true;

        public AnalogSignal()
        {
            FunctionCode = 0x03;
        }

        public override string GetModbusAddress()
        {
            return Address.ToString();
        }

        /// <summary>
        /// Получение количества регистров, занимаемых сигналом
        /// </summary>
        public int GetRegisterCount()
        {
            if (DataType == "32-Bit Floating" || DataType == "32-Bit Integer")
                return 2;
            return 1;
        }

        /// <summary>
        /// Форматированный адрес для отображения
        /// </summary>
        public string GetFormattedAddress()
        {
            if (Is32Bit)
                return $"HR {Address}-{Address + 1} (32-bit)";
            return $"HR {Address} (16-bit)";
        }

        /// <summary>
        /// Получение значения в физических единицах
        /// </summary>
        public double ToPhysicalValue(double rawValue)
        {
            if (ScalingFactor == 0) return rawValue;
            return rawValue / ScalingFactor;
        }

        /// <summary>
        /// Получение сырого значения из физического
        /// </summary>
        public double ToRawValue(double physicalValue)
        {
            if (ScalingFactor == 0) return physicalValue;
            return physicalValue * ScalingFactor;
        }

        /// <summary>
        /// Проверка выхода за пределы
        /// </summary>
        public (bool IsOutOfRange, string Limit) CheckLimits(double value)
        {
            if (!string.IsNullOrEmpty(LL) && double.TryParse(LL, out double ll))
            {
                if (value < ll) return (true, $"Ниже LL ({ll})");
            }
            if (!string.IsNullOrEmpty(LA) && double.TryParse(LA, out double la))
            {
                if (value < la) return (true, $"Ниже LA ({la})");
            }
            if (!string.IsNullOrEmpty(HA) && double.TryParse(HA, out double ha))
            {
                if (value > ha) return (true, $"Выше HA ({ha})");
            }
            if (!string.IsNullOrEmpty(HH) && double.TryParse(HH, out double hh))
            {
                if (value > hh) return (true, $"Выше HH ({hh})");
            }
            return (false, string.Empty);
        }
    }
}