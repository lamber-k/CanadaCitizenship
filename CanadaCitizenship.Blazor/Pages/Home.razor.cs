using Blazored.LocalStorage;
using CanadaCitizenship.Algorithm;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace CanadaCitizenship.Blazor.Pages
{
    public partial class Home
    {
        private const string STORAGE_PROFILES_KEY = "Profiles";

        [Inject]
        protected ILocalStorageService LocalStorageService { get; set; } = null!;
        [Inject]
        protected NotificationService NotificationService { get; set; } = null!;

        RadzenDataGrid<Period> outOfCountryDataGrid = null!;
        public ObservableCollection<Profile> Profiles { get; set; } = [];
        public Profile? SelectedProfile { get; set; }
        public string? NewProfileName { get; set; }
        public CitizenshipResult? Result { get; set; }
        public Period? ToUpdate { get; set; }
        public Period? ToCreate { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Profiles = new ObservableCollection<Profile>(await LocalStorageService.GetItemAsync<List<Profile>>(STORAGE_PROFILES_KEY) ?? []);
            Profiles.CollectionChanged += Profiles_CollectionChanged;
        }

        private async void Profiles_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await LocalStorageService.SetItemAsync(STORAGE_PROFILES_KEY, Profiles);
        }

        public void OnSelectionChange()
        {
            foreach (var profile in Profiles)
            {
                profile.PropertyChanged -= SelectedProfile_PropertyChanged;
            }
            if (SelectedProfile is not null)
            {
                SelectedProfile.PropertyChanged += SelectedProfile_PropertyChanged;
            }
        }

        private async void SelectedProfile_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            await LocalStorageService.SetItemAsync(STORAGE_PROFILES_KEY, Profiles);
        }

        public void ProfileDelete()
        {
            if (SelectedProfile is not null)
            {
                Profiles.Remove(SelectedProfile);
            }
        }

        public void AddProfile()
        {
            if (!string.IsNullOrEmpty(NewProfileName))
            {
                Profile newProfile = new Profile(NewProfileName);
                Profiles.Add(newProfile);
                SelectedProfile = newProfile;
            }
        }

        public async Task InsertRow()
        {
            if (SelectedProfile is not null)
            {
                ToCreate = new Period();
                await outOfCountryDataGrid.InsertRow(ToCreate);
            }
        }

        async Task EditRow(Period period)
        {
            if (SelectedProfile is not null)
            {
                ToUpdate = period;
                await outOfCountryDataGrid.EditRow(period);
            }
        }

        async Task SaveRow(Period period)
        {
            await outOfCountryDataGrid.UpdateRow(period);
        }

        void CancelEdit(Period period)
        {
            ToUpdate = null;
            ToCreate = null;

            outOfCountryDataGrid.CancelEditRow(period);
        }

        public async Task DeleteRow(Period period)
        {
            if (SelectedProfile is not null)
            {
                SelectedProfile.OutOfCountry.Remove(period);
                await LocalStorageService.SetItemAsync(STORAGE_PROFILES_KEY, Profiles);
                await outOfCountryDataGrid.Reload();
            }
        }

        public async Task OnUpdateRow()
        {
            await LocalStorageService.SetItemAsync(STORAGE_PROFILES_KEY, Profiles);
        }

        public async Task OnCreateRow()
        {
            if (SelectedProfile is not null && ToCreate is not null)
            {
                SelectedProfile.OutOfCountry.Add(ToCreate);
                ToCreate = null;
                await LocalStorageService.SetItemAsync(STORAGE_PROFILES_KEY, Profiles);
            }
        }

        public void Compute()
        {
            if (SelectedProfile is not null)
            {
                try
                {
                    Result = CitizenshipAlgorithm.Compute(SelectedProfile);
                }
                catch (InvalidOperationException iex) when (Messages.ResourceManager.GetString(iex.Message) is not null)
                {
                    NotificationService.Notify(NotificationSeverity.Error, "Result", Messages.ResourceManager.GetString(iex.Message));
                }
            }
        }
    }
}