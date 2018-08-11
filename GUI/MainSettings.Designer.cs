namespace GUI
{
	partial class MainSettings
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.mapDescriptionFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.generalSettingsGroupBox = new System.Windows.Forms.GroupBox();
			this.useRandomSeedCheckbox = new System.Windows.Forms.CheckBox();
			this.generatorSeedInput = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.numberOfLayoutsInput = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.mapDescriptionGroupBox = new System.Windows.Forms.GroupBox();
			this.uploadButton = new System.Windows.Forms.Button();
			this.uploadOrLabel = new System.Windows.Forms.Label();
			this.loadedMapDescriptionsComboBox = new System.Windows.Forms.ComboBox();
			this.chooseMapDescriptionLabel = new System.Windows.Forms.Label();
			this.infoGroupBox = new System.Windows.Forms.GroupBox();
			this.usedDescriptionInfoPanel = new System.Windows.Forms.Panel();
			this.usedDescriptionPassagesCount = new System.Windows.Forms.Label();
			this.usedDescriptionRoomsCount = new System.Windows.Forms.Label();
			this.usedDescription = new System.Windows.Forms.Label();
			this.descriptionNotChosen = new System.Windows.Forms.Label();
			this.generalSettingsPanel = new System.Windows.Forms.Panel();
			this.mainPanel = new System.Windows.Forms.Panel();
			this.drawingSettingsPanel = new System.Windows.Forms.Panel();
			this.drawingSettingsGroupBox = new System.Windows.Forms.GroupBox();
			this.label5 = new System.Windows.Forms.Label();
			this.fixedSquareExportValue = new System.Windows.Forms.NumericUpDown();
			this.fixedSquareExportCheckbox = new System.Windows.Forms.CheckBox();
			this.label8 = new System.Windows.Forms.Label();
			this.useOldPaperStyleCheckbox = new System.Windows.Forms.CheckBox();
			this.fixedFontSizeValue = new System.Windows.Forms.NumericUpDown();
			this.showRoomNamesCheckbox = new System.Windows.Forms.CheckBox();
			this.fixedFontSizeCheckbox = new System.Windows.Forms.CheckBox();
			this.progressPanel = new System.Windows.Forms.Panel();
			this.progressGroupBox = new System.Windows.Forms.GroupBox();
			this.exportShownLayoutsCheckbox = new System.Windows.Forms.CheckBox();
			this.label6 = new System.Windows.Forms.Label();
			this.showPerturbedLayoutsTime = new System.Windows.Forms.NumericUpDown();
			this.showPerturbedLayouts = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.showPartialValidLayoutsTime = new System.Windows.Forms.NumericUpDown();
			this.showPartialValidLayouts = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.showFinalLayoutsTime = new System.Windows.Forms.NumericUpDown();
			this.showFinalLayouts = new System.Windows.Forms.CheckBox();
			this.infoPanel = new System.Windows.Forms.Panel();
			this.mapDescriptionPanel = new System.Windows.Forms.Panel();
			this.generateButtonPanel = new System.Windows.Forms.Panel();
			this.generateButton = new System.Windows.Forms.Button();
			this.generalSettingsGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.generatorSeedInput)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numberOfLayoutsInput)).BeginInit();
			this.mapDescriptionGroupBox.SuspendLayout();
			this.infoGroupBox.SuspendLayout();
			this.usedDescriptionInfoPanel.SuspendLayout();
			this.generalSettingsPanel.SuspendLayout();
			this.mainPanel.SuspendLayout();
			this.drawingSettingsPanel.SuspendLayout();
			this.drawingSettingsGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.fixedSquareExportValue)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.fixedFontSizeValue)).BeginInit();
			this.progressPanel.SuspendLayout();
			this.progressGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.showPerturbedLayoutsTime)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.showPartialValidLayoutsTime)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.showFinalLayoutsTime)).BeginInit();
			this.infoPanel.SuspendLayout();
			this.mapDescriptionPanel.SuspendLayout();
			this.generateButtonPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// mapDescriptionFileDialog
			// 
			this.mapDescriptionFileDialog.FileName = "openFileDialog1";
			// 
			// generalSettingsGroupBox
			// 
			this.generalSettingsGroupBox.Controls.Add(this.useRandomSeedCheckbox);
			this.generalSettingsGroupBox.Controls.Add(this.generatorSeedInput);
			this.generalSettingsGroupBox.Controls.Add(this.label4);
			this.generalSettingsGroupBox.Controls.Add(this.numberOfLayoutsInput);
			this.generalSettingsGroupBox.Controls.Add(this.label3);
			this.generalSettingsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.generalSettingsGroupBox.Location = new System.Drawing.Point(0, 0);
			this.generalSettingsGroupBox.Margin = new System.Windows.Forms.Padding(5);
			this.generalSettingsGroupBox.Name = "generalSettingsGroupBox";
			this.generalSettingsGroupBox.Size = new System.Drawing.Size(541, 92);
			this.generalSettingsGroupBox.TabIndex = 0;
			this.generalSettingsGroupBox.TabStop = false;
			this.generalSettingsGroupBox.Text = "General settings";
			// 
			// useRandomSeedCheckbox
			// 
			this.useRandomSeedCheckbox.AutoSize = true;
			this.useRandomSeedCheckbox.Location = new System.Drawing.Point(417, 59);
			this.useRandomSeedCheckbox.Name = "useRandomSeedCheckbox";
			this.useRandomSeedCheckbox.Size = new System.Drawing.Size(105, 21);
			this.useRandomSeedCheckbox.TabIndex = 4;
			this.useRandomSeedCheckbox.Text = "use random";
			this.useRandomSeedCheckbox.UseVisualStyleBackColor = true;
			// 
			// generatorSeedInput
			// 
			this.generatorSeedInput.Location = new System.Drawing.Point(239, 55);
			this.generatorSeedInput.Name = "generatorSeedInput";
			this.generatorSeedInput.Size = new System.Drawing.Size(120, 22);
			this.generatorSeedInput.TabIndex = 3;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(8, 60);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(166, 17);
			this.label4.TabIndex = 2;
			this.label4.Text = "Random generator seed:";
			// 
			// numberOfLayoutsInput
			// 
			this.numberOfLayoutsInput.Location = new System.Drawing.Point(239, 27);
			this.numberOfLayoutsInput.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numberOfLayoutsInput.Name = "numberOfLayoutsInput";
			this.numberOfLayoutsInput.Size = new System.Drawing.Size(120, 22);
			this.numberOfLayoutsInput.TabIndex = 1;
			this.numberOfLayoutsInput.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(8, 32);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(127, 17);
			this.label3.TabIndex = 0;
			this.label3.Text = "Number of layouts:";
			// 
			// mapDescriptionGroupBox
			// 
			this.mapDescriptionGroupBox.Controls.Add(this.uploadButton);
			this.mapDescriptionGroupBox.Controls.Add(this.uploadOrLabel);
			this.mapDescriptionGroupBox.Controls.Add(this.loadedMapDescriptionsComboBox);
			this.mapDescriptionGroupBox.Controls.Add(this.chooseMapDescriptionLabel);
			this.mapDescriptionGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mapDescriptionGroupBox.Location = new System.Drawing.Point(0, 0);
			this.mapDescriptionGroupBox.Name = "mapDescriptionGroupBox";
			this.mapDescriptionGroupBox.Size = new System.Drawing.Size(541, 126);
			this.mapDescriptionGroupBox.TabIndex = 0;
			this.mapDescriptionGroupBox.TabStop = false;
			this.mapDescriptionGroupBox.Text = "Map description file";
			// 
			// uploadButton
			// 
			this.uploadButton.Location = new System.Drawing.Point(189, 81);
			this.uploadButton.Name = "uploadButton";
			this.uploadButton.Size = new System.Drawing.Size(144, 29);
			this.uploadButton.TabIndex = 3;
			this.uploadButton.Text = "Upload your own";
			this.uploadButton.UseVisualStyleBackColor = true;
			this.uploadButton.Click += new System.EventHandler(this.uploadButton_Click);
			// 
			// uploadOrLabel
			// 
			this.uploadOrLabel.AutoSize = true;
			this.uploadOrLabel.Location = new System.Drawing.Point(233, 55);
			this.uploadOrLabel.Name = "uploadOrLabel";
			this.uploadOrLabel.Size = new System.Drawing.Size(57, 17);
			this.uploadOrLabel.TabIndex = 2;
			this.uploadOrLabel.Text = "-- OR --";
			// 
			// loadedMapDescriptionsComboBox
			// 
			this.loadedMapDescriptionsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.loadedMapDescriptionsComboBox.FormattingEnabled = true;
			this.loadedMapDescriptionsComboBox.Location = new System.Drawing.Point(294, 25);
			this.loadedMapDescriptionsComboBox.Name = "loadedMapDescriptionsComboBox";
			this.loadedMapDescriptionsComboBox.Size = new System.Drawing.Size(212, 24);
			this.loadedMapDescriptionsComboBox.TabIndex = 1;
			this.loadedMapDescriptionsComboBox.SelectedIndexChanged += new System.EventHandler(this.loadedMapDescriptionsComboBox_SelectedIndexChanged);
			// 
			// chooseMapDescriptionLabel
			// 
			this.chooseMapDescriptionLabel.AutoSize = true;
			this.chooseMapDescriptionLabel.Location = new System.Drawing.Point(100, 28);
			this.chooseMapDescriptionLabel.Name = "chooseMapDescriptionLabel";
			this.chooseMapDescriptionLabel.Size = new System.Drawing.Size(143, 17);
			this.chooseMapDescriptionLabel.TabIndex = 0;
			this.chooseMapDescriptionLabel.Text = "Choose from existing:";
			// 
			// infoGroupBox
			// 
			this.infoGroupBox.Controls.Add(this.usedDescriptionInfoPanel);
			this.infoGroupBox.Controls.Add(this.descriptionNotChosen);
			this.infoGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.infoGroupBox.Location = new System.Drawing.Point(0, 0);
			this.infoGroupBox.Name = "infoGroupBox";
			this.infoGroupBox.Size = new System.Drawing.Size(541, 107);
			this.infoGroupBox.TabIndex = 1;
			this.infoGroupBox.TabStop = false;
			this.infoGroupBox.Text = "Info";
			// 
			// usedDescriptionInfoPanel
			// 
			this.usedDescriptionInfoPanel.Controls.Add(this.usedDescriptionPassagesCount);
			this.usedDescriptionInfoPanel.Controls.Add(this.usedDescriptionRoomsCount);
			this.usedDescriptionInfoPanel.Controls.Add(this.usedDescription);
			this.usedDescriptionInfoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.usedDescriptionInfoPanel.Location = new System.Drawing.Point(3, 18);
			this.usedDescriptionInfoPanel.Name = "usedDescriptionInfoPanel";
			this.usedDescriptionInfoPanel.Size = new System.Drawing.Size(535, 86);
			this.usedDescriptionInfoPanel.TabIndex = 1;
			this.usedDescriptionInfoPanel.Visible = false;
			// 
			// usedDescriptionPassagesCount
			// 
			this.usedDescriptionPassagesCount.AutoSize = true;
			this.usedDescriptionPassagesCount.Location = new System.Drawing.Point(11, 60);
			this.usedDescriptionPassagesCount.Name = "usedDescriptionPassagesCount";
			this.usedDescriptionPassagesCount.Size = new System.Drawing.Size(163, 17);
			this.usedDescriptionPassagesCount.TabIndex = 2;
			this.usedDescriptionPassagesCount.Text = "Number of passages: 16";
			// 
			// usedDescriptionRoomsCount
			// 
			this.usedDescriptionRoomsCount.AutoSize = true;
			this.usedDescriptionRoomsCount.Location = new System.Drawing.Point(11, 40);
			this.usedDescriptionRoomsCount.Name = "usedDescriptionRoomsCount";
			this.usedDescriptionRoomsCount.Size = new System.Drawing.Size(133, 17);
			this.usedDescriptionRoomsCount.TabIndex = 1;
			this.usedDescriptionRoomsCount.Text = "Number of rooms: 9";
			// 
			// usedDescription
			// 
			this.usedDescription.AutoSize = true;
			this.usedDescription.Location = new System.Drawing.Point(10, 11);
			this.usedDescription.Name = "usedDescription";
			this.usedDescription.Size = new System.Drawing.Size(237, 17);
			this.usedDescription.TabIndex = 0;
			this.usedDescription.Text = "Using uploaded map description file.";
			// 
			// descriptionNotChosen
			// 
			this.descriptionNotChosen.AutoSize = true;
			this.descriptionNotChosen.Location = new System.Drawing.Point(180, 52);
			this.descriptionNotChosen.Name = "descriptionNotChosen";
			this.descriptionNotChosen.Size = new System.Drawing.Size(182, 17);
			this.descriptionNotChosen.TabIndex = 0;
			this.descriptionNotChosen.Text = "Map description not chosen";
			this.descriptionNotChosen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// generalSettingsPanel
			// 
			this.generalSettingsPanel.Controls.Add(this.generalSettingsGroupBox);
			this.generalSettingsPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.generalSettingsPanel.Location = new System.Drawing.Point(5, 5);
			this.generalSettingsPanel.Name = "generalSettingsPanel";
			this.generalSettingsPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
			this.generalSettingsPanel.Size = new System.Drawing.Size(541, 102);
			this.generalSettingsPanel.TabIndex = 2;
			// 
			// mainPanel
			// 
			this.mainPanel.AutoScroll = true;
			this.mainPanel.Controls.Add(this.drawingSettingsPanel);
			this.mainPanel.Controls.Add(this.progressPanel);
			this.mainPanel.Controls.Add(this.infoPanel);
			this.mainPanel.Controls.Add(this.mapDescriptionPanel);
			this.mainPanel.Controls.Add(this.generalSettingsPanel);
			this.mainPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.mainPanel.Location = new System.Drawing.Point(0, 0);
			this.mainPanel.Name = "mainPanel";
			this.mainPanel.Padding = new System.Windows.Forms.Padding(5);
			this.mainPanel.Size = new System.Drawing.Size(572, 537);
			this.mainPanel.TabIndex = 4;
			// 
			// drawingSettingsPanel
			// 
			this.drawingSettingsPanel.Controls.Add(this.drawingSettingsGroupBox);
			this.drawingSettingsPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.drawingSettingsPanel.Location = new System.Drawing.Point(5, 512);
			this.drawingSettingsPanel.Name = "drawingSettingsPanel";
			this.drawingSettingsPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
			this.drawingSettingsPanel.Size = new System.Drawing.Size(541, 153);
			this.drawingSettingsPanel.TabIndex = 11;
			// 
			// drawingSettingsGroupBox
			// 
			this.drawingSettingsGroupBox.Controls.Add(this.label5);
			this.drawingSettingsGroupBox.Controls.Add(this.fixedSquareExportValue);
			this.drawingSettingsGroupBox.Controls.Add(this.fixedSquareExportCheckbox);
			this.drawingSettingsGroupBox.Controls.Add(this.label8);
			this.drawingSettingsGroupBox.Controls.Add(this.useOldPaperStyleCheckbox);
			this.drawingSettingsGroupBox.Controls.Add(this.fixedFontSizeValue);
			this.drawingSettingsGroupBox.Controls.Add(this.showRoomNamesCheckbox);
			this.drawingSettingsGroupBox.Controls.Add(this.fixedFontSizeCheckbox);
			this.drawingSettingsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.drawingSettingsGroupBox.Location = new System.Drawing.Point(0, 0);
			this.drawingSettingsGroupBox.Name = "drawingSettingsGroupBox";
			this.drawingSettingsGroupBox.Size = new System.Drawing.Size(541, 143);
			this.drawingSettingsGroupBox.TabIndex = 0;
			this.drawingSettingsGroupBox.TabStop = false;
			this.drawingSettingsGroupBox.Text = "Display settings";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(300, 115);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(22, 17);
			this.label5.TabIndex = 12;
			this.label5.Text = "px";
			// 
			// fixedSquareExportValue
			// 
			this.fixedSquareExportValue.Location = new System.Drawing.Point(223, 111);
			this.fixedSquareExportValue.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.fixedSquareExportValue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.fixedSquareExportValue.Name = "fixedSquareExportValue";
			this.fixedSquareExportValue.Size = new System.Drawing.Size(71, 22);
			this.fixedSquareExportValue.TabIndex = 14;
			this.fixedSquareExportValue.Value = new decimal(new int[] {
            800,
            0,
            0,
            0});
			// 
			// fixedSquareExportCheckbox
			// 
			this.fixedSquareExportCheckbox.AutoSize = true;
			this.fixedSquareExportCheckbox.Location = new System.Drawing.Point(6, 111);
			this.fixedSquareExportCheckbox.Name = "fixedSquareExportCheckbox";
			this.fixedSquareExportCheckbox.Size = new System.Drawing.Size(154, 21);
			this.fixedSquareExportCheckbox.TabIndex = 13;
			this.fixedSquareExportCheckbox.Text = "Fixed square export";
			this.fixedSquareExportCheckbox.UseVisualStyleBackColor = true;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(300, 88);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(22, 17);
			this.label8.TabIndex = 10;
			this.label8.Text = "px";
			// 
			// useOldPaperStyleCheckbox
			// 
			this.useOldPaperStyleCheckbox.AutoSize = true;
			this.useOldPaperStyleCheckbox.Location = new System.Drawing.Point(6, 57);
			this.useOldPaperStyleCheckbox.Name = "useOldPaperStyleCheckbox";
			this.useOldPaperStyleCheckbox.Size = new System.Drawing.Size(152, 21);
			this.useOldPaperStyleCheckbox.TabIndex = 8;
			this.useOldPaperStyleCheckbox.Text = "Use old paper style";
			this.useOldPaperStyleCheckbox.UseVisualStyleBackColor = true;
			// 
			// fixedFontSizeValue
			// 
			this.fixedFontSizeValue.Location = new System.Drawing.Point(223, 84);
			this.fixedFontSizeValue.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.fixedFontSizeValue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.fixedFontSizeValue.Name = "fixedFontSizeValue";
			this.fixedFontSizeValue.Size = new System.Drawing.Size(71, 22);
			this.fixedFontSizeValue.TabIndex = 11;
			this.fixedFontSizeValue.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
			// 
			// showRoomNamesCheckbox
			// 
			this.showRoomNamesCheckbox.AutoSize = true;
			this.showRoomNamesCheckbox.Location = new System.Drawing.Point(6, 30);
			this.showRoomNamesCheckbox.Name = "showRoomNamesCheckbox";
			this.showRoomNamesCheckbox.Size = new System.Drawing.Size(146, 21);
			this.showRoomNamesCheckbox.TabIndex = 7;
			this.showRoomNamesCheckbox.Text = "Show room names";
			this.showRoomNamesCheckbox.UseVisualStyleBackColor = true;
			// 
			// fixedFontSizeCheckbox
			// 
			this.fixedFontSizeCheckbox.AutoSize = true;
			this.fixedFontSizeCheckbox.Location = new System.Drawing.Point(6, 84);
			this.fixedFontSizeCheckbox.Name = "fixedFontSizeCheckbox";
			this.fixedFontSizeCheckbox.Size = new System.Drawing.Size(120, 21);
			this.fixedFontSizeCheckbox.TabIndex = 10;
			this.fixedFontSizeCheckbox.Text = "Fixed font size";
			this.fixedFontSizeCheckbox.UseVisualStyleBackColor = true;
			// 
			// progressPanel
			// 
			this.progressPanel.Controls.Add(this.progressGroupBox);
			this.progressPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.progressPanel.Location = new System.Drawing.Point(5, 360);
			this.progressPanel.Name = "progressPanel";
			this.progressPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
			this.progressPanel.Size = new System.Drawing.Size(541, 152);
			this.progressPanel.TabIndex = 6;
			// 
			// progressGroupBox
			// 
			this.progressGroupBox.Controls.Add(this.exportShownLayoutsCheckbox);
			this.progressGroupBox.Controls.Add(this.label6);
			this.progressGroupBox.Controls.Add(this.showPerturbedLayoutsTime);
			this.progressGroupBox.Controls.Add(this.showPerturbedLayouts);
			this.progressGroupBox.Controls.Add(this.label2);
			this.progressGroupBox.Controls.Add(this.showPartialValidLayoutsTime);
			this.progressGroupBox.Controls.Add(this.showPartialValidLayouts);
			this.progressGroupBox.Controls.Add(this.label1);
			this.progressGroupBox.Controls.Add(this.showFinalLayoutsTime);
			this.progressGroupBox.Controls.Add(this.showFinalLayouts);
			this.progressGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.progressGroupBox.Location = new System.Drawing.Point(0, 0);
			this.progressGroupBox.Name = "progressGroupBox";
			this.progressGroupBox.Size = new System.Drawing.Size(541, 142);
			this.progressGroupBox.TabIndex = 0;
			this.progressGroupBox.TabStop = false;
			this.progressGroupBox.Text = "Progress showing settings";
			// 
			// exportShownLayoutsCheckbox
			// 
			this.exportShownLayoutsCheckbox.AutoSize = true;
			this.exportShownLayoutsCheckbox.Location = new System.Drawing.Point(11, 109);
			this.exportShownLayoutsCheckbox.Name = "exportShownLayoutsCheckbox";
			this.exportShownLayoutsCheckbox.Size = new System.Drawing.Size(163, 21);
			this.exportShownLayoutsCheckbox.TabIndex = 10;
			this.exportShownLayoutsCheckbox.Text = "Export shown layouts";
			this.exportShownLayoutsCheckbox.UseVisualStyleBackColor = true;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(368, 84);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(26, 17);
			this.label6.TabIndex = 8;
			this.label6.Text = "ms";
			// 
			// showPerturbedLayoutsTime
			// 
			this.showPerturbedLayoutsTime.Location = new System.Drawing.Point(240, 82);
			this.showPerturbedLayoutsTime.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.showPerturbedLayoutsTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.showPerturbedLayoutsTime.Name = "showPerturbedLayoutsTime";
			this.showPerturbedLayoutsTime.Size = new System.Drawing.Size(120, 22);
			this.showPerturbedLayoutsTime.TabIndex = 7;
			this.showPerturbedLayoutsTime.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
			// 
			// showPerturbedLayouts
			// 
			this.showPerturbedLayouts.AutoSize = true;
			this.showPerturbedLayouts.Location = new System.Drawing.Point(11, 82);
			this.showPerturbedLayouts.Name = "showPerturbedLayouts";
			this.showPerturbedLayouts.Size = new System.Drawing.Size(197, 21);
			this.showPerturbedLayouts.TabIndex = 6;
			this.showPerturbedLayouts.Text = "Show all perturbed layouts";
			this.showPerturbedLayouts.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(368, 57);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(26, 17);
			this.label2.TabIndex = 5;
			this.label2.Text = "ms";
			// 
			// showPartialValidLayoutsTime
			// 
			this.showPartialValidLayoutsTime.Location = new System.Drawing.Point(240, 55);
			this.showPartialValidLayoutsTime.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.showPartialValidLayoutsTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.showPartialValidLayoutsTime.Name = "showPartialValidLayoutsTime";
			this.showPartialValidLayoutsTime.Size = new System.Drawing.Size(120, 22);
			this.showPartialValidLayoutsTime.TabIndex = 4;
			this.showPartialValidLayoutsTime.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
			// 
			// showPartialValidLayouts
			// 
			this.showPartialValidLayouts.AutoSize = true;
			this.showPartialValidLayouts.Location = new System.Drawing.Point(11, 55);
			this.showPartialValidLayouts.Name = "showPartialValidLayouts";
			this.showPartialValidLayouts.Size = new System.Drawing.Size(189, 21);
			this.showPartialValidLayouts.TabIndex = 3;
			this.showPartialValidLayouts.Text = "Show partial valid layouts";
			this.showPartialValidLayouts.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(368, 30);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(26, 17);
			this.label1.TabIndex = 2;
			this.label1.Text = "ms";
			// 
			// showFinalLayoutsTime
			// 
			this.showFinalLayoutsTime.Location = new System.Drawing.Point(240, 28);
			this.showFinalLayoutsTime.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
			this.showFinalLayoutsTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.showFinalLayoutsTime.Name = "showFinalLayoutsTime";
			this.showFinalLayoutsTime.Size = new System.Drawing.Size(120, 22);
			this.showFinalLayoutsTime.TabIndex = 1;
			this.showFinalLayoutsTime.Value = new decimal(new int[] {
            2000,
            0,
            0,
            0});
			// 
			// showFinalLayouts
			// 
			this.showFinalLayouts.AutoSize = true;
			this.showFinalLayouts.Location = new System.Drawing.Point(11, 28);
			this.showFinalLayouts.Name = "showFinalLayouts";
			this.showFinalLayouts.Size = new System.Drawing.Size(143, 21);
			this.showFinalLayouts.TabIndex = 0;
			this.showFinalLayouts.Text = "Show final layouts";
			this.showFinalLayouts.UseVisualStyleBackColor = true;
			// 
			// infoPanel
			// 
			this.infoPanel.Controls.Add(this.infoGroupBox);
			this.infoPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.infoPanel.Location = new System.Drawing.Point(5, 243);
			this.infoPanel.Name = "infoPanel";
			this.infoPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
			this.infoPanel.Size = new System.Drawing.Size(541, 117);
			this.infoPanel.TabIndex = 4;
			// 
			// mapDescriptionPanel
			// 
			this.mapDescriptionPanel.Controls.Add(this.mapDescriptionGroupBox);
			this.mapDescriptionPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.mapDescriptionPanel.Location = new System.Drawing.Point(5, 107);
			this.mapDescriptionPanel.Name = "mapDescriptionPanel";
			this.mapDescriptionPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
			this.mapDescriptionPanel.Size = new System.Drawing.Size(541, 136);
			this.mapDescriptionPanel.TabIndex = 3;
			// 
			// generateButtonPanel
			// 
			this.generateButtonPanel.Controls.Add(this.generateButton);
			this.generateButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.generateButtonPanel.Location = new System.Drawing.Point(0, 534);
			this.generateButtonPanel.Name = "generateButtonPanel";
			this.generateButtonPanel.Size = new System.Drawing.Size(572, 66);
			this.generateButtonPanel.TabIndex = 5;
			// 
			// generateButton
			// 
			this.generateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.generateButton.Location = new System.Drawing.Point(169, 9);
			this.generateButton.Name = "generateButton";
			this.generateButton.Size = new System.Drawing.Size(204, 49);
			this.generateButton.TabIndex = 0;
			this.generateButton.Text = "Generate";
			this.generateButton.UseVisualStyleBackColor = true;
			this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
			// 
			// MainSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(572, 600);
			this.Controls.Add(this.generateButtonPanel);
			this.Controls.Add(this.mainPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "MainSettings";
			this.Text = "MainSettings";
			this.generalSettingsGroupBox.ResumeLayout(false);
			this.generalSettingsGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.generatorSeedInput)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numberOfLayoutsInput)).EndInit();
			this.mapDescriptionGroupBox.ResumeLayout(false);
			this.mapDescriptionGroupBox.PerformLayout();
			this.infoGroupBox.ResumeLayout(false);
			this.infoGroupBox.PerformLayout();
			this.usedDescriptionInfoPanel.ResumeLayout(false);
			this.usedDescriptionInfoPanel.PerformLayout();
			this.generalSettingsPanel.ResumeLayout(false);
			this.mainPanel.ResumeLayout(false);
			this.drawingSettingsPanel.ResumeLayout(false);
			this.drawingSettingsGroupBox.ResumeLayout(false);
			this.drawingSettingsGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.fixedSquareExportValue)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.fixedFontSizeValue)).EndInit();
			this.progressPanel.ResumeLayout(false);
			this.progressGroupBox.ResumeLayout(false);
			this.progressGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.showPerturbedLayoutsTime)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.showPartialValidLayoutsTime)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.showFinalLayoutsTime)).EndInit();
			this.infoPanel.ResumeLayout(false);
			this.mapDescriptionPanel.ResumeLayout(false);
			this.generateButtonPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.OpenFileDialog mapDescriptionFileDialog;
		private System.Windows.Forms.GroupBox generalSettingsGroupBox;
		private System.Windows.Forms.CheckBox useRandomSeedCheckbox;
		private System.Windows.Forms.NumericUpDown generatorSeedInput;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown numberOfLayoutsInput;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox infoGroupBox;
		private System.Windows.Forms.GroupBox mapDescriptionGroupBox;
		private System.Windows.Forms.Button uploadButton;
		private System.Windows.Forms.Label uploadOrLabel;
		private System.Windows.Forms.ComboBox loadedMapDescriptionsComboBox;
		private System.Windows.Forms.Label chooseMapDescriptionLabel;
		private System.Windows.Forms.Panel generalSettingsPanel;
		private System.Windows.Forms.Panel mainPanel;
		private System.Windows.Forms.Label descriptionNotChosen;
		private System.Windows.Forms.Panel infoPanel;
		private System.Windows.Forms.Panel mapDescriptionPanel;
		private System.Windows.Forms.Panel generateButtonPanel;
		private System.Windows.Forms.Button generateButton;
		private System.Windows.Forms.Panel progressPanel;
		private System.Windows.Forms.GroupBox progressGroupBox;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown showPerturbedLayoutsTime;
		private System.Windows.Forms.CheckBox showPerturbedLayouts;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown showPartialValidLayoutsTime;
		private System.Windows.Forms.CheckBox showPartialValidLayouts;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown showFinalLayoutsTime;
		private System.Windows.Forms.CheckBox showFinalLayouts;
		private System.Windows.Forms.Panel usedDescriptionInfoPanel;
		private System.Windows.Forms.Label usedDescriptionPassagesCount;
		private System.Windows.Forms.Label usedDescriptionRoomsCount;
		private System.Windows.Forms.Label usedDescription;
		private System.Windows.Forms.Panel drawingSettingsPanel;
		private System.Windows.Forms.GroupBox drawingSettingsGroupBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown fixedSquareExportValue;
		private System.Windows.Forms.CheckBox fixedSquareExportCheckbox;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.CheckBox useOldPaperStyleCheckbox;
		private System.Windows.Forms.NumericUpDown fixedFontSizeValue;
		private System.Windows.Forms.CheckBox showRoomNamesCheckbox;
		private System.Windows.Forms.CheckBox fixedFontSizeCheckbox;
		private System.Windows.Forms.CheckBox exportShownLayoutsCheckbox;
	}
}