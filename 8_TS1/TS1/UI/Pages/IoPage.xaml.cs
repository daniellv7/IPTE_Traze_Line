using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Ipte.TS1.UI.Controls;
using Ipte.TS1.StateMachine.CAMX;
using Ipte.Machine;
using Ipte.Machine.Configuration;
using Ipte.TS1.UI.i18n;
using Ipte.TS1.IO.Beckhoff.Tc2;

namespace Ipte.UI.Pages
{
    /// <summary>
    /// Interaction logic for IoPage.xaml
    /// </summary>
    public partial class IoPage : UserControl
    {
        #region Constructor

        public IoPage()
        {
            InitializeComponent();

            //do not create controller when opened from visual studio/expression blend designer
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;

            //            InitializeIo();

            _updateTimer = new DispatcherTimer();
            _updateTimer.Interval = TimeSpan.FromMilliseconds(100);
            _updateTimer.Tick += new EventHandler(UpdatePage);
        }

        #endregion

        #region Page updating

        private DispatcherTimer _updateTimer;

        public bool UpdatingEnabled
        {
            get { return _updateTimer.IsEnabled; }
            set
            {
                if (value && Inputs.Groups.Count == 0)
                    InitializeIo();
                _updateTimer.IsEnabled = value;
            }
        }

        void UpdatePage(object sender, EventArgs e)
        {
            //if (Controller.Instance.Settings.Mode == Mode.Offline) return;

            foreach (GuiIoGroup group in Inputs.Groups)
            {
                if (!group.IsExpanded)
                    continue;

                foreach (GuiIoItem item in group.Items)
                {
                    try
                    {
                        Input<bool> input = (Input<bool>)item.Tag;
                        if (input.IsConnected)
                        {
                            item.Value = input.Value;
                        }
                    }
                    catch
                    {
                        break;
                    }
                }
            }

            foreach (GuiIoGroup group in Outputs.Groups)
            {
                if (!group.IsExpanded)
                    continue;

                foreach (GuiIoItem item in group.Items)
                {
                    try
                    {
                        Output<bool> output = (Output<bool>)item.Tag;
                        if (output.IsConnected)
                        {
                            item.Value = output.Value;
                        }
                    }
                    catch
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region Initialize I/O list

        private void InitializeIo()
        {
            Inputs.Groups.Clear();
            Outputs.Groups.Clear();

            Controller controller = Controller.Instance;
            controller.EquipmentChangeState += InvokeEquipmentChangeState;

            //Append controller IOs to the input/output list
            AttachIoGroup(controller.GeneralIO);
            AttachIoGroup(controller.LoadingSegmentIO);
            AttachIoGroup(controller.MiddleSegmentIO);
            AttachIoGroup(controller.UnloadingSegmentIO);

            //Collapse all but general IOs for better readability
            for (int i = 1; i < Inputs.Groups.Count; i++)
                Inputs.Groups[i].IsExpanded = false;

            for (int i = 1; i < Outputs.Groups.Count; i++)
                Outputs.Groups[i].IsExpanded = false;

            //Randomize io values when in simulation mode
            //if (Controller.Instance.Settings.Mode == Mode.Offline)
            //{
            //    Random dice = new Random();

            //    foreach (GuiIoGroup group in Inputs.Groups)
            //        foreach (GuiIoItem item in group.Items)
            //            item.Value = dice.Next(2) > 0;

            //    foreach (GuiIoGroup group in Outputs.Groups)
            //        foreach (GuiIoItem item in group.Items)
            //            item.Value = dice.Next(2) > 0;
            //}
        }

        private void AttachIoGroup(StructBase module)
        {
            //In simulation mode the IO points are not created
            if (module == null) return;

            GuiIoGroup inputGroup = new GuiIoGroup();

            inputGroup.Caption = module.Name;

            var inputs = from point in module.Inputs
                         orderby point.Address
                         select point;

            foreach (var input in inputs)
            {
                Input<bool> booleanInput = input as Input<bool>;
                if (booleanInput == null) continue; //ignore analog inputs

                GuiIoItem item = new GuiIoItem();
                item.Tag = input;
                item.Caption = booleanInput.Caption;
                item.Address = booleanInput.Address;
                item.EnabledLevel = UserLevel.Maintenance;
                inputGroup.Items.Add(item);
            }

            //Some terminals may only contain outputs
            if (inputGroup.Items.Count > 0)
            {
                Inputs.Groups.Add(inputGroup);
            }

            GuiIoGroup outputGroup = new GuiIoGroup();
            outputGroup.Caption = module.Name;

            var outputs = from point in module.Outputs
                          orderby point.Address
                          select point;

            foreach (var output in outputs)
            {
                Output<bool> booleanOutput = output as Output<bool>;
                if (booleanOutput == null) continue; //ignore analog outputs

                GuiIoItem item = new GuiIoItem();
                item.Tag = output;
                item.Caption = booleanOutput.Caption;
                item.Address = booleanOutput.Address;
                item.IsOutput = true;
                item.EnabledLevel = UserLevel.Maintenance;

                Output<bool> o = booleanOutput;//need to copy the loop variable to in scope variable
                item.Changed += (Object sender, RoutedEventArgs e) =>
                {
                    if (o.IsConnected) o.Value = item.Value;
                };
                outputGroup.Items.Add(item);
            }

            //Some terminals may only contain inputs
            if (outputGroup.Items.Count > 0)
            {
                Outputs.Groups.Add(outputGroup);
            }
        }

        #endregion

        #region Disable I/O when machine is started

        void InvokeEquipmentChangeState(object sender, EquipmentChangeStateEventArgs e)
        {
            Dispatcher.BeginInvoke((Action<object, EquipmentChangeStateEventArgs>)EquipmentChangeState, sender, e);
        }

        void EquipmentChangeState(object sender, EquipmentChangeStateEventArgs e)
        {
            if (e.PreviousState > CamxState.Down && e.CurrentState > CamxState.Down)
            {
                //Ignore when state changes between active states
                return;
            }

            foreach (GuiIoGroup group in Inputs.Groups)
                foreach (GuiIoItem item in group.Items)
                    item.IsEnabled = e.CurrentState < CamxState.Setup;

            foreach (GuiIoGroup group in Outputs.Groups)
                foreach (GuiIoItem item in group.Items)
                    item.IsEnabled = e.CurrentState < CamxState.Setup;
        }

        #endregion

    }
}

