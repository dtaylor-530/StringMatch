using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace FuzzyString.CoreApp.ViewModels
{
    public class Selectable<T> : ReactiveUI.ReactiveObject
    {
        public T Object { get; set; }

        private bool isSelected;


        public bool IsSelected
        {
            get => isSelected;
            set => this.RaiseAndSetIfChanged(ref isSelected, value);
        }
        public double Rank { get; set; }



    }
}
