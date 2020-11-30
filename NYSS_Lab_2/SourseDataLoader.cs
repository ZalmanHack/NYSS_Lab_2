using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;

namespace NYSS_Lab_2
{
    // Состояния положения каретки в файле

    public class SourseDataLoader
    {
        protected List<SourseData> Data { get; set; } = new List<SourseData>();
        public string URL { get; set; } = "";

        public delegate void ThrowEvents(string message);
        public event ThrowEvents ThrowEvent;

        public ExcelReader Reader;

        private static string downloadName = "downloaded.xlsx";
        private static string actualName = "actual.xlsx";
        private static string tempName = "temp.xlsx";
        private static string oldName = "old.xlsx";


        public SourseDataLoader(string url, List<string> headers)
        {
            URL = url;
            Reader = new ExcelReader(actualName, headers);
        }

        public static bool DataExists(string name)
        {
            return File.Exists(name);
        }

        public List<SourseData> Load()
        {
            if (DataExists(actualName))
            {
                Data = Reader.Read();
            }
            else
            {
                if (ThrowMessage("Файл базы данных не был найден.\nВыполнить загрузку фала?"))
                {
                    Download();
                    Data = Reader.Read();
                }
            }
            return Data;
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
