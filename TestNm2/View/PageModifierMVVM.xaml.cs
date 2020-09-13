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
using TestNm2.Model;

namespace TestNm2.View
{
    /// <summary>
    /// Logique d'interaction pour PageModifier.xaml
    /// </summary>
    public partial class PageModifierMVVM : Window
    {
        //int Id;
        //LOG_NCEntities context = new LOG_NCEntities();
        public PageModifierMVVM(NCI UpdateNCI)
        {
            InitializeComponent();
        }

        private void btnModifier_Click(object sender, RoutedEventArgs e)
        {
            //NCI UpdateNCI = (from m in context.NCIs
            //                where m.Id == Id
            //                select m).Single();
            //UpdateNCI.TitreNCI = textblockTitreNCI.Text;
            //context.SaveChanges();
            //MainWindow
        }
    }
}
