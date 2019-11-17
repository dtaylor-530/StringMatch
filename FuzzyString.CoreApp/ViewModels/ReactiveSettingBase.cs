using MoreLinq;
using Optional.Collections;
using ReactiveUI;
using SetItIn;
using SetItIn.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Threading.Tasks;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using System.IO;

namespace StringMatch
{
    class ReactiveSettingBase : ReactiveObject
    {
        protected Notifier notifier;

        public ReactiveSettingBase()
        {
            var dictionary = this.GetType()
                .GetProperties()
                .ToDictionary(a => a.Name, a => a);

            //var localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var directory = System.IO.Directory.CreateDirectory("../../../Settings");
            var jsonFile = System.IO.Path.Combine(directory.FullName, "Settings.json");
            ISettings settings = new JsonSettings(jsonFile);

            var enumerable = dictionary
                .Select(p => (p.Value.GetCustomAttributes(typeof(SettingAttribute), false).Cast<SettingAttribute>().SingleOrNone(), p.Value))
                .Choose(p => (p.Item1.HasValue, (p.Value, p.Item1.ValueOr(() => default))));
            settings.Load();
            foreach ((PropertyInfo prop, SettingAttribute setting) in enumerable)
            {

                settings.Get(setting.Name, out object value);
                prop.SetValue(this, value);

            }

            Changed.Select(c => (c, optional: enumerable.SingleOrNone(v => v.Value.Name == c.PropertyName)))
                .Where(a => a.optional.HasValue)
                .Subscribe(a =>
            {
                (PropertyInfo p, SettingAttribute settingAttribute) = a.optional.Match(c => c, () => { throw new Exception(); });

                if (settings.Get<string>(settingAttribute.Name, out string value))
                {
                    if (value.Equals(dictionary[p.Name]) == false)
                    {
                        settings.Set(settingAttribute.Name, dictionary[p.Name].GetValue(this));
                    }
                }
                else
                {
                    settings.Set(settingAttribute.Name, dictionary[p.Name].GetValue(this));
                }
                settings.Save();
            });

            notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.BottomRight,
                    offsetX: 10,
                    offsetY: 10);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(3),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
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