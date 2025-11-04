using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetClinic.Core
{
    public class Visit
    {
        public Guid Id { get; set; }
        public DateTime VisitDate { get; set; }

        public string Status { get; set; }

        public int PetId { get; set; }
        public Pet Pet { get; set; }

        public List<Procedure> Procedures { get; set; }

        public Visit()
        {
            Id = Guid.NewGuid();

            Status = "Зареєстрований";

            Procedures = new List<Procedure>();
        }
    }
}
