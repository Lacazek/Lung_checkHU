using System;
using System.Diagnostics;
using VMS.TPS.Common.Model.API;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace checkHU_lung
{
    internal class IndiceModel : INotifyPropertyChanged
    {
        private String _strat;
        private Patient _patient;
        private Structure UHRing;
        private double indexValue;
        private Stopwatch _stopwatch;
        private string message;
        private bool choix;

        private List<string> _recapList;
        private List<double> StackIndex;

        private string imagePath;
        private DirectoryInfo CurrentDirectory;

        public event PropertyChangedEventHandler PropertyChanged;

        public IndiceModel()
        {
            CurrentDirectory = Directory.GetParent(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            ImagePath = System.IO.Path.Combine(CurrentDirectory.ToString(), "Information.jpg");
            _recapList = new List<string>();
            StackIndex = new List<double>();
        }

        #region Message
        public void Update()
        {
            if (IndexValue > 25)
                ImagePath = System.IO.Path.Combine(CurrentDirectory.ToString(), "Exclamation.jpg");

            Message = "Les résultats suivant concerne la structure suivante :\n\n" + UHRING.Id + "\n\nNom du patient : " + _patient.Name +
                    "\nValeur de l'indice (% de Voxel < -900 UH) : " + IndexValue + " %" +
                    "\n\nUtiliser la stratégie : \n" + Strat +
                    "\n\n Le temps d'exécution est de " + _stopwatch.ElapsedMilliseconds / 1000 + " s";
            MessageCreation(IndexValue);
            StackIndex.Add(IndexValue);
        }
        private void MessageCreation(double IndexValue)
        {
            _recapList.Add("Nom Structure : " + UHRing.Id + "\n" +
                "Nom du patient : " + patient.Name + "\n" +
                "Indice = " + IndexValue +" %\n"+
                "Stratégie : " + _strat + "\n\n");
        }
        public void MessageFinal()
        {
            Message = string.Join(Environment.NewLine, GetMessageFinal);
        }
        #endregion
        #region Get and Set
        public string Strat
        {
            get { return _strat; }
            set { _strat = value; }
        }
        public bool Choix
        {
            get { return choix; }
            set { choix = value; }
        }
        public Patient patient
        {
            get { return _patient; }
            set { _patient = value; }
        }
        public Structure UHRING
        {
            get { return UHRing; }
            set { UHRing = value; }
        }
        public double IndexValue
        {
            get { return indexValue; }
            set { indexValue = value; }
        }
        public Stopwatch stopWatch
        {
            get { return _stopwatch; }
            set { _stopwatch = value; }
        }
        public string Message
        {
            get { return message; }
            set { message = value; OnPropertyChanged(nameof(Message)); }
        }
        private string ImagePath
        {
            get { return imagePath; }
            set { imagePath = value; OnPropertyChanged(nameof(ImagePath)); }
        }
        public List<string> GetMessageFinal
        {
            get { return _recapList; }
        }
        public List<double> GetIndex
        {
            get { return StackIndex; }
        }
        #endregion

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
