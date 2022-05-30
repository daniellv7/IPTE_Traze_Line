using IPTE_Base_Project.Models.Devices;
using System;
using System.ComponentModel;
using System.Reflection;

namespace IPTE_Base_Project.Models
{ // DUT = Device under test
    public class DutModel : BaseModel, IDeviceModel
    {
        private PlcModel plc;
        public PlcModel PLC
        {
            get { return plc; }
            set
            {
                plc = value;
                //if (plc != null)
                //    plc.PropertyChanged += DutModelPropertychanged;
            }
        }
        #region Event listeners
        private void DutModelPropertychanged(object sender, PropertyChangedEventArgs args)
        {
            DebugIn(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
            
            if (args.PropertyName == "Reference") return;

            RaisePropertyChanged(args.PropertyName);

            //if (plc.SerialNumber != SerialNumber)
            //    SerialNumber = plc.SerialNumber;

            DebugOut(MethodBase.GetCurrentMethod().ReflectedType, MethodBase.GetCurrentMethod().Name);
        }
        #endregion

        public DutModel()
        {
            DeviceName = "DUT";
        }
        public string DeviceName { get; set; }
        public string SerialNumber      //Always 10 digits
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string CodProducto       //Data introduction of the code of product. 3 digits number that identifies the product, this three digits are the same as the first three digits of the serial number.
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string Version           //Always 4 digits
        {
            get { return "0001"; }  
        }
        public string CodMaquina        //Codification of the equipment that generates the traceability information. Always 3 digits.
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string TipoMaquina       //Type of equipment that generates the traceability information. Always 3 digits
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string ManufacturingDate         //Date of manufacturing DD/MM/YY
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string ManufacturingHour         //Hour of manufacturing HH:MM:SS
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string Enlace1                   //Number of the piece inside the board. Example  PNL:1/6. This piece is the number One of a panel of 6 pieces.
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string Enlace2                   //NA
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string Enlace3                   //NA
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string Enlace4                   //NA
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string Enlace5                   //NA
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string DatosInternos             //Traceability information. Agreement between supplier and Lear, in order to trace information.
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }
        public string Palet                     //Number of pallet if applies
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public void DeviceReset()
        {
        }
    }
}
