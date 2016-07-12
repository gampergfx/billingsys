using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace HandwritingInstituteBillingSystem.Logic.Configs
{
    static class ConfigReader
    {
        internal const string Basavanagudi = "Basavanagudi";
        internal const string Koramangala = "Koramangala";
        internal const string Jayanagar = "Jayanagar";
        internal const string Vijaynagar = "Vijaynagar";
        internal const string Malleshwaram = "Malleshwaram";
        internal const string Indiranagar = "Indiranagar";
        internal const string Ganganagar = "Ganganagar";
        internal const string Yelahanka = "Yelahanka";
        internal const string Basaweshwarnagar = "Basaweshwarnagar";
        internal const string Others = "Others";
        private static readonly string BaseDirectory;

        static ConfigReader()
        {
            BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        public static IList<CourseDetails> GetCourseDetailsConfigs()
        {
            return ReadConfig<IList<CourseDetails>>("CourseList.json", InitializeCourseData);
        }

        public static IList<CenterDetails> GetCenterDetailsConfigs()
        {
            return ReadConfig<IList<CenterDetails>>("CenterDetails.json", InitializeCenterData);
        }

        public static IList<PayMode> GetPayModeConfigs()
        {
            return ReadConfig<IList<PayMode>>("PayMode.json", InitializePayModeData);
        }

        private static void InitializePayModeData(string filePath)
        {
            var listOfMode = new List<PayMode>();
            listOfMode.Add(new PayMode { Id = Guid.NewGuid(), Name = "Online" });
            listOfMode.Add(new PayMode { Id = Guid.NewGuid(), Name = "Card" });
            listOfMode.Add(new PayMode { Id = Guid.NewGuid(), Name = "Cash" });
            listOfMode.Add(new PayMode { Id = Guid.NewGuid(), Name = "Cheque" });
            listOfMode.Add(new PayMode { Id = Guid.NewGuid(), Name = "Others" });
            File.WriteAllText(filePath, JsonConvert.SerializeObject(listOfMode, Formatting.Indented));
        }

        private static void InitializeCenterData(string filePath)
        {
            var listofCenter = new List<CenterDetails>
            {
                new CenterDetails() {CenterName = Basavanagudi, Id = Guid.NewGuid()},
                new CenterDetails() {CenterName = Koramangala, Id = Guid.NewGuid()},
                new CenterDetails() {CenterName = Jayanagar, Id = Guid.NewGuid()},
                new CenterDetails() {CenterName = Vijaynagar, Id = Guid.NewGuid()},
                new CenterDetails() {CenterName = Malleshwaram, Id = Guid.NewGuid()},
                new CenterDetails() {CenterName = Indiranagar, Id = Guid.NewGuid()},
                new CenterDetails() {CenterName = Ganganagar, Id = Guid.NewGuid()},
                new CenterDetails() {CenterName = Yelahanka, Id = Guid.NewGuid()},
                new CenterDetails() {CenterName = Basaweshwarnagar, Id = Guid.NewGuid()},
                new CenterDetails() {CenterName = Others, Id = Guid.NewGuid()}
            };
            File.WriteAllText(filePath, JsonConvert.SerializeObject(listofCenter, Formatting.Indented));
        }

        private static T ReadConfig<T>(string fileName, Action<string> initializeMethod)
        {
            var filePath = Path.Combine(BaseDirectory, fileName);
            if (File.Exists(filePath) == false)
            {
                initializeMethod(filePath);
            }
            var fileContent = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(fileContent);
        }

        private static void InitializeCourseData(string filePath)
        {
            var listOfCourse = new List<CourseDetails>
            {
                new CourseDetails {CourseName = "Package PartPayment", Fee = 19999, Id = Guid.NewGuid()},
                new CourseDetails {CourseName = "Package FullPayment", Fee = 19999, Id = Guid.NewGuid()},
                new CourseDetails {CourseName = "Handwriting development Part", Fee = 11000, Id = Guid.NewGuid()},
                new CourseDetails {CourseName = "Handwriting development Full(Junior)", Fee = 9000, Id = Guid.NewGuid()},
                new CourseDetails {CourseName = "Handwriting development Full(Senior)", Fee = 11000, Id = Guid.NewGuid()},
                new CourseDetails {CourseName = "ExcelPlus part payment", Fee = 12750, Id = Guid.NewGuid()},
                new CourseDetails {CourseName = "ExcelPlus Instalment", Fee = 12750, Id = Guid.NewGuid()},
                new CourseDetails {CourseName = "ExcelPlus Full", Fee = 22500, Id = Guid.NewGuid()},
                new CourseDetails {CourseName = "OnlyExcel Full", Fee = 16000, Id = Guid.NewGuid()},
                new CourseDetails {CourseName = "OnlyExcel Part", Fee = 16000, Id = Guid.NewGuid()}
            };
            File.WriteAllText(filePath, JsonConvert.SerializeObject(listOfCourse, Formatting.Indented));
        }
    }
}