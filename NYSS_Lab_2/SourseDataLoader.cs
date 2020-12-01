using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Linq;
using System.Diagnostics;
using Microsoft.Win32;

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

        public delegate void InfoDelegte(string message);
        public event InfoDelegte InfoEvent;

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
                if (ThrowMessage("Файл базы данных не был найден.\nВыполнить загрузку фала?\n\nВ противном случае файл будет скачан по таймеру"))
                {
                    Download();
                    ONData.DataNew = ReaderNew.Read();
                }
            }
            if (DataExists(oldName))
            {
                ONData.DataOld = ReaderOld.Read();
            }
            return ONData;
        }

        public static void CloseProcess()
        {
            Process[] List;
            List = Process.GetProcessesByName("EXCEL");
            foreach (Process proc in List)
            {
                proc.Kill();

            }
        }

        public void AnalizData()
        {
            if(ONData.DataNew != null && ONData.DataOld != null)
            {
                int added = ReaderNew.RowsId.Except(ReaderOld.RowsId).Count();
                int deleted = ReaderOld.RowsId.Except(ReaderNew.RowsId).Count();
                InfoEvent?.Invoke($"Добавлено строк: {added}    Удалено строк: {deleted}    Всего строк: {ReaderNew.RowsId.Count}");
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

        public void Save(string path)
        {
            if (Path.GetFullPath(actualName) == null)
            {
                MessageBox.Show("Файл не найден");
            }
            else
            {
                File.Copy(Path.GetFullPath(actualName), path);
            }
        }

        public bool Download()
        {
            WebClient client = new WebClient();
            ReaderNew.Close();
            ReaderOld.Close();
            bool state = true;
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    client.DownloadFile(URL, downloadName);
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
                catch (System.Exception)
                {
                    if (i == 1)
                        state = false;
                }
            }
            return state;
        }
    }
}
