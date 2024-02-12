using System.Collections.Generic;
using System.Linq;
using VMS.TPS.Common.Model.API;

namespace checkHU_lung
{
    internal class ITVModel
    {
        #region private variable
        private bool _flag = false;
        private List<Structure> _ITV;
        private List<Structure> _PTV;
        private List<Structure> _UserITV;
        private List<double> _Index;
        private List<Structure> _UserPTV;
        private readonly double _UH;
        private readonly double _seuil;
        #endregion

        public ITVModel(List<Structure> ITV, List<Structure> PTV)
        {
            _flag = false;
            _ITV = new List<Structure>(ITV);
            _PTV = new List<Structure>(PTV);
            _Index = new List<double>();
            _UserITV = new List<Structure>();
            _UserPTV = new List<Structure>();
            _UH = -750;
            _seuil = -900;
        }
        public void AddUserStruct(Structure Struct)
        {
            _UserITV.Add(Struct);
            _UserPTV.Add(_PTV.FirstOrDefault(ptv => ptv.Id.Length >= 3 && ptv.Id.Substring(ptv.Id.Length - 3).ToUpper().Equals(Struct.Id.Substring(Struct.Id.Length - 3).ToUpper())));
        }
        public void AddIndex(double Index)
        {
            _Index.Add(Index);         
        }

        #region Get and Set
        public bool Flag
        {
            get { return _flag; }
            set { _flag = value; }
        }
        public List<Structure> Struct
        {
            get { return _ITV; }
            set { _ITV = value; }
        }
        public List<Structure> UserStruct
        {
            get { return _UserITV; }
            set { _UserITV = value; }
        }
        public double UHValue
        {
            get { return _UH; }
        }
        public double Seuil
        {
            get { return _seuil; }
        }
        #endregion
        public void AssignedUH()
        {       
            int i = 0;
            foreach (Structure Struct in _UserPTV)
            {
                if (_Index[i] > 25)
                    Struct.SetAssignedHU(_UH);
                i++;
            }        
        }
    }
}