using Ipte.Machine;
using Ipte.Machine.Config;
using Ipte.Machine.Interfaces;
using Ipte.TS1.StateMachine.CAMX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Ipte.Machine.Interfaces
{
    public interface IQueryProduct
    {
        object Product { get; }
    }
}

namespace Ipte.UI.Pages
{

    public class RefreshableAdapter : INotifyPropertyChanged
    {
        public virtual void Refresh()
        {
            //all properties for now. Should compare and only show for the ones that actually changed
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class HandshakeWithPreviousZoneAdapter : RefreshableAdapter
    {
        private HandshakeWithPreviousZone _hs;
        public HandshakeWithPreviousZoneAdapter(HandshakeWithPreviousZone hs)
        {
            _hs = hs;
        }

        public bool BoardAvailable { get { return _hs.BoardAvailable; } }
        public bool NotBusy { get { return _hs.NotBusy; } set { _hs.NotBusy = value; } }

    }

    public class HandshakeWithNextZoneAdapter : RefreshableAdapter
    {
        private HandshakeWithNextZone _hs;
        public HandshakeWithNextZoneAdapter(HandshakeWithNextZone hs)
        {
            _hs = hs;
        }

        public bool BoardAvailable { get { return _hs.BoardAvailable; } set { _hs.BoardAvailable = value; } }
        public bool NotBusy { get { return _hs.NotBusy; } }
    }

    public class DeviceAdapter : RefreshableAdapter
    {
        private ICamxEquipment _device;
        public DeviceAdapter(ICamxEquipment device)
        {
            _device = device;

            var handshakes = new Dictionary<string, RefreshableAdapter>();
            foreach (var prop in device.GetType().GetProperties())
            {
                if (prop.PropertyType.IsSubclassOf(typeof(HandshakeConnection)))
                {
                    var value = prop.GetValue(device) as HandshakeConnection;
                    if (value == null) continue;

                    if (value is HandshakeWithPreviousZone)
                        handshakes.Add(prop.Name, new HandshakeWithPreviousZoneAdapter(value as HandshakeWithPreviousZone));
                    else if (value is HandshakeWithNextZone)
                        handshakes.Add(prop.Name, new HandshakeWithNextZoneAdapter(value as HandshakeWithNextZone));

                }
                else if (typeof(IEnumerable<HandshakeConnection>).IsAssignableFrom(prop.PropertyType))
                {
                    var value = prop.GetValue(device) as IEnumerable<HandshakeConnection>;
                    if (value == null) continue;

                    int i = 0;
                    foreach (var hs in value)
                    {
                        if (hs is HandshakeWithPreviousZone)
                            handshakes.Add(prop.Name + $"[{i++}]", new HandshakeWithPreviousZoneAdapter(hs as HandshakeWithPreviousZone));
                        else if (hs is HandshakeWithNextZone)
                            handshakes.Add(prop.Name + $"[{i++}]", new HandshakeWithNextZoneAdapter(hs as HandshakeWithNextZone));
                    }
                }
            }

            Handshakes = handshakes;
        }

        //basic device properties
        public string ZoneId { get { return _device?.ZoneId; } }
        public string LaneId { get { return _device?.LaneId; } }
        public CamxState? State { get { return _device?.State; } }
        public bool HasError { get { return _device?.HasError == true; } }

        //product info. Product is either null, Panel, Module or Carrier
        //use data templates to display differnt items differntly
        public Product Product
        {
            get
            {
                return (_device as IQueryProduct)?.Product as Product;
            }
        }

        //handshake info; either HandshakeWithPreviousZone or HandshakeWithNextZone. 
        //use data templates to display differnt items differntly
        public Dictionary<string, RefreshableAdapter> Handshakes { get; private set; }

        public override void Refresh()
        {
            base.Refresh();
            foreach (var handshake in Handshakes) handshake.Value.Refresh();
        }
    }

    public partial class DevicesPage : UserControl
    {
        List<DeviceAdapter> _adapters;

        public DevicesPage()
        {
            InitializeComponent();

            //do not create controller when opened from visual studio/expression blend designer
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;

            _adapters = Controller.Instance.Zones
               .Select(d => new DeviceAdapter(d))
               //.OrderByDescending(d => d.LaneId)
               //.ThenBy(d => d.ZoneId)
               .ToList();

            //include controller
            _adapters.Insert(0, new DeviceAdapter(Controller.Instance));

            items.ItemsSource = _adapters;
            _updateTimer = new DispatcherTimer();
            _updateTimer.Interval = TimeSpan.FromMilliseconds(100);
            _updateTimer.Tick += new EventHandler(UpdatePage);
        }

        private DispatcherTimer _updateTimer;

        public bool UpdatingEnabled
        {
            get { return _updateTimer.IsEnabled; }
            set { _updateTimer.IsEnabled = value; }
        }

        void UpdatePage(object sender, EventArgs e)
        {
            foreach (var adapter in _adapters)
            {
                adapter.Refresh();
            }
        }
    }
}
