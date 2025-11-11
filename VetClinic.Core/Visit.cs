using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

    namespace VetClinic.Core
    {
    // Клас, що описує Візит до клініки
    public class Visit
    {
        // Унікальний ID візиту
        public Guid Id { get; set; }
        // Дата створення візиту
        public DateTime VisitDate { get; set; }
        // Номер кабінету
        public int CabinetNumber { get; set; }
        // Поточний статус (Зареєстрований, В процесі...)
        public string Status { get; set; }

        // ID тварини, яка на візиті
        public int PetId { get; set; }
        [JsonIgnore]
        public Pet Pet { get; set; }

        // ID лікаря, який веде візит
        public int VeterinarianId { get; set; }
        [JsonIgnore]
        public Veterinarian Veterinarian { get; set; }

        public List<Procedure> Procedures { get; set; }

        // Загальна вартість (розраховується при закритті)
        public decimal TotalCost { get; set; }
        // Час завершення (null, якщо ще не завершено)
        public DateTime? CompletionTime { get; set; }
        // Історія зміни статусів
        public List<StatusHistoryEntry> StatusHistory { get; set; }

        // Конструктор
        public Visit()
        {
            Id = Guid.NewGuid();
            Status = VisitStatus.Registered;
            Procedures = new List<Procedure>();
            StatusHistory = new List<StatusHistoryEntry>();
            StatusHistory.Add(new StatusHistoryEntry
            {
                Timestamp = DateTime.Now,
                Status = VisitStatus.Registered
            });
        }
    }
}