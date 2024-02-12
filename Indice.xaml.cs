using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using VMS.TPS.Common.Model.API;

namespace checkHU_lung
{
    /// <summary>
    /// Logique d'interaction pour Indice.xaml
    /// </summary>
    public partial class Indice : Window
    {
        private IndiceModel _IndiceModel;

        // Problème message qui s'actualise pas
        // Actualisation de la 1ere image

        public Indice()
        {
            InitializeComponent();
            _IndiceModel = new IndiceModel();
            DataContext = _IndiceModel;

            Bouton_final.Visibility = Visibility.Collapsed;
        }

        #region Message
        // Update du message et création du message final
        public void Affichage(String strat, Patient patient, Structure UHRing, double indexValue, Stopwatch stopwatch)
        {
            _IndiceModel.Strat = strat; _IndiceModel.patient = patient; _IndiceModel.UHRING = UHRing; _IndiceModel.IndexValue = indexValue; _IndiceModel.stopWatch = stopwatch;
            _IndiceModel.Update();
        }

        public void TheEnd()
        {
            if (_IndiceModel.GetIndex.Any(index=>index > 25))
            {
                OK.Visibility = Visibility.Collapsed;
                Bouton_final.Visibility = Visibility.Visible;
            }
            _IndiceModel.MessageFinal();
        }
        #endregion
        #region Get et Set
        internal IndiceModel GetModelIndice
        {
            get { return _IndiceModel; }
        }
        internal bool choice
        {
            get { return _IndiceModel.Choix; }
        }
        #endregion
        #region Button
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            _IndiceModel.Choix = true;
            MessageBox.Show("Script terminé, les UH du et/ou des PTV d'intérêt(s) vont être fixés à -750 UH !\n\n" +
                "Merci de vérifier pour éviter tout risque d'erreur !", "Forced UH", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion
    }
}