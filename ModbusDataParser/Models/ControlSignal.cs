using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ModbusDataParser.Models
{
    /// <summary>
    /// Управляющий сигнал (Function 06 - Write Single Register)
    /// </summary>
    public class ControlSignal : ModbusSignal
    {
        public int? ControlValue { get; set; }

        public ControlSignal()
        {
            FunctionCode = 0x06;
            DataType = "16-Bit Unsigned";
            AccessType = "Write";
        }

        public override string GetModbusAddress()
        {
            return Address.ToString();
        }

        /// <summary>
        /// Парсинг управляющих значений из описания
        /// </summary>
        public Dictionary<string, int> ParseControlValues()
        {
            var values = new Dictionary<string, int>();
            if (string.IsNullOrEmpty(Description))
                return values;

            // Ищем паттерн Value:X=Y или :Value:1=Stop,2=Start и т.д.
            var patterns = new[]
            {
                @"Value[:](\d+)=(\w+(?:\s+\w+)*)",
                @":Value:(\d+)=(\w+(?:\s+\w+)*)"
            };

            foreach (var pattern in patterns)
            {
                var matches = Regex.Matches(Description, pattern);
                foreach (Match match in matches)
                {
                    if (int.TryParse(match.Groups[1].Value, out int value))
                    {
                        string key = match.Groups[2].Value.Trim();
                        if (!values.ContainsKey(key))
                        {
                            values[key] = value;
                        }
                    }
                }
            }

            // Дополнительный парсинг для формата "1=Stop,2=Start"
            if (values.Count == 0)
            {
                var altMatches = Regex.Matches(Description, @"(\d+)=(\w+)");
                foreach (Match match in altMatches)
                {
                    if (int.TryParse(match.Groups[1].Value, out int value))
                    {
                        string key = match.Groups[2].Value.Trim();
                        if (!values.ContainsKey(key))
                        {
                            values[key] = value;
                        }
                    }
                }
            }

            return values;
        }

        /// <summary>
        /// Получение управляющего значения по команде
        /// </summary>
        public int? GetControlValue(string command)
        {
            var values = ParseControlValues();
            if (values.TryGetValue(command, out int value))
                return value;
            
            // Поиск без учета регистра
            foreach (var kvp in values)
            {
                if (kvp.Key.Equals(command, StringComparison.OrdinalIgnoreCase))
                    return kvp.Value;
            }
            
            return null;
        }

        /// <summary>
        /// Форматированный адрес для отображения
        /// </summary>
        public string GetFormattedAddress()
        {
            return $"HR {Address} (Write)";
        }

        /// <summary>
        /// Проверка является ли сигнал управлением пуском
        /// </summary>
        public bool IsStartCommand()
        {
            var values = ParseControlValues();
            return values.ContainsKey("Start") || values.ContainsKey("START");
        }

        /// <summary>
        /// Проверка является ли сигнал управлением остановом
        /// </summary>
        public bool IsStopCommand()
        {
            var values = ParseControlValues();
            return values.ContainsKey("Stop") || values.ContainsKey("STOP");
        }
    }
}