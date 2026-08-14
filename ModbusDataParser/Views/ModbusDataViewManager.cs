using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ModbusDataParser.Models;

namespace ModbusDataParser.Views
{
    /// <summary>
    /// Класс для управления отображением данных Modbus
    /// </summary>
    public class ModbusDataViewManager
    {
        private readonly DataGridView dataGridView;
        private ModbusSystemData? currentData;
        private string filterFunction = "All";

        public ModbusDataViewManager(DataGridView dataGridView)
        {
            this.dataGridView = dataGridView;
            ConfigureDataGridView();
        }

        private void ConfigureDataGridView()
        {
            if (dataGridView == null) return;

            dataGridView.AutoGenerateColumns = false;
            dataGridView.BackgroundColor = Color.White;
            dataGridView.ForeColor = Color.Black;
            dataGridView.DefaultCellStyle.BackColor = Color.White;
            dataGridView.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dataGridView.ScrollBars = ScrollBars.Both;
            dataGridView.RowHeadersVisible = true;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.ReadOnly = true;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.MultiSelect = true;
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dataGridView.DefaultCellStyle.SelectionForeColor = Color.White;

            // Добавление колонок
            dataGridView.Columns.Clear();
            dataGridView.Columns.AddRange(new DataGridViewColumn[]
            {
                CreateColumn("№", "Number", 50, DataGridViewContentAlignment.MiddleRight),
                CreateColumn("Функц. обозначение", "ProjectDesignation", 180),
                CreateColumn("Тег ПЛК", "PlcTag", 200),
                CreateColumn("Описание", "Description", 300),
                CreateColumn("Ед. изм.", "Unit", 70, DataGridViewContentAlignment.MiddleCenter),
                CreateColumn("FC", "FunctionCode", 50, DataGridViewContentAlignment.MiddleCenter),
                CreateColumn("Адрес/бит", "AddressBit", 100, DataGridViewContentAlignment.MiddleCenter),
                CreateColumn("Тип данных", "DataType", 120, DataGridViewContentAlignment.MiddleCenter),
                CreateColumn("Доступ", "AccessType", 80, DataGridViewContentAlignment.MiddleCenter),
                CreateColumn("Тип сигнала", "SignalType", 100, DataGridViewContentAlignment.MiddleCenter),
                CreateColumn("Канал РСУ", "DcsChannel", 100, DataGridViewContentAlignment.MiddleCenter),
                CreateColumn("Тег РСУ", "DcsTag", 180),
                CreateColumn("Примечание", "Note", 150)
            });
        }

        private DataGridViewColumn CreateColumn(string header, string property, int width, 
            DataGridViewContentAlignment alignment = DataGridViewContentAlignment.MiddleLeft)
        {
            var column = new DataGridViewTextBoxColumn
            {
                HeaderText = header,
                DataPropertyName = property,
                Width = width,
                MinimumWidth = 40,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = alignment,
                    BackColor = Color.White,
                    ForeColor = Color.Black,
                    SelectionBackColor = Color.FromArgb(52, 152, 219),
                    SelectionForeColor = Color.White
                },
                HeaderCell = new DataGridViewColumnHeaderCell
                {
                    Style = new DataGridViewCellStyle
                    {
                        BackColor = Color.FromArgb(0, 80, 150),
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                    }
                }
            };
            return column;
        }

        public void SetData(ModbusSystemData data)
        {
            currentData = data;
            var table = CreateDataTable(data);
            dataGridView.DataSource = table;
            ApplyFilter();
        }

        private DataTable CreateDataTable(ModbusSystemData data)
        {
            var table = new DataTable();
            table.Columns.Add("Number", typeof(int));
            table.Columns.Add("ProjectDesignation", typeof(string));
            table.Columns.Add("PlcTag", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("Unit", typeof(string));
            table.Columns.Add("FunctionCode", typeof(byte));
            table.Columns.Add("AddressBit", typeof(string));
            table.Columns.Add("DataType", typeof(string));
            table.Columns.Add("AccessType", typeof(string));
            table.Columns.Add("SignalType", typeof(string));
            table.Columns.Add("DcsChannel", typeof(string));
            table.Columns.Add("DcsTag", typeof(string));
            table.Columns.Add("Note", typeof(string));

            foreach (var signal in data.GetAllSignals().OrderBy(s => s.FunctionCode).ThenBy(s => s.Address))
            {
                var row = table.NewRow();
                row["Number"] = signal.Number;
                row["ProjectDesignation"] = signal.ProjectDesignation;
                row["PlcTag"] = signal.PlcTag;
                row["Description"] = signal.Description;
                row["Unit"] = signal.Unit;
                row["FunctionCode"] = signal.FunctionCode;
                row["AddressBit"] = signal.GetModbusAddress();
                row["DataType"] = signal.DataType;
                row["AccessType"] = signal.AccessType;
                row["SignalType"] = signal.SignalType;
                row["DcsChannel"] = signal.DcsChannel;
                row["DcsTag"] = signal.DcsTag;
                row["Note"] = signal.Note;
                table.Rows.Add(row);
            }

            return table;
        }

        public void FilterByFunction(byte? functionCode)
        {
            filterFunction = functionCode.HasValue ? functionCode.Value.ToString("X2") : "All";
            ApplyFilter();
        }

        public void FilterByDesignation(string designation)
        {
            if (currentData == null) return;

            if (string.IsNullOrEmpty(designation))
            {
                ApplyFilter();
                return;
            }

            var filtered = currentData.GetSignalsByDesignation(designation).ToList();
            var table = CreateFilteredDataTable(filtered);
            dataGridView.DataSource = table;
        }

        private void ApplyFilter()
        {
            if (currentData == null) return;

            IEnumerable<ModbusSignal> signals = currentData.GetAllSignals();

            if (filterFunction != "All" && byte.TryParse(filterFunction, System.Globalization.NumberStyles.HexNumber, 
                System.Globalization.CultureInfo.InvariantCulture, out byte code))
            {
                signals = signals.Where(s => s.FunctionCode == code);
            }

            var table = CreateFilteredDataTable(signals);
            dataGridView.DataSource = table;
        }

        private DataTable CreateFilteredDataTable(IEnumerable<ModbusSignal> signals)
        {
            var table = new DataTable();
            table.Columns.Add("Number", typeof(int));
            table.Columns.Add("ProjectDesignation", typeof(string));
            table.Columns.Add("PlcTag", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("Unit", typeof(string));
            table.Columns.Add("FunctionCode", typeof(byte));
            table.Columns.Add("AddressBit", typeof(string));
            table.Columns.Add("DataType", typeof(string));
            table.Columns.Add("AccessType", typeof(string));
            table.Columns.Add("SignalType", typeof(string));
            table.Columns.Add("DcsChannel", typeof(string));
            table.Columns.Add("DcsTag", typeof(string));
            table.Columns.Add("Note", typeof(string));

            foreach (var signal in signals.OrderBy(s => s.FunctionCode).ThenBy(s => s.Address))
            {
                var row = table.NewRow();
                row["Number"] = signal.Number;
                row["ProjectDesignation"] = signal.ProjectDesignation;
                row["PlcTag"] = signal.PlcTag;
                row["Description"] = signal.Description;
                row["Unit"] = signal.Unit;
                row["FunctionCode"] = signal.FunctionCode;
                row["AddressBit"] = signal.GetModbusAddress();
                row["DataType"] = signal.DataType;
                row["AccessType"] = signal.AccessType;
                row["SignalType"] = signal.SignalType;
                row["DcsChannel"] = signal.DcsChannel;
                row["DcsTag"] = signal.DcsTag;
                row["Note"] = signal.Note;
                table.Rows.Add(row);
            }

            return table;
        }

        public ModbusSystemData? GetCurrentData()
        {
            return currentData;
        }

        public int GetRowCount()
        {
            return dataGridView?.Rows?.Count ?? 0;
        }

        public void Clear()
        {
            dataGridView.DataSource = null;
            currentData = null;
            filterFunction = "All";
        }

        public void ExportToClipboard()
        {
            if (dataGridView.Rows.Count == 0) return;

            var sb = new System.Text.StringBuilder();
            // Заголовки
            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                if (i > 0) sb.Append('\t');
                sb.Append(dataGridView.Columns[i].HeaderText);
            }
            sb.AppendLine();

            // Данные
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                for (int i = 0; i < dataGridView.Columns.Count; i++)
                {
                    if (i > 0) sb.Append('\t');
                    sb.Append(row.Cells[i].Value?.ToString() ?? string.Empty);
                }
                sb.AppendLine();
            }

            Clipboard.SetText(sb.ToString());
        }
    }
}