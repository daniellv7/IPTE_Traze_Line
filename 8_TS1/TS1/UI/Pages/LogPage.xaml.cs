using Ipte.TS1.UI.Controls;
using Ipte.TS1.StateMachine.CAMX;
using Ipte.Machine;
using Ipte.Machine.Configuration;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Ipte.UI.Pages
{
    public partial class LogPage : UserControl
    {
        private int MaxMessageCount = 1000;
        private string LogDirectory = Paths.LogDirectory;

        public ObservableCollection<LogMessage> Messages { get; set; }
        public FilterItem<string>[] ZoneList { get; set; }
        public FilterItem<string>[] LaneList { get; set; }
        public FilterItem<Severity>[] SeverityList { get; set; }

        public LogPage()
        {
            ZoneList = Controller.Instance.Zones.Select(d => d.ZoneId).Distinct().Select(s => new FilterItem<string>() { Field = s, IsActive = true }).OrderBy(s => s.Field).ToArray();
            LaneList = Controller.Instance.Zones.Select(d => d.LaneId).Distinct().Select(s => new FilterItem<string>() { Field = s, IsActive = true }).OrderBy(s => s.Field).ToArray();
            SeverityList = (new[] { Severity.Info, Severity.Warning, Severity.Error, Severity.Alarm }).Select(s => new FilterItem<Severity>() { Field = s, IsActive = true }).ToArray();

            Messages = new ObservableCollection<LogMessage>();
            CollectionViewSource.GetDefaultView(Messages).Filter = FilterMessage;

            InitializeComponent();

            Controller.Instance.EquipmentInformation += (s, e) => Dispatcher.BeginInvoke((Action<object, EquipmentInformationEventArgs>)ShowEquipmentInfo, s, e);
            Controller.Instance.EquipmentError += (s, e) => Dispatcher.BeginInvoke((Action<object, EquipmentErrorEventArgs>)ShowEquipmentError, s, e);
            Controller.Instance.EquipmentAlarm += (s, e) => Dispatcher.BeginInvoke((Action<object, EquipmentAlarmEventArgs>)ShowEquipmentAlarm, s, e);
        }

        void ShowEquipmentInfo(object sender, EquipmentInformationEventArgs e)
        {
            AddMessage(new LogMessage() { DateTime = e.DateTime, Zone = e.ZoneList, Lane = e.LaneList, Severity = Severity.Info, Message = e.InformationId });
        }

        void ShowEquipmentError(object sender, EquipmentErrorEventArgs e)
        {
            AddMessage(new LogMessage() { DateTime = e.DateTime, Zone = e.ZoneList, Lane = e.LaneList, Severity = Severity.Error, Message = e.ErrorId + ":\t" + e.Description });
        }

        void ShowEquipmentAlarm(object sender, EquipmentAlarmEventArgs e)
        {
            AddMessage(new LogMessage() { DateTime = e.DateTime, Zone = e.ZoneList, Lane = e.LaneList, Severity = Severity.Alarm, Message = e.AlarmId + ":\t" + e.Description });
        }

        public void AddMessage(LogMessage message)
        {
            try
            {
                string logFile = string.Concat(LogDirectory, @"\log ", DateTime.Today.ToString("yyy MM dd"), ".log");
                File.AppendAllText(logFile, message + "\r\n");
            }
            catch
            {
                Messages.Insert(0, new LogMessage() { DateTime = DateTime.Now, Zone = "GUI", Lane = "", Severity = Severity.Error, Message = "Failed to write a log message to file" });
            }

            Messages.Insert(0, message);

            while (Messages.Count > MaxMessageCount)
                Messages.RemoveAt(Messages.Count - 1);
        }

        private void DeleteOldLogs(TimeSpan relevancy)
        {
            string[] logFiles = Directory.GetFiles(LogDirectory, "*.log", SearchOption.TopDirectoryOnly);
            foreach (string fileName in logFiles)
            {
                if (DateTime.Now - File.GetCreationTime(fileName) > relevancy)
                {
                    File.Delete(fileName);
                }
            }
        }

        private bool FilterMessage(object message)
        {
            if (!SeverityList.Any(z => z.IsActive && z.Field == (message as LogMessage).Severity)) return false;
            if (!LaneList.Any(z => z.IsActive && z.Field == (message as LogMessage).Lane)) return false;
            if (!ZoneList.Any(z => z.IsActive && z.Field == (message as LogMessage).Zone)) return false;
            return true;
        }

        private void FilterChanged(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(Messages).Refresh();
        }

        private void ClearScreen_Click(object sender, RoutedEventArgs e)
        {
            Messages.Clear();
        }
    }

    public class LogMessage
    {
        public DateTime DateTime { get; set; }
        public string Zone { get; set; }
        public string Lane { get; set; }
        public Severity Severity { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return string.Format("{0:d} {0:HH:mm:ss.fff}\t{1}\t{2}\t{3}\t{4}", DateTime, Zone, Lane, Severity, Message);
        }
    }

    public class FilterItem<T>
    {
        public T Field { get; set; }
        public bool IsActive { get; set; }
    }
}
