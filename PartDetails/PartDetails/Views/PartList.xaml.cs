using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PartDetails.ViewModels;


using PartDetails.DAL;

namespace PartDetails.Views
{
    /// <summary>
    /// Interaction logic for PartList.xaml
    /// </summary>
    public partial class PartList : Window
    {
        PartListViewModel objVM; 
        //List<TabItem> _tabItems = new List<TabItem>();

        public PartList()
        {
            objVM = new PartListViewModel();
            DataContext =  objVM;
            InitializeComponent();

        

            tabDynamic.SelectedIndex = 0;

            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

        }

        private void TabDynamic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
   
            TabItem tab = tabDynamic.SelectedItem as TabItem;
            if (tab != null && trVwPerson != null)
            {

                trVwPerson.ItemsSource= objVM.BindingTreeView(Convert.ToInt32(tab.Uid), objVM.IsShowPart);
            }
        }
    }

}
