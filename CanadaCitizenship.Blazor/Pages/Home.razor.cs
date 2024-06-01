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
        [Inject]
        protected ILogger<Home> Logger { get; set; } = null!;

        RadzenDataGrid<Period> excludedPeriodsDataGrid = null!;
        public IReadOnlyCollection<PeriodType> PeriodTypes { get; } = 
        [
            PeriodType.Vacation,
            PeriodType.Tourist, 
            PeriodType.NoStatus,
            PeriodType.Other
        ];
        public CitizenshipResult? Result { get; set; }
        public Period? ToUpdate { get; set; }
        public Period? ToCreate { get; set; }
        public Home()
        {
            _selectedProfile = Profiles.First();
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Load profiles, latest selected profile and initialize view.
        /// </remarks>
        protected override async Task OnInitializedAsync()
        {
            Profiles = new ObservableCollection<Profile>(await LocalStorageService.GetItemAsync<List<Profile>>(STORAGE_PROFILES_KEY) ?? [Profile.Default]);
            string? profileName = await LocalStorageService.GetItemAsStringAsync(STORAGE_CURRENT_PROFILE_KEY);
            SelectedProfile = Profiles.FirstOrDefault(p => p.Name == profileName, Profiles.First());
            Profiles.CollectionChanged += Profiles_CollectionChanged;
        }

        /// <summary>
        /// Insert a new exclusion period
        /// </summary>
        public async Task InsertRow()
        {
            ToCreate = new Period();
            await excludedPeriodsDataGrid.InsertRow(ToCreate);
        }

        /// <summary>
        /// Edit an exclusion period
        /// </summary>
        /// <param name="period">Exclusion period to edit</param>
        async Task EditRow(Period period)
        {
            ToUpdate = period;
            await excludedPeriodsDataGrid.EditRow(period);
        }

        /// <summary>
        /// Save an exclusion period
        /// </summary>
        /// <param name="period">Exclusion period to save</param>
        async Task SaveRow(Period period)
        {
            await excludedPeriodsDataGrid.UpdateRow(period);
            Compute();
        }

        /// <summary>
        /// Cancel an exclusion period edition or add
        /// </summary>
        /// <param name="period"></param>
        void CancelEdit(Period period)
        {
            ToUpdate = null;
            ToCreate = null;

            excludedPeriodsDataGrid.CancelEditRow(period);
            Compute();
        }

        /// <summary>
        /// Delete an exclusion period
        /// </summary>
        /// <param name="period">Exclusion period to remove</param>
        public async Task DeleteRow(Period period)
        {
            bool? result = await DialogService.Confirm(Loc["ExclusionPeriodDeleteConfirmText"], Loc["ExclusionPeriodDeleteConfirmTitle"], new ConfirmOptions
            {
                CancelButtonText = Loc["CancelBtn"],
                OkButtonText = Loc["ConfirmBtn"],
            });
            if (result ?? false)
            {
                SelectedProfile.ExclusionPeriods.Remove(period);
                await SaveProfiles();
                await excludedPeriodsDataGrid.Reload();
                Compute();
            }
        }

        /// <summary>
        /// Save the exclusion period in pending edit
        /// </summary>
        public Task OnUpdateRow() => SaveProfiles();

        /// <summary>
        /// Save a new exclusion period
        /// </summary>
        public async Task OnCreateRow()
        {
            if (ToCreate is not null)
            {
                SelectedProfile.ExclusionPeriods.Add(ToCreate);
                ToCreate = null;
                await SaveProfiles();
                Compute();
            }
        }

        /// <summary>
        /// Open the top-right context menu
        /// </summary>
        /// <param name="args">Mouse event</param>
        public void OpenContextMenu(MouseEventArgs args)
        {
            ContextMenuService.Open(args,
            [
                .. Profiles.Select(p => new ContextMenuItem() { Text = p.Name, Value = string.Concat("user_", p.Name), Icon = "account_circle" }),
                new ContextMenuItem(){ Text = Loc["ContextMenuNewProfileText"], Value = "profile_add", Icon = "add" },
                new ContextMenuItem(){ Text = Loc["ContextMenuDeleteProfileText"], Value = "profile_delete", Icon = "delete" },
                new ContextMenuItem(){ Text = Loc["ContextMenuImportProfilesText"], Value = "profile_import", Icon = "file_upload" },
                new ContextMenuItem(){ Text = Loc["ContextMenuSaveProfilesText"], Value = "profile_export", Icon = "save" },
                new ContextMenuItem(){ Text = Loc["ContextMenuSwitchCultureText"], Value = "switch_culture", Icon = "language" },
            ], OnMenuClick);
        }

        /// <summary>
        /// Triggered when the use click on any context menu item
        /// </summary>
        /// <param name="selected">Menu item that was clicked</param>
        public async void OnMenuClick(MenuItemEventArgs selected)
        {
            ContextMenuService.Close();
            try
            {
                switch (selected.Value)
                {
                    case string path when path.StartsWith("user_"):
                        SelectedProfile = Profiles.First(p => p.Name == path[5..]);
                        break;
                    case "profile_delete":
                        await ProfileDelete();
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
            catch (Exception ex)
            {
                Logger.LogError(ex, "Menu action {Menu} failed to execute", selected.Value);
                NotificationService.Notify(NotificationSeverity.Error, Loc["ErrorMenuClickActionTitle"], Loc["ErrorMenuClickAction", selected.Text]);
            }
        }

        /// <summary>
        /// Switch view culture - Opens a dialog selector
        /// </summary>
        public async Task SwitchCulture()
        {
            await DialogService.OpenSideAsync<SwitchCulture>(Loc["DialogSwitchCultureTitle"]);
        }

        /// <summary>
        /// Compute the data user input and prepare result
        /// </summary>
        public void Compute()
        {
            try
            {
                Result = CitizenshipAlgorithm.Compute(SelectedProfile);
            }
            catch (InvalidOperationException iex) when (Messages.ResourceManager.GetString(iex.Message) is not null)
            {
                NotificationService.Notify(NotificationSeverity.Error, Loc["ErrorComputationTitle"], Messages.ResourceManager.GetString(iex.Message));
            }
        }

        /// <summary>
        /// Transform current date format based on the computation result
        /// </summary>
        /// <param name="args">Current date to render</param>
        public void ComputeDateResult(DateRenderEventArgs args)
        {
            if (Result is not null)
            {
                if (args.Date < Result.StartTemporary || args.Date > Result.ProjectedDate)
                {
                    // Outside the range
                    args.Disabled = true;
                }
                else if (args.Date == Result.ProjectedDate)
                {
                    args.Attributes.Add("period-type", "projected");
                }
                else
                {
                    Period? found = Result.Periods.FirstOrDefault(p => p.DateEnclosed(args.Date));
                    string classType = found?.Type switch
                    {
                        PeriodType.Temporary => "temporary",
                        PeriodType.PR => "permanent",
                        _ => "excluded"
                    };
                    args.Attributes.Add("period-type", classType);
                }
            }
        }
    }
}