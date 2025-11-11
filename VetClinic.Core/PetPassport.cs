using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VetClinic.Core
{
    // Клас, що описує Паспорт тварини
    public class PetPassport
    {
        public int Id { get; set; }

        // ID тварини, якій належить паспорт
        public int PetId { get; set; }
        [JsonIgnore]
        public Pet Pet { get; set; }
        public int OwnerId { get; set; }
        [JsonIgnore]
        public Owner Owner { get; set; }

        // Всі медичні записи
        public List<MedicalRecord> MedicalHistory { get; set; }

        // Конструктор
        public PetPassport()
        {
            MedicalHistory = new List<MedicalRecord>();
        }
    }
}
