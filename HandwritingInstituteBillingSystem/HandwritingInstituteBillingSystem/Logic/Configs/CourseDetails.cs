﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HandwritingInstituteBillingSystem.Annotations;

namespace HandwritingInstituteBillingSystem.Logic.Configs
{
    public class CourseDetails: INotifyPropertyChanged
    {
        public string CourseName { get; set; }
        public decimal Fee { get; set; }
        public Guid Id { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return CourseName;
        }
    }
}
