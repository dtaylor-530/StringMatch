using FuzzyString;
using MoreLinq;
using Optional.Collections;
using PropertyTools.DataAnnotations;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;
using DynamicData;

namespace StringMatch
{
    internal class MainViewModel : ReactiveSettingBase
    {
        private string source;
        private string target;
        private string output;

        readonly ReadOnlyObservableCollection<PotentialMatch> items;
        private Queue<string> UnMatchedQueue = new Queue<string>();
        private Queue<string> MatchedQueue = new Queue<string>();


        /* * */
        Notifier notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.MainWindow,
                corner: Corner.TopRight,
                offsetX: 10,
                offsetY: 10);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(3),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            cfg.Dispatcher = Application.Current.Dispatcher;

        });

        public MainViewModel()
        {
            Algorithms = Enum.GetValues(typeof(Algorithm)).Cast<Algorithm>().Select(o => new SelectableEnum { Algorithm = o, IsSelected = false }).ToList();
            Algorithms.Where(c => c.Algorithm == Algorithm.JaroWinklerDistance).First(xo => xo.IsSelected = true);
            var ds = new System.Reactive.Concurrency.DispatcherScheduler(Application.Current.Dispatcher);
            SaveCommand = ReactiveCommand.Create(sfd);
            MatchCommand = ReactiveCommand.Create(Match, Observable.Return<bool>(true), ds);

            this.Changed.Subscribe(a =>
            {
                notifier.ShowInformation($"{a.PropertyName} changed");
            });

            try
            {
                TargetData = GetData(TargetFile).ToArray();
            }
            catch (Exception e)
            {
                notifier.ShowError($"Target File & {TargetFile} Missing ");
                TargetData = GetData(TargetFile).ToArray();
            }

            try
            {
                SourceData = GetData(SourceFile).ToArray();
            }
            catch (Exception e)
            {
                notifier.ShowError($"Source File & {SourceFile} Missing ");

                SourceData = GetData(SourceFile).ToArray();
            }

            if (File.Exists(OutputFolder + "\\" + "output.csv"))
            {
                MatchedData = CsvHelpers.ReadCsv(OutputFolder + "\\" + "output.csv");
            }
            else
            {
                MatchedData = new DataTable();
                MatchedData.Columns.Add(new DataColumn("Source"));
                MatchedData.Columns.Add(new DataColumn("Target"));
                foreach (string s in SourceData)
                {
                    MatchedData.Rows.Add(new string[2] { s, "" });
                }
            }

            //if (MatchedData != null)
            UnMatchedData = new ObservableCollection<string>(TargetData.Except(MatchedData.AsEnumerable().Select(a => a["Target"].ToString())));

            SourceData.ForEach(sd =>
            {
                var m = new PotentialMatch(sd);
                //m.PropertyChanged += Pmatch_PropertyChanged;
                //PotentialMatchData.Add(m);
            });
            //else
            //    UnMatchedData = new ObservableCollection<string>( TargetData);

            //this.PropertyChanged += MainViewModel_PropertyChanged;

            //var nameStatusObservable = this
            //    .WhenAnyValue(x => x.Name)
            //    .Select(name => GetLatestStatus(name));
            //name = nameStatusObservable
            //    .ToProperty(this, nameof(FirstName), deferSubscription: true);

            Items
                .Connect()
                //.ObserveOn(RxApp.MainThreadScheduler)

                .Bind(out items)
                .Sort(new comparer())
                .Take(20)
                .Subscribe();
        }


        class comparer : IComparer<PotentialMatch>
        {
            public int Compare(PotentialMatch x, PotentialMatch y)
            {
                return (int)(100 * (x.BestScore - y.BestScore));
            }
        }


        //// nameStatusObservable won't be subscribed until the
        //// Name property is accessed.
        //private readonly ObservableAsPropertyHelper<string> name;

        //public string Name => name.Value;

        [InputFilePath(".txt")]
        [FilterProperty("Filter")]
        [AutoUpdateText]
        [Setting]
        public string SourceFile
        {
            get => source ?? @"Resources\Source.txt";
            set => this.RaiseAndSetIfChanged(ref source, value);
        }

        [InputFilePath(".txt")]
        [FilterProperty("Filter")]
        [AutoUpdateText]
        [Setting]
        public string TargetFile
        {
            get => target ?? @"Resources\Target.txt";
            set => this.RaiseAndSetIfChanged(ref target, value);
        }

        [DirectoryPath]
        [AutoUpdateText]
        [Setting]
        public string OutputFolder
        {
            get => output;
            set => this.RaiseAndSetIfChanged(ref output, value);
        }

        [Browsable(false)]
        public string[] SourceData { get; set; }

        [Browsable(false)]
        public string[] TargetData { get; set; }

        [Browsable(false)]
        public SourceList<PotentialMatch> Items { get; } = new SourceList<PotentialMatch>();

        [Browsable(false)]
        public ReadOnlyObservableCollection<PotentialMatch> PotentialMatchData => items;


        [Browsable(false)]
        public ObservableCollection<string> UnMatchedData { get; set; }

        [Browsable(false)]
        public System.Data.DataTable MatchedData { get; set; }

        public List<SelectableEnum> Algorithms { get; set; }

        private int depth = 7;

        public int SelectedIndex { get; set; }

        private Task task1;

        [Browsable(false)]
        public ICommand SaveCommand { get; }

        private void sfd()
        {
            string csv = MatchedData.ToCsv();
            System.IO.Directory.CreateDirectory(OutputFolder);

            File.WriteAllText(OutputFolder + "\\" + "output.csv", csv);
        }


        [Browsable(false)]
        public ICommand MatchCommand { get; }

        private void Match()
        {
            Algorithm[] algorithms = null;

            algorithms = Algorithms.Where(op => op.IsSelected == true).Select(_ => _.Algorithm).ToArray();



            string[] sourceData = null;

            sourceData = SourceData.ToArray();

            string[] targetData = null;


            targetData = TargetData.ToArray();
            int i = 0;
            Items.Clear();
            IProgress<(string, Dictionary<string, double>)> progressHandler = new Progress<(string, Dictionary<string, double>)>(value =>
             {
                 Items.Add(new PotentialMatch(value.Item1, value.Item2));
                 i++;
             });

            task1 = Task.Factory.StartNew(() =>
                           {
                               var x = Parallel.ForEach(targetData, (s) =>
                               {
                                   var ss = Match(s, sourceData, algorithms, depth);

                                   progressHandler.Report(ss);
                               });

                               //foreach (var s in targetData)
                               //{
                               //    var ss = Match(s, sourceData, algorithms, depth);

                               //    progressHandler.Report(ss);
                               //}
                           }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);

        }



        private void Pmatch_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == nameof(PotentialMatch.IsMatch))
            //{
            //    if ((sender as PotentialMatch).IsMatch == true)
            //    {
            //        var row = MatchedData.Select($"Source = '{(sender as PotentialMatch).Source}'");
            //        row[0]["Target"] = (sender as PotentialMatch).Match1;
            //        NotifyChanged(nameof(MatchedData));

            //        if (!task1.IsCompleted)
            //        {
            //            UnMatchedQueue.Enqueue((sender as PotentialMatch).Match1);
            //        }
            //        else
            //        {
            //            UnMatchedData.Remove((sender as PotentialMatch).Match1);
            //        }
            //    }
            //    else if ((sender as PotentialMatch).IsMatch == false)
            //    {
            //        var row = MatchedData.Select($"Source = '{(sender as PotentialMatch).Source}'");
            //        row[0]["Target"] = "";
            //        NotifyChanged(nameof(MatchedData));

            //        if (!task1.IsCompleted)
            //        {
            //            MatchedQueue.Enqueue((sender as PotentialMatch).Match1);
            //        }
            //        else
            //        {
            //            UnMatchedData.Add((sender as PotentialMatch).Match1);
            //        }
            //    }
            //}
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



        //private void MainViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    switch (e.PropertyName)
        //    {
        //        case (nameof(SourceFile)):
        //            SourceData = GetData(SourceFile);
        //            NotifyChanged(nameof(SourceData));
        //            break;

        //        case (nameof(TargetFile)):
        //            TargetData = GetData(TargetFile);
        //            NotifyChanged(nameof(TargetData));
        //            break;

        //        case (nameof(OutputFolder)):
        //            {
        //                if (File.Exists(OutputFolder + "\\" + "output.csv"))
        //                {
        //                    File.Create(OutputFolder + "\\" + "output.csv");
        //                }
        //            }
        //            break;
        //    }
        //}

        public static (string, Dictionary<string, double>) Match(string source, IEnumerable<string> target, Algorithm[] algorithms, int depth)
        {
            var lst = source.Select(_ => new string[depth]).ToArray();


            //int combinedDepth = 1 + depth;
            var d = target.Distinct()
                    .ToDictionary(
                  t => t,
                  t =>
                  {
                      var q = StringComparison.GetQuality(source, t, algorithms);
                      var avg = q.Select(a=>a.Value).Average();
                      return avg;
                  })
                  .OrderByDescending(o => o.Value)
                   .Take(depth)
                   .ToDictionary(
                  t => t.Key,
                  t => t.Value);

            return (source, d);
        }

        public static IEnumerable<string> GetData(string file)
        {
            try
            {
                return System.IO.File.ReadAllLines(file);
            }
            catch (IOException ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

            return Enumerable.Empty<string>();
        }
    }




}