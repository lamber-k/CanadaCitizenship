using Blazored.LocalStorage;
using CanadaCitizenship.Algorithm;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using System.Collections.ObjectModel;

namespace CanadaCitizenship.Blazor.Pages
{
    public partial class Home
    {
        [Inject]
        protected DialogService DialogService { get; set; } = null!;
        [Inject]
        protected ILocalStorageService LocalStorageService { get; set; } = null!;
        [Inject]
        protected NotificationService NotificationService { get; set; } = null!;
        [Inject]
        protected IJSRuntime JsRuntime { get; set; } = null!;

        RadzenDataGrid<Period> outOfCountryDataGrid = null!;
        public ObservableCollection<Profile> Profiles { get; set; } = [];
        public Profile? SelectedProfile { get; set; }
        public CitizenshipResult? Result { get; set; }
        public Period? ToUpdate { get; set; }
        public Period? ToCreate { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Profiles = new ObservableCollection<Profile>(await LocalStorageService.GetItemAsync<List<Profile>>(STORAGE_PROFILES_KEY) ?? []);
            Profiles.CollectionChanged += Profiles_CollectionChanged;
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
                await SaveProfiles();
                await outOfCountryDataGrid.Reload();
            }
        }

        public Task OnUpdateRow() => SaveProfiles();

        public async Task OnCreateRow()
        {
            if (SelectedProfile is not null && ToCreate is not null)
            {
                SelectedProfile.OutOfCountry.Add(ToCreate);
                ToCreate = null;
                await SaveProfiles();
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