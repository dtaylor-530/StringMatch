using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringMatch
{
    public class PotentialMatch :ReactiveObject//: System.ComponentModel.INotifyPropertyChanged
    {
        private bool isMatch = false;

        public bool IsMatch
        {
            get => isMatch; 
            set => this.RaiseAndSetIfChanged(ref isMatch, value);
        }

        public string Source { get; set; }

        private Dictionary<string, double> matches;

        //public PotentialMatch(string item1, string[] item2)
        //{
        //    this.Source = item1;
        //    this.matches = item2;
        //}

        public PotentialMatch(string item1, Dictionary<string, double> matches = null)
        {
            this.Source = item1;
            this.matches = matches;
        }
        private string GetMatch(int i) => matches.Count > i ? matches.Keys.ElementAt(i) : string.Empty;
        //private void SetMatch(int i,string value)
        //{
        //    if (matches.Keys.ElementAt(i) != value)
        //    {
        //        matches.Keys.ElementAt(i) = value; NotifyChanged(nameof(Match1));
        //    }
        //}

        public double BestScore => matches.First().Value;

        public string Match1
        {
            get { return GetMatch(0); }
            //set { SetMatch(0); }
        }

        public string Match2
        {
            get { return GetMatch(1); }
         //   set { if (matches[1] != value) { matches[1] = value; NotifyChanged(nameof(Match2)); } }
        }

        public string Match3
        {
            get { return GetMatch(2); ; }
           // set { if (matches[2] != value) { matches[2] = value; NotifyChanged(nameof(Match3)); } }
        }

        public string Match4
        {
            get { return GetMatch(3); }
        //    set { if (matches[3] != value) { matches[3] = value; NotifyChanged(nameof(Match4)); } }
        }

        public string Match5
        {
            get { return GetMatch(4); }
          //  set { if (matches[4] != value) { matches[4] = value; NotifyChanged(nameof(Match5)); } }
        }

        public string Match6
        {
            get { return GetMatch(5); ; }
            //set { if (matches[5] != value) { matches[5] = value; NotifyChanged(nameof(Match6)); } }
        }

        public string Match7
        {
            get { return GetMatch(6); }
            //set { if (matches[6] != value) { matches[6] = value; NotifyChanged(nameof(Match7)); } }
        }

        //public string[] Matches
        //{
        //    set
        //    {
        //        try
        //        {
        //            matches[0] = value[0];
        //            matches[1] = value[1];
        //            matches[2] = value[2];
        //            matches[3] = value[3];
        //            matches[4] = value[4];
        //            matches[5] = value[5];
        //            matches[6] = value[6];
        //        }
        //        catch { }
        //        //matches[7] = value[7];
        //        NotifyChanged(nameof(Match1), nameof(Match2), nameof(Match3), nameof(Match4), nameof(Match5), nameof(Match6), nameof(Match7));
        //    }
        //}

        //#region INotifyPropertyChanged Implementation

        ///// <summary>
        ///// Occurs when any properties are changed on this object.
        ///// </summary>
        //public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        ///// <summary>
        ///// A helper method that raises the PropertyChanged event for a property.
        ///// </summary>
        ///// <param name="propertyNames">The names of the properties that changed.</param>
        //public virtual void NotifyChanged(params string[] propertyNames)
        //{
        //    foreach (string name in propertyNames)
        //    {
        //        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
        //    }
        //}

        //protected void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string caller = "")
        //{
        //    PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(caller));
        //}

        //#endregion INotifyPropertyChanged Implementation
    }
}
