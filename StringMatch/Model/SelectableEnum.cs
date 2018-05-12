using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringMatch
{

        public class SelectableEnum : INotifyPropertyChanged
        {

            public Algorithm Algorithm { get; set; }

            private bool isSelected;


            public bool IsSelected
            {
                get { return isSelected; }
                set
                {
                    isSelected = value;
                    NotifyChanged(nameof(IsSelected));
                }
            }


            #region INotifyPropertyChanged Implementation
            /// <summary>
            /// Occurs when any properties are changed on this object.
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;


            /// <summary>
            /// A helper method that raises the PropertyChanged event for a property.
            /// </summary>
            /// <param name="propertyNames">The names of the properties that changed.</param>
            public virtual void NotifyChanged(params string[] propertyNames)
            {
                foreach (string name in propertyNames)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
                }
            }


            protected void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string caller = "")
            {

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));

            }
        #endregion

    
    }
}

