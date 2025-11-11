using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetClinic.Core
{
    // Описує один запис в історії статусів Візиту
    public class StatusHistoryEntry
    {
        // Коли статус змінився
        public DateTime Timestamp { get; set; }
        // На який статус
        public string Status { get; set; }
    }
}