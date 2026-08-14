using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ModbusDataParser.Helpers
{
    /// <summary>
    /// Вспомогательный класс для работы с DataGridView
    /// </summary>
    public static class DataGridHelper
    {
        /// <summary>
        /// Настройка стандартного DataGridView
        /// </summary>
        public static void ConfigureDataGridView(DataGridView dgv, bool readOnly = true)
        {
            if (dgv == null) return;

            dgv.BackgroundColor = Color.White;
            dgv.ForeColor = Color.Black;
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgv.ScrollBars = ScrollBars.Both;
            dgv.RowHeadersVisible = true;
            dgv.RowHeadersWidth = 30;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = readOnly;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = true;
            dgv.EnableHeadersVisualStyles = false;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 80, 150);
            dgv.RowHeadersDefaultCellStyle.ForeColor = Color.White;

            // Стиль заголовков
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 80, 150);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        /// <summary>
        /// Создание колонки для DataGridView
        /// </summary>
        public static DataGridViewColumn CreateColumn(string header, string propertyName, int width, 
            bool visible = true, string format = "", DataGridViewContentAlignment alignment = DataGridViewContentAlignment.MiddleLeft)
        {
            var column = new DataGridViewTextBoxColumn
            {
                HeaderText = header,
                DataPropertyName = propertyName,
                Width = width,
                MinimumWidth = 40,
                Visible = visible,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = alignment,
                    BackColor = Color.White,
                    ForeColor = Color.Black,
                    SelectionBackColor = Color.FromArgb(52, 152, 219),
                    SelectionForeColor = Color.White,
                    Format = format
                }
            };
            return column;
        }

        /// <summary>
        /// Создание комбобокс-колонки
        /// </summary>
        public static DataGridViewComboBoxColumn CreateComboBoxColumn(string header, string propertyName, 
            object[] items, int width)
        {
            var column = new DataGridViewComboBoxColumn
            {
                HeaderText = header,
                DataPropertyName = propertyName,
                Width = width,
                MinimumWidth = 50,
                Items = items,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                FlatStyle = FlatStyle.Flat
            };
            return column;
        }

        /// <summary>
        /// Создание кнопочной колонки
        /// </summary>
        public static DataGridViewButtonColumn CreateButtonColumn(string header, string text, int width)
        {
            var column = new DataGridViewButtonColumn
            {
                HeaderText = header,
                Width = width,
                MinimumWidth = 50,
                Text = text,
                UseColumnTextForButtonValue = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                FlatStyle = FlatStyle.Flat
            };
            return column;
        }

        /// <summary>
        /// Создание колонки со счетчиком
        /// </summary>
        public static DataGridViewColumn CreateRowNumberColumn()
        {
            return new DataGridViewTextBoxColumn
            {
                HeaderText = "№",
                Width = 40,
                MinimumWidth = 30,
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    BackColor = Color.FromArgb(240, 240, 240)
                }
            };
        }

        /// <summary>
        /// Добавление порядковых номеров в DataGridView
        /// </summary>
        public static void AddRowNumbers(DataGridView dgv)
        {
            if (dgv == null) return;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                row.HeaderCell.Value = (row.Index + 1).ToString();
            }
            dgv.RowHeadersWidth = 50;
        }

        /// <summary>
        /// Экспорт DataGridView в CSV
        /// </summary>
        public static string ExportToCsv(DataGridView dgv, char separator = ';')
        {
            if (dgv == null || dgv.Rows.Count == 0)
                return string.Empty;

            var sb = new System.Text.StringBuilder();

            // Заголовки
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                if (i > 0) sb.Append(separator);
                var header = dgv.Columns[i].HeaderText;
                sb.Append(header.Contains(separator) ? $"\"{header}\"" : header);
            }
            sb.AppendLine();

            // Данные
            foreach (DataGridViewRow row in dgv.Rows)
            {
                for (int i = 0; i < dgv.Columns.Count; i++)
                {
                    if (i > 0) sb.Append(separator);
                    var value = row.Cells[i].Value?.ToString() ?? string.Empty;
                    sb.Append(value.Contains(separator) || value.Contains('"') ? $"\"{value.Replace("\"", "\"\"")}\"" : value);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Копирование выделенных строк в буфер обмена
        /// </summary>
        public static void CopySelectedRowsToClipboard(DataGridView dgv, bool includeHeader = true)
        {
            if (dgv == null || dgv.SelectedRows.Count == 0)
                return;

            var sb = new System.Text.StringBuilder();

            // Заголовки
            if (includeHeader)
            {
                for (int i = 0; i < dgv.Columns.Count; i++)
                {
                    if (i > 0) sb.Append('\t');
                    sb.Append(dgv.Columns[i].HeaderText);
                }
                sb.AppendLine();
            }

            // Данные
            foreach (DataGridViewRow row in dgv.SelectedRows)
            {
                if (row.IsNewRow) continue;
                for (int i = 0; i < dgv.Columns.Count; i++)
                {
                    if (i > 0) sb.Append('\t');
                    sb.Append(row.Cells[i].Value?.ToString() ?? string.Empty);
                }
                sb.AppendLine();
            }

            Clipboard.SetText(sb.ToString());
        }

        /// <summary>
        /// Фильтрация данных в DataGridView
        /// </summary>
        public static void FilterDataGridView(DataGridView dgv, string filterText, params string[] columnsToSearch)
        {
            if (dgv == null || dgv.DataSource == null)
                return;

            var dataTable = dgv.DataSource as DataTable;
            if (dataTable == null) return;

            if (string.IsNullOrEmpty(filterText))
            {
                dataTable.DefaultView.RowFilter = "";
                return;
            }

            if (columnsToSearch.Length == 0)
            {
                // Поиск по всем колонкам
                var filterParts = new System.Collections.Generic.List<string>();
                foreach (DataColumn col in dataTable.Columns)
                {
                    filterParts.Add($"[{col.ColumnName}] LIKE '%{filterText.Replace("'", "''")}%'");
                }
                dataTable.DefaultView.RowFilter = string.Join(" OR ", filterParts);
            }
            else
            {
                var filterParts = new System.Collections.Generic.List<string>();
                foreach (var col in columnsToSearch)
                {
                    filterParts.Add($"[{col}] LIKE '%{filterText.Replace("'", "''")}%'");
                }
                dataTable.DefaultView.RowFilter = string.Join(" OR ", filterParts);
            }
        }

        /// <summary>
        /// Очистка фильтрации
        /// </summary>
        public static void ClearFilter(DataGridView dgv)
        {
            if (dgv == null || dgv.DataSource == null)
                return;

            var dataTable = dgv.DataSource as DataTable;
            if (dataTable != null)
            {
                dataTable.DefaultView.RowFilter = "";
            }
        }

        /// <summary>
        /// Сохранение в Excel
        /// </summary>
        public static void SaveToExcel(DataGridView dgv, string filePath)
        {
            if (dgv == null || dgv.Rows.Count == 0)
                return;

            using var package = new OfficeOpenXml.ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Data");

            // Заголовки
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                worksheet.Cells[1, i + 1].Value = dgv.Columns[i].HeaderText;
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 80, 150));
                worksheet.Cells[1, i + 1].Style.Font.Color.SetColor(Color.White);
            }

            // Данные
            int rowIndex = 2;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;
                for (int i = 0; i < dgv.Columns.Count; i++)
                {
                    worksheet.Cells[rowIndex, i + 1].Value = row.Cells[i].Value?.ToString();
                }
                rowIndex++;
            }

            // Авторазмер
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            // Сохранение
            System.IO.File.WriteAllBytes(filePath, package.GetAsByteArray());
        }

        /// <summary>
        /// Поиск строки в DataGridView
        /// </summary>
        public static int FindRow(DataGridView dgv, string searchText, int columnIndex = 0, 
            StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (dgv == null || string.IsNullOrEmpty(searchText))
                return -1;

            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                if (dgv.Rows[i].IsNewRow) continue;
                var value = dgv.Rows[i].Cells[columnIndex].Value?.ToString();
                if (value != null && value.Equals(searchText, comparison))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Получение выбранных строк в DataTable
        /// </summary>
        public static DataTable GetSelectedRowsAsDataTable(DataGridView dgv)
        {
            var table = new DataTable();
            if (dgv == null || dgv.SelectedRows.Count == 0)
                return table;

            // Колонки
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                table.Columns.Add(col.DataPropertyName ?? col.HeaderText, typeof(string));
            }

            // Данные
            foreach (DataGridViewRow row in dgv.SelectedRows)
            {
                if (row.IsNewRow) continue;
                var newRow = table.NewRow();
                for (int i = 0; i < dgv.Columns.Count; i++)
                {
                    newRow[i] = row.Cells[i].Value?.ToString();
                }
                table.Rows.Add(newRow);
            }

            return table;
        }

        /// <summary>
        /// Подсветка строк по условию
        /// </summary>
        public static void HighlightRows(DataGridView dgv, Func<DataGridViewRow, bool> condition, 
            Color backColor, Color? foreColor = null)
        {
            if (dgv == null) return;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;
                if (condition(row))
                {
                    row.DefaultCellStyle.BackColor = backColor;
                    if (foreColor.HasValue)
                        row.DefaultCellStyle.ForeColor = foreColor.Value;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            }
        }

        /// <summary>
        /// Сброс подсветки строк
        /// </summary>
        public static void ResetRowColors(DataGridView dgv)
        {
            if (dgv == null) return;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;
                row.DefaultCellStyle.BackColor = Color.White;
                row.DefaultCellStyle.ForeColor = Color.Black;
            }
        }

        /// <summary>
        /// Автоматическая настройка ширины колонок с учетом содержимого
        /// </summary>
        public static void AutoFitColumns(DataGridView dgv, bool withHeader = true)
        {
            if (dgv == null) return;
            
            if (withHeader)
                dgv.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            else
                dgv.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
        }

        /// <summary>
        /// Установка фиксированной ширины для колонок
        /// </summary>
        public static void SetColumnWidths(DataGridView dgv, params (string ColumnName, int Width)[] columnWidths)
        {
            if (dgv == null) return;

            foreach (var (columnName, width) in columnWidths)
            {
                if (dgv.Columns.Contains(columnName))
                {
                    dgv.Columns[columnName].Width = width;
                    dgv.Columns[columnName].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                }
            }
        }
    }
}