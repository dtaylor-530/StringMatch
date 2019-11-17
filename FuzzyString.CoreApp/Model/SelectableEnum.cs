
using FuzzyString;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringMatch
{

    public class SelectableEnum : ReactiveUI.ReactiveObject
    {

        public Algorithm Algorithm { get; set; }

        private bool isSelected;


        public bool IsSelected
        {
            get => isSelected;
            set => this.RaiseAndSetIfChanged(ref isSelected, value);
        }
    }
}

