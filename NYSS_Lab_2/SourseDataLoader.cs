using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;

namespace NYSS_Lab_2
{
    // Состояния положения каретки в файле
    public struct OldNewData
    {
        public List<SourseData> DataOld;
        public List<SourseData> DataNew;
    }
    public class SourseDataLoader
    {

        public string URL { get; set; } = "";

        public delegate void ThrowEvents(string message);
        public event ThrowEvents ThrowEvent;

        public ExcelReader ReaderNew;
        public ExcelReader ReaderOld;

        OldNewData ONData;

        private static string downloadName = "downloaded.xlsx";
        private static string actualName = "actual.xlsx";
        private static string tempName = "temp.xlsx";
        private static string oldName = "old.xlsx";


        public SourseDataLoader(string url, List<string> headers)
        {
            URL = url;
            ReaderNew = new ExcelReader(actualName, headers);
            ReaderOld = new ExcelReader(oldName, headers);
            ONData = new OldNewData();
        }

        public static bool DataExists(string name)
        {
            return File.Exists(name);
        }

        public OldNewData Next()
        {
            if (DataExists(actualName))
            {
                ONData.DataNew = ReaderNew.Next();
            }
            if (DataExists(oldName))
            {
                ONData.DataOld = ReaderOld.Next();
            }
            CompareData();
            return ONData;
        }

        public OldNewData Back()
        {
            if (DataExists(actualName))
            {
                ONData.DataNew = ReaderNew.Back();
            }
            if (DataExists(oldName))
            {
                ONData.DataOld = ReaderOld.Back();
            }
            CompareData();
            return ONData;
        }

        public void Close()
        {
            if (DataExists(actualName))
            {
                ReaderNew.Close();
            }
            if (DataExists(oldName))
            {
                ReaderOld.Close();
            }
        }

        public OldNewData Load()
        {
            if (DataExists(actualName))
            {
                ONData.DataNew = ReaderNew.Read();
            }
            else
            {
                if (ThrowMessage("Файл базы данных не был найден.\nВыполнить загрузку фала?"))
                {
                    Download();
                    ONData.DataNew = ReaderNew.Read();
                }
            }
            if (DataExists(oldName))
            {
                ONData.DataOld = ReaderOld.Read();
            }
            CompareData();
            return ONData;
        }

        protected void GetIdData(in List<SourseData> data, out List<string> result)
        {
            result = new List<string>();
            for (int i = 0; i < data.Count; i++)
                result.Add(data[i].Id);
        }

        public void CompareData()
        {
            if (ONData.DataNew != null && ONData.DataOld != null)
            {
                // получаем все ID 
                List<string> idNew = new List<string>();
                List<string> idOld = new List<string>();
                GetIdData(in ONData.DataNew, out idNew);
                GetIdData(in ONData.DataOld, out idOld);
                for (int i = 0; i < idNew.Count; i++)
                {
                    if (idOld.Contains(idNew[i]))
                    {
                        int j = idOld.IndexOf(idNew[i]);
                        if (ONData.DataNew[i] != ONData.DataOld[j])
                        {
                            ONData.DataNew[i].RecordStatus = RecordStatuses.Changed;
                            ONData.DataOld[j].RecordStatus = RecordStatuses.Changed;
                        }
                        else
                        {
                            ONData.DataNew[i].RecordStatus = RecordStatuses.Actual;
                            ONData.DataOld[j].RecordStatus = RecordStatuses.Actual;
                        }
                    }
                    else
                    {
                        ONData.DataNew[i].RecordStatus = RecordStatuses.New;
                    }
                }
            }
        }

        protected bool ThrowMessage(string message)
        {
            MessageBoxResult result = MessageBox.Show(message, "My App", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            switch (result)
            {
                case MessageBoxResult.Yes: return true;
                case MessageBoxResult.No: return false;
                default: return false;
            }
        }

        public void Download()
        {

            new WebClient().DownloadFile(URL, downloadName);
            if (DataExists(tempName))
            {
                File.Delete(tempName);
            }
            if (DataExists(actualName))
            {
                if (DataExists(oldName))
                {
                    File.Delete(oldName);
                }
                File.Move(actualName, oldName);
                File.Move(downloadName, actualName);
            }
            else
            {
                File.Move(downloadName, actualName);
            }
        }
    }
}
