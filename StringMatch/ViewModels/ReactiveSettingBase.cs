using MoreLinq;
using Optional.Collections;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StringMatch
{
    class ReactiveSettingBase : ReactiveObject
    {
        public ReactiveSettingBase()
        {
            var xs = this.GetType()
                .GetProperties()
                .ToDictionary(a => a.Name, a => a);

            var xx =

               xs
                .Select(p => (p.Value.GetCustomAttributes(typeof(SettingAttribute), false).Cast<SettingAttribute>().SingleOrNone(), p.Value))
                .Choose(p => (p.Item1.HasValue, (p.Value, p.Item1.ValueOr(() => default))));


            foreach (var v in xx)
            {
                v.Value.SetValue(this, Properties.Settings.Default[v.Item2.Name]);
            }

            Changed.Select(c => (c, optional: xx.SingleOrNone(v => v.Value.Name == c.PropertyName)))
                    .Where(a => a.optional.HasValue)
                    .Subscribe(a =>
                {

                    (PropertyInfo p, SettingAttribute jka) = a.optional.Match(c => c, () => { throw new Exception(); });

                    if (Properties.Settings.Default[jka.Name].Equals(xs[p.Name]) == false)
                    {
                        Properties.Settings.Default[jka.Name] = xs[p.Name].GetValue(this);
                        Properties.Settings.Default.Save();
                    }
                });

        }
    }
}
