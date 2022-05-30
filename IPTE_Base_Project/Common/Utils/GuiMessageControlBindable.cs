using GuiControlLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IPTE_Base_Project.Common.Utils
{
    /// <summary>
    /// Class that extends GuiMessageControl to be able to bind a collection of messages in the ViewModel
    /// </summary>
    public class GuiMessageControlBindable : GuiMessageControl
    {
        public static ObservableCollection<GuiMessageItem> GetMessageItems(DependencyObject obj)
        {
            return (ObservableCollection<GuiMessageItem>)obj.GetValue(MessageItemsProperty);
        }

        public static void SetMessageItems(DependencyObject obj, ObservableCollection<GuiMessageItem> value)
        {
            obj.SetValue(MessageItemsProperty, value);
        }

        public static readonly DependencyProperty MessageItemsProperty =
            DependencyProperty.RegisterAttached(
                "MessageItems", 
                typeof(ObservableCollection<GuiMessageItem>),
                typeof(GuiMessageControlBindable),
                new UIPropertyMetadata( new PropertyChangedCallback(OnMessageItemsChange))
            );

        private static void OnMessageItemsChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GuiMessageControl)
            {
                // When an item is added or removed from a collection the notifyPropertyChanged event is not fired
                // and the view is not notified. So it is needed to add the collectionChangedHandler to fire the
                // event and notify the view
                var OnMessageItemsCollectionChange = new NotifyCollectionChangedEventHandler(
                        (o, args) =>
                        {
                            var gmc = d as GuiMessageControl;

                            if (gmc != null)
                            {
                                gmc.Messages.Clear();

                                foreach (GuiMessageItem m in (ObservableCollection<GuiMessageItem>)e.NewValue)
                                {
                                    gmc.Messages.Add(m);
                                }
                            }
                        }
                    );                

                if (e.OldValue != null)
                {
                    var coll = (INotifyCollectionChanged)e.OldValue;
                    // Unsubscribe from CollectionChanged on the old collection
                    coll.CollectionChanged -= OnMessageItemsCollectionChange;
                }

                if (e.NewValue != null)
                {
                    var coll = (ObservableCollection<GuiMessageItem>)e.NewValue;
                    // Subscribe to CollectionChanged on the new collection
                    coll.CollectionChanged += OnMessageItemsCollectionChange;
                }
            }
        }
    }
}
