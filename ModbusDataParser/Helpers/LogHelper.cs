using System;
using System.IO;
using System.Windows.Forms;

namespace ModbusDataParser.Helpers
{
    /// <summary>
    /// Вспомогательный класс для логирования
    /// </summary>
    public static class LogHelper
    {
        private static RichTextBox? logControl;
        private static string? logFile;

        public static void Initialize(RichTextBox control, string? filePath = null)
        {
            logControl = control;
            logFile = filePath;
        }

        public static void Log(string message, LogLevel level = LogLevel.Info)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string prefix = level switch
            {
                LogLevel.Info => "[INFO]",
                LogLevel.Warning => "[WARN]",
                LogLevel.Error => "[ERROR]",
                LogLevel.Success => "[OK]",
                LogLevel.Debug => "[DEBUG]",
                _ => "[INFO]"
            };

            string formattedMessage = $"[{timestamp}] {prefix} {message}";

            // Вывод в RichTextBox
            if (logControl != null)
            {
                if (logControl.InvokeRequired)
                {
                    logControl.Invoke(new Action(() => AppendToLog(logControl, formattedMessage, level)));
                }
                else
                {
                    AppendToLog(logControl, formattedMessage, level);
                }
            }

            // Запись в файл
            if (!string.IsNullOrEmpty(logFile))
            {
                try
                {
                    File.AppendAllText(logFile, formattedMessage + Environment.NewLine);
                }
                catch { /* Игнорируем ошибки записи в файл */ }
            }

            // Вывод в консоль для отладки
            System.Diagnostics.Debug.WriteLine(formattedMessage);
        }

        private static void AppendToLog(RichTextBox control, string message, LogLevel level)
        {
            var color = level switch
            {
                LogLevel.Error => System.Drawing.Color.Red,
                LogLevel.Warning => System.Drawing.Color.Orange,
                LogLevel.Success => System.Drawing.Color.LightGreen,
                LogLevel.Debug => System.Drawing.Color.Gray,
                _ => System.Drawing.Color.White
            };

            control.SelectionColor = color;
            control.AppendText(message + Environment.NewLine);
            control.ScrollToCaret();
        }

        public static void Clear()
        {
            if (logControl != null && logControl.InvokeRequired)
            {
                logControl.Invoke(new Action(() => logControl.Clear()));
            }
            else if (logControl != null)
            {
                logControl.Clear();
            }
        }
    }

    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Success,
        Debug
    }
}