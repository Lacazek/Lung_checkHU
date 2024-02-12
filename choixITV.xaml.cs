using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using VMS.TPS.Common.Model.API;


namespace checkHU_lung
{
    /// <summary>
    /// Logique d'interaction pour choixITV.xaml
    /// </summary>

    public partial class ChoixITV : Window
    {
        private ITVModel _ITVModel;

        public ChoixITV(List<Structure> ITV, List<Structure> PTV)
        {
            InitializeComponent();
            _ITVModel = new ITVModel(ITV,PTV);
            DataContext = _ITVModel;
            comboITV_2.Visibility = Visibility.Collapsed;
            comboITV_3.Visibility = Visibility.Collapsed;
        }
        public void AddIndexPTV(double Index)
        {
            _ITVModel.AddIndex(Index);
        }
        public void ForcedUH()
        {
            _ITVModel.AssignedUH();
        }
        #region Get et Set
        public bool theuserflag
        {
            get { return _ITVModel.Flag; }
        }
        public List<Structure> theuserchoice
        {
            get { return _ITVModel.Struct; }
            set { _ITVModel.Struct = value; }
        }
        public List<Structure> theuserITVchoice
        {
            get { return _ITVModel.UserStruct; }
            set { _ITVModel.UserStruct = value; }
        }
        public double theuserITVUH
        {
            get { return _ITVModel.UHValue; }
        }
        public double Seuil
        {
            get { return _ITVModel.Seuil; }
        }
        #endregion
        #region Button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _ITVModel.Flag = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("L'exécution a été interrompu !", "Information ", 0, MessageBoxImage.Warning);
            this.Close();
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _ITVModel.AddUserStruct((Structure)comboITV.SelectedItem);                
                comboITV_2.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _ITVModel.AddUserStruct((Structure)comboITV_2.SelectedItem);
                comboITV_3.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ComboBox3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _ITVModel.AddUserStruct((Structure)comboITV_3.SelectedItem);
            }
            catch (Exception ex)
            {
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }
        #endregion
    }
}


