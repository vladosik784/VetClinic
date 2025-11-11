using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.Core;
using VetClinic.DAL;

namespace VetClinic.BLL
{
    public class VisitService
    {
        private const string VisitFileName = "visits.json";
        private readonly FileRepository<Visit> _visitRepository;
        private List<Visit> _visits;

        private readonly PetService _petService;
        private readonly ProcedureService _procedureService;

        public VisitService(PetService petService, ProcedureService procedureService)
        {
            _petService = petService;
            _procedureService = procedureService;

            _visitRepository = new FileRepository<Visit>(VisitFileName);
            _visits = _visitRepository.ReadAll();

            _RestoreRelationships();
        }

        private void _SaveChanges()
        {
            _visitRepository.SaveChanges(_visits);
        }

        private void _RestoreRelationships()
        {
            if (_visits.Count == 0) return;
            foreach (var visit in _visits)
            {
                var pet = _petService.GetPetById(visit.PetId);
                if (pet != null)
                {
                    visit.Pet = pet;
                }
            }
        }

        public Visit RegisterVisit(int petId, List<int> procedureIds, int cabinetNumber)
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
                Pet = pet,
                CabinetNumber = cabinetNumber
            };

            foreach (var procId in procedureIds)
            {
                var procedure = _procedureService.GetProcedureById(procId);
                if (procedure != null && !procedure.IsBlocked)
                {
                    newVisit.Procedures.Add(procedure);
                }
            }

            if (newVisit.Procedures.Count == 0)
            {
                Console.WriteLine("[VisitService] Помилка: Візит не може бути створений без доступних процедур.");
                return null;
            }

            _visits.Add(newVisit);
            _SaveChanges();

            Console.WriteLine($"[VisitService] Успішно зареєстровано візит у кабінеті {cabinetNumber} для {pet.Name}.");
            return newVisit;
        }
        
        public Visit RegisterVisit(int petId, List<int> procedureIds)
        {
            return RegisterVisit(petId, procedureIds, 0);
        }

        public List<Visit> GetAllVisits()
        {
            return new List<Visit>(_visits);
        }

        public Visit GetVisitById(Guid id)
        {
            return _visits.FirstOrDefault(v => v.Id == id);
        }

        public bool UpdateVisitStatus(Guid visitId, string newStatus)
        {
            var visit = GetVisitById(visitId);
            if (visit == null)
            {
                Console.WriteLine($"[VisitService] Помилка: Візит {visitId} не знайдено.");
                return false;
            }

            visit.Status = newStatus;

            visit.StatusHistory.Add(new StatusHistoryEntry
            {
                Timestamp = DateTime.Now,
                Status = newStatus
            });

            _SaveChanges();

            Console.WriteLine($"[VisitService] Візит {visitId} оновлено. Новий статус: {newStatus}");
            return true;
        }

        public bool CloseVisit(Guid visitId)
        {
            var visit = GetVisitById(visitId);
            if (visit == null)
            {
                Console.WriteLine($"[VisitService] Помилка: Візит {visitId} не знайдено.");
                return false;
            }

            if (visit.Status == VisitStatus.Completed || visit.Status == VisitStatus.Cancelled)
            {
                Console.WriteLine($"[VisitService] Помилка: Візит {visitId} вже закрито або скасовано.");
                return false;
            }

            visit.TotalCost = visit.Procedures.Sum(p => p.Price);

            visit.CompletionTime = DateTime.Now;

            UpdateVisitStatus(visitId, VisitStatus.Completed);

            Console.WriteLine($"[VisitService] Візит {visitId} закрито. Загальна вартість: {visit.TotalCost} грн.");
            return true;
        }
    }
}
