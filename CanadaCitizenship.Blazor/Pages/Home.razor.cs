using Blazored.LocalStorage;
using CanadaCitizenship.Algorithm;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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
        [Inject]
        protected ContextMenuService ContextMenuService { get; set; } = null!;

        RadzenDataGrid<Period> outOfCountryDataGrid = null!;

        public CitizenshipResult? Result { get; set; }
        public Period? ToUpdate { get; set; }
        public Period? ToCreate { get; set; }

        public Home()
        {
            _selectedProfile = Profiles.First();
        }

        protected override async Task OnInitializedAsync()
        {
            Profiles = new ObservableCollection<Profile>(await LocalStorageService.GetItemAsync<List<Profile>>(STORAGE_PROFILES_KEY) ?? [Profile.Default]);
            SelectedProfile = Profiles.First();
            Profiles.CollectionChanged += Profiles_CollectionChanged;
        }

        public async Task InsertRow()
        {
            ToCreate = new Period();
            await outOfCountryDataGrid.InsertRow(ToCreate);
        }

        async Task EditRow(Period period)
        {
            ToUpdate = period;
            await outOfCountryDataGrid.EditRow(period);
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
            SelectedProfile.OutOfCountry.Remove(period);
            await SaveProfiles();
            await outOfCountryDataGrid.Reload();
        }

        public Task OnUpdateRow() => SaveProfiles();

        public async Task OnCreateRow()
        {
            if (ToCreate is not null)
            {
                SelectedProfile.OutOfCountry.Add(ToCreate);
                ToCreate = null;
                await SaveProfiles();
            }
        }

        public void OpenContextMenu(MouseEventArgs args)
        {
            ContextMenuService.Open(args,
            [
                .. Profiles.Select(p => new ContextMenuItem() { Text = p.Name, Value = string.Concat("user_", p.Name), Icon = "account_circle" }),
                new ContextMenuItem(){ Text = "New Profile", Value = "profile_add", Icon = "add" },
                new ContextMenuItem(){ Text = "Delete Profile", Value = "profile_delete", Icon = "delete" },
                new ContextMenuItem(){ Text = "Import Profiles", Value = "profile_import", Icon = "file_upload" },
                new ContextMenuItem(){ Text = "Save Profile", Value = "profile_export", Icon = "save" },
                new ContextMenuItem(){ Text = "Switch Language", Value = "switch_culture", Icon = "language" },
            ], OnMenuClick);
        }

        public async void OnMenuClick(MenuItemEventArgs selected)
        {
            ContextMenuService.Close();
            switch (selected.Value)
            {
                case string path when path.StartsWith("user_"):
                    SelectedProfile = Profiles.First(p => p.Name == path[5..]);
                    break;
                case "profile_delete":
                    ProfileDelete();
                    break;
                case "profile_import":
                    await ImportProfiles();
                    break;
                case "profile_export":
                    ExportProfiles();
                    break;
                case "profile_add":
                    await AddProfile();
                    break;
                case "switch_culture":
                    await SwitchCulture();
                    break;
            }
            StateHasChanged();
        }

        public async Task SwitchCulture()
        {
            await DialogService.OpenSideAsync<SwitchCulture>("Switch Culture");
        }

        public void Compute()
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