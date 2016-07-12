using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HandwritingInstituteBillingSystem.Annotations;

namespace HandwritingInstituteBillingSystem.Logic.Configs
{
    public class PayMode: INotifyPropertyChanged
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return Name;
        }
    }
}