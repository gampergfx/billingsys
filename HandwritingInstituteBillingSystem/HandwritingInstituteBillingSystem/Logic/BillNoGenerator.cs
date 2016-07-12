using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandwritingInstituteBillingSystem.Logic
{
    static class BillNoGenerator
    {
        private static readonly string MachineNumber;
        public static double MinAmountForBill { get; }

        static BillNoGenerator()
        {
            MachineNumber = AppSettings.Default.MachineUniqueNumberForBilling;
            MinAmountForBill = AppSettings.Default.MinBillAmount;
        }

        public static string Get()
        {
            var currentSeqNo = AppSettings.Default.CurentBillSeqNo;
            var dateString = DateTime.Now.ToString("ddMMyy");
            var seperator = "/";
            return string.Concat(dateString,seperator, MachineNumber, seperator, currentSeqNo);
        }

        private static void UpdateCurrentSeqNo(int currentSeqNo)
        {
            var nextSeqNumber = AppSettings.Default.CurentBillSeqNo+1;
            AppSettings.Default.CurentBillSeqNo = nextSeqNumber;
            AppSettings.Default.Save();
        }

        public static void IncrementBillSequence()
        {
            var currentSeqNo = AppSettings.Default.CurentBillSeqNo;
            UpdateCurrentSeqNo(currentSeqNo);
        }
    }
}
