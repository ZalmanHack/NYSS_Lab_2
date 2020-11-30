using System.Collections.Generic;

namespace NYSS_Lab_2
{
    public class SourseDataController
    {
        public SourseData DataFrame { get; protected set; } = new SourseData();
        public SourseDataLoader DataLoader;

        public SourseDataController()
        {
            DataLoader = new SourseDataLoader("https://bdu.fstec.ru/files/documents/thrlist.xlsx", DataFrame.GetHeaders());
        }

        public List<SourseData> Load()
        {
            return DataLoader.Load();
        }
        public List<SourseData> Back()
        {
            return DataLoader.Reader.Back();
        }
        public List<SourseData> Next()
        {
            return DataLoader.Reader.Next();
        }

        public void Save()
        {
            DataLoader.Reader.Close();
        }
    }
}
