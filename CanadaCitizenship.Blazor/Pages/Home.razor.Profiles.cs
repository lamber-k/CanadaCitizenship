using CanadaCitizenship.Algorithm;
using Microsoft.JSInterop;
using Radzen;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;

namespace CanadaCitizenship.Blazor.Pages
{
    public partial class Home
    {
        private const string STORAGE_PROFILES_KEY = "Profiles";
        private const string STORAGE_CURRENT_PROFILE_KEY = "CurrentProfile";

        public ObservableCollection<Profile> Profiles { get; set; } = [Profile.Default];
        Profile _selectedProfile = Profile.Default;
        public Profile SelectedProfile
        {
            get => _selectedProfile;
            set
            {
                if (_selectedProfile != value)
                {
                    if (_selectedProfile is not null)
                        _selectedProfile.PropertyChanged -= SelectedProfile_PropertyChanged;
                    _selectedProfile = value;
                    if (_selectedProfile is not null)
                    {
                        LocalStorageService.SetItemAsStringAsync(STORAGE_CURRENT_PROFILE_KEY, _selectedProfile.Name);
                        _selectedProfile.PropertyChanged += SelectedProfile_PropertyChanged;
                        Result = null;
                        Compute();
                    }
                }
            }
        }

        private async void Profiles_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => await SaveProfiles();

        private async Task SaveProfiles()
        {
            await LocalStorageService.SetItemAsync(STORAGE_PROFILES_KEY, Profiles);
        }

        private async void SelectedProfile_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            await SaveProfiles();
            Compute();
        }

        public async Task ProfileDelete()
        {
            bool? result = await DialogService.Confirm(Loc["ProfileDeleteConfirmText"], Loc["ProfileDeleteConfirmTitle"], new ConfirmOptions
            {
                CancelButtonText = Loc["CancelBtn"],
                OkButtonText = Loc["ConfirmBtn"],
            });
            if (result ?? false)
            {
                Profiles.Remove(SelectedProfile);
                if (!Profiles.Any())
                {
                    Profiles.Add(Profile.Default);
                }
                SelectedProfile = Profiles.First();
            }
        }

        public async Task AddProfile()
        {
            var result = await DialogService.OpenAsync<AddProfile>(Loc["DialogCreateProfileTitle"], new Dictionary<string, object>() { { "Profiles", Profiles } });
            if (result is Profile newProfile)
            {
                Profiles.Add(newProfile);
                SelectedProfile = newProfile;
            }
        }

        public async Task ImportProfiles()
        {
            var result = await DialogService.OpenAsync<ImportProfiles>(Loc["DialogImportProfilesTitle"]);
            if (result is List<Profile> loadedProfiles)
            {
                Profiles.CollectionChanged -= Profiles_CollectionChanged;
                Profiles = new ObservableCollection<Profile>(loadedProfiles);
                await SaveProfiles();
                Profiles.CollectionChanged += Profiles_CollectionChanged;
                SelectedProfile = Profiles.First();
            }
        }

        public void ExportProfiles() => JsRuntime.InvokeAsync<object>(
            "saveFile",
            "profiles.json",
            JsonSerializer.Serialize(Profiles));

    }
}
