using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace CanadaCitizenship.Algorithm
{
    /// <summary>
    /// User Profile
    /// </summary>
    public class Profile : INotifyPropertyChanged
    {
        /// <summary>
        /// Default empty profile
        /// </summary>
        public static Profile Default => new Profile("Default");

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name">Profile name</param>
        [SetsRequiredMembers]
        [JsonConstructor]
        public Profile(string name)
        {
            Name = name;
        }

        private string _name = null!;
        /// <summary>
        /// Profile Name
        /// </summary>
        public required string Name 
        { 
            get => _name;
            set => SetMember(value, ref _name); 
        }

        private DateTime? _temporaryDate;
        /// <summary>
        /// Start of user's Temporary date
        /// </summary>
        public DateTime? TemporaryDate
        {
            get => _temporaryDate;
            set => SetMember(value, ref _temporaryDate);
        }

        private DateTime? _PRDate;
        /// <summary>
        /// Start of PR date. Must be set for computation
        /// </summary>
        public DateTime? PRDate
        {
            get => _PRDate;
            set => SetMember(value, ref _PRDate);
        }

        /// <summary>
        /// List of all exclusion periods
        /// </summary>
        public ObservableCollection<Period> ExclusionPeriods { get; set; } = [];

        /// <summary>
        /// Set one of the properties and trigger a property change
        /// </summary>
        /// <typeparam name="T">Type of the property to update</typeparam>
        /// <param name="value">new Value</param>
        /// <param name="member">existing property value</param>
        /// <param name="propertyName">Name of the property to update</param>
        private void SetMember<T>(T value, ref T member, [CallerMemberName] string? propertyName = default)
        {
            if (!(value?.Equals(member) ?? false))
            {
                member = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
