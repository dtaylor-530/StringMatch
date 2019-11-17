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
using DynamicData;
using ToastNotifications;
using ToastNotifications.Position;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using System.Reactive.Concurrency;

namespace StringMatch
{
    internal class MainViewModel : ReactiveSettingBase
    {
        private string source;
        private string target;
        private string output;
        private int depth = 7;

        readonly ReadOnlyObservableCollection<PotentialMatch> items;
        readonly ReadOnlyObservableCollection<PotentialMatch> matchedItems;
        readonly ReadOnlyObservableCollection<PotentialMatch> unMatchedItems;
        readonly ReadOnlyObservableCollection<PotentialMatch> outputItems;

        private Queue<string> UnMatchedQueue = new Queue<string>();
        private Queue<string> MatchedQueue = new Queue<string>();
        private ObservableAsPropertyHelper<string[]> xx;
        private ObservableAsPropertyHelper<string[]> xt;
        private ILookup<string, string> dict;

        public MainViewModel()
        {
            Algorithms = Enum.GetValues(typeof(Algorithm)).Cast<Algorithm>().Select(o => new SelectableEnum { Algorithm = o, IsSelected = false }).ToList();

            foreach (SelectableEnum selectableEnum in Algorithms.Where(c => c.Algorithm == Algorithm.FuzzyString || c.Algorithm == Algorithm.FuzzySharp))
            {
                selectableEnum.IsSelected = true;
            }
            // var ds = new System.Reactive.Concurrency.DispatcherScheduler(Application.Current.Dispatcher);

            SaveCommand = ReactiveCommand.Create(Save);
            MatchCommand = ReactiveCommand.Create(Match, Observable.Return(true));

            string file = System.IO.Path.Combine(OutputFolder, "Output.txt");
            if (this.OutputFolder != null && System.IO.File.Exists(file))
            {
                dict = System.IO.File.ReadAllLines(System.IO.Path.Combine(OutputFolder, "Output.txt")).Select(a => a.Split(',')).ToLookup(a => a.First(), a => a.Last());

            }
            else
            {
                dict = default(ILookup<string, string>);
            }

            this.Changed.Subscribe(a =>
            {
                try
                {
                    notifier.ShowInformation($"{a.PropertyName} changed");
                }
                catch (Exception e)
                {
                }
            });

            xx = this.WhenAnyValue(a => a.SourceFile).Select(a =>
           {
               try
               {
                   return GetData(SourceFile).ToArray();
               }
               catch (Exception e)
               {
                   MessageBox.Show($"Source File, {SourceFile}, Missing ");
               }
               return new string[0];
           }).ToProperty(this, a => a.SourceData);

            xt = this.WhenAnyValue(a => a.TargetFile).Select(a =>
            {
                try
                {
                    return GetData(TargetFile).ToArray();
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Target File, {TargetFile}, Missing ");
                }
                return new string[0];
            }).ToProperty(this, a => a.TargetData);


            //foreach (var sd in SourceData ?? new string[0])
            //{
            //    var m = new PotentialMatch(sd);
            //}

            var c = Items
                .Connect();

            c
                //.Sort(new comparer())
                .Take(60)
                .Bind(out items)
                .Subscribe();

            c
                .AutoRefresh()
                          .Filter(a => a.IsNotMatch)

                          .Bind(out unMatchedItems)

       .Subscribe();

            c
                .AutoRefresh()
                .Filter(a => a.IsMatch)
                .Bind(out matchedItems)
                .Subscribe();

            //c
            //    .Bind(out outputItems)
            //    .Subscribe();




            _ = this
                .WhenAnyValue(a => a.OutputFolder)
                .SubscribeOn(TaskPoolScheduler.Default)
                .CombineLatest(this.MatchedData?.WhenAnyValue(a => a.Count), (a, b) => (a, b))
                .Where(a=> Items2.Items.Any())
                .Subscribe(c =>
                {
                    File.WriteAllLines(Path.Combine(c.a, "Output.txt"), Items2.Items.Select(it => $"{it.Source},{it.Target}"));
                });

            Items2
                .Connect()
                .Bind(out outputItems)
                .Subscribe();




            //Items.WhenAnyValue(a => a.Count).Where(a=>a!=0).Subscribe(a =>
            //  {
            //      var last = Items.Items.Last();
            //      var x = Items2.Items.Single(ax => ax.Source == last.Target);
            //      x.AddMatch(last.Source,1);
            //  });

            matchedItems.ActOnEveryObject(a =>
            {
                if (a.Target != null)
                {
                    fds(a, Items2.Items);
                }
                else
                {
                    a.WhenAnyValue(b => b.Target)
                        .Where(c => c != default)
                        .Take(1)
                        .Subscribe(cc =>
                    {
                        fds(a, Items2.Items);
                    });
                }
            },
            a =>
            {
                //var x = Items2.Items.Single(ax => ax.Source == a.Target);
                //x.RemoveMatch(a.Source, 1);
            });
            //c
            // .AutoRefresh()
            // //.Filter(a => a.IsMatch)
            // .ActOnEveryObject(a =>
            // {
            //     if (a.Target != null)
            //     {
            //         var xx = Items2.Items.Single(ax => ax.Source == a.Target);
            //     }
            // }, a =>
            //  {

            //  });


        }

        private static void fds(PotentialMatch a, IEnumerable<PotentialMatch> potentialMatches)
        {
            foreach (var match in potentialMatches.Where(ax => ax.Source == a.Target))
            {
                match.AddMatch(a.Source, 1);
                match.IsMatch = true;
            }
        }

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
            get => output ?? "../../../Data";
            set => this.RaiseAndSetIfChanged(ref output, value);
        }

        [Browsable(false)]
        public List<SelectableEnum> Algorithms { get; set; }

        [Browsable(false)]
        public ICommand SaveCommand { get; }

        [Browsable(false)]
        public ICommand MatchCommand { get; }


        [Browsable(false)]
        public string[] SourceData => xx?.Value;

        [Browsable(false)]
        public string[] TargetData => xt?.Value;

        [Browsable(false)]
        public SourceList<PotentialMatch> Items { get; } = new SourceList<PotentialMatch>();

        [Browsable(false)]
        public SourceList<PotentialMatch> Items2 { get; } = new SourceList<PotentialMatch>();

        [Browsable(false)]
        public ReadOnlyObservableCollection<PotentialMatch> PotentialMatchData => items;

        [Browsable(false)]
        public ReadOnlyObservableCollection<PotentialMatch> MatchedData => matchedItems;

        [Browsable(false)]
        public ReadOnlyObservableCollection<PotentialMatch> UnMatchedData => unMatchedItems;

        [Browsable(false)]
        public ReadOnlyObservableCollection<PotentialMatch> OutputData => outputItems;


        //public int SelectedIndex { get; set; }



        private void Save()
        {
            string csv = MatchedData.ToDataTable().ToCsv();
            System.IO.Directory.CreateDirectory(OutputFolder);

            File.WriteAllText(OutputFolder + "\\" + "output.csv", csv);
        }




        private void Match()
        {
            Algorithm[] algorithms = null;

            algorithms = Algorithms.Where(op => op.IsSelected == true).Select(_ => _.Algorithm).ToArray();
            var indexes = SourceData.Select((a, i) => (a, i)).ToDictionary(c => c.i, c => c.a);


            int i = 0;

            foreach (var targ in TargetData)
            {
                if (i < 40)
                    if (dict.Any(l => l.Contains(targ)))
                    {
                        var match = new PotentialMatch(targ);
                        foreach (var x in dict.Where(l => l.Contains(targ)))
                        {
                            match.AddMatch(x.Key, 1);
                            match.IsMatch = true;
                            Items.Add(match);
                        }
                    }
                    else
                        Items.Add(new PotentialMatch(targ));

                i++;
            }

            foreach (var src in SourceData)
            {
                //if (i < 40)
                if (dict.Contains(src))
                {
                    var match = new PotentialMatch(src);
                    foreach(var x in dict[src].Where(a=>string.IsNullOrEmpty(a)==false))
                    {
                        match.AddMatch(x, 1);
                        match.IsMatch = true;
                        Items2.Add(match);
                    } 
                }
                else
                    Items2.Add(new PotentialMatch(src));

                i++;
            }

            IProgress<(string, Dictionary<string, double>)> progressHandler = new Progress<(string, Dictionary<string, double>)>(value =>
             {
                 var single = Items.Items.SingleOrDefault(a => a.Source == value.Item1);

                 foreach (var kvp in value.Item2)
                 {
                     single?.AddMatch(kvp.Key, kvp.Value);
                 }
             });

            string[] sourceData = SourceData.Except(dict.Select(a => a.Key)).ToArray();
            string[] targetData = TargetData.Except(dict.SelectMany(a => a)).ToArray();

            var task1 = Task.Factory.StartNew(() =>
                           {
                               var x = Parallel.ForEach(targetData, (s) =>
                               {
                                   var ss = Helper.Match(s, sourceData, algorithms, depth);
                                   progressHandler.Report(ss);
                               });
                               notifier.ShowSuccess("Finished Matching");

                           }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }


        private static class Helper
        {
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
                          var avg = q.Select(a => a.Value).Average();
                          return avg;
                      })
                      .OrderByDescending(o => o.Value)
                       .Take(depth)
                       .ToDictionary(
                      t => t.Key,
                      t => t.Value);

                return (source, d);
            }
        }


        class comparer : IComparer<PotentialMatch>
        {
            public int Compare(PotentialMatch x, PotentialMatch y)
            {
                return (int)(100 * (x.BestScore - y.BestScore));
            }
        }

    }




}