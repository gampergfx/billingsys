using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using HandwritingInstituteBillingSystem.Annotations;
using HandwritingInstituteBillingSystem.CommonViewHandlers;
using HandwritingInstituteBillingSystem.Logic;

namespace HandwritingInstituteBillingSystem.ViewModels
{
    class TashViewModel: INotifyPropertyChanged
    {
        private readonly PeopleManager _peopleManager = Store.Get();

        public TashViewModel()
        {
            TrashUserDetailViewModels = new ObservableCollection<UserDetails>(_peopleManager.GetTrash());
            TrashHandler.MoveToTrash += OnMoveToTrash;
        }

        private void OnMoveToTrash(object sender, EventArgs e)
        {
            TrashUserDetailViewModels.Add(sender as UserDetails);
            OnPropertyChanged(nameof(TrashUserDetailViewModels));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand _restoreCommand { get; set; }
        public ICommand _deleteCommand { get; set; }

        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                {
                    _deleteCommand = new RelayCommand(DeleteExecute);
                }
                return _deleteCommand;
            }

        }

        public ICommand RestoreCommand
        {
            get
            {
                if (_restoreCommand == null)
                {
                    _restoreCommand = new RelayCommand(RestoreExecute);
                }
                return _restoreCommand;
            }

        }

        private UserDetails _selectedUserDetails;

        public UserDetails SelectedUserDetails
        {
            get { return _selectedUserDetails; }
            set
            {
                _selectedUserDetails = value;
                OnPropertyChanged(nameof(SelectedUserDetails));
            }
        }
        private ObservableCollection<UserDetails> _trashUserDetailViewModels;

        public ObservableCollection<UserDetails> TrashUserDetailViewModels
        {
            get
            {
                return _trashUserDetailViewModels;
            }

            set
            {
                _trashUserDetailViewModels = value;
                OnPropertyChanged(nameof(TrashUserDetailViewModels));
            }
        }

        private void RestoreExecute(object obj)
        {
            if (_selectedUserDetails == null)
            {
                MessageBox.Show("No Item selected");
                return;
            }

            MessageBox.Show("Selected item will be moved to people table");
            _peopleManager.Store(SelectedUserDetails);
            _peopleManager.DeleteTrash(SelectedUserDetails.Id);
            TrashHandler.OnRestoreTrash(SelectedUserDetails);
            TrashUserDetailViewModels.Remove(SelectedUserDetails);


            OnPropertyChanged(nameof(TrashUserDetailViewModels));
        }

        private void DeleteExecute(object obj)
        {
            if (_selectedUserDetails == null)
            {
                MessageBox.Show("No Item selected");
                return;
            }

            MessageBox.Show("Selected item will be permanently deleted.");
            _peopleManager.DeleteTrash(SelectedUserDetails.Id);

            TrashUserDetailViewModels.Remove(_selectedUserDetails);
            OnPropertyChanged(nameof(TrashUserDetailViewModels));
        }
    }
}