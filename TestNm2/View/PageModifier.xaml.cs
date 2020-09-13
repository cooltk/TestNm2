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
    public partial class PageModifier : Window
    {
        LOG_NCEntities context = new LOG_NCEntities();
        int Id;
        public PageModifier(int memberId)
        {
            InitializeComponent();
            Id = memberId;
            NCI UpdateNCI = (from n in context.NCIs
                             where n.Id == Id
                             select n).Single();
            comboboxzone.Text = UpdateNCI.Zone;
            textblockTitreNCI.Text = UpdateNCI.TitreNCI;
            textblockCommentNCI.Text = UpdateNCI.CommentNCI;
            textblockActionNCI.Text = UpdateNCI.ActionNCI;
            textblockCreele.Text = UpdateNCI.DateCreaNCI.ToString();
            textblockCreepar.Text = UpdateNCI.CreateurNCI;
        }

        private void btnCloturer_Click(object sender, RoutedEventArgs e)
        {
            NCI UpdateNCI = (from n in context.NCIs
                             where n.Id == Id
                             select n).Single();
            UpdateNCI.Zone = comboboxzone.Text;
            UpdateNCI.TitreNCI = textblockTitreNCI.Text;
            UpdateNCI.CommentNCI = textblockCommentNCI.Text;
            UpdateNCI.ActionNCI = textblockActionNCI.Text;
            UpdateNCI.DateTermine = DateTime.Today;
            UpdateNCI.Termine = true;

            context.SaveChanges();
            //MainWindow.datagrid.ItemsSource = context.NCIs.ToList();
            this.Hide();
        }

        private void btnModifier_Click(object sender, RoutedEventArgs e)
        {
            NCI UpdateNCI = (from n in context.NCIs
                             where n.Id == Id
                             select n).Single();
            UpdateNCI.Zone = comboboxzone.Text;
            UpdateNCI.TitreNCI = textblockTitreNCI.Text;
            UpdateNCI.CommentNCI = textblockCommentNCI.Text;
            UpdateNCI.ActionNCI = textblockActionNCI.Text;

            context.SaveChanges();
            //MainWindow.datagrid.ItemsSource = context.NCIs.ToList();
            this.Hide();
        }
    }
}
