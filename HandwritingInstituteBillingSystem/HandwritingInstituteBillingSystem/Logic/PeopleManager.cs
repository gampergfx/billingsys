using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HandwritingInstituteBillingSystem.Logic.Configs;
using HandwritingInstituteBillingSystem.ViewModels;
using Newtonsoft.Json;

namespace HandwritingInstituteBillingSystem.Logic
{
     static class Store
     {
         private static readonly PeopleManager PeopleManager;
         static Store()
         {
            PeopleManager = new PeopleManager();
         }

         public static PeopleManager Get()
         {
             return PeopleManager;
         }
     }

    class PeopleManager
    {
        private readonly IList<CourseDetails> _courseList;
        private readonly IList<CenterDetails> _centerList;
        private readonly IList<PayMode> _modeList;
        private IList<UserPaymentData> _paymentData;
        private readonly string _filePathOfUsers;
        private readonly string _filePathOfTarsh;
        private IList<UserPaymentData> _trashData;

        public PeopleManager()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _courseList = ConfigReader.GetCourseDetailsConfigs();
            _centerList = ConfigReader.GetCenterDetailsConfigs();
            _modeList = ConfigReader.GetPayModeConfigs();
            var fileName = "UserPaymentData.Json";
            var tasrshfileName = "UserTrashData.Json";
            _filePathOfUsers = Path.Combine(baseDirectory, fileName);
            _filePathOfTarsh = Path.Combine(baseDirectory, tasrshfileName);
        }

        private T ReadConfig<T>(Action<string> initializeMethod, string filePath)
        {
            if (File.Exists(filePath) == false)
            {
                initializeMethod(filePath);
            }
            var fileContent = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(fileContent);
        }

        public IList<UserDetails> GetFromStorage()
        {
            _paymentData = ReadConfig<IList<UserPaymentData>>(filePath => WriteToFile(filePath, new List<UserPaymentData>()), _filePathOfUsers);
            var userDetails = _paymentData.Select(GetUserDetails);
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
                TotalAmountPaid = userPaymentData.TotalAmountPaid,
                BillNo = userPaymentData.BillNo,
                Notes = userPaymentData.Notes,
                Course = courseDetails.CourseName,
                Center = _centerList.First(x => x.Id == userPaymentData.CenterId).CenterName,
                ModeOfPayment = _modeList.First(x => x.Id == userPaymentData.ModeOfPaymentId).Name,
                Balance = (double)courseDetails.Fee - userPaymentData.TotalAmountPaid,
                Cashier = userPaymentData.Cashier,
                TimeStamp = userPaymentData.TimeStamp,
                Id = userPaymentData.Id,
                UserUniqueId = userPaymentData.UserUniqueId,
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
            _paymentData.Add(userpayment);
            StoreUpdated(_filePathOfUsers, _paymentData);
        }

        public void Trash(UserDetails userDetails)
        {
            var userpayment = GetUserPayment(userDetails);
            _trashData.Add(userpayment);
            StoreUpdated(_filePathOfTarsh, _trashData);
        }

        private void StoreUpdated(string filePath, IList<UserPaymentData> data)
        {
            File.Delete(filePath);
            WriteToFile(filePath, data);
        }

        private UserPaymentData GetUserPayment(UserDetails userDetails)
        {
            return new UserPaymentData()
            {
                Id = userDetails.Id,
                UserUniqueId = userDetails.UserUniqueId,
                Name = userDetails.Name,
                Phone = userDetails.Phone,
                AmountPaid = userDetails.AmountPaid,
                TotalAmountPaid = userDetails.TotalAmountPaid,
                BillNo = userDetails.BillNo,
                Cashier = userDetails.Cashier,
                Notes = userDetails.Notes,
                CenterId = _centerList.First(x => x.CenterName == userDetails.Center).Id,
                CourseId = _courseList.First(x => x.CourseName == userDetails.Course).Id,
                ModeOfPaymentId = _modeList.First(x => x.Name == userDetails.ModeOfPayment).Id,
                TimeStamp = userDetails.TimeStamp
            };
        }

        public void DeletePayment(Guid id)
        {
            var item = _paymentData.First(x => x.Id == id);
            _paymentData.Remove(item);
                 StoreUpdated(_filePathOfUsers, _paymentData);
        }

        public void DeleteTrash(Guid id)
        {
            var item = _trashData.First(x => x.Id == id);
            _trashData.Remove(item);
            StoreUpdated(_filePathOfTarsh, _trashData);
        }


        public NewEntryViewModel Get(Guid id)
        {
            var item = _paymentData.First(x => x.Id == id);
            var courseDetails = _courseList.First(x => x.Id == item.CourseId);
            return new NewEntryViewModel()
            {
                Name = item.Name,
                Phone = item.Phone,
                AmountPaid = item.AmountPaid,
                TotalAmountPaid = item.TotalAmountPaid,
                Center = _centerList.First(x => x.Id == item.CenterId),
                Course = courseDetails,
                CourseFee = (double) courseDetails.Fee,
                ModeOfPayment = _modeList.First(x => x.Id == item.ModeOfPaymentId),
                Balance = (courseDetails.Fee -  (decimal) item.TotalAmountPaid).ToString("##.###"),
                Notes = item.Notes,
                BillNo = item.BillNo,
                Cashier = item.Cashier,
                TimeStamp = item.TimeStamp,
                UserUniqueId = item.UserUniqueId
            };
        }

        public List<UserDetails> GetTrash()
        {
            _trashData = ReadConfig<IList<UserPaymentData>>(filePath => WriteToFile(filePath, new List<UserPaymentData>()), _filePathOfTarsh);
            var userDetails = _trashData.Select(GetUserDetails);
            return userDetails.ToList();
        }

        public NewEntryViewModel GetLatest(long userUniqueValue)
        {
            var value = _paymentData.Where(x => x.UserUniqueId == userUniqueValue).OrderByDescending(x=>x.TimeStamp).First().Id;
            return Get(value);
        }
    }

    class UserPaymentData
    {
        public Guid Id { get; set; }
        public long UserUniqueId { get; set; }
        public DateTime TimeStamp { set; get; }
        public string Name { set; get; }
        public string Phone { set; get; }
        public Guid CourseId { set; get; }
        public Guid CenterId { set; get; }
        public double AmountPaid { set; get; }
        public double TotalAmountPaid { set; get; }
        public string Cashier { set; get; }
        public string BillNo { set; get; }
        public Guid ModeOfPaymentId { set; get; }
        public string Notes { set; get; }
    }
}
