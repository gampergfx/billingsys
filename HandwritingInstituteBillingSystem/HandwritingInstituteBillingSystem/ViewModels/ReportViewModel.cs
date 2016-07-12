using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HandwritingInstituteBillingSystem.Annotations;
using HandwritingInstituteBillingSystem.CommonViewHandlers;
using HandwritingInstituteBillingSystem.Logic;
using HandwritingInstituteBillingSystem.Logic.Configs;
using System.Text;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using System.IO;
using System.Diagnostics;

namespace HandwritingInstituteBillingSystem.ViewModels
{
    class ReportViewModel : INotifyPropertyChanged
    {

        private IList<UserDetails> listOfUserData;
        public ReportViewModel()
        {
            listOfUserData = new PeopleManager().GetFromStorage();
            _reportDatas = new ObservableCollection<ReportData>();
            ReportHandler.OnItemsListChanged += CalculateReportForList;
            if (listOfUserData.Count == 0)
            {
                StartTime = DateTime.Today;
                EndTime = DateTime.Today;
                return;
            }
            StartTime = listOfUserData.Min(x => x.TimeStamp);
            EndTime = DateTime.Today;
            CalculateReport();
        }

        private void CalculateReportForList(object sender, EventArgs e)
        {
            var list = sender as List<UserDetails>;
            if (list == null)
            {
                return;
            }
            listOfUserData = list;
            CalculateReport();
        }

        private void CalculateReport()
        {
            var listToCalulate = listOfUserData.Where(x => x.TimeStamp.Date <= EndTime.Date && x.TimeStamp.Date >= StartTime.Date).ToArray();
            if (listToCalulate.Length == 0)
            {
                GrandTotal = "0";
                ReportDatas = new ObservableCollection<ReportData>();
                return;
            }
            GrandTotal = listToCalulate.Sum(x => x.AmountPaid).ToString("##.###");

            var basavanGudiTotal = GetTotalByCenter(listToCalulate, ConfigReader.Basavanagudi);
            var kormaglaTotal = GetTotalByCenter(listToCalulate, ConfigReader.Koramangala);
            var basavershwar = GetTotalByCenter(listToCalulate, ConfigReader.Basaweshwarnagar);
            var ganganagar = GetTotalByCenter(listToCalulate, ConfigReader.Ganganagar);
            var indiranagar = GetTotalByCenter(listToCalulate, ConfigReader.Indiranagar);
            var jayanagar = GetTotalByCenter(listToCalulate, ConfigReader.Jayanagar);
            var malleshawarum = GetTotalByCenter(listToCalulate, ConfigReader.Malleshwaram);
            var others = GetTotalByCenter(listToCalulate, ConfigReader.Others);
            var vijayanagar = GetTotalByCenter(listToCalulate, ConfigReader.Vijaynagar);
            var yelanka = GetTotalByCenter(listToCalulate, ConfigReader.Yelahanka);
            int i = 0;
            var totalReport = new ReportData()
            {
                SlNo = i,
                CourseName = "Grand Total",
                Basavanagudi = basavanGudiTotal,
                Basaweshwarnagar = basavershwar,
                Ganganagar = ganganagar,
                Indiranagar = indiranagar,
                Jayanagar = jayanagar,
                Koramangala = kormaglaTotal,
                Malleshwaram = malleshawarum,
                Vijaynagar = vijayanagar,
                Yelahanka = yelanka,
                Others = others
            };

            var listOfReportData = new List<ReportData>() { totalReport };
            var groupCourse = listToCalulate.GroupBy(x => x.Course);
          
            foreach (IGrouping<string, UserDetails> grouping in groupCourse)
            {
                i++;
                var data = new ReportData();
                data.CourseName = grouping.Key;
                data.SlNo = i;
                var list = grouping.ToArray();
                data.Basavanagudi = GetTotalByCenter(list, ConfigReader.Basavanagudi);
                data.Koramangala = GetTotalByCenter(list, ConfigReader.Koramangala);
                data.Basaweshwarnagar = GetTotalByCenter(list, ConfigReader.Basaweshwarnagar);
                data.Ganganagar = GetTotalByCenter(list, ConfigReader.Ganganagar);
                data.Indiranagar = GetTotalByCenter(list, ConfigReader.Indiranagar);
                data.Jayanagar = GetTotalByCenter(list, ConfigReader.Jayanagar);
                data.Malleshwaram = GetTotalByCenter(list, ConfigReader.Malleshwaram);
                data.Others = GetTotalByCenter(list, ConfigReader.Others);
                data.Vijaynagar = GetTotalByCenter(list, ConfigReader.Vijaynagar);
                data.Yelahanka = GetTotalByCenter(list, ConfigReader.Yelahanka);
                listOfReportData.Add(data);
            }

            ReportDatas = new ObservableCollection<ReportData>(listOfReportData);
        }

        private static string GetTotalByCenter(UserDetails[] listToCalulate, string basavanagudi)
        {
            var resultList = listToCalulate.Where(x => x.Center == basavanagudi).ToArray();
            if (resultList.Any() == false)
            {
                return "";
            }
            var count = resultList.Length;
            return string.Concat(count , "(Rs ", resultList.Sum(x => x.AmountPaid).ToString("###.##"),")");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GrandTotal
        {
            get { return _grandTotal; }
            set
            {
                _grandTotal = value;
                OnPropertyChanged();
            }
        }

        public DateTime StartTime
        {
            get { return _startTime; }
            set
            {
                OnPropertyChanged();
                _startTime = value;
            }
        }

        public DateTime EndTime
        {
            get { return _endTime; }
            set
            {
                OnPropertyChanged();
                _endTime = value;
            }
        }

        public ICommand Submit
        {
            get
            {
                if (_submit == null)
                {
                    _submit = new RelayCommand(OnSubmit);
                }
                return _submit;
            }
        }

        public ICommand Export
        {
            get
            {
                if (_export == null)
                {
                    _export = new RelayCommand(OnExport);
                }
                return _export;
            }
        }

        private void OnExport(object obj)
        {
            if (ReportDatas.Count == 0)
            {
                MessageBox.Show("No Data to export", "Export data");
                return;
            }
            var saveFileWindow = new SaveFileDialog();
            saveFileWindow.CheckPathExists = true;
            saveFileWindow.Filter = @"saveAsCsv|*.csv";
            var canSaveResult = saveFileWindow.ShowDialog();
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
            csvString.Append("SL No,List Of Courses,Basavanagudi,Koramangala,Jayanagar,Vijaynagar,Malleshwaram,Indiranagar,Ganganagar,Yelahanka,Basaweshwarnagar,Others\n");
            foreach (var report in ReportDatas)
            {
                csvString.Append($"{report.SlNo},{report.CourseName},{report.Basavanagudi},{report.Koramangala},{report.Jayanagar},{report.Vijaynagar},{report.Malleshwaram},{report.Indiranagar},{report.Ganganagar},{report.Yelahanka},{report.Basaweshwarnagar},{report.Others}\n");
            }

            File.WriteAllText(filePath, csvString.ToString());

            Process.Start(filePath);
        }

        private void OnSubmit(object obj)
        {
            if (StartTime > EndTime)
            {
                MessageBox.Show("Invalid Start and end date selected");
                return;
            }
            CalculateReport();
        }

        public ICommand _export;

        public DateTime _startTime;
        public DateTime _endTime;


        private string _grandTotal;

        public ICommand _submit;

        private ObservableCollection<ReportData> _reportDatas;
        public ObservableCollection<ReportData> ReportDatas
        {
            get
            {
                return _reportDatas;
            }

            set
            {
                _reportDatas = value;
                OnPropertyChanged();
            }
        }
    }

    class ReportData
    {
        public int SlNo { get; set; }
        public string CourseName { get; set; }
        public string Basavanagudi { get; set; }
        public string Koramangala { get; set; }
        public string Jayanagar { get; set; }
        public string Vijaynagar { get; set; }
        public string Malleshwaram { get; set; }
        public string Indiranagar { get; set; }
        public string Ganganagar { get; set; }
        public string Yelahanka { get; set; }
        public string Basaweshwarnagar { get; set; }
        public string Others { get; set; }
    }
}