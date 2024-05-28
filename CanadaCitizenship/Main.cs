using CanadaCitizenship.Resources;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;

namespace CanadaCitizenship
{
    public partial class Main : Form
    {
        static readonly string AppDirectory = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Citizenship");
        static readonly string StoredConfiguration = Path.Join(AppDirectory, "profiles.json");

        public BindingList<Period> OutOfCountry { get; set; } = [];
        public BindingList<Profile> Profiles { get; } = [];
        public Profile? Selected { get; set; }
        public Main()
        {
            InitializeComponent();
            if (File.Exists(StoredConfiguration))
            {
                Profiles = new BindingList<Profile>(JsonSerializer.Deserialize<List<Profile>>(File.ReadAllText(StoredConfiguration)) ?? []);
            }
            profilesComboBox.DisplayMember = nameof(Profile.Name);
            profilesComboBox.DataSource = Profiles;
            outOfCountryDataGrid.DataSource = OutOfCountry;
            Text = UI.WindowName;
            firstEntryLabel.Text = UI.FirstEntryLabel;
            PRLabel.Text = UI.PermanentResidencyLabel;
            currentProfileLabel.Text = UI.CurrentProfileLabel;
            outOfCountryBeginLabel.Text = UI.OutOfCountryBeginLabel;
            outOfCountryEndLabel.Text = UI.OutOfCountryEndLabel;
            newProfileButton.Text = UI.NewProfileButton;
            deleteProfileButton.Text = UI.DeleteProfileButton;
            AddOOCButton.Text = UI.OutOfCountryAddButton;
            computeButton.Text = UI.ComputeButton;
            outOfCountryDataGrid.Columns["Begin"].HeaderText = UI.OutOfCountry_Grid_Header_Begin;
            outOfCountryDataGrid.Columns["End"].HeaderText = UI.OutOfCountry_Grid_Header_End;
            outOfCountryDataGrid.Columns["Delete"].HeaderText = UI.OutOfCountry_Grid_Header_Delete;
            ((DataGridViewButtonColumn)outOfCountryDataGrid.Columns["Delete"]).Text = UI.OutOfCountry_Grid_Header_Delete;
        }

        private void computeButton_Click(object sender, EventArgs e)
        {
            if (Selected is null)
            {
                MessageBox.Show("No Profile selected", "Computation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    CitizenshipResult result = CitizenshipAlgorithm.Compute(Selected);
                    Debug.WriteLine($"Begin Temporary date taken: {result.StartTemporary}");
                    beginDateLabel.Text = string.Format(Messages.BeginTemporaryFormat, result.StartTemporary);
                    beginDateLabel.Visible = true;
                    Debug.WriteLine($"Total Temporary: {result.TemporaryDays} day(s)");
                    totalTemporaryLabel.Text = string.Format(Messages.TotalTemporaryFormat, result.TemporaryDays);
                    totalTemporaryLabel.Visible = true;
                    Debug.WriteLine($"Total PR: {result.PRDays} day(s)");
                    totalPRLabel.Text = string.Format(Messages.TotalPRFormat, result.PRDays);
                    totalPRLabel.Visible = true;
                    Debug.WriteLine($"Remaining required: {result.RemainingDays} day(s)");
                    remainingDaysLabel.Text = string.Format(Messages.RemainingFormat, result.RemainingDays);
                    remainingDaysLabel.Visible = true;
                    Debug.WriteLine($"Projected Citezenship Process Date: {result.ProjectedDate}");
                    projectedDateLabel.Text = string.Format(Messages.ProjectedApplyDateFormat, result.ProjectedDate);
                    projectedDateLabel.Visible = true;
                }
                catch (InvalidOperationException iex) when (Messages.ResourceManager.GetString(iex.Message) is not null)
                {
                    MessageBox.Show(Messages.ResourceManager.GetString(iex.Message), "Computation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception)
                {
                    MessageBox.Show("Computation failed", "Computation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (!Directory.Exists(AppDirectory))
                {
                    Directory.CreateDirectory(AppDirectory);
                }
                File.WriteAllText(StoredConfiguration, JsonSerializer.Serialize(Profiles));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void newProfileButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(newProfileNameTextBox.Text))
            {
                var newProfile = new Profile(newProfileNameTextBox.Text);
                Profiles.Add(newProfile);
                profilesComboBox.SelectedItem = newProfile;
            }
        }

        private void deleteProfileButton_Click(object sender, EventArgs e)
        {
            Profile? selected = profilesComboBox.SelectedItem as Profile;
            if (selected is not null)
            {
                Profiles.Remove(selected);
            }
        }

        private void profilesComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            Profile? selected = profilesComboBox.SelectedItem as Profile;
            beginDateLabel.Visible = false;
            totalTemporaryLabel.Visible = false;
            totalPRLabel.Visible = false;
            remainingDaysLabel.Visible = false;
            projectedDateLabel.Visible = false;
            if (selected is not null)
            {
                Selected = selected;
                temporaryDTP.Value = selected.TemporaryDate ?? DateTime.Today;
                permanentResidencyDTP.Value = selected.PRDate ?? DateTime.Today;
                OutOfCountry.Clear();
                foreach (var period in selected.OutOfCountry)
                {
                    OutOfCountry.Add(period);
                }
            }
        }

        private void permanentResidencyDTP_ValueChanged(object sender, EventArgs e)
        {
            if (Selected is not null)
            {
                Selected.PRDate = permanentResidencyDTP.Value;
            }
        }

        private void temporaryDTP_ValueChanged(object sender, EventArgs e)
        {
            if (Selected is not null)
            {
                Selected.TemporaryDate = temporaryDTP.Value;
            }
        }

        private void AddOOCButton_Click(object sender, EventArgs e)
        {
            OutOfCountry.Add(new Period
            {
                Begin = OOCBeginDTP.Value,
                End = OOCEndDTP.Value,
            });
        }

        private void outOfCountryDataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == outOfCountryDataGrid.Columns["Delete"].Index)
            {
                OutOfCountry.RemoveAt(e.RowIndex);
            }
        }
    }
}
