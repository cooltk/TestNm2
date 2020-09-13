using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TestNm2.Model;
using TestNm2.ViewModel;

namespace TestNm2
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void deletebtn_Click(object sender, RoutedEventArgs e)
        {
            var context2 = this.DataContext;
            //this.DataContext.itemCollectionViewSource.Source = context.NCIs.ToList();
            int Id = (dg.SelectedItem as NCI).Id;
            //DeleteNC(Id);
            //NCI deleteNCI = context.NCIs.Where(n => n.Id == Id).Single();
            //context.NCIs.Remove(deleteNCI);
            //context.SaveChanges();
            //CV = (CollectionView)Application.Current.MainWindow.FindName("ItemCollectionViewSource");
            //NotifyPropertyChanged("deletebtn_Click");            
        }
    }
}
