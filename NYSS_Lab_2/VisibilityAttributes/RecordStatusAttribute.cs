using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NYSS_Lab_2
{
    public enum RecordStatuses
    {
        New, Actual, Deleted, Changed
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class RecordStatusAttribute : Attribute
    {
        public RecordStatuses Status { get; set; }

        public RecordStatusAttribute(RecordStatuses status)
        {
            Status = status;
        }

        public RecordStatusAttribute()
        {
            Status = RecordStatuses.Actual;
        }
    }
}
