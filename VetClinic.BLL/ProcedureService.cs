using System;
using System.Collections.Generic;
using System.Linq;
using VetClinic.Core;
using VetClinic.DAL;

namespace VetClinic.BLL
{
    // Сервіс для керування Послугами (Процедурами)
    public class ProcedureService
    {
        private const string ProcedureFileName = "procedures.json";
        private readonly FileRepository<Procedure> _procedureRepository;
        private List<Procedure> _procedures;
        private readonly IdCounterService _idService;

        // Конструктор
        public ProcedureService(IdCounterService idService)
        {
            _idService = idService;
            _procedureRepository = new FileRepository<Procedure>(ProcedureFileName);
            _procedures = _procedureRepository.ReadAll();
        }

        // Зберігає зміни у файл
        private void _SaveChanges()
        {
            _procedureRepository.SaveChanges(_procedures);
        }

        // Отримати всі процедури
        public List<Procedure> GetAllProcedures()
        {
            return new List<Procedure>(_procedures);
        }

        // Отримати одну процедуру за її ID
        public Procedure GetProcedureById(int id)
        {
            return _procedures.FirstOrDefault(p => p.Id == id);
        }

        // Додати нову процедуру
        public Procedure AddProcedure(string name, decimal price, decimal costPrice, List<string> tags)
        {
            if (string.IsNullOrWhiteSpace(name) || price <= 0 || costPrice < 0)
            {
                return null;
            }

            var procedure = new Procedure
            {
                Id = _idService.GetNextId(nameof(Procedure)),
                Name = name,
                Price = price,
                CostPrice = costPrice,
                Tags = tags ?? new List<string>(),
                IsBlocked = false
            };

            _procedures.Add(procedure);
            _SaveChanges();
            return procedure;
        }

        // Оновити існуючу процедуру
        public bool UpdateProcedure(int id, string name, decimal price, decimal costPrice, List<string> tags, bool isBlocked)
        {
            var proc = GetProcedureById(id);
            if (proc == null)
            {
                return false;
            }

            // Оновлюємо поля
            proc.Name = name;
            proc.Price = price;
            proc.CostPrice = costPrice;
            proc.Tags = tags ?? proc.Tags;
            proc.IsBlocked = isBlocked;

            _SaveChanges();
            return true;
        }

        // Видалити процедуру
        public bool DeleteProcedure(int id)
        {
            var proc = GetProcedureById(id);
            if (proc == null)
            {
                return false;
            }

            _procedures.Remove(proc);
            _SaveChanges();
            return true;
        }

        // Заблокувати/розблокувати процедуру
        public bool BlockProcedure(int id, bool isBlocked)
        {
            var procedure = GetProcedureById(id);
            if (procedure == null)
            {
                return false;
            }

            procedure.IsBlocked = isBlocked;
            _SaveChanges();
            return true;
        }
    }
}
