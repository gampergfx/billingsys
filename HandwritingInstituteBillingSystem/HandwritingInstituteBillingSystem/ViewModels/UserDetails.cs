using System;

namespace HandwritingInstituteBillingSystem.ViewModels
{
    class UserDetails
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }

        public string Course { get; set; }

        public double AmountPaid { get; set; }

        public string Cashier { get; set; }

        public string BillNo { get; set; }
        
        public string Center { get; set; }

        public string CourseFee { get; set; }

        public double Balance  { get; set; }

        public string ModeOfPayment { get; set; }
        
        public string Notes { get; set; }
    }

    class UserDetailsExcelData
    {
        public string TimeStamp { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Course { get; set; }  
        public double AmountPaid { get; set; }
        public string Cashier { get; set; }
        public string BillNo { get; set; }
        public string Center { get; set; }
        public double Balance { get; set; }
        public string ModeOfPayment { get; set; }
        public string Notes { get; set; }
    }
}
