using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Reflection;
using VMS.TPS.Common.Model.API;
using System.Diagnostics;
using checkHU_lung;

// This line is necessary to "write" in database
[assembly: ESAPIScript(IsWriteable = true)]
[assembly: AssemblyVersion("2.0.0.4")]

//
//


namespace VMS.TPS
{
    public class Script
    {
        public Script()   //Constructor
        { }

        public void Execute(ScriptContext context)
        {
            #region Verification de l'existence des données et déclaration des variables
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            StructureSet ss = context.StructureSet;
            Patient patient = context.Patient;

            if (patient == null)
            {
                MessageBox.Show("Merci de charger un patient");
                return;
            }
            patient.BeginModifications();   //Mandatory to write in DataBase

            if (ss == null)
            {
                MessageBox.Show("Merci de charger un groupe de structures");
                return;
            }

            List<Structure> ITV = ss.Structures.Where(x => x.Id.ToUpper().StartsWith("ITV")).ToList(); 
            List<Structure> PTV = ss.Structures.Where(x => x.Id.ToUpper().StartsWith("PTV")).ToList(); 
            if (ITV.Count == 0)
            {
                MessageBox.Show("L'ITV n'existe pas.");
                return;
            }

            var _volume = new Volume();
            var window = new ChoixITV(ITV, PTV);
            window.ShowDialog();
            var window2 = new Indice();
            #endregion

            if (window.theuserflag)
            {
                foreach (Structure Str in window.theuserITVchoice)
                {
                    if (!Str.IsEmpty) _volume.CreateVolume(ss, Str);
                    else MessageBox.Show("La structure est vide.");
                }

                #region Structure et Indice
                try
                {
                    List<Structure> s = ss.Structures.Where(v => v.Id.ToUpper().Contains("RINGUH")).ToList();
                    if (s != null)
                        foreach (Structure UHRing in s)
                        {
                            if (!UHRing.IsEmpty && window.theuserchoice.Any(x => UHRing.Id.Contains(x.Id.Substring(x.Id.Length - 3))))
                            {
                                string Strat = "Aucune modification à réaliser (Indice <= 25%)";
                                List<double> voxels = _volume.GetInterior(ss.Image, UHRing);
                                _volume.Index(voxels, window.Seuil);
                                if (_volume.IndexValue > 25)
                                    Strat = "PTVP --> fixer les UH du PTV à -750 UH pour l'optimisation (Indice > 25%)";
                                try
                                {
                                    window2.Affichage(Strat, patient, UHRing, _volume.IndexValue, stopwatch);
                                    if (!window2.IsVisible) window2.Show();
                                    window.AddIndexPTV(_volume.IndexValue);
                                }
                                catch (Exception ex) { MessageBox.Show(ex.Message); MessageBox.Show("pouet pouet"); }
                            }
                        }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                window2.TheEnd();
                if (window2.IsVisible) window2.Hide();
                window2.ShowDialog();
                if (window2.choice)
                    window.ForcedUH();
            }
            stopwatch.Stop();
            ITV.Clear();
            #endregion
        }
    }
}
