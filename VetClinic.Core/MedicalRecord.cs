using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetClinic.Core
{
    // Описує один запис в Паспорті тварини
    public class MedicalRecord
    {
        // Коли зробили запис
        public DateTime Date { get; set; }
        // Тип (Вакцинація, Алергія, ...)
        public string RecordType { get; set; }
        // Детальний опис
        public string Description { get; set; }
    }
}
