using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPTE_Base_Project.Models;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using IPTE_Base_Project.Common;
using System.Windows.Input;
using IPTE_Base_Project.Common.Utils.MediatorPattern;
using IPTE_Base_Project.Variants;
using System.Windows;
using System.Reflection;
using Ipte.TS1.UI.Controls;
using System.Threading;

namespace IPTE_Base_Project.ViewModels.Configure
{
    class Variant_ViewModel: BaseViewModel
    { 
        private enum WorkMode : int { Read, Create, Modify }



        public Variant_Model Model { get; set; }


        private int _selectedvariant;
      
        private int _selectedindex;
        public int SelectedIndex
        {
            get { return _selectedindex; }
            set
            {
                _selectedindex = value;
                OnPropertyChanged();

            }
        }

        private int _selectedworkmode;
        public int SelectedWorkMode
        {
            get { return _selectedworkmode; }
            set
            {
                _selectedworkmode = value;
                SelectedWorkModeCondition(value);
                OnPropertyChanged("SelectedWorkMode");
            }
        }

        public Visibility NewVariantVisibility { get; set; }
        public Visibility ReadVariantVisibility { get; set; }
        public Visibility ModVariantVisibility { get; set; }

        public Variant Current_Variant { get; set;}

        public Variant _new_variant = new Variant();
        public Variant New_Variant
        {
            get { return _new_variant; }
            set { _new_variant = value; }
        }

        public Variant _mod_Variant = new Variant();
        public Variant Mod_Variant
        {
            get { return _mod_Variant; }
            set { _mod_Variant = value; }
        }


        public Dictionary<string, Variant> AvailableVariants { get; set; }

        public ObservableCollection<string> VariantList { get; set; }


      

        private void Levelchange()
        {
            if (OperatorLevel)
                SelectedWorkMode = 0;
        }
        #region Commands

        private RelayCommand newvariantcommand;
        

        private RelayCommand modifyvariantcommand;
      

        private RelayCommand copyvariantcommand;
      
        private RelayCommand deletevariantcommand;
       

        private RelayCommand loadvariantcommand;
        public ICommand LoadVariantCommand
        {
            get
            {
                if (loadvariantcommand == null)
                {
                    loadvariantcommand = new RelayCommand(param => Load_Variant(), CanExe);
                }
                return loadvariantcommand;
            }
        }


        public bool CanExe(object o)
        {
            return true;
        }
        #endregion

        #region Metodos

        //private void Save_New_Variant()
        //{
        //   // AdminLevel = false;
        //    if (!VariantList.Contains(New_Variant.MODEL_NAME))
        //    {
        //        Mediator.NotifyColleagues("DB_NewVariant", new object[] { New_Variant });
        //    }
            
        //}

        //private void Modify_Variant()
        //{
        //    MessageBoxResult Result = GuiMessageBox.Show("This will Replace the data on variant " + (SelectedVariant + 1) + " , Ok?" + "", "Save", MessageBoxButton.OKCancel);
        //    if (Result == MessageBoxResult.OK)
        //    {
        //        Variant Old;
        //        string Selected = VariantList[SelectedVariant];
        //        Mod_Variant.MODEL_ID = SelectedVariant;
        //        if (Mod_Variant.MODEL_NAME != null && Mod_Variant.MODEL_NAME != "")
        //        {
        //            if (Selected != "--")
        //            {
        //               // DB_RecordchangeVariant
        //                AvailableVariants.TryGetValue(Mod_Variant.MODEL_NAME, out Old);
        //                // DB_RecordchangeVariant test
        //                Mediator.NotifyColleagues("DB_RecordchangeVariant", new object[] { Mod_Variant, Old });
        //                Mediator.NotifyColleagues("DB_ModVariant", new object[] { Mod_Variant, Old });
        //            }
        //            else
        //            {
        //                Mediator.NotifyColleagues("DB_NewVariant", new object[] { Mod_Variant });
        //            }
        //        }
        //        else
        //        {
        //            GuiMessageBox.Show("Invalid Variant name", "Error");
        //        }
        //    }
           
        //}

       

      

        private void Load_Variant()
        {
            Mediator.NotifyColleagues("PLC_LoadVariant", new object[] { Current_Variant });
        }
        #endregion

        #region Functions



        private void ModifyVariant()
        {
            
        }
        
        //private void OnReadVariants(object[] param)
        //{
        //    ObservableCollection<string> list = new ObservableCollection<string>();
        //    AvailableVariants = param[0] as Dictionary<string, Variant>;
        //    foreach (KeyValuePair<string,Variant> variant in AvailableVariants)
        //    {
        //        list.Add(variant.Value.MODEL_NAME);
        //    }
        //    VariantList = list;
        //    Mediator.NotifyColleagues("VariantReads", new object[] { VariantList });
        //    OnPropertyChanged("VariantList");
        //    SelectedVariant = SelectedVariant;
        //}

       

        private void SelectedWorkModeCondition(int Mode)
        {
            switch(Mode)
            {
                case (int)WorkMode.Read:
                    NewVariantVisibility = Visibility.Hidden;
                    ReadVariantVisibility = Visibility.Visible;
                    ModVariantVisibility = Visibility.Hidden;
                    break;
                case (int)WorkMode.Create:
                    NewVariantVisibility = Visibility.Visible;
                    ReadVariantVisibility = Visibility.Hidden;
                    ModVariantVisibility = Visibility.Hidden;
                    break;
                case (int)WorkMode.Modify:
                    NewVariantVisibility = Visibility.Hidden;
                    ReadVariantVisibility = Visibility.Hidden;
                    ModVariantVisibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
            OnPropertyChanged("NewVariantVisibility");
            OnPropertyChanged("ReadVariantVisibility");
            OnPropertyChanged("ModVariantVisibility");
        }

       
        #endregion

    }
}

