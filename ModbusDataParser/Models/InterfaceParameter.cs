using System;
using System.Text.RegularExpressions;

namespace ModbusDataParser.Models
{
    /// <summary>
    /// Параметры интерфейса связи
    /// </summary>
    public class InterfaceParameter
    {
        public int Number { get; set; }
        public string InterfaceType { get; set; } = string.Empty;
        public string ProtocolType { get; set; } = string.Empty;
        public string SlaveStation { get; set; } = string.Empty;
        public string SlaveIdMain { get; set; } = string.Empty;
        public string SlaveIdBackup { get; set; } = string.Empty;
        public string Speed { get; set; } = string.Empty;
        public string ParityBit { get; set; } = string.Empty;
        public string StopBit { get; set; } = string.Empty;
        public string DataBit { get; set; } = string.Empty;
        public string Reverse { get; set; } = string.Empty;
        public string Timeout { get; set; } = string.Empty;
        public string MaximumLength { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;

        /// <summary>
        /// Парсинг адреса TCP/IP
        /// </summary>
        public (string Ip, string Mask, int Port) ParseTcpAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
                return (string.Empty, string.Empty, 0);

            var parts = address.Split('/');
            if (parts.Length >= 3)
            {
                // Формат: IP/маска/порт
                string ip = parts[0].Trim();
                string mask = parts[1].Trim();
                int port = 0;
                string portStr = parts[2].Trim().Replace("port ", "").Replace("Port ", "");
                int.TryParse(portStr, out port);
                return (ip, mask, port);
            }
            return (string.Empty, string.Empty, 0);
        }

        /// <summary>
        /// Парсинг Slave ID для Modbus RTU
        /// </summary>
        public int ParseRtuSlaveId(string address)
        {
            if (string.IsNullOrEmpty(address))
                return 0;
            if (int.TryParse(address, out int id))
                return id;
            return 0;
        }

        /// <summary>
        /// Определение типа интерфейса (TCP или последовательный)
        /// </summary>
        public bool IsTcpInterface()
        {
            return InterfaceType?.ToUpper().Contains("TCP") == true ||
                   ProtocolType?.ToUpper().Contains("TCP") == true;
        }

        /// <summary>
        /// Определение типа протокола
        /// </summary>
        public string GetProtocolFamily()
        {
            if (string.IsNullOrEmpty(ProtocolType))
                return "Unknown";
            
            if (ProtocolType.Contains("TCP", StringComparison.OrdinalIgnoreCase))
                return "TCP";
            if (ProtocolType.Contains("RTU", StringComparison.OrdinalIgnoreCase))
                return "RTU";
            if (ProtocolType.Contains("ASCII", StringComparison.OrdinalIgnoreCase))
                return "ASCII";
            if (ProtocolType.Contains("OPC", StringComparison.OrdinalIgnoreCase))
                return "OPC";
            if (ProtocolType.Contains("МЭК") || ProtocolType.Contains("IEC"))
                return "IEC";
            
            return "Other";
        }

        public override string ToString()
        {
            return $"[{Number}] {InterfaceType} - {ProtocolType} - {SlaveStation}";
        }
    }
}