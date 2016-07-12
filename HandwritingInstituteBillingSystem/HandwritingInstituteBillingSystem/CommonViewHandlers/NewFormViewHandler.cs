using System;
using System.Collections.Generic;
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

    static class InstallmentHandler 
    {
        public static event EventHandler<long> ShowInstallmentPaymentForm;
        public static event EventHandler<NewEntryViewModel> InstallmentPayment;

        public static void OnShowInstallmentPaymentForm(long e)
        {
           ShowInstallmentPaymentForm?.Invoke(null, e);
        }

        public static void OnInstallmentPayment(NewEntryViewModel e)
        {
            InstallmentPayment?.Invoke(null, e);
        }

        public static event EventHandler OnCloseForm;

        public static void CloseForm()
        {
            OnCloseForm?.Invoke(null, EventArgs.Empty);
        }
    }

    static class TrashHandler   
    {
        public static event EventHandler MoveToTrash;
        public static event EventHandler RestoreTrash;


        public static void OnMoveToTrash(UserDetails selectedUserDetails)
        {
            MoveToTrash?.Invoke(selectedUserDetails, EventArgs.Empty);
        }

        public static void OnRestoreTrash(UserDetails selectedUserDetails)
        {
            RestoreTrash?.Invoke(selectedUserDetails, EventArgs.Empty);
        }
    }
}
