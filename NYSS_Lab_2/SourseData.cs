using System.Collections.Generic;
using System.Reflection;

namespace NYSS_Lab_2
{
    public class SourseData
    {
        private string id = "";
        private string confidentialityd = "";
        private string integrity = "";
        private string access = "";


        [ColNormal]
        [ColMinimaze]
        [ColumnName("Идентификатор УБИ")]
        public string Id
        { 
            get { return string.IsNullOrEmpty(id) == true ? "" : "УБИ." + id; }
            set { id = value; } 
        }

        [ColNormal]
        [ColMinimaze]
        [ColumnName("Наименование УБИ")]
        public string Name { get; set; }

        [ColNormal]
        [ColumnName("Описание")]
        public string Description { get; set; }

        [ColNormal]
        [ColumnName("Источник угрозы (характеристика и потенциал нарушителя)")]
        public string Sourse { get; set; }

        [ColNormal]
        [ColumnName("Объект воздействия")]
        public string Target { get; set; }

        [ColNormal]
        [ColumnName("Нарушение конфиденциальности")]
        public string Confidentiality
        { 
            get { return confidentialityd == "0" ? "Нет" : "Да"; } 
            set { confidentialityd = value; } 
        }

        [ColNormal]
        [ColumnName("Нарушение целостности")]
        public string Integrity
        {
            get { return integrity == "0" ? "Нет" : "Да"; }
            set { integrity = value; }
        }

        [ColNormal]
        [ColumnName("Нарушение доступности")]
        public string Access
        {
            get { return access == "0" ? "Нет" : "Да"; }
            set { access = value; }
        }

        public SourseData() { }

        public SourseData(string id, string name, string description, string sourse, string target, string confidentiality, string integrity, string access)
        {
            Id = id;
            Name = name;
            Description = description;
            Sourse = sourse;
            Target = target;
            Confidentiality = confidentiality;
            Integrity = integrity;
            Access = access;
        }

        public List<string> GetHeaders()
        {
            List<string> result = new List<string>();
            var prop = this.GetType().GetRuntimeProperties();
            foreach (var item in prop)
            {
                var att = item.GetCustomAttribute(typeof(ColumnNameAttribute));
                if (att != null)
                {
                    result.Add((att as ColumnNameAttribute).Name);
                }
            }
            return result;
        }
    }
}
