using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.Core;
using VetClinic.DAL;

namespace VetClinic.BLL
{
    // Сервіс для керування Візитами
    public class VisitService
    {
        private const string VisitFileName = "visits.json";
        private readonly FileRepository<Visit> _visitRepository;
        private List<Visit> _visits;

        // Залежності від інших сервісів
        private readonly PetService _petService;
        private readonly ProcedureService _procedureService;
        private readonly VeterinarianService _veterinarianService;

        // Конструктор
        public VisitService(PetService petService,
                            ProcedureService procedureService,
                            VeterinarianService veterinarianService)
        {
            _petService = petService;
            _procedureService = procedureService;
            _veterinarianService = veterinarianService;

            _visitRepository = new FileRepository<Visit>(VisitFileName);
            _visits = _visitRepository.ReadAll();
            _RestoreRelationships();
        }

        // Зберегти зміни
        private void _SaveChanges()
        {
            _visitRepository.SaveChanges(_visits);
        }

        // Відновлює зв'язки (Visit -> Pet, Visit -> Vet)
        private void _RestoreRelationships()
        {
            foreach (var visit in _visits)
            {
                var pet = _petService.GetPetById(visit.PetId);
                if (pet != null)
                {
                    visit.Pet = pet;
                }

                var vet = _veterinarianService.GetVeterinarianById(visit.VeterinarianId);
                if (vet != null)
                {
                    visit.Veterinarian = vet;
                }
            }
        }

        // Зареєструвати новий візит
        public Visit RegisterVisit(int petId, List<int> procedureIds, int cabinetNumber, int veterinarianId)
        {
            var pet = _petService.GetPetById(petId);
            if (pet == null) return null;

            var vet = _veterinarianService.GetVeterinarianById(veterinarianId);
            if (vet == null) return null;

            // Створення
            var newVisit = new Visit
            {
                VisitDate = DateTime.Now,
                PetId = petId,
                Pet = pet,
                CabinetNumber = cabinetNumber,
                VeterinarianId = veterinarianId,
                Veterinarian = vet
            };

            // Додаємо процедури
            foreach (var procId in procedureIds)
            {
                var procedure = _procedureService.GetProcedureById(procId);
                if (procedure != null && !procedure.IsBlocked)
                {
                    newVisit.Procedures.Add(procedure);
                }
                else if (procedure == null || procedure.IsBlocked)
                {
                    return null;
                }
            }

            if (newVisit.Procedures.Count == 0)
            {
                return null;
            }

            // Збереження
            _visits.Add(newVisit);
            _SaveChanges();
            return newVisit;
        }

        // Отримати всі візити
        public List<Visit> GetAllVisits()
        {
            return new List<Visit>(_visits);
        }

        // Знайти візит за ID
        public Visit GetVisitById(Guid id)
        {
            return _visits.FirstOrDefault(v => v.Id == id);
        }

        // Оновити статус візиту
        public bool UpdateVisitStatus(Guid visitId, string newStatus)
        {
            var visit = GetVisitById(visitId);
            if (visit == null) return false;

            visit.Status = newStatus;
            visit.StatusHistory.Add(new StatusHistoryEntry
            {
                Timestamp = DateTime.Now,
                Status = newStatus
            });

            _SaveChanges();
            return true;
        }

        // Закрити візит (розрахувати вартість)
        public bool CloseVisit(Guid visitId)
        {
            var visit = GetVisitById(visitId);
            if (visit == null || visit.Status == VisitStatus.Completed)
            {
                return false;
            }

            visit.TotalCost = visit.Procedures.Sum(p => p.Price);
            visit.CompletionTime = DateTime.Now;

            return UpdateVisitStatus(visitId, VisitStatus.Completed);
        }
    }
}
