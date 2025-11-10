using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.BLL;
using VetClinic.Core;
using VetClinic.DAL;

namespace VetClinic.BLL
{
    public class ProcedureService
    {
        private const string ProcedureFileName = "procedures.json";

        private readonly FileRepository<Procedure> _procedureRepository;

        private List<Procedure> _procedures;

        private int _nextProcedureId;

        public ProcedureService()
        {
            _procedureRepository = new FileRepository<Procedure>(ProcedureFileName);

            _procedures = _procedureRepository.ReadAll();

            _nextProcedureId = _GetNextId();
        }

        private void _SaveChanges()
        {
            _procedureRepository.SaveChanges(_procedures);
        }

        private int _GetNextId()
        {
            if (_procedures.Count == 0)
            {
                return 1; 
            }

            return _procedures.Max(p => p.Id) + 1;
        }

        public Procedure AddProcedure(string name, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name) || price <= 0)
            {
                Console.WriteLine("[ProcedureService] Невірні дані для процедури.");
                return null;
            }

            var newProcedure = new Procedure
            {
                Id = _nextProcedureId,
                Name = name,
                Price = price
            };

            _procedures.Add(newProcedure);
            _nextProcedureId++; 

            _SaveChanges();

            Console.WriteLine($"[ProcedureService] Додана послуга (збережено у файл): {name}");
            return newProcedure;
        }

        public List<Procedure> GetAllProcedures()
        {
            return new List<Procedure>(_procedures);
        }
        public Procedure GetProcedureById(int id)
        {
            return _procedures.FirstOrDefault(p => p.Id == id);
        }

        public bool UpdateProcedure(int id, string newName, decimal newPrice)
        {
            var procedureToUpdate = GetProcedureById(id);

            if (procedureToUpdate != null)
            {
                if (string.IsNullOrWhiteSpace(newName) || newPrice <= 0)
                {
                    Console.WriteLine("[ProcedureService] Невірні дані для оновлення.");
                    return false;
                }

                procedureToUpdate.Name = newName;
                procedureToUpdate.Price = newPrice;

                _SaveChanges();

                Console.WriteLine($"[ProcedureService] Оновлена послуга ID: {id} (збережено у файл)");
                return true;
            }

            Console.WriteLine($"[ProcedureService] Послуга з ID {id} не знайдена.");
            return false;
        }
        public bool DeleteProcedure(int id)
        {
            var procedureToDelete = GetProcedureById(id);

            if (procedureToDelete != null)
            {
                _procedures.Remove(procedureToDelete);

                _SaveChanges();

                Console.WriteLine($"[ProcedureService] Видалена послуга ID: {id} (збережено у файл)");
                return true;
            }

            Console.WriteLine($"[ProcedureService] Послуга з ID {id} не знайдена.");
            return false;
        }
    }
}
