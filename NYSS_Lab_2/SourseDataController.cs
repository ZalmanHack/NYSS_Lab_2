using System.Collections.Generic;

namespace NYSS_Lab_2
{
    public enum SourseTypes
    {
        Old, New
    }
    public class SourseDataController
    {

        public SourseData DataFrame { get; protected set; } = new SourseData();
        public SourseDataLoader DataLoader;
        // Событие для логики работы кнопок пагинации
        public delegate void RowsLimit(RowsLinits value);
        public event RowsLimit RowsLimitEvent;

        private SourseTypes sourseType;
        public SourseTypes SourseType
        {
            get
            {
                return sourseType;
            }
            set
            {
                sourseType = value;
                if(value == SourseTypes.New)
                {
                    DataLoader.ReaderOld.RowsLimitEvent -= RowsLimitUpdated;
                    DataLoader.ReaderNew.RowsLimitEvent += RowsLimitUpdated;
                }
                else if (value == SourseTypes.Old)
                {
                    DataLoader.ReaderOld.RowsLimitEvent += RowsLimitUpdated;
                    DataLoader.ReaderNew.RowsLimitEvent -= RowsLimitUpdated;
                }
            }
        }

        public void RowsLimitUpdated(RowsLinits value)
        {
            RowsLimitEvent?.Invoke(value);
        }

        public SourseDataController()
        {
            DataLoader = new SourseDataLoader("https://bdu.fstec.ru/files/documents/thrlist.xlsx", DataFrame.GetHeaders());
            SourseType = SourseTypes.New;
        }

        private List<SourseData> ExtraxtData(in OldNewData data)
        {
            if (SourseType == SourseTypes.New)
                return data.DataNew;
            else if (SourseType == SourseTypes.Old)
                return data.DataNew;
            return data.DataNew;
        }

        public List<SourseData> Load()
        {
            return ExtraxtData(DataLoader.Load());
        }
        public List<SourseData> Back()
        {
            return ExtraxtData(DataLoader.Back());
        }
        public List<SourseData> Next()
        {
            return ExtraxtData(DataLoader.Next());
        }

        public void Save()
        {
            DataLoader.Close();
        }
    }
}
