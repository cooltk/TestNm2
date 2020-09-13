using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using TestNm2.Model;
using TestNm2.View;

namespace TestNm2.ViewModel
{
    public class ViewModel: INotifyPropertyChanged
    {
        private ObservableCollection<string> _zones;
        private ObservableCollection<string> _createurs;
        private string _selectedZone;
        private string _selectedTermine;
        private string _selectedMesNC;
        private bool _ckbMesNC;
        private bool _ckbTermine;
        private bool _canCanRemoveFilter;
        private bool _canCanRemoveZoneFilter;
        private bool _canCanRemoveTermineFilter;
        private bool _canCanRemoveMesNCFilter;
        public CollectionViewSource itemCollectionViewSource;
        private ICommand _clickCommand;
        private ICommand _clickCommand2;
        private ICommand _clickCommand3;
        private ICommand _clickCommand4;
        ICommand _command;
        ICommand _command2;
        ICommand _command3;
        public PageModifierMVVM PageU;
        public LOG_NCEntities context;

        public ViewModel()
        {
            InitialisationData();
        }

        public void InitialisationData()
        {
            //Déclaration de la variable collectionviewsource et on la lit à la ressource de la fenêtre
            itemCollectionViewSource = (CollectionViewSource)Application.Current.MainWindow.FindResource("ItemCollectionViewSource");
            //Création du contexte à partir d'entity framework
            context = new LOG_NCEntities();
            //On charge les données dans le collectionviewsource
            itemCollectionViewSource.Source = context.NCIs.ToList();
            //Initialisation des données pour la combobox Zones
            //on récupère la liste des zones disponibles
            var q1 = from t in context.NCIs.ToList().OrderBy(v => v.Zone)
                     select t.Zone;
            Zones = new ObservableCollection<string>(q1.Distinct());
            var q2 = from t in context.NCIs.ToList().OrderBy(v => v.Zone)
                     select t.CreateurNCI;
            Createurs = new ObservableCollection<string>(q2.Distinct());
        }

        //Création d'un membre Zones 
        public ObservableCollection<string> Zones
        {
            get { return _zones; }
            set
            {
                if (_zones == value)
                    return;
                _zones = value;
                NotifyPropertyChanged("Zones");
            }
        }

        //Création d'un membre Createurs 
        public ObservableCollection<string> Createurs
        {
            get { return _createurs; }
            set
            {
                if (_createurs == value)
                    return;
                _createurs = value;
                NotifyPropertyChanged("Createurs");
            }
        }

        public string SelectedZone
        {
            get { return _selectedZone; }
            set
            {
                if (_selectedZone == value)
                    return;
                _selectedZone = value;
                NotifyPropertyChanged("SelectedZone");
                ApplyFilter(!string.IsNullOrEmpty(_selectedZone) ? FilterField.Zone : FilterField.None);
            }
        }

        public string SelectedTermine
        {
            get { return _selectedTermine; }
            set
            {
                if (_selectedTermine == value)
                    return;
                _selectedTermine = value;
                NotifyPropertyChanged("SelectedTermine");
                ApplyFilter(!string.IsNullOrEmpty(_selectedTermine) ? FilterField.Termine : FilterField.None);
            }
        }

        public string SelectedMesNC
        {
            get { return _selectedMesNC; }
            set
            {
                if (_selectedMesNC == value)
                    return;
                _selectedMesNC = value;
                NotifyPropertyChanged("SelectedMesNC");
                ApplyFilter(!string.IsNullOrEmpty(_selectedMesNC) ? FilterField.MesNC : FilterField.None);
            }
        }

        public bool ckbMesNC
        {
            get { return _ckbMesNC; }
            set
            {
                if (_ckbMesNC == value)
                    return;
                _ckbMesNC = value;
                if(_ckbMesNC)
                    _selectedMesNC = System.Environment.UserName;
                else
                {
                    _selectedMesNC = null;
                    RemoveMesNCFilter();
                }
                NotifyPropertyChanged("SelectedMesNC");
                ApplyFilter(!string.IsNullOrEmpty(_selectedMesNC) ? FilterField.MesNC : FilterField.None);
            }
        }

        public bool ckbTermine
        {
            get { return _ckbTermine; }
            set
            {
                if (_ckbTermine == value)
                    return;
                _ckbTermine = value;
                if (_ckbTermine)
                    _selectedTermine = "Termine";
                else
                {
                    _selectedTermine = null;
                    RemoveTermineFilter();
                }
                NotifyPropertyChanged("SelectedTermine");
                ApplyFilter(!string.IsNullOrEmpty(_selectedTermine) ? FilterField.Termine : FilterField.None);
            }
        }

        public virtual event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged(params string[] propertyNames)
        {
            if (PropertyChanged != null)
            {
                foreach (string propertyName in propertyNames) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                PropertyChanged(this, new PropertyChangedEventArgs("HasError"));
            }
        }

        private void ApplyFilter(FilterField field)
        {
            switch (field)
            {
                case FilterField.Zone:
                    AddZoneFilter();
                    break;
                case FilterField.Termine:
                    AddTermineFilter();
                    break;
                case FilterField.MesNC:
                    AddMesNCFilter();
                    break;
                default:
                    break;
            }
        }

        private enum FilterField
        {
            Zone,
            Termine,
            MesNC,
            None
        }

        /* Notes on Adding Filters:
         *   Each filter is added by subscribing a filter method to the Filter event
         *   of the CVS.  Filters are applied in the order in which they were added. 
         *   To prevent adding filters mulitple times ( because we are using drop down lists
         *   in the view), the CanRemove***Filter flags are used to ensure each filter
         *   is added only once.  If a filter has been added, its corresponding CanRemove***Filter
         *   is set to true.       
         *   
         *   If a filter has been applied already (for example someone selects "Canada" to filter by country
         *   and then they change their selection to another value (say "Mexico") we need to undo the previous
         *   country filter then apply the new one.  This does not completey Reset the filter, just
         *   allows it to be changed to another filter value. This applies to the other filters as well
         */
        public void AddZoneFilter()
        {
            // see Notes on Adding Filters:
            if (CanRemoveZoneFilter)
            {
                itemCollectionViewSource.Filter -= new FilterEventHandler(FilterByZone);
                itemCollectionViewSource.Filter += new FilterEventHandler(FilterByZone);
            }
            else
            {
                itemCollectionViewSource.Filter += new FilterEventHandler(FilterByZone);
                CanRemoveZoneFilter = true;
                CanRemoveFilter = true;
            }
        }

        public void AddTermineFilter()
        {
            // see Notes on Adding Filters:
            if (CanRemoveTermineFilter)
            {
                itemCollectionViewSource.Filter -= new FilterEventHandler(FilterByTermine);
                itemCollectionViewSource.Filter += new FilterEventHandler(FilterByTermine);
            }
            else
            {
                itemCollectionViewSource.Filter += new FilterEventHandler(FilterByTermine);
                CanRemoveTermineFilter = true;
                CanRemoveFilter = true;
            }
        }

        public void AddMesNCFilter()
        {
            // see Notes on Adding Filters:
            if (CanRemoveMesNCFilter)
            {
                itemCollectionViewSource.Filter -= new FilterEventHandler(FilterByMesNC);
                itemCollectionViewSource.Filter += new FilterEventHandler(FilterByMesNC);
            }
            else
            {
                itemCollectionViewSource.Filter += new FilterEventHandler(FilterByMesNC);
                CanRemoveMesNCFilter = true;
                CanRemoveFilter = true;
            }
        }

        public bool CanRemoveFilter
        {
            get { return _canCanRemoveFilter; }
            set
            {
                _canCanRemoveFilter = CanRemoveZoneFilter || CanRemoveMesNCFilter || CanRemoveTermineFilter;
                NotifyPropertyChanged("CanRemoveFilter");
            }
        }

        public bool CanRemoveZoneFilter
        {
            get { return _canCanRemoveZoneFilter; }
            set
            {
                _canCanRemoveZoneFilter = value;
                _canCanRemoveFilter = CanRemoveZoneFilter || CanRemoveMesNCFilter || CanRemoveTermineFilter;
                NotifyPropertyChanged("CanRemoveZoneFilter");
            }
        }

        public bool CanRemoveTermineFilter
        {
            get { return _canCanRemoveTermineFilter; }
            set
            {
                _canCanRemoveTermineFilter = value;
                _canCanRemoveFilter = CanRemoveZoneFilter || CanRemoveMesNCFilter || CanRemoveTermineFilter;
                NotifyPropertyChanged("CanRemoveTermineFilter");
            }
        }

        public bool CanRemoveMesNCFilter
        {
            get { return _canCanRemoveMesNCFilter; }
            set
            {
                _canCanRemoveMesNCFilter = value;
                _canCanRemoveFilter = CanRemoveZoneFilter || CanRemoveMesNCFilter || CanRemoveTermineFilter;
                NotifyPropertyChanged("CanRemoveMesNCFilter");
            }
        }

        /* Notes on Filter Methods:
         * When using multiple filters, do not explicitly set anything to true.  Rather,
         * only hide things which do not match the filter criteria
         * by setting e.Accepted = false.  If you set e.Accept = true, if effectively
         * clears out any previous filters applied to it.  
         */
        private void FilterByZone(object sender, FilterEventArgs e)
        {
            // see Notes on Filter Methods:
            var src = e.Item as NCI;
            if (src == null)
                e.Accepted = false;
            else if (string.Compare(SelectedZone, src.Zone) != 0)
                e.Accepted = false;
        }

        private void FilterByTermine(object sender, FilterEventArgs e)
        {
            // see Notes on Filter Methods:
            var src = e.Item as NCI;
            if (src == null)
                e.Accepted = false;
            else if (src.Termine == true)
                e.Accepted = false;
        }

        private void FilterByMesNC(object sender, FilterEventArgs e)
        {
            // see Notes on Filter Methods:
            var src = e.Item as NCI;
            if (src == null)
                e.Accepted = false;
            else if (string.Compare(SelectedMesNC, src.CreateurNCI) != 0)
                e.Accepted = false;
        }

        public ICommand ResetFiltersCommand
        {
            get
            {
                return _clickCommand ?? (_clickCommand = new CommandHandler(() => ResetFilters(), () => CanRemoveFilter));
            }
        }

        public ICommand RemoveZoneFilterCommand
        {
            get
            {
                return _clickCommand2 ?? (_clickCommand2 = new CommandHandler(() => RemoveZoneFilter(), () => CanRemoveZoneFilter));
            }
        }

        public ICommand RemoveTermineFilterCommand
        {
            get
            {
                return _clickCommand3 ?? (_clickCommand3 = new CommandHandler(() => RemoveTermineFilter(), () => CanRemoveTermineFilter));
            }
        }

        public ICommand RemoveMesNCFilterCommand
        {
            get
            {
                return _clickCommand4 ?? (_clickCommand4 = new CommandHandler(() => RemoveMesNCFilter(), () => CanRemoveMesNCFilter));
            }
        }

        public ICommand DeleteNCCommand
        {
            get
            {
                if (_command == null)
                {
                    _command = new DelegateCommand(CanExecute, ExecuteDeleteNC);
                }
                return _command;
            }
        }

        private void ExecuteDeleteNC(object parameter)
        {
            NCI DeleteNCI = (NCI)parameter;
            context.NCIs.Remove(DeleteNCI);
            context.SaveChanges();
            NotifyPropertyChanged("CanRemoveTermineFilter");
            itemCollectionViewSource.Source = context.NCIs.ToList();
        }

        public ICommand UpdateNCCommand
        {
            get
            {
                if (_command2 == null)
                {
                    _command2 = new DelegateCommand(CanExecute, ExecuteUpdateNC);
                }
                return _command2;
            }
        }

        private void ExecuteUpdateNC(object parameter)
        {
            NCI UpdateNCI = (NCI)parameter;
            //int Id = DeleteNCI.Id;
            PageU = new PageModifierMVVM(UpdateNCI);
            //PageU.DataContext = UpdateNCI;
            PageU.DataContext = this;
            //PageU.DataContext = context;
            PageU.ShowDialog();   
        }

        public ICommand SaveUpdateCommand
        {
            get
            {
                if (_command3 == null)
                {
                    _command3 = new DelegateCommand(CanExecute, ExecuteSaveUpdateNC);
                }
                return _command3;
            }
        }

        private void ExecuteSaveUpdateNC(object parameter)
        {
            context.SaveChanges();
            //this.CloseAssociatedWindow();
            //MessageBox.Show();
            //this.Close();
            //Application.Current.Windows[Application.Current.Windows.Count - 1].Close();
        }

        private bool CanExecute(object parameter)
        {
            return true;
        }

        // Command methods (called by the commands) ===============
        public void ResetFilters()
        {
            // clear filters 
            RemoveZoneFilter();
            RemoveTermineFilter();
            RemoveMesNCFilter();
        }

        public void RemoveZoneFilter()
        {
            itemCollectionViewSource.Filter -= new FilterEventHandler(FilterByZone);
            SelectedZone = null;
            CanRemoveZoneFilter = false;
        }

        public void RemoveTermineFilter()
        {
            itemCollectionViewSource.Filter -= new FilterEventHandler(FilterByTermine);
            SelectedTermine = null;
            CanRemoveTermineFilter = false;
            ckbTermine = false;
            var ckb = (CheckBox)Application.Current.MainWindow.FindName("CkbTermine");
            ckb.IsChecked = false;
        }

        public void RemoveMesNCFilter()
        {
            itemCollectionViewSource.Filter -= new FilterEventHandler(FilterByMesNC);
            SelectedMesNC = null;
            CanRemoveMesNCFilter = false;
            ckbMesNC = false;
            var ckb = (CheckBox)Application.Current.MainWindow.FindName("CkbMesNC");
            ckb.IsChecked = false;
        }
    }
}
