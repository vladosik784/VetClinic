using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.Core;

namespace VetClinic.BLL
{
    public class VisitService
    {
        private static List<Visit> _visits = new List<Visit>();

        private readonly PetService _petService;
        private readonly ProcedureService _procedureService;

        public VisitService(PetService petService, ProcedureService procedureService)
        {
            _petService = petService;
            _procedureService = procedureService;
        }
        public Visit RegisterVisit(int petId, List<int> procedureIds)
        {
            var pet = _petService.GetPetById(petId);
            if (pet == null)
            {
                Console.WriteLine($"[VisitService] Помилка: Тварина з ID {petId} не знайдена.");
                return null;
            }

            var newVisit = new Visit
            {
                VisitDate = DateTime.Now,
                PetId = petId,
                Pet = pet
            };

            foreach (var procId in procedureIds)
            {
                var procedure = _procedureService.GetProcedureById(procId);
                if (procedure != null)
                {
                    newVisit.Procedures.Add(procedure);
                }
                else
                {
                    Console.WriteLine($"[VisitService] Попередження: Процедура з ID {procId} не знайдена і не буде додана.");
                }
            }

            if (newVisit.Procedures.Count == 0)
            {
                Console.WriteLine("[VisitService] Помилка: Візит не може бути створений без процедур.");
                return null;
            }

            _visits.Add(newVisit);

            Console.WriteLine($"[VisitService] Успішно зареєстровано візит {newVisit.Id} для {pet.Name}.");
            return newVisit;
        }

        public List<Visit> GetAllVisits()
        {
            return new List<Visit>(_visits);
        }
    }
}
