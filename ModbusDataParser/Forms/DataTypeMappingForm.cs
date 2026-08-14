using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ModbusDataParser.Models;

namespace ModbusDataParser.Forms
{
    public partial class DataTypeMappingForm : Form
    {
        private DataTypeMappingSettings _settings;
        private DataGridView _dataGridView = new();
        private Button _btnSave = new();
        private Button _btnCancel = new();
        private Button _btnResetDefaults = new();
        private Button _btnAddRow = new();
        private Button _btnDeleteRow = new();
        private Label _lblInfo = new();
        private Label _lblScadaType = new();
        private ComboBox _cmbTargetType = new();
        private List<string> _availableSourceTypes = new();

        // Список доступных SCADA типов
        private readonly string[] _scadaTypes = new[]
        {
            "BOOL", "WORD", "INT", "UINT", "UDINT", "REAL"
        };

        public DataTypeMappingSettings Settings => _settings;

        public DataTypeMappingForm(DataTypeMappingSettings? initialSettings = null, List<string>? sourceTypes = null)
        {
            _settings = initialSettings ?? new DataTypeMappingSettings();
            _availableSourceTypes = sourceTypes ?? new List<string>();
            
            if (_settings.Mappings.Count == 0)
            {
                _settings.Mappings = DataTypeMappingDefaults.GetDefaultMappings();
            }
            InitializeComponent();
            LoadMappings();
            UpdateStatus();
        }

        private void InitializeComponent()
        {
            this.Text = "Data Type Mapping";
            this.Size = new Size(750, 580);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(10)
            };
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));

            // ============ Верхняя панель - добавление нового маппинга ============
            var topPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 2,
                Padding = new Padding(5)
            };
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            topPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));

            // Информационная строка
            _lblInfo = new Label
            {
                Text = "Source Data Types are read from Excel files. Select SCADA type for each source type.",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.DarkBlue
            };
            topPanel.Controls.Add(_lblInfo, 0, 0);
            topPanel.SetColumnSpan(_lblInfo, 4);

            // SCADA Type Label
            _lblScadaType = new Label
            {
                Text = "SCADA Data Type:",
                Size = new Size(110, 28),
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            topPanel.Controls.Add(_lblScadaType, 0, 1);

            // ComboBox для SCADA типа (можно вводить вручную)
            _cmbTargetType = new ComboBox
            {
                Size = new Size(150, 28),
                DropDownStyle = ComboBoxStyle.DropDown,
                Font = new Font("Segoe UI", 9),
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.ListItems
            };
            _cmbTargetType.Items.AddRange(_scadaTypes);
            _cmbTargetType.Text = "REAL";
            topPanel.Controls.Add(_cmbTargetType, 1, 1);

            // Кнопка Add
            _btnAddRow = new Button
            {
                Text = "➕ Add New",
                Size = new Size(100, 30),
                UseVisualStyleBackColor = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.LightGreen
            };
            _btnAddRow.Click += BtnAddRow_Click;
            topPanel.Controls.Add(_btnAddRow, 3, 1);

            mainPanel.Controls.Add(topPanel, 0, 0);

            // ============ Таблица маппинга ============
            _dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D
            };
            _dataGridView.SelectionChanged += DataGridView_SelectionChanged;
            _dataGridView.CellBeginEdit += DataGridView_CellBeginEdit;
            _dataGridView.CellEndEdit += DataGridView_CellEndEdit;
            mainPanel.Controls.Add(_dataGridView, 0, 1);

            // ============ Нижняя панель - кнопки ============
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(5)
            };

            _btnSave = new Button
            {
                Text = "💾 Save",
                Size = new Size(100, 35),
                UseVisualStyleBackColor = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.LightBlue
            };
            _btnSave.Click += BtnSave_Click;

            _btnCancel = new Button
            {
                Text = "❌ Cancel",
                Size = new Size(100, 35),
                UseVisualStyleBackColor = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            _btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            _btnResetDefaults = new Button
            {
                Text = "🔄 Reset Defaults",
                Size = new Size(130, 35),
                UseVisualStyleBackColor = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            _btnResetDefaults.Click += BtnResetDefaults_Click;

            _btnDeleteRow = new Button
            {
                Text = "🗑 Delete Selected",
                Size = new Size(130, 35),
                UseVisualStyleBackColor = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.LightCoral,
                Enabled = false
            };
            _btnDeleteRow.Click += BtnDeleteRow_Click;

            buttonPanel.Controls.Add(_btnSave);
            buttonPanel.Controls.Add(_btnCancel);
            buttonPanel.Controls.Add(_btnResetDefaults);
            buttonPanel.Controls.Add(_btnDeleteRow);

            mainPanel.Controls.Add(buttonPanel, 0, 2);
            this.Controls.Add(mainPanel);
        }

        private void LoadMappings()
        {
            if (_dataGridView == null) return;

            _dataGridView.Columns.Clear();

            // Колонка Source Data Type - только для чтения
            var sourceColumn = new DataGridViewTextBoxColumn
            {
                Name = "SourceType",
                HeaderText = "Source Data Type",
                ReadOnly = true,
                Width = 200
            };
            _dataGridView.Columns.Add(sourceColumn);

            // Колонка SCADA Data Type - можно редактировать
            var targetColumn = new DataGridViewTextBoxColumn
            {
                Name = "TargetType",
                HeaderText = "SCADA Data Type",
                ReadOnly = false,
                Width = 200
            };
            _dataGridView.Columns.Add(targetColumn);

            // Колонка Mapped - чекбокс
            var checkBoxColumn = new DataGridViewCheckBoxColumn
            {
                Name = "IsMapped",
                HeaderText = "Mapped",
                TrueValue = true,
                FalseValue = false,
                Width = 80
            };
            _dataGridView.Columns.Add(checkBoxColumn);

            // Загружаем данные
            foreach (var mapping in _settings.Mappings)
            {
                int rowIndex = _dataGridView.Rows.Add();
                _dataGridView.Rows[rowIndex].Cells[0].Value = mapping.SourceDataType;
                _dataGridView.Rows[rowIndex].Cells[1].Value = mapping.TargetDataType;
                _dataGridView.Rows[rowIndex].Cells[2].Value = mapping.IsMapped;
            }

            // Добавляем пустую строку для нового ввода
            int newRowIndex = _dataGridView.Rows.Add();
            _dataGridView.Rows[newRowIndex].Cells[0].Value = "";
            _dataGridView.Rows[newRowIndex].Cells[1].Value = "";
            _dataGridView.Rows[newRowIndex].Cells[2].Value = false;

            foreach (DataGridViewColumn column in _dataGridView.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            _btnDeleteRow.Enabled = false;
        }

        private void DataGridView_SelectionChanged(object? sender, EventArgs e)
        {
            if (_dataGridView == null) return;
            _btnDeleteRow.Enabled = _dataGridView.SelectedRows.Count > 0 && 
                                   _dataGridView.SelectedRows[0].Index < _dataGridView.Rows.Count - 1;
        }

        private void DataGridView_CellBeginEdit(object? sender, DataGridViewCellCancelEventArgs e)
        {
            // Запрещаем редактирование колонки Source Data Type
            if (e.ColumnIndex == 0)
            {
                e.Cancel = true;
                MessageBox.Show("Source Data Type cannot be edited. It is read from Excel files.", 
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DataGridView_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            // При редактировании SCADA типа в последней строке
            if (e.RowIndex == _dataGridView.Rows.Count - 1 && e.ColumnIndex == 1)
            {
                var targetType = _dataGridView.Rows[e.RowIndex].Cells[1].Value?.ToString() ?? "";
                if (!string.IsNullOrEmpty(targetType))
                {
                    // Добавляем новую пустую строку
                    int newRowIndex = _dataGridView.Rows.Add();
                    _dataGridView.Rows[newRowIndex].Cells[0].Value = "";
                    _dataGridView.Rows[newRowIndex].Cells[1].Value = "";
                    _dataGridView.Rows[newRowIndex].Cells[2].Value = false;
                }
            }
        }

        private void BtnAddRow_Click(object? sender, EventArgs e)
        {
            var targetType = _cmbTargetType.Text?.Trim() ?? "";

            // Проверяем, есть ли Source типы без маппинга
            var existingSourceTypes = new List<string>();
            foreach (DataGridViewRow row in _dataGridView.Rows)
            {
                if (row.IsNewRow) continue;
                var source = row.Cells[0].Value?.ToString();
                if (!string.IsNullOrEmpty(source))
                {
                    existingSourceTypes.Add(source);
                }
            }

            var availableSourceTypes = _availableSourceTypes
                .Where(s => !existingSourceTypes.Contains(s))
                .ToList();

            if (!availableSourceTypes.Any())
            {
                MessageBox.Show("All Source Data Types already have mappings.", 
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Выбираем первый доступный Source Type
            var sourceType = availableSourceTypes.First();

            if (string.IsNullOrEmpty(targetType))
            {
                MessageBox.Show("Please enter SCADA Data Type.", "Warning", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _cmbTargetType.Focus();
                return;
            }

            // Добавляем новую строку перед пустой - ИСПРАВЛЕННЫЙ КОД
            int insertIndex = _dataGridView.Rows.Count - 1;
            _dataGridView.Rows.Insert(insertIndex, 1);
            _dataGridView.Rows[insertIndex].Cells[0].Value = sourceType;
            _dataGridView.Rows[insertIndex].Cells[1].Value = targetType;
            _dataGridView.Rows[insertIndex].Cells[2].Value = true;

            // Очищаем поле ввода
            _cmbTargetType.Text = "REAL";
            _cmbTargetType.Focus();

            _btnDeleteRow.Enabled = false;

            // Обновляем статус
            UpdateStatus();
        }

        private void BtnDeleteRow_Click(object? sender, EventArgs e)
        {
            if (_dataGridView == null) return;

            if (_dataGridView.SelectedRows.Count == 0)
                return;

            var selectedRow = _dataGridView.SelectedRows[0];
            
            if (selectedRow.Index == _dataGridView.Rows.Count - 1)
                return;

            var sourceType = selectedRow.Cells[0].Value?.ToString() ?? "";
            var targetType = selectedRow.Cells[1].Value?.ToString() ?? "";
            
            var result = MessageBox.Show(
                $"Delete mapping for '{sourceType}' -> '{targetType}'?", 
                "Confirm Delete", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Возвращаем Source Type в список доступных
                if (!_availableSourceTypes.Contains(sourceType))
                {
                    _availableSourceTypes.Add(sourceType);
                }

                _dataGridView.Rows.RemoveAt(selectedRow.Index);
                _btnDeleteRow.Enabled = false;
                UpdateStatus();
            }
        }

        private void UpdateStatus()
        {
            var existingSourceTypes = new List<string>();
            foreach (DataGridViewRow row in _dataGridView.Rows)
            {
                if (row.IsNewRow) continue;
                var source = row.Cells[0].Value?.ToString();
                if (!string.IsNullOrEmpty(source))
                {
                    existingSourceTypes.Add(source);
                }
            }

            var remaining = _availableSourceTypes.Count;
            var total = existingSourceTypes.Count + remaining;

            if (_lblInfo != null)
            {
                _lblInfo.Text = $"Source Data Types: {existingSourceTypes.Count} mapped, {remaining} remaining (total: {total})";
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            var newMappings = new List<DataTypeMapping>();

            foreach (DataGridViewRow row in _dataGridView.Rows)
            {
                if (row.IsNewRow) continue;

                var sourceType = row.Cells[0].Value?.ToString()?.Trim() ?? "";
                var targetType = row.Cells[1].Value?.ToString()?.Trim() ?? "";
                var isMapped = row.Cells[2] is DataGridViewCheckBoxCell checkBox
                    ? (checkBox.Value as bool? ?? false)
                    : false;

                if (string.IsNullOrEmpty(sourceType))
                    continue;

                if (string.IsNullOrEmpty(targetType))
                    targetType = sourceType;

                newMappings.Add(new DataTypeMapping
                {
                    SourceDataType = sourceType,
                    TargetDataType = targetType,
                    IsMapped = isMapped
                });
            }

            _settings.Mappings = newMappings;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnResetDefaults_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Reset all mappings to default values?\nThis will remove all custom mappings.", 
                "Confirm Reset", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                var defaults = DataTypeMappingDefaults.GetDefaultMappings();
                _settings.Mappings = defaults;
                LoadMappings();
                _btnDeleteRow.Enabled = false;
                
                // Обновляем список доступных Source Type
                var existingTypes = defaults.Select(m => m.SourceDataType).ToList();
                _availableSourceTypes = _availableSourceTypes
                    .Where(s => !existingTypes.Contains(s))
                    .ToList();
                
                UpdateStatus();
            }
        }
    }
}
