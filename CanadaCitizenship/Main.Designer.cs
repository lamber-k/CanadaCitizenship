
namespace CanadaCitizenship
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            temporaryDTP = new DateTimePicker();
            firstEntryLabel = new Label();
            computeButton = new Button();
            PRLabel = new Label();
            permanentResidencyDTP = new DateTimePicker();
            profilesComboBox = new ComboBox();
            profileBindingSource = new BindingSource(components);
            newProfileButton = new Button();
            newProfileNameTextBox = new TextBox();
            deleteProfileButton = new Button();
            currentProfileLabel = new Label();
            excludedPeriodsDataGrid = new DataGridView();
            Begin = new DataGridViewTextBoxColumn();
            End = new DataGridViewTextBoxColumn();
            Delete = new DataGridViewButtonColumn();
            OOCBeginDTP = new DateTimePicker();
            OOCEndDTP = new DateTimePicker();
            outOfCountryBeginLabel = new Label();
            outOfCountryEndLabel = new Label();
            AddOOCButton = new Button();
            beginDateLabel = new Label();
            totalTemporaryLabel = new Label();
            totalPRLabel = new Label();
            remainingDaysLabel = new Label();
            projectedDateLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)profileBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)excludedPeriodsDataGrid).BeginInit();
            SuspendLayout();
            // 
            // temporaryDTP
            // 
            temporaryDTP.Format = DateTimePickerFormat.Short;
            temporaryDTP.Location = new Point(189, 13);
            temporaryDTP.Name = "temporaryDTP";
            temporaryDTP.Size = new Size(200, 23);
            temporaryDTP.TabIndex = 0;
            temporaryDTP.ValueChanged += temporaryDTP_ValueChanged;
            // 
            // firstEntryLabel
            // 
            firstEntryLabel.AutoSize = true;
            firstEntryLabel.Location = new Point(12, 19);
            firstEntryLabel.Name = "firstEntryLabel";
            firstEntryLabel.Size = new Size(59, 15);
            firstEntryLabel.TabIndex = 1;
            firstEntryLabel.Text = "First Entry";
            // 
            // computeButton
            // 
            computeButton.Location = new Point(358, 391);
            computeButton.Name = "computeButton";
            computeButton.Size = new Size(75, 23);
            computeButton.TabIndex = 2;
            computeButton.Text = "Compute";
            computeButton.UseVisualStyleBackColor = true;
            computeButton.Click += computeButton_Click;
            // 
            // PRLabel
            // 
            PRLabel.AutoSize = true;
            PRLabel.Location = new Point(12, 62);
            PRLabel.Name = "PRLabel";
            PRLabel.Size = new Size(121, 15);
            PRLabel.TabIndex = 3;
            PRLabel.Text = "Permanent Residency";
            // 
            // permanentResidencyDTP
            // 
            permanentResidencyDTP.Format = DateTimePickerFormat.Short;
            permanentResidencyDTP.Location = new Point(189, 54);
            permanentResidencyDTP.Name = "permanentResidencyDTP";
            permanentResidencyDTP.Size = new Size(200, 23);
            permanentResidencyDTP.TabIndex = 4;
            permanentResidencyDTP.ValueChanged += permanentResidencyDTP_ValueChanged;
            // 
            // profilesComboBox
            // 
            profilesComboBox.FormattingEnabled = true;
            profilesComboBox.Location = new Point(667, 19);
            profilesComboBox.Name = "profilesComboBox";
            profilesComboBox.Size = new Size(121, 23);
            profilesComboBox.TabIndex = 5;
            profilesComboBox.SelectedValueChanged += profilesComboBox_SelectedValueChanged;
            // 
            // profileBindingSource
            // 
            profileBindingSource.DataSource = typeof(Profile);
            // 
            // newProfileButton
            // 
            newProfileButton.Location = new Point(584, 129);
            newProfileButton.Name = "newProfileButton";
            newProfileButton.Size = new Size(99, 23);
            newProfileButton.TabIndex = 6;
            newProfileButton.Text = "New Profile";
            newProfileButton.UseVisualStyleBackColor = true;
            newProfileButton.Click += newProfileButton_Click;
            // 
            // newProfileNameTextBox
            // 
            newProfileNameTextBox.Location = new Point(584, 100);
            newProfileNameTextBox.Name = "newProfileNameTextBox";
            newProfileNameTextBox.Size = new Size(100, 23);
            newProfileNameTextBox.TabIndex = 7;
            // 
            // deleteProfileButton
            // 
            deleteProfileButton.Location = new Point(689, 129);
            deleteProfileButton.Name = "deleteProfileButton";
            deleteProfileButton.Size = new Size(99, 23);
            deleteProfileButton.TabIndex = 8;
            deleteProfileButton.Text = "Delete Profile";
            deleteProfileButton.UseVisualStyleBackColor = true;
            deleteProfileButton.Click += deleteProfileButton_Click;
            // 
            // currentProfileLabel
            // 
            currentProfileLabel.AutoSize = true;
            currentProfileLabel.Location = new Point(577, 22);
            currentProfileLabel.Name = "currentProfileLabel";
            currentProfileLabel.Size = new Size(84, 15);
            currentProfileLabel.TabIndex = 9;
            currentProfileLabel.Text = "Current Profile";
            // 
            // excludedPeriodsDataGrid
            // 
            excludedPeriodsDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            excludedPeriodsDataGrid.Columns.AddRange(new DataGridViewColumn[] { Begin, End, Delete });
            excludedPeriodsDataGrid.Location = new Point(12, 129);
            excludedPeriodsDataGrid.Name = "excludedPeriodsDataGrid";
            excludedPeriodsDataGrid.Size = new Size(383, 209);
            excludedPeriodsDataGrid.TabIndex = 10;
            excludedPeriodsDataGrid.CellClick += excludedPeriodsDataGrid_CellClick;
            // 
            // Begin
            // 
            Begin.DataPropertyName = "begin";
            dataGridViewCellStyle1.Format = "d";
            dataGridViewCellStyle1.NullValue = null;
            Begin.DefaultCellStyle = dataGridViewCellStyle1;
            Begin.Frozen = true;
            Begin.HeaderText = "Begin";
            Begin.Name = "Begin";
            Begin.Width = 120;
            // 
            // End
            // 
            End.DataPropertyName = "end";
            dataGridViewCellStyle2.Format = "d";
            dataGridViewCellStyle2.NullValue = null;
            End.DefaultCellStyle = dataGridViewCellStyle2;
            End.Frozen = true;
            End.HeaderText = "End";
            End.Name = "End";
            End.Width = 120;
            // 
            // Delete
            // 
            Delete.Frozen = true;
            Delete.HeaderText = "Delete";
            Delete.Name = "Delete";
            Delete.Text = "Delete";
            Delete.UseColumnTextForButtonValue = true;
            // 
            // OOCBeginDTP
            // 
            OOCBeginDTP.Format = DateTimePickerFormat.Short;
            OOCBeginDTP.Location = new Point(55, 344);
            OOCBeginDTP.Name = "OOCBeginDTP";
            OOCBeginDTP.Size = new Size(121, 23);
            OOCBeginDTP.TabIndex = 11;
            // 
            // OOCEndDTP
            // 
            OOCEndDTP.Format = DateTimePickerFormat.Short;
            OOCEndDTP.Location = new Point(274, 344);
            OOCEndDTP.Name = "OOCEndDTP";
            OOCEndDTP.Size = new Size(121, 23);
            OOCEndDTP.TabIndex = 12;
            // 
            // outOfCountryBeginLabel
            // 
            outOfCountryBeginLabel.AutoSize = true;
            outOfCountryBeginLabel.Location = new Point(12, 350);
            outOfCountryBeginLabel.Name = "outOfCountryBeginLabel";
            outOfCountryBeginLabel.Size = new Size(37, 15);
            outOfCountryBeginLabel.TabIndex = 13;
            outOfCountryBeginLabel.Text = "Begin";
            // 
            // outOfCountryEndLabel
            // 
            outOfCountryEndLabel.AutoSize = true;
            outOfCountryEndLabel.Location = new Point(241, 350);
            outOfCountryEndLabel.Name = "outOfCountryEndLabel";
            outOfCountryEndLabel.Size = new Size(27, 15);
            outOfCountryEndLabel.TabIndex = 14;
            outOfCountryEndLabel.Text = "End";
            // 
            // AddOOCButton
            // 
            AddOOCButton.Location = new Point(12, 373);
            AddOOCButton.Name = "AddOOCButton";
            AddOOCButton.Size = new Size(75, 23);
            AddOOCButton.TabIndex = 15;
            AddOOCButton.Text = "Add";
            AddOOCButton.UseVisualStyleBackColor = true;
            AddOOCButton.Click += AddOOCButton_Click;
            // 
            // beginDateLabel
            // 
            beginDateLabel.AutoSize = true;
            beginDateLabel.Location = new Point(469, 187);
            beginDateLabel.Name = "beginDateLabel";
            beginDateLabel.Size = new Size(67, 15);
            beginDateLabel.TabIndex = 16;
            beginDateLabel.Text = "Begin Date:";
            beginDateLabel.Visible = false;
            // 
            // totalTemporaryLabel
            // 
            totalTemporaryLabel.AutoSize = true;
            totalTemporaryLabel.Location = new Point(469, 215);
            totalTemporaryLabel.Name = "totalTemporaryLabel";
            totalTemporaryLabel.Size = new Size(94, 15);
            totalTemporaryLabel.TabIndex = 17;
            totalTemporaryLabel.Text = "Total Temporary:";
            totalTemporaryLabel.Visible = false;
            // 
            // totalPRLabel
            // 
            totalPRLabel.AutoSize = true;
            totalPRLabel.Location = new Point(469, 245);
            totalPRLabel.Name = "totalPRLabel";
            totalPRLabel.Size = new Size(52, 15);
            totalPRLabel.TabIndex = 18;
            totalPRLabel.Text = "Total PR:";
            totalPRLabel.Visible = false;
            // 
            // remainingDaysLabel
            // 
            remainingDaysLabel.AutoSize = true;
            remainingDaysLabel.Location = new Point(469, 273);
            remainingDaysLabel.Name = "remainingDaysLabel";
            remainingDaysLabel.Size = new Size(94, 15);
            remainingDaysLabel.TabIndex = 19;
            remainingDaysLabel.Text = "Remaining days:";
            remainingDaysLabel.Visible = false;
            // 
            // projectedDateLabel
            // 
            projectedDateLabel.AutoSize = true;
            projectedDateLabel.Location = new Point(469, 301);
            projectedDateLabel.Name = "projectedDateLabel";
            projectedDateLabel.Size = new Size(74, 15);
            projectedDateLabel.TabIndex = 20;
            projectedDateLabel.Text = "Project Date:";
            projectedDateLabel.Visible = false;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(projectedDateLabel);
            Controls.Add(remainingDaysLabel);
            Controls.Add(totalPRLabel);
            Controls.Add(totalTemporaryLabel);
            Controls.Add(beginDateLabel);
            Controls.Add(AddOOCButton);
            Controls.Add(outOfCountryEndLabel);
            Controls.Add(outOfCountryBeginLabel);
            Controls.Add(OOCEndDTP);
            Controls.Add(OOCBeginDTP);
            Controls.Add(excludedPeriodsDataGrid);
            Controls.Add(currentProfileLabel);
            Controls.Add(deleteProfileButton);
            Controls.Add(newProfileNameTextBox);
            Controls.Add(newProfileButton);
            Controls.Add(profilesComboBox);
            Controls.Add(permanentResidencyDTP);
            Controls.Add(PRLabel);
            Controls.Add(computeButton);
            Controls.Add(firstEntryLabel);
            Controls.Add(temporaryDTP);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Main";
            Text = "Citizenship Calc";
            FormClosing += Form1_FormClosing;
            ((System.ComponentModel.ISupportInitialize)profileBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)excludedPeriodsDataGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DateTimePicker temporaryDTP;
        private Label firstEntryLabel;
        private Button computeButton;
        private Label PRLabel;
        private DateTimePicker permanentResidencyDTP;
        private ComboBox profilesComboBox;
        private Button newProfileButton;
        private TextBox newProfileNameTextBox;
        private Button deleteProfileButton;
        private Label currentProfileLabel;
        private BindingSource profileBindingSource;
        private DataGridView excludedPeriodsDataGrid;
        private DateTimePicker OOCBeginDTP;
        private DateTimePicker OOCEndDTP;
        private Label outOfCountryBeginLabel;
        private Label outOfCountryEndLabel;
        private Button AddOOCButton;
        private DataGridViewTextBoxColumn Begin;
        private DataGridViewTextBoxColumn End;
        private DataGridViewButtonColumn Delete;
        private Label beginDateLabel;
        private Label totalTemporaryLabel;
        private Label totalPRLabel;
        private Label remainingDaysLabel;
        private Label projectedDateLabel;
    }
}
