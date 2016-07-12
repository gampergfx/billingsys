using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandwritingInstituteBillingSystem.Logic;
using HandwritingInstituteBillingSystem.ViewModels;

namespace HandwritingInstituteBillingSystem.CommonViewHandlers
{
    static class NewFormViewHandler
    {
        public static event EventHandler OnCloseForm;

        public static void CloseForm()
        {
            OnCloseForm?.Invoke(null, EventArgs.Empty);
        }


        public static event EventHandler<NewEntryViewModel> NewEntry;

        public static void OnNewEntry(NewEntryViewModel e)
        {
            BillNoGenerator.IncrementBillSequence();
            NewEntry?.Invoke(null, e);
        }
    }

    static class ViewPrintViewHandler
    {
        public static event EventHandler OnCloseForm;

        public static void CloseForm()
        {
            OnCloseForm?.Invoke(null, EventArgs.Empty);
        }
    }


    static class ReportHandler
    {
        public static event EventHandler OnItemsListChanged;

        public static void ItemsListChanged(List<UserDetails> toList)
        {
            OnItemsListChanged?.Invoke(toList, EventArgs.Empty);
        }
    }
}
