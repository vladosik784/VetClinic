using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

    namespace VetClinic.Core
    {
        public class Visit
        {
            public Guid Id { get; set; }
            public DateTime VisitDate { get; set; }
            
            public int CabinetNumber { get; set; }  
            
            public string Status { get; set; }

            public int PetId { get; set; }

            [JsonIgnore]
            public Pet Pet { get; set; }

            public List<Procedure> Procedures { get; set; }

            public decimal TotalCost { get; set; }

            public DateTime? CompletionTime { get; set; }

            public List<StatusHistoryEntry> StatusHistory { get; set; }

            public Visit()
            {
                Id = Guid.NewGuid();
                Procedures = new List<Procedure>();

                StatusHistory = new List<StatusHistoryEntry>();

                Status = VisitStatus.Registered;

                StatusHistory.Add(new StatusHistoryEntry
                {
                    Timestamp = DateTime.Now,
                    Status = VisitStatus.Registered
                });
            }
        }
    }