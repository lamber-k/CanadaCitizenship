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

        public ObservableCollection<Profile> Profiles { get; set; } = [];
        Profile? _selectedProfile;
        public Profile? SelectedProfile
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
                        _selectedProfile.PropertyChanged += SelectedProfile_PropertyChanged;
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

        public void ProfileDelete()
        {
            if (SelectedProfile is not null)
            {
                Profiles.Remove(SelectedProfile);
                SelectedProfile = Profiles.FirstOrDefault();
            }
        }

        public async Task AddProfile()
        {
            var result = await DialogService.OpenAsync<AddProfile>("Create Profile", new Dictionary<string, object>() { { "Profiles", Profiles } });
            if (result is Profile newProfile)
            {
                Profiles.Add(newProfile);
                SelectedProfile = newProfile;
            }
        }

        public async Task ImportProfiles()
        {
            var result = await DialogService.OpenAsync<ImportProfiles>("Import Profile");
            if (result is List<Profile> loadedProfiles)
            {
                Profiles.CollectionChanged -= Profiles_CollectionChanged;
                Profiles = new System.Collections.ObjectModel.ObservableCollection<Profile>(loadedProfiles);
                await SaveProfiles();
                Profiles.CollectionChanged += Profiles_CollectionChanged;
                SelectedProfile = Profiles.FirstOrDefault();
            }
        }

        public void ExportProfiles() => JsRuntime.InvokeAsync<object>(
            "saveFile",
            "profiles.json",
            JsonSerializer.Serialize(Profiles));

    }
}
