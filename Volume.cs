using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using System.Windows;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace checkHU_lung
{
    // A modifier :
    // Suppression des structures non contenues dans le poumon (résultat plus propre)
    // Réaliser la structure D2cm => problème car nécessite de passer en HR pour avoir une ring continue 

    internal class Volume
    {
        #region Variable global
        static double Min = 0.00;
        static double Max = 0.00;
        static double Median = 0.00;
        static double Mean = 0.00;
        static double UHIndex = 0.00;
        private bool flag = false;
        #endregion

        #region CreateVolume
        public void CreateVolume(StructureSet ss, Structure ITV)
        {
            try
            {
                string RenameITV;
                RenameITV = ITV.Id.Substring(ITV.Id.Length - 3);

                Structure Poumon_Homo;
                Structure Canal_5;
                Structure BODY = ss.Structures.Where(x => x.DicomType.ToUpper().Equals("EXTERNAL")).SingleOrDefault();
                Structure Poumons = ss.Structures.Where(x => x.Id.ToUpper().Equals("POUMONS")).SingleOrDefault();
                Structure Canalmed = ss.Structures.Where(x => x.Id.ToUpper().Contains("CANAL MED")).SingleOrDefault();
                Structure PTV = ss.AddStructure("PTV", "_");
                PTV.SegmentVolume = ITV.Margin(5.00);

                if (!flag)
                {
                    Canal_5= ss.AddStructure("Avoidance", "Canal+5");
                    Canal_5.SegmentVolume = Canalmed.Margin(5.00);
                    //Canal_5.SegmentVolume = Canal_5.Sub(Canalmed);
                    flag = true;
                }

                if (ITV.CenterPoint.x > 0) Poumon_Homo = ss.Structures.Where(x => x.Id.ToUpper().Contains("POUMONG")).SingleOrDefault();
                else Poumon_Homo = ss.Structures.Where(x => x.Id.ToUpper().Contains("POUMOND")).SingleOrDefault();

                #region Ring
                try
                {
                    Structure Ring1 = ss.AddStructure("Control", "Ring1 " + RenameITV);
                    Ring1.SegmentVolume = PTV.Margin(5.00);
                    Structure Ring2 = ss.AddStructure("Control", "Ring2 " + RenameITV);
                    Ring2.SegmentVolume = PTV.Margin(10.00);
                    Structure Ring3 = ss.AddStructure("Control", "Ring3 " + RenameITV);
                    Ring3.SegmentVolume = PTV.Margin(30.00);

                    Ring3.SegmentVolume = Ring3.Sub(Ring2).And(BODY);
                    Ring2.SegmentVolume = Ring2.Sub(Ring1).And(BODY);
                    Ring1.SegmentVolume = Ring1.Sub(PTV).And(BODY);

                    Structure RingUH = ss.AddStructure("Control", "RingUH " + RenameITV);
                    RingUH.SegmentVolume = ITV.Margin(15.00);
                    RingUH.SegmentVolume = RingUH.Sub(ITV).And(BODY).And(Poumons);
                }
                catch { }
                #endregion
                #region Structure d'optimisation 
                try
                {
                    Structure PoumonHomo_PTV_1cm = ss.AddStructure("Organ", "PoumH-PTV+1 " + RenameITV);
                    Structure Poumons_ITV = ss.AddStructure("Avoidance", "Poumons-ITV " + RenameITV);
                    Structure Poumons_PTV = ss.AddStructure("Avoidance", "Poumons-PTV " + RenameITV);
                    Structure PTV_ITV = ss.AddStructure("PTV", "PTV-ITV " + RenameITV);

                    PoumonHomo_PTV_1cm.SegmentVolume = Poumon_Homo.Margin(0.00);
                    PoumonHomo_PTV_1cm.SegmentVolume = Poumon_Homo.Sub(PTV.Margin(10.00)).And(BODY);
                    Poumons_ITV.SegmentVolume = Poumons.Margin(0.00);
                    Poumons_ITV.SegmentVolume = Poumons.Sub(ITV).And(BODY);
                    Poumons_PTV.SegmentVolume = Poumons.Margin(0.00);
                    Poumons_PTV.SegmentVolume = Poumons.Sub(PTV).And(BODY);
                    PTV_ITV.SegmentVolume = PTV.Margin(0.00);
                    PTV_ITV.SegmentVolume = PTV.Sub(ITV).And(BODY);
                }
                catch { }
                ss.RemoveStructure(PTV);                
                #endregion
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show("Le PTV n'existe pas.\nImpossible de créer les volumes d'évaluation.\nVérifier l'existence de l'ITV et du PTV.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur inattendue s'est produite : " + ex.ToString());
            }
        }
        #endregion
        #region GetInterior
        public List<double> GetInterior(Image dose, Structure st)
        {
            List<double> result = new List<double>();
            VVector p = new VVector();
            Rect3D st_bounds = st.MeshGeometry.Bounds;
            VVector centre = st.CenterPoint;

            int x = (int)((centre.x - dose.Origin.x) / dose.XRes);
            int y = (int)((centre.y - dose.Origin.y) / dose.YRes);
            int z = (int)((centre.z - dose.Origin.z) / dose.ZRes);

            int zinit = z - (int)st_bounds.SizeZ / 2;
            int zfin = z + (int)st_bounds.SizeZ / 2;

            int xinit = x - (int)st_bounds.SizeX / 2;
            int xfin = x + (int)st_bounds.SizeX / 2;

            int yinit = y - (int)st_bounds.SizeY / 2;
            int yfin = y + (int)st_bounds.SizeY / 2;

            for (z = zinit; z < zfin; ++z)
            {
                for (y = yinit; y < yfin; ++y)
                {
                    for (x = xinit; x < xfin; ++x)
                    {
                        p.x = x * dose.XRes;
                        p.y = y * dose.YRes;
                        p.z = z * dose.ZRes;

                        p += dose.Origin;

                        if (st_bounds.Contains(p.x, p.y, p.z) // trimming
                        && st.IsPointInsideSegment(p)) // this is an expensive call
                        {
                            int[,] voxels = new int[dose.XSize, dose.YSize];
                            dose.GetVoxels(z, voxels);

                            result.Add(dose.VoxelToDisplayValue(voxels[x, y]));
                        }
                    }
                }
                GC.Collect(); // do this to avoid time out
                GC.WaitForPendingFinalizers();
            }
            return result;
        }
        #endregion
        #region Index
        public void Index(List<double> Voxel, double seuil)
        {
            Min = Voxel.Min();
            Max = Voxel.Max();
            Mean = Voxel.Average();
            Median = Voxel[Voxel.OrderBy(v => v).ToList().Count / 2];
            UHIndex = Voxel.Where(v => v >= Voxel.Min() && v < seuil).ToList().Count * 100 / Voxel.Count;
        }
        #endregion
        #region Get
        public double IndexValue
        {
            get { return UHIndex; }
        }
        #endregion
    }
}

