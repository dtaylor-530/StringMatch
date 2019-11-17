using DynamicData;
using FuzzyString.CoreApp.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace StringMatch
{
    public class PotentialMatch : ReactiveObject//: System.ComponentModel.INotifyPropertyChanged
    {
        bool isMatch = false;
        ObservableAsPropertyHelper<bool> isNotMatch;
        ObservableAsPropertyHelper<string> target;
        private readonly ReadOnlyObservableCollection<Selectable<string>> matches;
        private readonly ISubject<(string, double)> subjectMatches = new Subject<(string, double)>();
        private readonly ISubject<bool> stMatches = new Subject<bool>();
        private readonly ISubject<string> strMatches = new Subject<string>();


        public PotentialMatch(string item1)
        {
            this.Source = item1;

            var mms = subjectMatches.Select(m => new Selectable<string> { Object = m.Item1, Rank = m.Item2 });

            _ = mms.ToObservableChangeSet(a => a.Object)
                .Bind(out matches)
                .Subscribe();

            var xx = mms.Subscribe(a =>

            matches.ActOnEveryObject(a =>
            {
                a.WhenAny(b => b.IsSelected, a => a).Subscribe(xc =>
                {
                    if (a.IsSelected)
                    {
                        IsMatch = true;
                        strMatches.OnNext(a.Object);
                    }
                });
            },a=>
            {

            }));



                //.SelectMany(a => a)
                //.Where(a => a.Value);

            //var xsd = xx?
            //    .Subscribe(a => { IsMatch = a.Value; });

            target = strMatches.ToProperty(this, x => x.Target);

            this.WhenAnyValue(a => a.IsNotMatch).Subscribe(a =>
              {
                  if (Matches != null && a==true)
                      foreach (var m in Matches)
                      {
                          m.IsSelected = false;
                      }
              });

            this.WhenAnyValue(a => a.IsMatch).Subscribe(a =>
            {
                if (Matches != null && a && !Matches.Any(a=>a.IsSelected))
                    Matches.First().IsSelected = true;

            });

            isNotMatch = this
                            .WhenAnyValue(x => x.IsMatch)
                            .Select(a => !a)
                            .ToProperty(this, x => x.IsNotMatch);


        }


        class Comparer : IComparer<Selectable<string>>
        {
            public int Compare([AllowNull] Selectable<string> x, [AllowNull] Selectable<string> y)
            {
                return (x?.Rank ?? 0 - y?.Rank ?? 0) > 0 ? -1 : 1;
            }
        }

        public bool IsMatch
        {
            get => isMatch;
            set { isMatch = value; this.RaisePropertyChanged(nameof(IsMatch)); }
        }

        public bool IsNotMatch => isNotMatch?.Value ?? false;

        public string Target => target.Value;


        public string Source { get; set; }

        public void AddMatch(string match, double score)
        {
            subjectMatches.OnNext((match, score));
        }

        public void RemoveMatch(string match, double score)
        {
            throw new NotImplementedException();
          //  subjectMatches.OnNext((match, score));
        }

        public ReadOnlyObservableCollection<Selectable<string>> Matches => matches;

        public ISubject<bool> observable { get; } = new Subject<bool>();


        public double BestScore => matches.FirstOrDefault()?.Rank ?? 0;

 
    }
}
