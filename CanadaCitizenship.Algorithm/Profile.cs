using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CanadaCitizenship.Algorithm
{
    public class Profile : INotifyPropertyChanged
    {
        [SetsRequiredMembers]
        [JsonConstructor]
        public Profile(string name)
        {
            Name = name;
        }

        private string? _name;
        public required string Name 
        { 
            get => _name;
            set => SetMember(value, ref _name); 
        }

        private DateTime? _temporaryDate;
        public DateTime? TemporaryDate
        {
            get => _temporaryDate;
            set => SetMember(value, ref _temporaryDate);
        }

        private DateTime? _PRDate;
        public DateTime? PRDate
        {
            get => _PRDate;
            set => SetMember(value, ref _PRDate);
        }

        public ObservableCollection<Period> OutOfCountry { get; set; } = [];

        private void SetMember<T>(T value, ref T member, [CallerMemberName] string? propertyName = default)
        {
            if (!(value?.Equals(member) ?? false))
            {
                member = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
