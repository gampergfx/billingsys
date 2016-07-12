using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HandwritingInstituteBillingSystem.Logic.Configs;
using HandwritingInstituteBillingSystem.ViewModels;
using Newtonsoft.Json;

namespace HandwritingInstituteBillingSystem.Logic
{
    class PeopleManager
    {
        private readonly IList<CourseDetails> _courseList;
        private readonly IList<CenterDetails> _centerList;
        private readonly IList<PayMode> _modeList;
        private IList<UserPaymentData> _data;
        private readonly string _filePath;

        public PeopleManager()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _courseList = ConfigReader.GetCourseDetailsConfigs();
            _centerList = ConfigReader.GetCenterDetailsConfigs();
            _modeList = ConfigReader.GetPayModeConfigs();
            var fileName = "UserPaymentData.Json";
            _filePath = Path.Combine(baseDirectory, fileName);
        }

        private T ReadConfig<T>(Action<string> initializeMethod)
        {
            if (File.Exists(_filePath) == false)
            {
                initializeMethod(_filePath);
            }
            var fileContent = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<T>(fileContent);
        }

        public IList<UserDetails> GetFromStorage()
        {
            _data = ReadConfig<IList<UserPaymentData>>(filePath => WriteToFile(filePath, new List<UserPaymentData>()));
            var userDetails = _data.Select(GetUserDetails);
            return userDetails.ToList();
        }

        private UserDetails GetUserDetails(UserPaymentData userPaymentData)
        {
            var courseDetails = _courseList.First(x => x.Id == userPaymentData.CourseId);
            return new UserDetails
            {
                Name = userPaymentData.Name,
                Phone = userPaymentData.Phone,
                AmountPaid = userPaymentData.AmountPaid,
                BillNo = userPaymentData.BillNo,
                Notes = userPaymentData.Notes,
                Course = courseDetails.CourseName,
                Center = _centerList.First(x => x.Id == userPaymentData.CenterId).CenterName,
                ModeOfPayment = _modeList.First(x => x.Id == userPaymentData.ModeOfPaymentId).Name,
                Balance = (double)courseDetails.Fee - userPaymentData.AmountPaid,
                Cashier = userPaymentData.Cashier,
                TimeStamp = userPaymentData.TimeStamp,
                Id = userPaymentData.Id,
                CourseFee = courseDetails.Fee.ToString("##.###")
            };
        }

        private void WriteToFile(string filePath, IList<UserPaymentData> userDetailses)
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(userDetailses, Formatting.Indented));
        }

        public void Store(UserDetails userDetails)
        {
            var userpayment = GetUserPayment(userDetails);
            _data.Add(userpayment);
            StoreUpdated();
        }

        private void StoreUpdated()
        {
            File.Delete(_filePath);
            WriteToFile(_filePath, _data);
        }

        private UserPaymentData GetUserPayment(UserDetails userDetails)
        {
            return new UserPaymentData()
            {
                Id = userDetails.Id,
                Name = userDetails.Name,
                Phone = userDetails.Phone,
                AmountPaid = userDetails.AmountPaid,
                BillNo = userDetails.BillNo,
                Cashier = userDetails.Cashier,
                Notes = userDetails.Notes,
                CenterId = _centerList.First(x => x.CenterName == userDetails.Center).Id,
                CourseId = _courseList.First(x => x.CourseName == userDetails.Course).Id,
                ModeOfPaymentId = _modeList.First(x => x.Name == userDetails.ModeOfPayment).Id,
                TimeStamp = userDetails.TimeStamp
            };
        }

        public void Delete(Guid id)
        {
            var item = _data.First(x => x.Id == id);
            _data.Remove(item);
                 StoreUpdated();
        }

        public NewEntryViewModel Get(Guid id)
        {
            var item = _data.First(x => x.Id == id);
            var courseDetails = _courseList.First(x => x.Id == item.CourseId);
            return new NewEntryViewModel()
            {
                Name = item.Name,
                Phone = item.Phone,
                AmountPaid = item.AmountPaid,
                Center = _centerList.First(x => x.Id == item.CenterId),
                Course = courseDetails,
                ModeOfPayment = _modeList.First(x => x.Id == item.ModeOfPaymentId),
                Balance = (courseDetails.Fee -  (decimal) item.AmountPaid).ToString("##.###"),
                Notes = item.Notes,
                BillNo = item.BillNo,
                Cashier = item.Cashier,
                TimeStamp = item.TimeStamp
            };
        }
    }

    class UserPaymentData
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { set; get; }
        public string Name { set; get; }
        public string Phone { set; get; }
        public Guid CourseId { set; get; }
        public Guid CenterId { set; get; }
        public double AmountPaid { set; get; }
        public string Cashier { set; get; }
        public string BillNo { set; get; }
        public Guid ModeOfPaymentId { set; get; }
        public string Notes { set; get; }
    }
}
