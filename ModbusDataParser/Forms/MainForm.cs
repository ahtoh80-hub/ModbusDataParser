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
        private DataGridView? _dataGridView;
        private ComboBox? _fileSelector;
        private ComboBox? _sheetSelector;
        private Button? _loadButton;
        private Button? _exportCsvButton;
        private Button? _exportAddressMapButton;
        private Label? _statusLabel;
        private TextBox? _searchBox;
        private Button? _searchButton;
        private NumericUpDown? _filterFunctionCode;
        private CheckBox? _filterReadOnly;

        public MainForm()
        {
            InitializeComponent();
            _dataService.DataChanged += OnDataChanged;
        }

        private void InitializeComponent()
        {
            this.Text = "Modbus Data Parser";
            this.Size = new Size(1400, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(10)
            };
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            this.Controls.Add(mainPanel);

            var topPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(5)
            };

            _loadButton = new Button
            {
                Text = "Load File(s)",
                Size = new Size(120, 35),
                UseVisualStyleBackColor = true
            };
            _loadButton.Click += LoadButton_Click;

            _fileSelector = new ComboBox
            {
                Size = new Size(200, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false
            };
            _fileSelector.SelectedIndexChanged += FileSelector_SelectedIndexChanged;

            _sheetSelector = new ComboBox
            {
                Size = new Size(200, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false
            };
            _sheetSelector.SelectedIndexChanged += SheetSelector_SelectedIndexChanged;

            _searchBox = new TextBox
            {
                Size = new Size(200, 30),
                PlaceholderText = "Search..."
            };
            _searchBox.KeyPress += SearchBox_KeyPress;

            _searchButton = new Button
            {
                Text = "Search",
                Size = new Size(80, 35),
                UseVisualStyleBackColor = true
            };
            _searchButton.Click += SearchButton_Click;

            _filterFunctionCode = new NumericUpDown
            {
                Size = new Size(80, 30),
                Minimum = 0,
                Maximum = 255,
                Value = 0,
                Enabled = false
            };

            _filterReadOnly = new CheckBox
            {
                Text = "Show only Read",
                Size = new Size(130, 30),
                Enabled = false
            };
            _filterReadOnly.CheckedChanged += FilterReadOnly_CheckedChanged;

            _exportCsvButton = new Button
            {
                Text = "Export CSV",
                Size = new Size(100, 35),
                UseVisualStyleBackColor = true,
                Enabled = false
            };
            _exportCsvButton.Click += ExportCsvButton_Click;

            _exportAddressMapButton = new Button
            {
                Text = "Export Address Map",
                Size = new Size(130, 35),
                UseVisualStyleBackColor = true,
                Enabled = false
            };
            _exportAddressMapButton.Click += ExportAddressMapButton_Click;

            topPanel.Controls.Add(_loadButton);
            topPanel.Controls.Add(_fileSelector);
            topPanel.Controls.Add(_sheetSelector);
            topPanel.Controls.Add(_searchBox);
            topPanel.Controls.Add(_searchButton);
            topPanel.Controls.Add(new Label { Text = "FC:", Size = new Size(25, 30) });
            topPanel.Controls.Add(_filterFunctionCode);
            topPanel.Controls.Add(_filterReadOnly);
            topPanel.Controls.Add(_exportCsvButton);
            topPanel.Controls.Add(_exportAddressMapButton);

            mainPanel.Controls.Add(topPanel, 0, 0);

            _dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = true
            };
            mainPanel.Controls.Add(_dataGridView, 0, 1);

            _statusLabel = new Label
            {
                Dock = DockStyle.Fill,
                Text = "Ready",
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5)
            };
            mainPanel.Controls.Add(_statusLabel, 0, 2);
        }

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
                    _statusLabel!.Text = $"Loaded {openFileDialog.FileNames.Length} file(s), {_dataService.GetAllSignals().Count} signals total";
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
            if (_fileSelector?.SelectedItem != null)
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

        private void ExportCsvButton_Click(object? sender, EventArgs e)
        {
            ExportData(_dataService.GenerateImportData, "CSV files (*.csv)|*.csv", "export");
        }

        private void ExportAddressMapButton_Click(object? sender, EventArgs e)
        {
            ExportData(_dataService.GenerateAddressMap, "CSV files (*.csv)|*.csv", "address_map");
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
                    _statusLabel!.Text = $"Exported to {saveFileDialog.FileName}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OnDataChanged(object? sender, EventArgs e)
        {
            UpdateControls();
            UpdateGrid();
        }

        private void UpdateFileSelector()
        {
            if (_fileSelector == null) return;

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
            if (_sheetSelector == null || _dataService.CurrentData == null)
            {
                _sheetSelector?.Items.Clear();
                _sheetSelector!.Enabled = false;
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
            if (_exportCsvButton != null)
                _exportCsvButton.Enabled = _dataService.CurrentData?.Signals.Count > 0;
            if (_exportAddressMapButton != null)
                _exportAddressMapButton.Enabled = _dataService.CurrentData?.Signals.Count > 0;
            if (_filterFunctionCode != null)
                _filterFunctionCode.Enabled = _dataService.CurrentData?.Signals.Count > 0;
            if (_filterReadOnly != null)
                _filterReadOnly.Enabled = _dataService.CurrentData?.Signals.Count > 0;
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
                "Access Type", "Data Type", "Function Code",
                "DCS Channel", "DCS Tag", "DCS Functions", "Note", "Sheet"
            };

            foreach (var col in columns)
            {
                _dataGridView.Columns.Add(col, col);
            }

            foreach (var signal in filteredSignals)
            {
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
                    signal.FunctionCode,
                    signal.DcsChannel,
                    signal.DcsTag,
                    signal.DcsFunctions,
                    signal.Note,
                    signal.SheetName
                };

                _dataGridView.Rows.Add(row);
            }

            foreach (DataGridViewColumn column in _dataGridView.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            _dataGridView.AutoResizeColumns();

            if (_statusLabel != null)
            {
                var sourceInfo = "";
                if (_dataService.CurrentData != null)
                {
                    sourceInfo = $"File: {_dataService.CurrentData.FileName}, ";
                }
                _statusLabel.Text = $"{sourceInfo}Showing {filteredSignals.Count} of {signals.Count} signals";
            }
        }

        private List<ModbusSignal> GetCurrentSignals()
        {
            var signals = new List<ModbusSignal>();

            if (_sheetSelector == null || _dataService.CurrentData == null)
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

            if (!string.IsNullOrWhiteSpace(_searchBox?.Text))
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

            if (_filterFunctionCode != null && _filterFunctionCode.Value > 0)
            {
                var fc = (int)_filterFunctionCode.Value;
                result = result.Where(s => s.FunctionCode == fc);
            }

            if (_filterReadOnly?.Checked == true)
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
