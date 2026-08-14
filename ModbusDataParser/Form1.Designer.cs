using System;
using System.Drawing;
using System.Windows.Forms;



namespace ModbusDataParser
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private Panel panelTop;
        private Panel panelMain;
        private Panel panelLeft;
        private Panel panelCenter;
        private Panel panelRight;
        private Panel panelBottom;
        private Panel panelButtons;
        private TableLayoutPanel tableLayoutButtons;

        private MenuStrip menuStrip;
        private ToolStripMenuItem fileMenu;
        private ToolStripMenuItem toolsMenu;
        private ToolStripMenuItem helpMenu;
        private ToolStripMenuItem loadTemplateMenuItem;
        private ToolStripMenuItem loadExcelMenuItem;
        private ToolStripMenuItem validateMenuItem;
        private ToolStripMenuItem convertTagsMenuItem;
        private ToolStripMenuItem generateMenuItem;
        private ToolStripMenuItem clearAllMenuItem;
        private ToolStripMenuItem exitMenuItem;
        private ToolStripMenuItem aboutMenuItem;

        private Label lblTitle;
        private Label lblTemplateInfo;
        private Label lblExcelInfo;
        private Label lblActiveReplacements;

        private Button btnLoadTemplate;
        private Button btnLoadExcel;
        private Button btnConvertTags;
        private Button btnGenerate;
        private Button btnClearAll;
        private Button btnValidate;

        private DataGridView dgvReplacements;
        private Label lblReplacementsTitle;

        private TextBox txtTemplatePreview;
        private Label lblPreviewTitle;

        private RichTextBox txtMappingInfo;
        private Label lblMappingTitle;

        private RichTextBox rtbLog;
        private Label lblLogTitle;

        private Splitter splitterRight;
        public Form1()
        {
            InitializeComponent();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelTop = new Panel();
            this.panelMain = new Panel();
            this.panelLeft = new Panel();
            this.panelCenter = new Panel();
            this.panelRight = new Panel();
            this.panelBottom = new Panel();
            this.panelButtons = new Panel();
            this.tableLayoutButtons = new TableLayoutPanel();

            this.splitterRight = new Splitter();

            this.menuStrip = new MenuStrip();
            this.fileMenu = new ToolStripMenuItem();
            this.toolsMenu = new ToolStripMenuItem();
            this.helpMenu = new ToolStripMenuItem();
            this.loadTemplateMenuItem = new ToolStripMenuItem();
            this.loadExcelMenuItem = new ToolStripMenuItem();
            this.validateMenuItem = new ToolStripMenuItem();
            this.convertTagsMenuItem = new ToolStripMenuItem();
            this.generateMenuItem = new ToolStripMenuItem();
            this.clearAllMenuItem = new ToolStripMenuItem();
            this.exitMenuItem = new ToolStripMenuItem();
            this.aboutMenuItem = new ToolStripMenuItem();

            this.lblTitle = new Label();
            this.lblTemplateInfo = new Label();
            this.lblExcelInfo = new Label();
            this.lblActiveReplacements = new Label();

            this.btnLoadTemplate = new Button();
            this.btnLoadExcel = new Button();
            this.btnConvertTags = new Button();
            this.btnGenerate = new Button();
            this.btnClearAll = new Button();
            this.btnValidate = new Button();

            this.dgvReplacements = new DataGridView();
            this.lblReplacementsTitle = new Label();

            this.txtTemplatePreview = new TextBox();
            this.lblPreviewTitle = new Label();

            this.txtMappingInfo = new RichTextBox();
            this.lblMappingTitle = new Label();

            this.rtbLog = new RichTextBox();
            this.lblLogTitle = new Label();

            this.SuspendLayout();

            // MenuStrip
            this.menuStrip.BackColor = Color.FromArgb(0, 80, 150);
            this.menuStrip.ForeColor = Color.Black;
            this.menuStrip.Font = new Font("Segoe UI", 10F);
            this.menuStrip.Items.AddRange(new ToolStripItem[] {
                this.fileMenu,
                this.toolsMenu,
                this.helpMenu
            });
            this.menuStrip.Location = new Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new Size(1400, 28);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip";

            // File Menu
            this.fileMenu.Text = "Файл";
            this.fileMenu.ForeColor = Color.Black;
            this.fileMenu.DropDownItems.AddRange(new ToolStripItem[] {
                this.loadTemplateMenuItem,
                this.loadExcelMenuItem,
                new ToolStripSeparator(),
                this.exitMenuItem
            });

            this.loadTemplateMenuItem.Text = "Загрузить шаблон";
            this.loadTemplateMenuItem.ForeColor = Color.Black;
            this.loadTemplateMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            this.loadTemplateMenuItem.Click += (s, e) => btnLoadTemplate.PerformClick();

            this.loadExcelMenuItem.Text = "Загрузить экземпляры";
            this.loadExcelMenuItem.ForeColor = Color.Black;
            this.loadExcelMenuItem.ShortcutKeys = Keys.Control | Keys.E;
            this.loadExcelMenuItem.Click += (s, e) => btnLoadExcel.PerformClick();

            this.exitMenuItem.Text = "Выход";
            this.exitMenuItem.ForeColor = Color.Black;
            this.exitMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
            this.exitMenuItem.Click += (s, e) => Application.Exit();

            // Tools Menu
            this.toolsMenu.Text = "Инструменты";
            this.toolsMenu.ForeColor = Color.Black;
            this.toolsMenu.DropDownItems.AddRange(new ToolStripItem[] {
                this.validateMenuItem,
                this.convertTagsMenuItem,
                new ToolStripSeparator(),
                this.generateMenuItem,
                this.clearAllMenuItem
            });

            this.validateMenuItem.Text = "Проверить данные";
            this.validateMenuItem.ForeColor = Color.Black;
            this.validateMenuItem.ShortcutKeys = Keys.Control | Keys.V;
            this.validateMenuItem.Click += (s, e) => btnValidate.PerformClick();

            this.convertTagsMenuItem.Text = "Преобразование";
            this.convertTagsMenuItem.ForeColor = Color.Black;
            this.convertTagsMenuItem.ShortcutKeys = Keys.Control | Keys.T;
            this.convertTagsMenuItem.Click += (s, e) => btnConvertTags.PerformClick();

            this.generateMenuItem.Text = "Сгенерировать";
            this.generateMenuItem.ForeColor = Color.Black;
            this.generateMenuItem.ShortcutKeys = Keys.Control | Keys.G;
            this.generateMenuItem.Click += (s, e) => btnGenerate.PerformClick();

            this.clearAllMenuItem.Text = "Очистить все";
            this.clearAllMenuItem.ForeColor = Color.Black;
            this.clearAllMenuItem.ShortcutKeys = Keys.Control | Keys.C;
            this.clearAllMenuItem.Click += (s, e) => btnClearAll.PerformClick();

            // Help Menu
            this.helpMenu.Text = "Помощь";
            this.helpMenu.ForeColor = Color.Black;
            this.helpMenu.DropDownItems.AddRange(new ToolStripItem[] {
                this.aboutMenuItem
            });

            this.aboutMenuItem.Text = "О программе";
            this.aboutMenuItem.ForeColor = Color.Black;
            this.aboutMenuItem.Click += (s, e) => {
                MessageBox.Show(
                    "Генератор экземпляров по шаблону\nВерсия 3.0\n\nРазработчик: Антон Решетов\nЗаказчик: ТЭКОН-Системы\n\n© 2026",
                    "О программе",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            };

            // Form1
            this.Text = "Modbus Data Parser - Готов к работе";
            this.BackColor = Color.FromArgb(13, 37, 63);
            this.ForeColor = Color.White;
            this.Size = new Size(1400, 900);
            this.MinimumSize = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MainMenuStrip = this.menuStrip;

            // panelTop
            this.panelTop.Dock = DockStyle.Top;
            this.panelTop.Height = 60;
            this.panelTop.BackColor = Color.FromArgb(0, 80, 150);
            this.panelTop.Padding = new Padding(15, 5, 15, 5);

            this.lblTitle.Text = "Modbus Data Parser";
            this.lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.White;
            this.lblTitle.Dock = DockStyle.Top;
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblTitle.Height = 35;

            this.lblTemplateInfo.Text = "Шаблон: -";
            this.lblTemplateInfo.Font = new Font("Segoe UI", 9F);
            this.lblTemplateInfo.ForeColor = Color.White;
            this.lblTemplateInfo.AutoSize = true;
            this.lblTemplateInfo.Location = new Point(20, 40);

            this.lblExcelInfo.Text = "Excel: -";
            this.lblExcelInfo.Font = new Font("Segoe UI", 9F);
            this.lblExcelInfo.ForeColor = Color.White;
            this.lblExcelInfo.AutoSize = true;
            this.lblExcelInfo.Location = new Point(280, 40);

            this.lblActiveReplacements.Text = "Найдено: 0";
            this.lblActiveReplacements.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblActiveReplacements.ForeColor = Color.FromArgb(46, 204, 113);
            this.lblActiveReplacements.AutoSize = true;
            this.lblActiveReplacements.Location = new Point(550, 40);
            this.lblActiveReplacements.BackColor = Color.Transparent;

            this.panelTop.Controls.Add(this.lblTitle);
            this.panelTop.Controls.Add(this.lblTemplateInfo);
            this.panelTop.Controls.Add(this.lblExcelInfo);
            this.panelTop.Controls.Add(this.lblActiveReplacements);

            // panelMain
            this.panelMain.Dock = DockStyle.Fill;
            this.panelMain.BackColor = Color.FromArgb(13, 37, 63);
            this.panelMain.Padding = new Padding(5);

            // panelLeft - Таблица замен
            this.panelLeft.Dock = DockStyle.Left;
            this.panelLeft.Width = 380;
            this.panelLeft.BackColor = Color.FromArgb(20, 50, 95);
            this.panelLeft.Padding = new Padding(5);

            var panelLeftContainer = new Panel();
            panelLeftContainer.Dock = DockStyle.Fill;
            panelLeftContainer.BackColor = Color.FromArgb(20, 50, 95);

            this.lblReplacementsTitle.Text = "Сигналы Modbus";
            this.lblReplacementsTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblReplacementsTitle.ForeColor = Color.White;
            this.lblReplacementsTitle.Dock = DockStyle.Top;
            this.lblReplacementsTitle.Height = 35;
            this.lblReplacementsTitle.TextAlign = ContentAlignment.MiddleLeft;
            this.lblReplacementsTitle.Padding = new Padding(5, 0, 0, 0);
            this.lblReplacementsTitle.BackColor = Color.FromArgb(0, 80, 150);

            this.dgvReplacements.Dock = DockStyle.Fill;
            this.dgvReplacements.BackgroundColor = Color.White;
            this.dgvReplacements.ForeColor = Color.Black;
            this.dgvReplacements.BorderStyle = BorderStyle.FixedSingle;
            this.dgvReplacements.DefaultCellStyle.BackColor = Color.White;
            this.dgvReplacements.DefaultCellStyle.ForeColor = Color.Black;
            this.dgvReplacements.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            this.dgvReplacements.ScrollBars = ScrollBars.Both;
            this.dgvReplacements.RowHeadersVisible = false;
            this.dgvReplacements.AllowUserToAddRows = false;
            this.dgvReplacements.AllowUserToDeleteRows = false;
            this.dgvReplacements.EditMode = DataGridViewEditMode.EditProgrammatically;
            this.dgvReplacements.AutoGenerateColumns = false;

            panelLeftContainer.Controls.Add(this.dgvReplacements);
            panelLeftContainer.Controls.Add(this.lblReplacementsTitle);
            this.panelLeft.Controls.Add(panelLeftContainer);

            // panelRight - Информация о заменах
            this.panelRight.Dock = DockStyle.Right;
            this.panelRight.Width = 350;
            this.panelRight.BackColor = Color.FromArgb(20, 50, 95);
            this.panelRight.Padding = new Padding(5);
            this.panelRight.MinimumSize = new Size(200, 0);

            var panelRightContainer = new Panel();
            panelRightContainer.Dock = DockStyle.Fill;
            panelRightContainer.BackColor = Color.FromArgb(20, 50, 95);

            this.lblMappingTitle.Text = "Информация о системе";
            this.lblMappingTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblMappingTitle.ForeColor = Color.White;
            this.lblMappingTitle.Dock = DockStyle.Top;
            this.lblMappingTitle.Height = 35;
            this.lblMappingTitle.TextAlign = ContentAlignment.MiddleLeft;
            this.lblMappingTitle.Padding = new Padding(5, 0, 0, 0);
            this.lblMappingTitle.BackColor = Color.FromArgb(0, 80, 150);

            this.txtMappingInfo.Dock = DockStyle.Fill;
            this.txtMappingInfo.BackColor = Color.FromArgb(20, 40, 60);
            this.txtMappingInfo.ForeColor = Color.FromArgb(200, 200, 200);
            this.txtMappingInfo.Font = new Font("Consolas", 9F);
            this.txtMappingInfo.ScrollBars = RichTextBoxScrollBars.Both;
            this.txtMappingInfo.ReadOnly = true;
            this.txtMappingInfo.WordWrap = false;
            this.txtMappingInfo.DetectUrls = false;
            this.txtMappingInfo.BorderStyle = BorderStyle.FixedSingle;

            panelRightContainer.Controls.Add(this.txtMappingInfo);
            panelRightContainer.Controls.Add(this.lblMappingTitle);
            this.panelRight.Controls.Add(panelRightContainer);

            // Splitter
            this.splitterRight.Dock = DockStyle.Right;
            this.splitterRight.Width = 6;
            this.splitterRight.BackColor = Color.FromArgb(0, 80, 150);
            this.splitterRight.MinSize = 250;

            // panelCenter - Предпросмотр
            this.panelCenter.Dock = DockStyle.Fill;
            this.panelCenter.BackColor = Color.FromArgb(20, 50, 95);
            this.panelCenter.Padding = new Padding(5);

            this.lblPreviewTitle.Text = "Предпросмотр шаблона";
            this.lblPreviewTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblPreviewTitle.ForeColor = Color.White;
            this.lblPreviewTitle.Dock = DockStyle.Top;
            this.lblPreviewTitle.Height = 30;
            this.lblPreviewTitle.TextAlign = ContentAlignment.MiddleLeft;

            this.txtTemplatePreview.Dock = DockStyle.Fill;
            this.txtTemplatePreview.BackColor = Color.FromArgb(30, 30, 30);
            this.txtTemplatePreview.ForeColor = Color.FromArgb(200, 200, 200);
            this.txtTemplatePreview.Font = new Font("Consolas", 10F);
            this.txtTemplatePreview.Multiline = true;
            this.txtTemplatePreview.ScrollBars = ScrollBars.Both;
            this.txtTemplatePreview.ReadOnly = true;
            this.txtTemplatePreview.WordWrap = false;

            this.panelCenter.Controls.Add(this.lblPreviewTitle);
            this.panelCenter.Controls.Add(this.txtTemplatePreview);

            // Сборка panelMain
            this.panelMain.Controls.Add(this.panelCenter);
            this.panelMain.Controls.Add(this.splitterRight);
            this.panelMain.Controls.Add(this.panelRight);
            this.panelMain.Controls.Add(this.panelLeft);

            // panelButtons
            this.panelButtons.Dock = DockStyle.Bottom;
            this.panelButtons.Height = 55;
            this.panelButtons.BackColor = Color.FromArgb(0, 80, 150);
            this.panelButtons.Padding = new Padding(10, 8, 10, 8);

            this.tableLayoutButtons.Dock = DockStyle.Fill;
            this.tableLayoutButtons.BackColor = Color.Transparent;
            this.tableLayoutButtons.ColumnCount = 3;
            this.tableLayoutButtons.RowCount = 1;
            this.tableLayoutButtons.AutoSize = false;
            this.tableLayoutButtons.Padding = new Padding(0);

            this.tableLayoutButtons.ColumnStyles.Clear();
            this.tableLayoutButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            this.tableLayoutButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            this.tableLayoutButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));

            var leftPanel = new FlowLayoutPanel();
            leftPanel.Dock = DockStyle.Fill;
            leftPanel.FlowDirection = FlowDirection.LeftToRight;
            leftPanel.WrapContents = true;
            leftPanel.BackColor = Color.Transparent;
            leftPanel.Margin = new Padding(0);
            leftPanel.AutoSize = true;
            leftPanel.Padding = new Padding(0);

            var centerPanel = new FlowLayoutPanel();
            centerPanel.Dock = DockStyle.Fill;
            centerPanel.FlowDirection = FlowDirection.LeftToRight;
            centerPanel.WrapContents = true;
            centerPanel.BackColor = Color.Transparent;
            centerPanel.Margin = new Padding(0);
            centerPanel.AutoSize = true;
            centerPanel.Padding = new Padding(0);

            var rightPanel = new FlowLayoutPanel();
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.FlowDirection = FlowDirection.RightToLeft;
            rightPanel.WrapContents = true;
            rightPanel.BackColor = Color.Transparent;
            rightPanel.Margin = new Padding(0);
            rightPanel.AutoSize = true;
            rightPanel.Padding = new Padding(0);

            // Кнопка "Загрузить шаблон"
            this.btnLoadTemplate.Text = "Загрузить шаблон";
            this.btnLoadTemplate.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnLoadTemplate.BackColor = Color.FromArgb(52, 152, 219);
            this.btnLoadTemplate.ForeColor = Color.White;
            this.btnLoadTemplate.FlatStyle = FlatStyle.Flat;
            this.btnLoadTemplate.FlatAppearance.BorderSize = 0;
            this.btnLoadTemplate.Size = new Size(135, 35);
            this.btnLoadTemplate.MinimumSize = new Size(100, 35);
            this.btnLoadTemplate.Cursor = Cursors.Hand;
            this.btnLoadTemplate.TextAlign = ContentAlignment.MiddleCenter;
            this.btnLoadTemplate.Padding = new Padding(0);
            this.btnLoadTemplate.UseVisualStyleBackColor = false;
            this.btnLoadTemplate.Click += btnLoadTemplate_Click;

            // Кнопка "Загрузить экземпляры"
            this.btnLoadExcel.Text = "Загрузить экземпляры";
            this.btnLoadExcel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnLoadExcel.BackColor = Color.FromArgb(46, 204, 113);
            this.btnLoadExcel.ForeColor = Color.White;
            this.btnLoadExcel.FlatStyle = FlatStyle.Flat;
            this.btnLoadExcel.FlatAppearance.BorderSize = 0;
            this.btnLoadExcel.Size = new Size(160, 35);
            this.btnLoadExcel.MinimumSize = new Size(120, 35);
            this.btnLoadExcel.Cursor = Cursors.Hand;
            this.btnLoadExcel.TextAlign = ContentAlignment.MiddleCenter;
            this.btnLoadExcel.Padding = new Padding(0);
            this.btnLoadExcel.UseVisualStyleBackColor = false;
            this.btnLoadExcel.Click += btnLoadExcel_Click;

            // Кнопка "Проверить"
            this.btnValidate.Text = "Проверить";
            this.btnValidate.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnValidate.BackColor = Color.FromArgb(39, 174, 96);
            this.btnValidate.ForeColor = Color.White;
            this.btnValidate.FlatStyle = FlatStyle.Flat;
            this.btnValidate.FlatAppearance.BorderSize = 0;
            this.btnValidate.Size = new Size(100, 35);
            this.btnValidate.MinimumSize = new Size(80, 35);
            this.btnValidate.Cursor = Cursors.Hand;
            this.btnValidate.TextAlign = ContentAlignment.MiddleCenter;
            this.btnValidate.Padding = new Padding(0);
            this.btnValidate.UseVisualStyleBackColor = false;
            this.btnValidate.Click += btnValidate_Click;

            // Кнопка "Преобразование"
            this.btnConvertTags.Text = "Преобразование Вкл";
            this.btnConvertTags.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnConvertTags.BackColor = Color.FromArgb(46, 204, 113);
            this.btnConvertTags.ForeColor = Color.White;
            this.btnConvertTags.FlatStyle = FlatStyle.Flat;
            this.btnConvertTags.FlatAppearance.BorderSize = 0;
            this.btnConvertTags.Size = new Size(160, 35);
            this.btnConvertTags.MinimumSize = new Size(120, 35);
            this.btnConvertTags.Cursor = Cursors.Hand;
            this.btnConvertTags.TextAlign = ContentAlignment.MiddleCenter;
            this.btnConvertTags.Padding = new Padding(0);
            this.btnConvertTags.UseVisualStyleBackColor = false;
            this.btnConvertTags.Click += btnConvertTags_Click;

            // Кнопка "Сгенерировать"
            this.btnGenerate.Text = "Сгенерировать";
            this.btnGenerate.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnGenerate.BackColor = Color.FromArgb(243, 156, 18);
            this.btnGenerate.ForeColor = Color.White;
            this.btnGenerate.FlatStyle = FlatStyle.Flat;
            this.btnGenerate.FlatAppearance.BorderSize = 0;
            this.btnGenerate.Size = new Size(120, 35);
            this.btnGenerate.MinimumSize = new Size(90, 35);
            this.btnGenerate.Cursor = Cursors.Hand;
            this.btnGenerate.TextAlign = ContentAlignment.MiddleCenter;
            this.btnGenerate.Padding = new Padding(0);
            this.btnGenerate.UseVisualStyleBackColor = false;
            this.btnGenerate.Click += btnGenerate_Click;

            // Кнопка "Очистить все"
            this.btnClearAll.Text = "Очистить все";
            this.btnClearAll.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnClearAll.BackColor = Color.FromArgb(192, 57, 43);
            this.btnClearAll.ForeColor = Color.White;
            this.btnClearAll.FlatStyle = FlatStyle.Flat;
            this.btnClearAll.FlatAppearance.BorderSize = 0;
            this.btnClearAll.Size = new Size(100, 35);
            this.btnClearAll.MinimumSize = new Size(80, 35);
            this.btnClearAll.Cursor = Cursors.Hand;
            this.btnClearAll.TextAlign = ContentAlignment.MiddleCenter;
            this.btnClearAll.Padding = new Padding(0);
            this.btnClearAll.UseVisualStyleBackColor = false;
            this.btnClearAll.Click += btnClearAll_Click;

            leftPanel.Controls.Add(this.btnLoadTemplate);
            leftPanel.Controls.Add(this.btnLoadExcel);

            centerPanel.Controls.Add(this.btnValidate);
            centerPanel.Controls.Add(this.btnConvertTags);

            rightPanel.Controls.Add(this.btnClearAll);
            rightPanel.Controls.Add(this.btnGenerate);

            this.tableLayoutButtons.Controls.Add(leftPanel, 0, 0);
            this.tableLayoutButtons.Controls.Add(centerPanel, 1, 0);
            this.tableLayoutButtons.Controls.Add(rightPanel, 2, 0);

            this.panelButtons.Controls.Add(this.tableLayoutButtons);

            // panelBottom - Лог
            this.panelBottom.Dock = DockStyle.Bottom;
            this.panelBottom.Height = 150;
            this.panelBottom.BackColor = Color.FromArgb(10, 25, 45);
            this.panelBottom.Padding = new Padding(5);

            this.lblLogTitle.Text = "Окно событий и ошибок";
            this.lblLogTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblLogTitle.ForeColor = Color.FromArgb(52, 152, 219);
            this.lblLogTitle.Dock = DockStyle.Top;
            this.lblLogTitle.Height = 25;
            this.lblLogTitle.TextAlign = ContentAlignment.MiddleLeft;

            this.rtbLog.Dock = DockStyle.Fill;
            this.rtbLog.BackColor = Color.FromArgb(10, 25, 45);
            this.rtbLog.ForeColor = Color.White;
            this.rtbLog.Font = new Font("Consolas", 9F);
            this.rtbLog.BorderStyle = BorderStyle.None;
            this.rtbLog.ReadOnly = true;
            this.rtbLog.ScrollBars = RichTextBoxScrollBars.Vertical;

            this.panelBottom.Controls.Add(this.rtbLog);
            this.panelBottom.Controls.Add(this.lblLogTitle);

            // Добавление элементов на форму
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelButtons);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.menuStrip);

            this.dgvReplacements.CellValueChanged += this.dgvReplacements_CellValueChanged;
            this.dgvReplacements.CurrentCellDirtyStateChanged += this.dgvReplacements_CurrentCellDirtyStateChanged;

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}