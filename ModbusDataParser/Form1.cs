using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ModbusDataParser.Generators;
using ModbusDataParser.Helpers;
using ModbusDataParser.Models;
using ModbusDataParser.Parsers;
using ModbusDataParser.Views;

namespace ModbusDataParser
{
    public partial class Form1 : Form
    {
        private ModbusSystemData? modbusData;
        private ModbusDataViewManager? dataViewManager;
        private ImportDataGenerator? importGenerator;

        public Form1()
        {
            InitializeComponent(); // ЭТОТ ВЫЗОВ ОБЯЗАТЕЛЕН!
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Инициализация лога
            LogHelper.Initialize(rtbLog);

            // Инициализация менеджера представления
            if (dgvReplacements != null)
            {
                dataViewManager = new ModbusDataViewManager(dgvReplacements);
            }

            LogHelper.Log("Программа запущена", LogLevel.Info);
            LogHelper.Log("Готов к работе", LogLevel.Success);
            UpdateStatus("Готов к работе");
        }

        #region Event Handlers

        // Обработчик загрузки Excel файла
        private async void btnLoadExcel_Click(object sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            openFileDialog.Title = "Выберите файл Register Table Modbus";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    LogHelper.Log($"Загрузка файла: {openFileDialog.FileName}", LogLevel.Info);
                    UpdateStatus("Загрузка данных...");

                    var parser = new ModbusExcelParser();
                    modbusData = await Task.Run(() => parser.ParseFile(openFileDialog.FileName));

                    if (dataViewManager != null && modbusData != null)
                    {
                        dataViewManager.SetData(modbusData);
                    }

                    importGenerator = modbusData != null ? new ImportDataGenerator(modbusData) : null;

                    // Обновление информации
                    if (lblExcelInfo != null)
                    {
                        lblExcelInfo.Text = $"Excel: {Path.GetFileName(openFileDialog.FileName)}";
                    }

                    if (modbusData != null)
                    {
                        var totalSignals = modbusData.TotalSignals;
                        if (lblActiveReplacements != null)
                        {
                            lblActiveReplacements.Text = $"Найдено: {totalSignals}";
                        }

                        LogHelper.Log($"Загружено интерфейсов: {modbusData.Interfaces.Count}", LogLevel.Info);
                        LogHelper.Log($"Дискретных сигналов (FC 02): {modbusData.DiscreteSignals.Count}", LogLevel.Info);
                        LogHelper.Log($"Аналоговых сигналов (FC 03): {modbusData.AnalogSignals.Count}", LogLevel.Info);
                        LogHelper.Log($"Управляющих сигналов (FC 06): {modbusData.ControlSignals.Count}", LogLevel.Info);
                        LogHelper.Log($"Всего сигналов: {totalSignals}", LogLevel.Success);

                        UpdateStatus($"Загружено {totalSignals} сигналов");
                        
                        // Отображение информации в MappingInfo
                        ShowSummaryInfo(modbusData);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Log($"ОШИБКА: {ex.Message}", LogLevel.Error);
                    UpdateStatus("Ошибка загрузки");
                    MessageBox.Show($"Ошибка загрузки файла:\n{ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Обработчик генерации
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (modbusData == null || importGenerator == null)
            {
                MessageBox.Show("Сначала загрузите данные из Excel файла.", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv|JSON files (*.json)|*.json|SQL files (*.sql)|*.sql|XML files (*.xml)|*.xml|All files (*.*)|*.*";
            saveFileDialog.Title = "Сохранить данные для импорта";
            saveFileDialog.FileName = $"{modbusData.SystemName}_signals";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    UpdateStatus("Генерация данных...");

                    var extension = Path.GetExtension(saveFileDialog.FileName).ToLower();
                    ExportFormat format = extension switch
                    {
                        ".json" => ExportFormat.Json,
                        ".sql" => ExportFormat.Sql,
                        ".xml" => ExportFormat.Xml,
                        _ => ExportFormat.Csv
                    };

                    importGenerator.SaveToFile(saveFileDialog.FileName, format);

                    LogHelper.Log($"Данные сохранены: {saveFileDialog.FileName}", LogLevel.Success);
                    UpdateStatus($"Данные сохранены");

                    MessageBox.Show($"Данные успешно сохранены в:\n{saveFileDialog.FileName}",
                        "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    LogHelper.Log($"ОШИБКА при сохранении: {ex.Message}", LogLevel.Error);
                    UpdateStatus("Ошибка сохранения");
                    MessageBox.Show($"Ошибка сохранения:\n{ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Обработчик очистки
        private void btnClearAll_Click(object sender, EventArgs e)
        {
            modbusData = null;
            importGenerator = null;

            if (dataViewManager != null)
            {
                dataViewManager.Clear();
            }

            if (lblExcelInfo != null) lblExcelInfo.Text = "Excel: -";
            if (lblActiveReplacements != null) lblActiveReplacements.Text = "Найдено: 0";
            if (txtTemplatePreview != null) txtTemplatePreview.Clear();
            if (txtMappingInfo != null) txtMappingInfo.Clear();

            LogHelper.Log("Все данные очищены", LogLevel.Info);
            UpdateStatus("Готов к работе");
        }

        // Обработчик для кнопки проверки
        private void btnValidate_Click(object sender, EventArgs e)
        {
            if (modbusData == null)
            {
                MessageBox.Show("Нет данных для проверки. Загрузите файл Excel.",
                    "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var total = modbusData.TotalSignals;
            var duplicates = modbusData.FindDuplicateAddresses().ToList();
            var warnings = new System.Collections.Generic.List<string>();

            // Проверка на дубликаты адресов
            if (duplicates.Any())
            {
                warnings.Add($"Найдено {duplicates.Count} дубликатов адресов:");
                foreach (var dup in duplicates.Take(5))
                {
                    var first = dup.First();
                    warnings.Add($"  - {first.ProjectDesignation}: Адрес {first.Address}, Функция {first.FunctionCode:X2} ({dup.Count()} сигналов)");
                }
                if (duplicates.Count > 5)
                    warnings.Add($"  ... и еще {duplicates.Count - 5}");
            }

            // Проверка на отсутствие описания
            var noDescription = modbusData.GetAllSignals()
                .Where(s => string.IsNullOrEmpty(s.Description))
                .Count();
            if (noDescription > 0)
            {
                warnings.Add($"Найдено {noDescription} сигналов без описания");
            }

            // Проверка на отсутствие тега
            var noTag = modbusData.GetAllSignals()
                .Where(s => string.IsNullOrEmpty(s.PlcTag))
                .Count();
            if (noTag > 0)
            {
                warnings.Add($"Найдено {noTag} сигналов без тега ПЛК");
            }

            // Проверка на отсутствие единиц измерения для аналоговых
            var noUnit = modbusData.AnalogSignals
                .Where(s => string.IsNullOrEmpty(s.Unit))
                .Count();
            if (noUnit > 0)
            {
                warnings.Add($"Найдено {noUnit} аналоговых сигналов без единиц измерения");
            }

            var message = $"Проверка данных:\n\n" +
                         $"Всего сигналов: {total}\n" +
                         $"Дискретных (FC 02): {modbusData.DiscreteSignals.Count}\n" +
                         $"Аналоговых (FC 03): {modbusData.AnalogSignals.Count}\n" +
                         $"Управляющих (FC 06): {modbusData.ControlSignals.Count}\n" +
                         $"Интерфейсов: {modbusData.Interfaces.Count}\n";

            if (warnings.Any())
            {
                message += $"\n⚠️ Предупреждения ({warnings.Count}):\n" + string.Join("\n", warnings);
            }
            else
            {
                message += "\n✅ Все сигналы корректны";
            }

            MessageBox.Show(message, "Результаты проверки",
                MessageBoxButtons.OK,
                warnings.Any() ? MessageBoxIcon.Warning : MessageBoxIcon.Information);

            LogHelper.Log($"Проверка завершена: {total} сигналов, {warnings.Count} предупреждений", 
                warnings.Any() ? LogLevel.Warning : LogLevel.Success);
        }

        // Обработчик для кнопки преобразования
        private void btnConvertTags_Click(object sender, EventArgs e)
        {
            if (modbusData == null)
            {
                MessageBox.Show("Нет данных для преобразования. Загрузите файл Excel.",
                    "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Переключение состояния кнопки
            bool isEnabled = btnConvertTags.BackColor == System.Drawing.Color.FromArgb(46, 204, 113);
            btnConvertTags.BackColor = isEnabled 
                ? System.Drawing.Color.FromArgb(231, 76, 60) 
                : System.Drawing.Color.FromArgb(46, 204, 113);

            btnConvertTags.Text = isEnabled 
                ? "Преобразование Выкл" 
                : "Преобразование Вкл";

            if (isEnabled)
            {
                LogHelper.Log("Преобразование тегов выключено", LogLevel.Info);
            }
            else
            {
                LogHelper.Log("Преобразование тегов включено", LogLevel.Info);
                ConvertTags();
            }
        }

        // Обработчик загрузки шаблона
        private void btnLoadTemplate_Click(object sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.Title = "Выберите файл шаблона";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string content = File.ReadAllText(openFileDialog.FileName);
                    if (txtTemplatePreview != null)
                    {
                        txtTemplatePreview.Text = content;
                    }
                    if (lblTemplateInfo != null)
                    {
                        lblTemplateInfo.Text = $"Шаблон: {Path.GetFileName(openFileDialog.FileName)}";
                    }
                    LogHelper.Log($"Загружен шаблон: {openFileDialog.FileName}", LogLevel.Success);
                }
                catch (Exception ex)
                {
                    LogHelper.Log($"Ошибка загрузки шаблона: {ex.Message}", LogLevel.Error);
                    MessageBox.Show($"Ошибка загрузки шаблона:\n{ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Обработчик изменения ячейки в DataGridView
        private void dgvReplacements_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Можно добавить логику обработки изменений
        }

        private void dgvReplacements_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvReplacements != null && dgvReplacements.IsCurrentCellDirty)
            {
                dgvReplacements.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        #endregion

        #region Helper Methods

        private void UpdateStatus(string status)
        {
            Text = $"Modbus Data Parser - {status}";
        }

        private void ShowSummaryInfo(ModbusSystemData data)
        {
            if (txtMappingInfo == null) return;

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== ИНФОРМАЦИЯ О СИСТЕМЕ ===");
            sb.AppendLine($"Система: {data.SystemName}");
            sb.AppendLine($"Дата загрузки: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine();

            sb.AppendLine("=== ИНТЕРФЕЙСЫ ===");
            foreach (var iface in data.Interfaces)
            {
                sb.AppendLine($"  [{iface.Number}] {iface.InterfaceType} - {iface.ProtocolType}");
                sb.AppendLine($"      Slave: {iface.SlaveStation}");
                sb.AppendLine($"      Main ID: {iface.SlaveIdMain}");
                if (!string.IsNullOrEmpty(iface.SlaveIdBackup))
                    sb.AppendLine($"      Backup ID: {iface.SlaveIdBackup}");
                sb.AppendLine($"      Speed: {iface.Speed}, Parity: {iface.ParityBit}");
                sb.AppendLine($"      Timeout: {iface.Timeout}");
            }
            sb.AppendLine();

            sb.AppendLine("=== СТАТИСТИКА СИГНАЛОВ ===");
            var stats = data.GetSignalTypeStatistics();
            foreach (var kvp in stats)
            {
                sb.AppendLine($"  {kvp.Key}: {kvp.Value}");
            }
            sb.AppendLine();

            sb.AppendLine("=== ГРУППИРОВКА ПО ФУНКЦИЯМ ===");
            var groups = data.GroupByFunction();
            foreach (var group in groups.OrderBy(g => g.Key))
            {
                string functionName = group.Key switch
                {
                    0x02 => "Discrete Inputs",
                    0x03 => "Holding Registers",
                    0x06 => "Single Register Write",
                    _ => $"Function {group.Key:X2}"
                };
                sb.AppendLine($"  FC {group.Key:X2} ({functionName}): {group.Value.Count} сигналов");
                
                var addresses = group.Value.Select(s => s.Address).Distinct().OrderBy(a => a).ToList();
                if (addresses.Count <= 10)
                {
                    sb.AppendLine($"      Адреса: {string.Join(", ", addresses)}");
                }
                else
                {
                    sb.AppendLine($"      Адреса: {addresses.First()} ... {addresses.Last()} (всего {addresses.Count})");
                }
            }

            txtMappingInfo.Text = sb.ToString();
        }

        private void ConvertTags()
        {
            LogHelper.Log("Начало преобразования тегов...", LogLevel.Info);
            
            int count = 0;
            foreach (var signal in modbusData!.GetAllSignals())
            {
                if (!string.IsNullOrEmpty(signal.PlcTag))
                {
                    var newTag = signal.PlcTag.Replace('_', '.');
                    if (newTag != signal.PlcTag)
                    {
                        signal.PlcTag = newTag;
                        count++;
                    }
                }
            }

            if (dataViewManager != null)
            {
                dataViewManager.SetData(modbusData);
            }

            LogHelper.Log($"Преобразовано {count} тегов", LogLevel.Success);
        }

        #endregion
    }
}