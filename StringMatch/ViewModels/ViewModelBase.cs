
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Input;
using System.Xml.Linq;
using System.ComponentModel;
using PropertyTools.DataAnnotations;


namespace StringMatch
{

    public class MainViewModelBase : INotifyPropertyChanged
    {
        #region Parameters
        private readonly MvvmDialogs.IDialogService DialogService;

        /// <summary>
        /// Title of the application, as displayed in the top bar of the window
        /// </summary>
        [PropertyTools.DataAnnotations.Browsable(false)]
        public string Title
        {
            get { return "WpfApp1"; }
        }
        #endregion

        #region Constructors
        public MainViewModelBase()
        {
            //DialogService is used to handle dialogs
            this.DialogService = new MvvmDialogs.DialogService();
        }

        #endregion

        #region Methods

        #endregion

        #region Commands
        public RelayCommand<object> SampleCmdWithArgument { get { return new RelayCommand<object>(OnSampleCmdWithArgument); } }

        //public ICommand SaveAsCmd { get { return new RelayCommand(OnSaveAsTest, AlwaysFalse); } }
        //public ICommand SaveCmd { get { return new RelayCommand(OnSaveTest, AlwaysFalse); } }
        //public ICommand NewCmd { get { return new RelayCommand(OnNewTest, AlwaysFalse); } }

        [PropertyTools.DataAnnotations.Browsable(false)]
        public ICommand ShowAboutDialogCmd { get { return new RelayCommand(OnShowAboutDialog, AlwaysTrue); } }
        [PropertyTools.DataAnnotations.Browsable(false)]
        public ICommand ExitCmd { get { return new RelayCommand(OnExitApp, AlwaysTrue); } }

        protected bool AlwaysTrue() { return true; }
        protected bool AlwaysFalse() { return false; }

        private void OnSampleCmdWithArgument(object obj)
        {
            // TODO
        }

        //private void OnSaveAsTest()
        //{
        //    var settings = new SaveFileDialogSettings
        //    {
        //        Title = "Save As",
        //        Filter = "Sample (.xml)|*.xml",
        //        CheckFileExists = false,
        //        OverwritePrompt = true
        //    };

        //    bool? success = DialogService.ShowSaveFileDialog(this, settings);
        //    if (success == true)
        //    {
        //        // Do something
        //        Log.Info("Saving file: " + settings.FileName);
        //    }
        //}

        private void OnSaveTest()
        {
            // TODO
        }
        private void OnNewTest()
        {
            // TODO

        }

        public string OnOpenTest(string fileExt)
        {
            var settings = new MvvmDialogs.FrameworkDialogs.OpenFile.OpenFileDialogSettings
            {
                Title = "Open",
                Filter = $"Sample (.{fileExt})|*.{fileExt}",
                CheckFileExists = false
            };

            bool? success = DialogService.ShowOpenFileDialog(this, settings);
            if (success == true)
            {
                // Do something
                //Log.Info("Opening file: " + settings.FileName);
                return settings.FileName;
            }

            return "";
        }

        private void OnShowAboutDialog()
        {
            //Log.Info("Opening About dialog");
            AboutViewModel dialog = new AboutViewModel();
            var result = DialogService.ShowDialog<About>(this, dialog);

        }

        private void OnExitApp()
        {
            System.Windows.Application.Current.MainWindow.Close();
        }
        #endregion

        #region Events

        #endregion


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