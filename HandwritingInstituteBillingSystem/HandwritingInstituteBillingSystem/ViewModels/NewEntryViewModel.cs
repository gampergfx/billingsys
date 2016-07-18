using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using HandwritingInstituteBillingSystem.CommonViewHandlers;
using HandwritingInstituteBillingSystem.Logic;
using HandwritingInstituteBillingSystem.Logic.Configs;
using HandwritingInstituteBillingSystem.Printing;

namespace HandwritingInstituteBillingSystem.ViewModels
{
    public class NewEntryViewModel : INotifyPropertyChanged
    {
        private DateTime _timeStamp;
        private string _name;
        private string _phone;
        private CourseDetails _course;
        private double _amountPaid;
        private double _totalAmountPaid;
        private string _cashier;
        private string _billNo;
        private double _courseFee;
        private CenterDetails _center;
        private double _balance;
        private PayMode _modeOfPayment;
        private string _notes;
        private long _userUniqueId = -1;


        public NewEntryViewModel()
        {
            CoursesList = new ObservableCollection<CourseDetails>(ConfigReader.GetCourseDetailsConfigs());
            CenterList = new ObservableCollection<CenterDetails>(ConfigReader.GetCenterDetailsConfigs());
            ModeList = new ObservableCollection<PayMode>(ConfigReader.GetPayModeConfigs());
            InitNew();
        }

        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set
            {
                _timeStamp = value;
                NotifyPropertyChanged();
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged();

            }
        }

        public string Phone
        {
            get { return _phone; }
            set
            {
                _phone = value;
                NotifyPropertyChanged();

            }
        }

        public CourseDetails Course
        {
            get { return _course; }
            set
            {
                _course = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Balance));
            }
        }

        public string AmountPaidString
        {
            get { return string.Empty; }
            set
            {
                double r;
                if(double.TryParse(value,out r))
                {
                    _amountPaid = r;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(Balance));
                    NotifyPropertyChanged(nameof(TotalAmountPaid));
                    return;
                }
                _amountPaid = 0;
            }
        }

        public double AmountPaid
        {
            get { return _amountPaid; }
            set
            {
                _amountPaid = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Balance));
                NotifyPropertyChanged(nameof(TotalAmountPaid));
            }
        }

        public double TotalAmountPaid
        {
            get
            {
                return
                  _totalAmountPaid + _amountPaid;
            }
            set
            {
                _totalAmountPaid = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Balance));
            }
        }

        public double TotalAmountPaidForView
        {
            get
            {
                return
                    _totalAmountPaid;
            }
            set
            {
            }
        }

        public string Cashier
        {
            get { return _cashier; }
            set
            {
                _cashier = value;
                NotifyPropertyChanged();

            }
        }

        public string BillNo
        {
            get { return _billNo; }
            set
            {
                _billNo = value;
                NotifyPropertyChanged();

            }
        }

        public double CourseFee
        {
            get { return _courseFee; }
            set
            {
                _courseFee = value;
                NotifyPropertyChanged();

            }
        }

        public CenterDetails Center
        {
            get { return _center; }
            set
            {
                _center = value;
                NotifyPropertyChanged();

            }
        }

        public string Balance
        {
            get
            {
                _balance = ((double)_course.Fee - (_totalAmountPaid + _amountPaid));
                if (_balance == 0)
                {
                    return "0.0";
                }
                return _balance.ToString("##.###");
            }
            set
            {

            }
        }

        public PayMode ModeOfPayment
        {
            get { return _modeOfPayment; }
            set
            {
                _modeOfPayment = value;
                NotifyPropertyChanged();
            }
        }

        public string Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                NotifyPropertyChanged();

            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void InitNew()
        {
            _name = string.Empty;
            _phone = string.Empty;
            _cashier = AppSettings.Default.CasherName;
            _course = CoursesList[0];
            _modeOfPayment = ConfigReader.GetPayModeConfigs()[0];
            _billNo = BillNoGenerator.Get();
            _amountPaid = 0;
            _timeStamp = DateTime.Now;
            _notes = string.Empty;
            _center = ConfigReader.GetCenterDetailsConfigs()[0];
            _courseFee = (double)_course.Fee;
        }

        public ObservableCollection<CourseDetails> CoursesList { get; set; }
        public ObservableCollection<CenterDetails> CenterList { get; set; }
        public ObservableCollection<PayMode> ModeList { get; set; }

        private ICommand _newEntryCommand;
        public ICommand NewEntryCommand
        {
            get
            {
                if (_newEntryCommand == null)
                {
                    _newEntryCommand = new RelayCommand(NewEntryFormClicked, CanExecute);

                }
                return _newEntryCommand;
            }
        }

        public bool StartValidation { get; set; }
        private bool CanExecute(object param)
        {
            if (StartValidation == false)
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(_name) || string.IsNullOrWhiteSpace(_phone))
            {
                MessageBox.Show("Name or Phone number is empty");

                return false;
            }

            if (_amountPaid.CompareTo(0) == 0)
            {
                MessageBox.Show("Amount paid is zero.");

                return false;
            }

            return true;
        }

        private bool CanExecuteInstallment(object param)
        {
            if (StartValidation == false)
            {
                return true;
            }

            if (_amountPaid.CompareTo(0) == 0)
            {
                MessageBox.Show("Amount paid is zero.");

                return false;
            }

            return true;
        }

        private void NewEntryFormClicked(object obj)
        {
            var newEntryViewModel = obj as NewEntryViewModel;
            NewFormViewHandler.OnNewEntry(newEntryViewModel);
            PrintForm();
            NewFormViewHandler.CloseForm();
        }


        private ICommand _printdoc;
        private bool _printPos = true;

        public ICommand PrintCommand
        {
            get
            {
                if (_printdoc == null)
                {
                    _printdoc = new RelayCommand(PrintDoc, null);

                }
                return _printdoc;
            }
        }

        private void PrintDoc(object obj)
        {
            PrintForm();
            ViewPrintViewHandler.CloseForm();
        }

        private void PrintForm()
        {
            if (_printPos == false)
            {
                PrintHelper.PrintDocAsDocx(this);
            }
            else
            {
                PrintHelper.PrintDocAsPoc(this);
            }
        }

        public bool PrintPos
        {
            get { return _printPos; }
            set
            {
                _printPos = value;
                NotifyPropertyChanged();
            }
        }

        public bool PrintA4
        {
            get { return !_printPos; }
            set
            {
                _printPos = !value;
                NotifyPropertyChanged();
            }
        }

        private ICommand _nextInstallment;
        public ICommand NextInstallment
        {
            get
            {
                if (_nextInstallment == null)
                {
                    _nextInstallment = new RelayCommand(OnNextInstallment, CanExecuteInstallment);

                }
                return _nextInstallment;
            }
        }

        public long UserUniqueId
        {
            get { return _userUniqueId; }
            set { _userUniqueId = value; }
        }

        private void OnNextInstallment(object obj)
        {
            var newEntryViewModel = obj as NewEntryViewModel;
            InstallmentHandler.OnInstallmentPayment(newEntryViewModel);
            PrintForm();
            InstallmentHandler.CloseForm();
        }

        public void Set(NewEntryViewModel value)
        {
            Name = value.Name;
            Phone = value.Phone;
            TotalAmountPaid = value.TotalAmountPaid;
            Notes = value.Notes;
            Course = value.Course;
            CourseFee = value.CourseFee;
            Center = value.Center;
            UserUniqueId = value.UserUniqueId;
        }

        public void SetForPrint(NewEntryViewModel value)
        {
            Name = value.Name;
            Phone = value.Phone;
            //TotalAmountPaid = value.TotalAmountPaid;
            TotalAmountPaidForView = value.TotalAmountPaidForView;
            AmountPaid = value.AmountPaid;
            Notes = value.Notes;
            Course = value.Course;
            CourseFee = value.CourseFee;
            Center = value.Center;
            UserUniqueId = value.UserUniqueId;
        }
    }
}
