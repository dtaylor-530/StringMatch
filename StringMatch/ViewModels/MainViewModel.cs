using FuzzyString;
using PropertyTools.DataAnnotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Data;
using System.Collections.ObjectModel;


namespace StringMatch
{
    class MainViewModel : MainViewModelBase
    {


        //private string sourceFile;
        //private string targetFile;



        [InputFilePath(".txt")]
        [FilterProperty("Filter")]
        [AutoUpdateText]
        public string SourceFile
        {
            get { return Properties.Settings.Default.sourceFile; }
            set { if (Properties.Settings.Default.sourceFile != value)
                { Properties.Settings.Default.sourceFile = value; Properties.Settings.Default.Save(); NotifyChanged(nameof(SourceFile)); } }
        }


        [InputFilePath(".txt")]
        [FilterProperty("Filter")]
        [AutoUpdateText]
        public string TargetFile
        {
            get { return Properties.Settings.Default.targetFile; }
            set { if (Properties.Settings.Default.targetFile != value)
                { Properties.Settings.Default.targetFile = value; Properties.Settings.Default.Save(); NotifyChanged(nameof(TargetFile)); } }
        }



        //[InputFilePath(".txt")]
        //[FilterProperty("Filter")]
        //[AutoUpdateText]
        //public string MatchedFile
        //{
        //    get { return Properties.Settings.Default.matchedFile; }
        //    set { if (Properties.Settings.Default.matchedFile != value) { Properties.Settings.Default.matchedFile = value; Properties.Settings.Default.Save(); NotifyChanged(nameof(MatchedFile)); } }
        //}



        [DirectoryPath]
        [AutoUpdateText]
        public string OutputFolder
        {
            get { return Properties.Settings.Default.outputFolder; }
            set { if (Properties.Settings.Default.outputFolder != value) { Properties.Settings.Default.outputFolder = value; Properties.Settings.Default.Save(); NotifyChanged(nameof(OutputFolder)); } }
        }


        [Browsable(false)]
        public IEnumerable<string> SourceData { get; set; }


        [Browsable(false)]
        public IEnumerable<string> TargetData { get; set; }


        [Browsable(false)]
        public List<PotentialMatch> PotentialMatchData { get; set; } = new List<PotentialMatch>();

        [Browsable(false)]
        public ObservableCollection<string>  UnMatchedData { get; set; }

        [Browsable(false)]
        public System.Data.DataTable MatchedData { get; set; }



        public List<SelectableEnum> Algorithms { get; set; }

        //public ICommand OpenSourceCommand { get { return new RelayCommand(() => { SourceFile = OnOpenTest("csv"); NotifyChanged(nameof(SourceFile)); }, AlwaysTrue); } }
        //public ICommand OpenTargetCommand { get { return new RelayCommand(() => { TargetFile = OnOpenTest("csv"); NotifyChanged(nameof(TargetFile)); }, AlwaysTrue); } }

        private int depth = 7;

        public int SelectedIndex {
            get;
            set; }


        Task task1;

        [Browsable(false)]
        public ICommand SaveCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    string csv = MatchedData.ToCsv();
                    System.IO.Directory.CreateDirectory(OutputFolder);

                    File.WriteAllText(OutputFolder+"\\" + "output.csv", csv);
                }, AlwaysTrue);
                }
        }

[Browsable(false)]
        public ICommand MatchCommand
        {
            get
            {
                return new RelayCommand( () =>
                {
                    //PotentialMatchData.Clear();


                    //PotentialMatchData = new System.Data.DataTable();
                    //PotentialMatchData.Columns.Add(new System.Data.DataColumn("IsMatch",typeof(bool)));
                    //PotentialMatchData.Columns.Add(new System.Data.DataColumn("Name"));
                    //var op = source.Select(s => new System.Data.DataColumn(s, typeof(string)));
                    ////PotentialMatchData.Columns.AddRange(Enumerable.Range(1, depth).Select(r => new System.Data.DataColumn(r.ToString())).ToArray());

                    int? length = null;

                    var algs = Algorithms.Where(op => op.IsSelected == true).Select(_ => _.Algorithm).ToArray();


                    IProgress<Tuple<string, string[]>> progressHandler = new Progress<Tuple<string, string[]>>(value =>
                   {
                       //var row = PotentialMatchData.NewRow();
                       //row[0] = false;
                       //for (int i = 0; i < value.Length; i++)
                       //    row[i+1 ] = value[i];

                    PotentialMatchData.First(pm=>pm.Source==value.Item1).Matches= value.Item2;
                
                           //PotentialMatchData.Rows.Add(row);
          



                   });

                    length = length ?? SourceData.Count();


                    var ssd = SourceData.Take((int)length).ToList();


                     task1=Task.Factory.StartNew(() =>
                      {
                          var x = Parallel.ForEach(ssd, (s) =>
                            {
                                var ss = Match(s, UnMatchedData, algs, depth);
                                if (ss.Item2.Length != 0)
                                    progressHandler.Report(ss);
                            });

                        //foreach(string s in ssd)
                        //{
                        //    var ss = Match(s, UnMatchedData, algs, depth);
                        //    if (ss[1] != null)
                        //        progressHandler.Report(ss);
                        //}

                    }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default)
                     .ContinueWith(t =>
                     {
                         while(UnMatchedQueue.Count!=0)
                         {
                             UnMatchedData.Remove(UnMatchedQueue.Dequeue());
                         }

                         while (MatchedQueue.Count != 0)
                         {
                             UnMatchedData.Add(MatchedQueue.Dequeue());
                         }
                         Console.WriteLine("done.");

                     
                    });
                    //task1.Wait();


  
                    //MatchData = await dt;

                    //MatchData.Rows.Add(ss);
                    //NotifyChanged(nameof(MatchData));

                    // File.WriteAllLines(OutputFolder + "\\" + DateTime.Now.ToFileTimeUtc() + ".csv", PotentialMatchData.Rows.Cast<System.Data.DataRow>().Select(_ => String.Join(",", _.ItemArray.Select(__ => __.ToString()).ToArray())));
                    //Match(SourceData, TargetData, algs);
                }, AlwaysTrue);

            }
        }

        Queue<string> UnMatchedQueue = new Queue<string>();
        Queue<string> MatchedQueue = new Queue<string>();


        private void Pmatch_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (e.PropertyName ==nameof(PotentialMatch.IsMatch))
            {
                if ((sender as PotentialMatch).IsMatch == true)
                {
                    var row = MatchedData.Select($"Source = '{(sender as PotentialMatch).Source}'");
                    row[0]["Target"] = (sender as PotentialMatch).Match1;
                    NotifyChanged(nameof(MatchedData));



                    if (!task1.IsCompleted)
                    {
                        UnMatchedQueue.Enqueue((sender as PotentialMatch).Match1);
                    }
                    else
                    {
                        UnMatchedData.Remove((sender as PotentialMatch).Match1);
                    }
                }
                else if ((sender as PotentialMatch).IsMatch == false)
                {
                    var row = MatchedData.Select($"Source = '{(sender as PotentialMatch).Source}'");
                    row[0]["Target"] = "";
                    NotifyChanged(nameof(MatchedData));



                    if (!task1.IsCompleted)
                    {
                        MatchedQueue.Enqueue((sender as PotentialMatch).Match1);
                    }
                    else
                    {
                        UnMatchedData.Add((sender as PotentialMatch).Match1);
                    }
                }


            }

      
        }

        //[Browsable(false)]
        //public ICommand TestCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(() =>
        //        {

        //            var algs = Algorithms.Select(_ => _.Algorithm).ToArray();

        //            MatchData = Match(SourceData, TargetData, algs);
        //            NotifyChanged(nameof(MatchData));
        //        }, AlwaysTrue);

        //    }
        //}

        public MainViewModel()
        {


            Algorithms = Enum.GetValues(typeof(Algorithm)).Cast<Algorithm>().Select(o => new SelectableEnum { Algorithm = o, IsSelected = false }).ToList();
            Algorithms.Where(c => c.Algorithm == Algorithm.JaroWinklerDistance).First(x => x.IsSelected = true);

            this.PropertyChanged += MainViewModel_PropertyChanged;

            SourceFile = SourceFile ?? @"Resources\Source.txt";
            TargetFile = TargetFile ?? @"Resources\Target.txt";


            try
            {
                TargetData = GetData(TargetFile);
            }
            catch(Exception e)
            {

                Console.WriteLine($"Target File & {TargetFile} Missing ");
                TargetFile = @"Resources\Target.txt"; ;
                TargetData = GetData(TargetFile);
            }


            try
            {
                SourceData = GetData(SourceFile);
            }
            catch (Exception e)
            {

                Console.WriteLine($"Source File & {SourceFile} Missing ");
                SourceFile = @"Resources\Target.txt"; ;
                SourceData = GetData(SourceFile);
            }


            if (File.Exists(OutputFolder + "\\" + "output.csv"))
                MatchedData = CsvHelpers.ReadCsv(OutputFolder + "\\" + "output.csv");
            else
            { MatchedData = new DataTable();
                MatchedData.Columns.Add(new DataColumn("Source"));
                MatchedData.Columns.Add(new DataColumn("Target"));
                foreach (string s in SourceData)
                    MatchedData.Rows.Add(new string[2] { s, "" });
            }

            //if (MatchedData != null)
                UnMatchedData =new ObservableCollection<string>( TargetData.Except(MatchedData.AsEnumerable().Select(x => x["Target"].ToString())));


            SourceData.ToList().ForEach(sd =>
            {
                var m = new PotentialMatch(sd);
                m.PropertyChanged += Pmatch_PropertyChanged;
                PotentialMatchData.Add(m);
            });
            //else
            //    UnMatchedData = new ObservableCollection<string>( TargetData);

            //this.PropertyChanged += MainViewModel_PropertyChanged;
        }



        private void MainViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            switch (e.PropertyName)
            {
                case (nameof(SourceFile)):
                    SourceData = GetData(SourceFile);
                    NotifyChanged(nameof(SourceData));
                    break;
                case (nameof(TargetFile)):
                    TargetData = GetData(TargetFile);
                    NotifyChanged(nameof(TargetData));
                    break;
                case (nameof(OutputFolder)):
                    {
                    if(File.Exists(OutputFolder+"\\"+ "output.csv"))
                        {
                            File.Create(OutputFolder + "\\" + "output.csv");
                        }
       
                    }
                    break;
            }



        }



        public static Tuple<string,string[]> Match(string source, IEnumerable<string> target, Algorithm[] algorithms, int depth)
        {


            var lst = source.Select(_ => new string[depth]).ToArray();


            var d = target.AsParallel().ToDictionary(
                  t => t,
                  t => StringComparison.GetQuality(source, t, algorithms).Average())
                  .Where(x => x.Value > 0.7)
                  .OrderByDescending(o => o.Value);

            int combinedDepth = 1 + depth;
            //var zxx = Array.CreateInstance(typeof(string), combinedDepth) as string[];
            //var x = new List<string>(combinedDepth) { source }; 
            //zxx[0] = source;

            return new Tuple<string, string[]>( source, d.Take(depth).Select((_) => _.Key).ToArray());

            //int i = 0;
            // foreach(string s in xxpo)
            //{
            //    i++;
            //    zxx[i] = s;
           
            //}

            //return zxx;




        }


        public static IEnumerable<string> GetData(string file)
        {
            //FileInfo fi = new FileInfo(file);



            return System.IO.File.ReadAllLines(file);

            //if (fi.Extension == "txt")
            //     filestring.Split('');

        }


    }


    public class StringToBrushConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = (string)value;
            return new System.Windows.Media.SolidColorBrush(val == "male" ? System.Windows.Media.Colors.Blue : System.Windows.Media.Colors.Pink);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }




    public class PotentialMatch: System.ComponentModel.INotifyPropertyChanged
    {

        private bool isMatch = false;
        public bool IsMatch
        {
            get { return isMatch; }
            set { if (isMatch != value) { isMatch = value; NotifyChanged(nameof(IsMatch)); } }
        }

        public string Source { get; set; }

        private string[] matches = new string[7];


    //public PotentialMatch(string item1, string[] item2)
    //{
    //    this.Source = item1;
    //    this.matches = item2;
    //}

        public PotentialMatch(string item1)
        {
            this.Source = item1;
            //this.matches = item2;
        }

        public string Match1
        {
            get { return matches[0]; }
            set { if (matches[0] != value) { matches[0] = value; NotifyChanged(nameof(Match1)); } }
        }
        public string Match2
        {
            get { return matches[1]; }
            set { if (matches[1] != value) { matches[1] = value; NotifyChanged(nameof(Match2)); } }
        }
        public string Match3
        {
            get { return matches[2]; }
            set { if (matches[2] != value) { matches[2] = value; NotifyChanged(nameof(Match3)); } }
        }
        public string Match4
        {
            get { return matches[3]; }
            set { if (matches[3] != value) { matches[3] = value; NotifyChanged(nameof(Match4)); } }
        }
        public string Match5
        {
            get { return matches[4]; }
            set { if (matches[4] != value) { matches[4] = value; NotifyChanged(nameof(Match5)); } }
        }
        public string Match6
        {
            get { return matches[5]; }
            set { if (matches[5] != value) { matches[5] = value; NotifyChanged(nameof(Match6)); } }
        }
        public string Match7
        {
            get { return matches[6]; }
            set { if (matches[6] != value) { matches[6] = value; NotifyChanged(nameof(Match7)); } }
        }

        public string[] Matches { set {
                try
                {
                    matches[0] = value[0];
                    matches[1] = value[1];
                    matches[2] = value[2];
                    matches[3] = value[3];
                    matches[4] = value[4];
                    matches[5] = value[5];
                    matches[6] = value[6];
                }
                catch { }
                //matches[7] = value[7];
                NotifyChanged(nameof(Match1), nameof(Match2), nameof(Match3), nameof(Match4), nameof(Match5), nameof(Match6), nameof(Match7)); } }

        #region INotifyPropertyChanged Implementation
        /// <summary>
        /// Occurs when any properties are changed on this object.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// A helper method that raises the PropertyChanged event for a property.
        /// </summary>
        /// <param name="propertyNames">The names of the properties that changed.</param>
        public virtual void NotifyChanged(params string[] propertyNames)
        {
            foreach (string name in propertyNames)
            {
                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
            }
        }


        protected void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string caller = "")
        {

            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(caller));

        }
        #endregion



    }
}
