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
            listOfMode.Add(new PayMode { Id = Guid.Parse("412e4257-2db1-4b82-8751-0cedbd12b931"), Name = "Cash" });
            listOfMode.Add(new PayMode { Id = Guid.Parse("2666536c-ef7e-4ed4-99ef-ea8c41c6c0a5"), Name = "Online" });
            listOfMode.Add(new PayMode { Id = Guid.Parse("20e521a3-fb7f-406f-bfd7-126b3ab35d83"), Name = "Card" });
            listOfMode.Add(new PayMode { Id = Guid.Parse("fa9bde64-bf97-4c08-91ca-014cd0a19479"), Name = "Cheque" });
            listOfMode.Add(new PayMode { Id = Guid.Parse("c77aca7e-9460-49bf-b7ae-a0d4f1a5580c"), Name = "Others" });
            File.WriteAllText(filePath, JsonConvert.SerializeObject(listOfMode, Formatting.Indented));
        }

        private static void InitializeCenterData(string filePath)
        {
            var listofCenter = new List<CenterDetails>
            {
                new CenterDetails() {CenterName = Basavanagudi, Id = Guid.Parse("7be8a0fe-3b79-4350-8652-86174575a5cf")},
                new CenterDetails() {CenterName = Koramangala, Id = Guid.Parse("041f471c-0bc4-4577-882a-5d846345ac1b")},
                new CenterDetails() {CenterName = Jayanagar, Id = Guid.Parse("624674c7-b482-4671-913f-55543558c223")},
                new CenterDetails() {CenterName = Vijaynagar, Id = Guid.Parse("bc18429d-4c77-43d0-9d6c-9f580076d46d")},
                new CenterDetails() {CenterName = Malleshwaram, Id = Guid.Parse("66cae4ff-a7ed-429e-8e79-efd179f4c54b")},
                new CenterDetails() {CenterName = Indiranagar, Id = Guid.Parse("68df583b-0704-47f0-bd66-5e7680220f4b")},
                new CenterDetails() {CenterName = Ganganagar, Id = Guid.Parse("4ed4d0ee-c9f9-4323-9f2f-43f215c510e9")},
                new CenterDetails() {CenterName = Yelahanka, Id = Guid.Parse("7a513d87-34cf-44f9-9ad4-64afa17a704c")},
                new CenterDetails() {CenterName = Basaweshwarnagar, Id = Guid.Parse("0310e0de-c604-4242-a47e-d04e68b5368d")},
                new CenterDetails() {CenterName = Others, Id = Guid.Parse("4b812a3c-09a6-4bff-8b9e-1c1b1cd5575a")}
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
                new CourseDetails {CourseName = "Package PartPayment", Fee = 19999, Id = Guid.Parse("53d210b3-420d-473d-8262-459f30f44d97")},
                new CourseDetails {CourseName = "Package FullPayment", Fee = 19999, Id = Guid.Parse("02fea44b-64a9-4112-9102-a26406dcebf9")},
                new CourseDetails {CourseName = "Handwriting development Part", Fee = 11000, Id = Guid.Parse("8e6c6450-0fc1-46a9-bd63-5b7d1490faaa")},
                new CourseDetails {CourseName = "Handwriting development Full(Junior)", Fee = 9000, Id = Guid.Parse("7ae979fe-a0af-4092-933f-ac12d8d5019f")},
                new CourseDetails {CourseName = "Handwriting development Full(Senior)", Fee = 11000, Id = Guid.Parse("efa0f352-19a2-49f0-ad05-cef1294b803d")},
                new CourseDetails {CourseName = "ExcelPlus part payment", Fee = 12750, Id = Guid.Parse("39dc2316-fd0d-4a4a-ba7e-44d47796ae5c")},
                new CourseDetails {CourseName = "ExcelPlus Installment", Fee = 12750, Id = Guid.Parse("f71b8118-ed29-442a-ab57-083f8551cf4e")},
                new CourseDetails {CourseName = "ExcelPlus Full", Fee = 22500, Id = Guid.Parse("b15dff82-3712-47d9-a799-c2fc6bba9bf5")},
                new CourseDetails {CourseName = "OnlyExcel Full", Fee = 16000, Id = Guid.Parse("3848b551-4aac-4e6a-802c-f2f564e38504")},
                new CourseDetails {CourseName = "OnlyExcel Part", Fee = 16000, Id = Guid.Parse("de2ea948-385b-4b5a-af07-bc46dca1dfa0")}
            };
            File.WriteAllText(filePath, JsonConvert.SerializeObject(listOfCourse, Formatting.Indented));
        }
    }
}