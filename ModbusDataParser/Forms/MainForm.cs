using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ModbusDataParser.Models;
using ModbusDataParser.Services;

namespace ModbusDataParser.Forms
{
    public partial class MainForm : Form
    {
        private readonly ModbusDataService _dataService = new();
        private DataGridView _dataGridView = new();
        private ComboBox _fileSelector = new();
        private ComboBox _sheetSelector = new();
        private Button _loadButton = new();
        private Button _exportCsvButton = new();
        private Button _exportAddressMapButton = new();
        private Button _exportScadaButton = new();
        private Button _btnDataTypeMapping = new();
        private Label _statusLabel = new();
        private TextBox _searchBox = new();
        private Button _searchButton = new();
        private NumericUpDown _filterFunctionCode = new();
        private CheckBox _filterReadOnly = new();
        private DataTypeMappingSettings _dataTypeMappingSettings = new();

        public MainForm()
        {
            InitializeComponent();
            _dataService.DataChanged += OnDataChanged;
        }

        private void InitializeComponent()
        {
            this.Text = "Modbus Data Parser";
            this.Size = new Size(1600, 950);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1200, 650);

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(10)
            };
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 135));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            this.Controls.Add(mainPanel);

            var topPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(3)
            };
            topPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            topPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            // Первая строка
            var row1 = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(3)
            };

            _loadButton = new Button
            {
                Text = "📂 Load File(s)",
                Size = new Size(120, 32),
                UseVisualStyleBackColor = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            _loadButton.Click += LoadButton_Click;

            _fileSelector = new ComboBox
            {
                Size = new Size(180, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false,
                Font = new Font("Segoe UI", 9)
            };
            _fileSelector.SelectedIndexChanged += FileSelector_SelectedIndexChanged;

            _sheetSelector = new ComboBox
            {
                Size = new Size(160, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false,
                Font = new Font("Segoe UI", 9)
            };
            _sheetSelector.SelectedIndexChanged += SheetSelector_SelectedIndexChanged;

            _searchBox = new TextBox
            {
                Size = new Size(180, 28),
                Font = new Font("Segoe UI", 9),
                PlaceholderText = "🔍 Search..."
            };
            _searchBox.KeyPress += SearchBox_KeyPress;

            _searchButton = new Button
            {
                Text = "Search",
                Size = new Size(70, 32),
                UseVisualStyleBackColor = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            _searchButton.Click += SearchButton_Click;

            row1.Controls.Add(_loadButton);
            row1.Controls.Add(_fileSelector);
            row1.Controls.Add(_sheetSelector);
            row1.Controls.Add(_searchBox);
            row1.Controls.Add(_searchButton);

            // Вторая строка
            var row2 = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(3)
            };

            var fcLabel = new Label
            {
                Text = "FC:",
                Size = new Size(25, 28),
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9)
            };

            _filterFunctionCode = new NumericUpDown
            {
                Size = new Size(60, 28),
                Minimum = 0,
                Maximum = 255,
                Value = 0,
                Enabled = false,
                Font = new Font("Segoe UI", 9)
            };

            _filterReadOnly = new CheckBox
            {
                Text = "Read Only",
                Size = new Size(85, 28),
                Enabled = false,
                Font = new Font("Segoe UI", 9)
            };
            _filterReadOnly.CheckedChanged += FilterReadOnly_CheckedChanged;

            _btnDataTypeMapping = new Button
            {
                Text = "⚙ Data Type Map",
                Size = new Size(130, 32),
                UseVisualStyleBackColor = true,
                Enabled = false,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            _btnDataTypeMapping.Click += BtnDataTypeMapping_Click;

            _exportCsvButton = new Button
            {
                Text = "📊 Export CSV",
                Size = new Size(110, 32),
                UseVisualStyleBackColor = true,
                Enabled = false,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.LightBlue
            };
            _exportCsvButton.Click += ExportCsvButton_Click;

            _exportAddressMapButton = new Button
            {
                Text = "📍 Address Map",
                Size = new Size(120, 32),
                UseVisualStyleBackColor = true,
                Enabled = false,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.LightYellow
            };
            _exportAddressMapButton.Click += ExportAddressMapButton_Click;

            _exportScadaButton = new Button
            {
                Text = "📋 Export SCADA",
                Size = new Size(125, 32),
                UseVisualStyleBackColor = true,
                Enabled = false,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.LightGreen
            };
            _exportScadaButton.Click += ExportScadaButton_Click;

            row2.Controls.Add(fcLabel);
            row2.Controls.Add(_filterFunctionCode);
            row2.Controls.Add(_filterReadOnly);
            row2.Controls.Add(_btnDataTypeMapping);
            row2.Controls.Add(_exportCsvButton);
            row2.Controls.Add(_exportAddressMapButton);
            row2.Controls.Add(_exportScadaButton);

            topPanel.Controls.Add(row1, 0, 0);
            topPanel.Controls.Add(row2, 0, 1);

            mainPanel.Controls.Add(topPanel, 0, 0);

            _dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = true,
                BackgroundColor = Color.White
            };
            mainPanel.Controls.Add(_dataGridView, 0, 1);

            _statusLabel = new Label
            {
                Dock = DockStyle.Fill,
                Text = "Ready",
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(240, 240, 240)
            };
            mainPanel.Controls.Add(_statusLabel, 0, 2);
        }

        // ============ ОБРАБОТЧИКИ СОБЫТИЙ ============

        private void LoadButton_Click(object? sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Title = "Select Modbus Register Excel Files",
                Filter = "Excel files (*.xlsx;*.xls)|*.xlsx;*.xls|All files (*.*)|*.*",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _dataService.LoadFiles(openFileDialog.FileNames);
                    UpdateFileSelector();
                    UpdateSheetSelector();
                    UpdateGrid();
                    UpdateControls();
                    _statusLabel.Text = $"Loaded {openFileDialog.FileNames.Length} file(s), {_dataService.GetAllSignals().Count} signals total";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading files: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void FileSelector_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_fileSelector.SelectedItem != null)
            {
                var fileName = _fileSelector.SelectedItem.ToString();
                _dataService.SetCurrentFile(fileName!);
                UpdateSheetSelector();
                UpdateGrid();
            }
        }

        private void SheetSelector_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateGrid();
        }

        private void SearchBox_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                UpdateGrid();
            }
        }

        private void SearchButton_Click(object? sender, EventArgs e)
        {
            UpdateGrid();
        }

        private void FilterReadOnly_CheckedChanged(object? sender, EventArgs e)
        {
            UpdateGrid();
        }

        // ============================================================
        // ОБНОВЛЕННЫЙ МЕТОД - Собирает Source Types из данных и передает в форму
        // ============================================================
        private void BtnDataTypeMapping_Click(object? sender, EventArgs e)
        {
            // Собираем все уникальные Source Data Types из загруженных сигналов
            var allSignals = _dataService.GetAllSignals();
            var sourceTypes = allSignals
                .Where(s => !string.IsNullOrEmpty(s.DataType))
                .Select(s => s.DataType!)
                .Distinct()
                .ToList();

            // Если нет данных, показываем сообщение
            if (!sourceTypes.Any())
            {
                MessageBox.Show("No data types found. Please load Excel files first.", 
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var form = new DataTypeMappingForm(_dataTypeMappingSettings, sourceTypes);
            if (form.ShowDialog() == DialogResult.OK)
            {
                _dataTypeMappingSettings = form.Settings;
                _statusLabel.Text = "Data type mapping updated";
                UpdateGrid();
            }
        }

        private void ExportCsvButton_Click(object? sender, EventArgs e)
        {
            ExportData(_dataService.GenerateImportData, "CSV files (*.csv)|*.csv", "export");
        }

        private void ExportAddressMapButton_Click(object? sender, EventArgs e)
        {
            ExportData(_dataService.GenerateAddressMap, "CSV files (*.csv)|*.csv", "address_map");
        }

        private void ExportScadaButton_Click(object? sender, EventArgs e)
        {
            var signals = GetCurrentSignals();
            if (!signals.Any())
            {
                MessageBox.Show("No signals to export", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var settingsForm = new ScadaExportSettingsForm();
            if (settingsForm.ShowDialog() != DialogResult.OK)
                return;

            var settings = settingsForm.Settings;

            using var saveFileDialog = new SaveFileDialog
            {
                Title = "Export SCADA Template",
                Filter = "Excel files (*.xlsx)|*.xlsx",
                FileName = $"SCADA_{settings.SubsystemName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                var generator = new ScadaRowGenerator(_dataTypeMappingSettings);
                var rows = generator.GenerateRows(signals, settings);

                var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Шаблон Modbus SCADA.xls");
                var exporter = new ScadaExcelExporter(templatePath);

                if (File.Exists(templatePath))
                {
                    try
                    {
                        exporter.LoadHeaderFromTemplate();
                    }
                    catch
                    {
                        // Если не удалось загрузить шаблон, используем стандартную шапку
                    }
                }

                exporter.ExportToExcel(rows, saveFileDialog.FileName);
                _statusLabel.Text = $"Exported to {saveFileDialog.FileName}";

                MessageBox.Show($"Export completed successfully!\nFile: {saveFileDialog.FileName}",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============ ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ============

        private void OnDataChanged(object? sender, EventArgs e)
        {
            UpdateControls();
            UpdateGrid();
        }

        private void ExportData(Func<IEnumerable<ModbusSignal>, string> generator, string filter, string defaultName)
        {
            var signals = GetCurrentSignals();
            if (!signals.Any())
            {
                MessageBox.Show("No signals to export", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var saveFileDialog = new SaveFileDialog
            {
                Title = "Export Data",
                Filter = filter,
                FileName = $"{defaultName}_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var content = generator(signals);
                    File.WriteAllText(saveFileDialog.FileName, content);
                    _statusLabel.Text = $"Exported to {saveFileDialog.FileName}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateFileSelector()
        {
            _fileSelector.Items.Clear();
            var files = _dataService.LoadedFiles;
            if (files.Count > 0)
            {
                _fileSelector.Enabled = true;
                foreach (var file in files)
                {
                    _fileSelector.Items.Add(file);
                }
                _fileSelector.SelectedIndex = 0;
            }
            else
            {
                _fileSelector.Enabled = false;
            }
        }

        private void UpdateSheetSelector()
        {
            if (_dataService.CurrentData == null)
            {
                _sheetSelector.Items.Clear();
                _sheetSelector.Enabled = false;
                return;
            }

            _sheetSelector.Items.Clear();
            _sheetSelector.Enabled = true;
            _sheetSelector.Items.Add("All Sheets");

            foreach (var sheetName in _dataService.CurrentData.SignalsBySheet.Keys)
            {
                _sheetSelector.Items.Add(sheetName);
            }

            _sheetSelector.SelectedIndex = 0;
        }

        private void UpdateControls()
        {
            bool hasSignals = _dataService.CurrentData?.Signals.Count > 0;
            _exportCsvButton.Enabled = hasSignals;
            _exportAddressMapButton.Enabled = hasSignals;
            _exportScadaButton.Enabled = hasSignals;
            _btnDataTypeMapping.Enabled = hasSignals;
            _filterFunctionCode.Enabled = hasSignals;
            _filterReadOnly.Enabled = hasSignals;
        }

        private void UpdateGrid()
        {
            if (_dataGridView == null) return;

            var signals = GetCurrentSignals();
            var filteredSignals = ApplyFilters(signals);

            _dataGridView.DataSource = null;
            _dataGridView.Columns.Clear();

            if (!filteredSignals.Any())
            {
                _dataGridView.Rows.Clear();
                _dataGridView.Columns.Add("NoData", "No signals found");
                _dataGridView.Rows.Add("No signals found in the selected sheet");
                return;
            }

            var columns = new[]
            {
                "Number", "Project Functional Designation", "PLC Tag", "Description",
                "Unit", "Scale", "LL", "LA", "HA", "HH",
                "Scaling Factors", "Signal Type", "Register Type", "Address/Bit",
                "Access Type", "Data Type", "SCADA Type", "Function Code",
                "DCS Channel", "DCS Tag", "DCS Functions", "Note", "Sheet", "KKS"
            };

            foreach (var col in columns)
            {
                _dataGridView.Columns.Add(col, col);
            }

            var generator = new ScadaRowGenerator(_dataTypeMappingSettings);

            foreach (var signal in filteredSignals)
            {
                var scadaType = generator.GetMappedDataType(signal.DataType ?? "32-Bit Floating");
                
                var row = new object?[]
                {
                    signal.Number,
                    signal.ProjectFunctionalDesignation,
                    signal.PlcTag,
                    signal.Description,
                    signal.Unit,
                    signal.Scale,
                    signal.LL,
                    signal.LA,
                    signal.HA,
                    signal.HH,
                    signal.ScalingFactors,
                    signal.SignalType,
                    signal.RegisterType,
                    signal.AddressBit,
                    signal.AccessType,
                    signal.DataType,
                    scadaType,
                    signal.FunctionCode,
                    signal.DcsChannel,
                    signal.DcsTag,
                    signal.DcsFunctions,
                    signal.Note,
                    signal.SheetName,
                    signal.DcsTag
                };

                _dataGridView.Rows.Add(row);
            }

            foreach (DataGridViewColumn column in _dataGridView.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            _dataGridView.AutoResizeColumns();

            var sourceInfo = "";
            if (_dataService.CurrentData != null)
            {
                sourceInfo = $"File: {_dataService.CurrentData.FileName}, ";
            }
            _statusLabel.Text = $"{sourceInfo}Showing {filteredSignals.Count} of {signals.Count} signals";
        }

        private List<ModbusSignal> GetCurrentSignals()
        {
            var signals = new List<ModbusSignal>();

            if (_dataService.CurrentData == null)
                return signals;

            var selectedSheet = _sheetSelector.SelectedItem?.ToString();

            if (selectedSheet == "All Sheets" || string.IsNullOrEmpty(selectedSheet))
            {
                signals.AddRange(_dataService.CurrentData.Signals);
            }
            else
            {
                var sheetSignals = _dataService.CurrentData.SignalsBySheet
                    .FirstOrDefault(s => s.Key == selectedSheet);
                if (sheetSignals.Value != null)
                {
                    signals.AddRange(sheetSignals.Value);
                }
            }

            return signals;
        }

        private List<ModbusSignal> ApplyFilters(List<ModbusSignal> signals)
        {
            var result = signals.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(_searchBox.Text))
            {
                var searchText = _searchBox.Text.ToLower();
                result = result.Where(s =>
                    (s.PlcTag?.ToLower().Contains(searchText) ?? false) ||
                    (s.Description?.ToLower().Contains(searchText) ?? false) ||
                    (s.AddressBit?.ToLower().Contains(searchText) ?? false) ||
                    (s.DcsTag?.ToLower().Contains(searchText) ?? false) ||
                    (s.ProjectFunctionalDesignation?.ToLower().Contains(searchText) ?? false)
                );
            }

            if (_filterFunctionCode.Value > 0)
            {
                var fc = (int)_filterFunctionCode.Value;
                result = result.Where(s => s.FunctionCode == fc);
            }

            if (_filterReadOnly.Checked)
            {
                result = result.Where(s =>
                    s.AccessType?.Contains("Read", StringComparison.OrdinalIgnoreCase) == true ||
                    s.AccessType == "Read"
                );
            }

            return result.ToList();
        }
    }
}
