using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using HandwritingInstituteBillingSystem.CommonViewHandlers;
using HandwritingInstituteBillingSystem.Logic;
using MessageBox = System.Windows.MessageBox;

namespace HandwritingInstituteBillingSystem.ViewModels
{
    class PeopleViewModel: INotifyPropertyChanged
    {
        private ObservableCollection<UserDetails> _userDetailViewModels;
        private readonly PeopleManager _peopleManager = Store.Get();
        private UserDetails _selectedUserDetails;
        private NewEntryViewModel _newEntryViewModel;

        public PeopleViewModel()
        {
            UserDetailViewModels = new ObservableCollection<UserDetails>(_peopleManager.GetFromStorage());
            NewFormViewHandler.NewEntry -= OnNewEntry;
            NewFormViewHandler.NewEntry += OnNewEntry;
            TrashHandler.RestoreTrash += OnRestore;
            InstallmentHandler.InstallmentPayment += OnInstallmentPaid;
        }


        private void OnRestore(object sender, EventArgs e)
        {
            UserDetailViewModels.Add(sender as UserDetails);
            RaisePropertyChanged(nameof(UserDetailViewModels));
            ReportHandler.ItemsListChanged(UserDetailViewModels.ToList());
        }

        private void OnNewEntry(object sender, NewEntryViewModel e)
        {
            var userDetails = GetUserDetails(e);
            _peopleManager.Store(userDetails);
            UserDetailViewModels.Add(userDetails);
            RaisePropertyChanged(nameof(UserDetailViewModels));
            ReportHandler.ItemsListChanged(UserDetailViewModels.ToList());
        }


        private void OnInstallmentPaid(object sender, NewEntryViewModel e)
        {
            var userDetails = GetUserDetails(e);
            if (e.UserUniqueId > 0)
            {
                userDetails.UserUniqueId = e.UserUniqueId;
            }
            _peopleManager.Store(userDetails);
            UserDetailViewModels.Add(userDetails);
            RaisePropertyChanged(nameof(UserDetailViewModels));
            ReportHandler.ItemsListChanged(UserDetailViewModels.ToList());
        }

        private static UserDetails GetUserDetails(NewEntryViewModel newEntryViewModel)
        {
            return new UserDetails
            {
                Id =  Guid.NewGuid(),
                UserUniqueId = DateTimeOffset.Now.Ticks,
                Name = newEntryViewModel.Name,
                Phone = newEntryViewModel.Phone,
                AmountPaid = newEntryViewModel.AmountPaid,
                TotalAmountPaid = newEntryViewModel.TotalAmountPaid,
                Balance = double.Parse(newEntryViewModel.Balance),
                Course = newEntryViewModel.Course.CourseName,
                Center = newEntryViewModel.Center.CenterName,
                ModeOfPayment = newEntryViewModel.ModeOfPayment.Name,
                TimeStamp = newEntryViewModel.TimeStamp,
                BillNo = newEntryViewModel.BillNo,
                Cashier = newEntryViewModel.Cashier,
                Notes = newEntryViewModel.Notes,
                CourseFee = newEntryViewModel.Course.Fee.ToString("##.###")
            };
        }

        public ObservableCollection<UserDetails> UserDetailViewModels
        {
            get
            {
                return _userDetailViewModels;
            }

            set
            {
                _userDetailViewModels = value;
                RaisePropertyChanged(nameof(UserDetailViewModels));
            }
        }

        private string _filterText;

        public string FilterText
        {
            set
            {
                _filterText = value;
                RaisePropertyChanged(nameof(UserDetailViewModelsFilter));
                RaisePropertyChanged(nameof(FilterText));
            }
            get { return _filterText; }
        }

        public ObservableCollection<UserDetails> UserDetailViewModelsFilter
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FilterText))
                {
                    return _userDetailViewModels;
                }
                return new ObservableCollection < UserDetails > (_userDetailViewModels.Where(x=>x.Name.StartsWith(FilterText)).ToList());
            }

            set
            {
                _userDetailViewModels = value;
                RaisePropertyChanged(nameof(UserDetailViewModels));
            }
        }

        public UserDetails SelectedUserDetails
        {
            get { return _selectedUserDetails; }
            set
            {
                _selectedUserDetails = value;
                RaisePropertyChanged(nameof(SelectedUserDetails));
            }
        }

        public NewEntryViewModel GetSelectedData()
        {
           return _peopleManager.Get(_selectedUserDetails.Id);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public NewEntryViewModel NewEntryViewModel
        {
            get { return _newEntryViewModel; }
            set
            {
                RaisePropertyChanged(nameof(NewEntryViewModel));
                _newEntryViewModel = value;
            }
        }

        private ICommand _deleteCommand;
        private ICommand _nextInstallment;
        public ICommand NextInstallment
        {
            get
            {
                if (_nextInstallment == null)
                {
                    _nextInstallment = new RelayCommand(OnNextInstallment, null);

                }
                return _nextInstallment;
            }
        }

        private void OnNextInstallment(object obj)
        {

            if (_selectedUserDetails == null)
            {
                MessageBox.Show("No item selected", "Pay Installment");
                return;
            }

            var newEntryViewModel = _peopleManager.GetLatest(_selectedUserDetails.UserUniqueId);
            var balance = newEntryViewModel.CourseFee -  newEntryViewModel.TotalAmountPaidForView;
            if (balance <= 0)
            {
                MessageBox.Show("No installment left to be paid", "Pay Installment");
                return;
            }

            InstallmentHandler.OnShowInstallmentPaymentForm(SelectedUserDetails.UserUniqueId);
        }

        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                {
                    _deleteCommand = new RelayCommand(DeleteClicked, CanDeleteExecute);

                }
                return _deleteCommand;
            }
        }
      

        private void DeleteClicked(object obj)
        {
            if (_selectedUserDetails == null)
            {
                MessageBox.Show("No item selected", "Move to Trash");
                return;
            }

            MessageBox.Show("Selected item will be moved to trash", "Move to Trash");

            _peopleManager.DeletePayment(SelectedUserDetails.Id);
            _peopleManager.Trash(SelectedUserDetails);

            TrashHandler.OnMoveToTrash(SelectedUserDetails);


            _userDetailViewModels.Remove(SelectedUserDetails);
            RaisePropertyChanged(nameof(UserDetailViewModels));

            ReportHandler.ItemsListChanged(UserDetailViewModels.ToList());
        }

        private bool CanDeleteExecute(object arg)
        {
            return true;
        }

        private ICommand _exportData;

        public ICommand ExportData
        {
            get
            {
                if (_exportData == null)
                {
                    _exportData = new RelayCommand(ExportDataExecute);
                }
                return _exportData;
            }

        }

        private void ExportDataExecute(object obj)
        {
            if (UserDetailViewModelsFilter.Count == 0)
            {
                MessageBox.Show("No Data to export", "Export data");
                return;
            }

            if (string.IsNullOrWhiteSpace(_filterText) == false)
            {
                var result = MessageBox.Show("Filter is active. Do you want to continue export of filtered data?",
                    "Export Filtered Data?",MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            var saveFileWindow = new SaveFileDialog();
            saveFileWindow.CheckPathExists = true;
            saveFileWindow.Filter = @"saveAsCsv|*.csv";
           var canSaveResult =  saveFileWindow.ShowDialog();
            if (canSaveResult == DialogResult.Cancel)
            {
                return;
            }
            var filePath = saveFileWindow.FileName;
            try
            {
                File.Delete(filePath);
            }
            catch (IOException exception)
            {
                MessageBox.Show(exception.Message, "Close file and retry export");
                return;
            }

            var csvString = new StringBuilder();
            csvString.Append("TimeStamp,Name,Phone,Course,Center,CourseFee,Balance,AmountPaid,BillNo,TotalAmountPaidTillDate,ModeOfPayment,Cashier,Notes\n");
            foreach (var userDetails in UserDetailViewModelsFilter)
            {
                csvString.Append($"{userDetails.TimeStamp.ToString("dd-MM-yy hh:mm tt")},{userDetails.Name},{userDetails.Phone},{userDetails.Course},{userDetails.Center},{userDetails.CourseFee},{userDetails.Balance},{userDetails.AmountPaid},{userDetails.BillNo},{userDetails.TotalAmountPaid},{userDetails.ModeOfPayment},{userDetails.Cashier},{userDetails.Notes}\n");
            }

            File.WriteAllText(filePath,csvString.ToString());

            Process.Start(filePath);
        }
    }
}
