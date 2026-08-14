using ModbusDataParser.Models;

namespace ModbusDataParser.Forms
{
    public partial class ScadaExportSettingsForm : Form
    {
        public ScadaExportSettings Settings { get; private set; } = new();

        private TextBox _txtSubsystemName = new();
        private NumericUpDown _numObjectNumber = new();
        private NumericUpDown _numArchivePeriod = new();
        private NumericUpDown _numSliceMask = new();
        private TextBox _txtClassifier = new();
        private TextBox _txtEventGroup = new();
        private TextBox _txtController = new();
        private CheckBox _chkAddInterface = new();
        private Button _btnOk = new();
        private Button _btnCancel = new();

        public ScadaExportSettingsForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "SCADA Export Settings";
            this.Size = new Size(500, 420);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 10,
                Padding = new Padding(10),
                AutoSize = true
            };

            int row = 0;
            AddLabelAndControl(mainPanel, row++, "Subsystem Name:",
                _txtSubsystemName = new TextBox { Width = 250, Text = "ASU", Anchor = AnchorStyles.Left });

            AddLabelAndControl(mainPanel, row++, "Object Number:",
                _numObjectNumber = new NumericUpDown { Width = 250, Minimum = 0, Maximum = 9999, Value = 0, Anchor = AnchorStyles.Left });

            AddLabelAndControl(mainPanel, row++, "Archive Period:",
                _numArchivePeriod = new NumericUpDown { Width = 250, Minimum = 0, Maximum = 9999, Value = 0, Anchor = AnchorStyles.Left });

            AddLabelAndControl(mainPanel, row++, "Slice Mask:",
                _numSliceMask = new NumericUpDown { Width = 250, Minimum = 0, Maximum = 9999, Value = 1, Anchor = AnchorStyles.Left });

            AddLabelAndControl(mainPanel, row++, "Classifier:",
                _txtClassifier = new TextBox { Width = 250, Text = "[ВСЕ]", Anchor = AnchorStyles.Left });

            AddLabelAndControl(mainPanel, row++, "Event Group:",
                _txtEventGroup = new TextBox { Width = 250, Text = "[ВСЕ]", Anchor = AnchorStyles.Left });

            AddLabelAndControl(mainPanel, row++, "Controller:",
                _txtController = new TextBox { Width = 250, Text = "", Anchor = AnchorStyles.Left });

            _chkAddInterface = new CheckBox
            {
                Text = "Add Interface Parameters",
                Checked = true,
                AutoSize = true,
                Margin = new Padding(10, 10, 0, 0)
            };
            mainPanel.Controls.Add(_chkAddInterface, 0, row);
            mainPanel.SetColumnSpan(_chkAddInterface, 2);
            row++;

            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                Padding = new Padding(0, 10, 0, 0)
            };

            _btnOk = new Button { Text = "OK", Size = new Size(100, 35), DialogResult = DialogResult.OK };
            _btnOk.Click += BtnOk_Click;

            _btnCancel = new Button { Text = "Cancel", Size = new Size(100, 35), DialogResult = DialogResult.Cancel };

            buttonPanel.Controls.Add(_btnOk);
            buttonPanel.Controls.Add(_btnCancel);
            mainPanel.Controls.Add(buttonPanel, 0, row);
            mainPanel.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(mainPanel);
        }

        private void AddLabelAndControl(TableLayoutPanel panel, int row, string labelText, Control control)
        {
            var label = new Label
            {
                Text = labelText,
                AutoSize = true,
                Margin = new Padding(0, 5, 10, 0),
                TextAlign = ContentAlignment.MiddleLeft
            };

            panel.Controls.Add(label, 0, row);
            panel.Controls.Add(control, 1, row);
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            Settings = new ScadaExportSettings
            {
                SubsystemName = _txtSubsystemName.Text ?? "",
                ObjectNumber = (int)(_numObjectNumber.Value),
                ArchivePeriod = (int)(_numArchivePeriod.Value),
                SliceMask = (int)(_numSliceMask.Value),
                Classifier = _txtClassifier.Text ?? "[ВСЕ]",
                EventGroup = _txtEventGroup.Text ?? "[ВСЕ]",
                Controller = _txtController.Text ?? "",
                AddInterfaceParameters = _chkAddInterface.Checked
            };
        }
    }
}
