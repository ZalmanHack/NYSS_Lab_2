using System;
using System.Collections.Generic;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace NYSS_Lab_2
{
    public enum RowsLinits
    {
        Start, Middle, End
    }
    public class ExcelReader
    {
        public bool IsOpen { get; private set; } = false;
        public int Step { get; set; } = 20;
        public int Count { private get; set; } = 0;
        public string FilePath { get; set; } = "";
        public List<string> RowsId { get; set; } = new List<string>();
        public int Position
        {
            get
            {
                return position;
            }
            set
            {
                if (value > 3 && value <= Count)
                {
                    position = value;
                    if (value + Step > Count)
                        RowsLimitEvent?.Invoke(RowsLinits.End);
                    else
                        RowsLimitEvent?.Invoke(RowsLinits.Middle);
                }
                else
                {
                    position = 3;
                    RowsLimitEvent?.Invoke(RowsLinits.Start);
                }
            }
        }

        // Событие для логики работы кнопок пагинации
        public delegate void RowsLimit(RowsLinits value);
        public event RowsLimit RowsLimitEvent;
        // Положение каретки при чтении строк из файла
        private int position;
        private Excel.Application App;
        private Excel.Workbook Workbook;
        private Excel.Worksheet Worksheet;
        private Excel.Range Range;
        private List<int> ColumnIndexes = new List<int>();
        private List<string> Headers = new List<string>();

        public ExcelReader(string path, List<string> headers)
        {
            FilePath = path;
            Headers = headers;
            App = new Excel.Application();
        }

        ~ExcelReader()
        {
            Close();
        }

        public List<SourseData> Next()
        {
            Position += Step;
            return Read();
        }

        public List<SourseData> Back()
        {
            Position -= Step;
            return Read();
        }

        private void Open()
        {
            IsOpen = true;
            Workbook = App.Workbooks.Open(Path.GetFullPath(FilePath));
            Worksheet = (Excel.Worksheet)Workbook.Worksheets["Sheet"];
            Range = Worksheet.UsedRange;
            List<string> readHeader = new List<string>();
            for (int i = 1; i < Range.Columns.Count; i++)
            {
                readHeader.Add(Range.Cells[2, i].Text);
            }
            ColumnIndexes = GetColIndexes(readHeader);
            Count = Range.Rows.Count;
            Position = 3;
            RowsId.Clear();
            for (int i = Position; i < Range.Rows.Count; i++)
            {
                RowsId.Add(Range.Cells[i, 1].Text);
            }
        }

        public List<SourseData> Read()
        {
            try
            {
                if (!IsOpen)
                {
                    Open();
                }
                List<SourseData> data = new List<SourseData>();
                for (var row = Position; row < Position + Step; row++)
                {
                    SourseData rowData = new SourseData(
                        Range.Cells[row, ColumnIndexes[0]].Text,
                        Range.Cells[row, ColumnIndexes[1]].Text,
                        Range.Cells[row, ColumnIndexes[2]].Text,
                        Range.Cells[row, ColumnIndexes[3]].Text,
                        Range.Cells[row, ColumnIndexes[4]].Text,
                        Range.Cells[row, ColumnIndexes[5]].Text,
                        Range.Cells[row, ColumnIndexes[6]].Text,
                        Range.Cells[row, ColumnIndexes[7]].Text
                        );
                    data.Add(rowData);
                }
                return data;
            }
            catch (Exception)
            {
                return new List<SourseData>();
            }
        }

        public void Close()
        {
            if(IsOpen)
            {
                IsOpen = false;
                if (Workbook != null)
                {
                    Workbook.Close();
                    // Workbook = null;
                }
                if (App != null)
                {
                    //App.Quit();
                    //App = null;
                }
                    
            }
        }

        private List<int> GetColIndexes(List<string> readHeader)
        {
            List<int> result = new List<int>();
            foreach (var item in Headers)
            {
                if (readHeader.Contains(item))
                {
                    result.Add(readHeader.IndexOf(item) + 1); // так как в excel с 1 начинается счет
                }
                else
                {
                    throw new Exception("Полученый документ не соответствует необходимым требованиям");
                }
            }
            return result;
        }
    }
}
