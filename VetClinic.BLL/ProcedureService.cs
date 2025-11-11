using System;
using System.Collections.Generic;
using System.Linq;
using VetClinic.Core;
using VetClinic.DAL;

namespace VetClinic.BLL
{
    public class ProcedureService
    {
        private const string ProcedureFileName = "procedures.json";
        private readonly FileRepository<Procedure> _procedureRepository;
        private readonly List<Procedure> _procedures;

        public ProcedureService()
        {
            _procedureRepository = new FileRepository<Procedure>(ProcedureFileName);
            _procedures = _procedureRepository.ReadAll();
        }

        private void _SaveChanges()
        {
            _procedureRepository.SaveChanges(_procedures);
        }

        public List<Procedure> GetAllProcedures() => new List<Procedure>(_procedures);

        public Procedure GetProcedureById(int id)
        {
            return _procedures.FirstOrDefault(p => p.Id == id);
        }

        //додати процедуру
        public void AddProcedure(string name, decimal price, decimal costPrice, List<string> tags=null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("[ProcedureService] Помилка: назва не може бути порожньою.");
                return;
            }

            var procedure = new Procedure
            {
                Id = _procedures.Count > 0 ? _procedures.Max(p => p.Id) + 1 : 1,
                Name = name,
                Price = price,
                CostPrice = costPrice,
                Tags = tags ?? new List<string>(),
                IsBlocked = false
            };

            _procedures.Add(procedure);
            _SaveChanges();

            Console.WriteLine($"[ProcedureService] Додано процедуру '{name}' з ціною {price} грн.");
        }
       
        //оновити процедуру
        public void UpdateProcedure(int id, string name = null, decimal? price = null, decimal? costPrice = null, List<string> tags = null, bool? isBlocked = null)
        {
            var proc = GetProcedureById(id);
            if (proc == null)
            {
                Console.WriteLine($"[ProcedureService] Процедуру ID {id} не знайдено.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(name)) proc.Name = name;
            if (price.HasValue) proc.Price = price.Value;
            if (costPrice.HasValue) proc.CostPrice = costPrice.Value;
            if (tags != null) proc.Tags = tags;
            if (isBlocked.HasValue) proc.IsBlocked = isBlocked.Value;

            _SaveChanges();
            Console.WriteLine($"[ProcedureService] Процедуру ID {id} оновлено.");
        }
        //видалити процедуру
        public void DeleteProcedure(int id)
        {
            var proc = GetProcedureById(id);
            if (proc == null)
            {
                Console.WriteLine($"[ProcedureService] Процедуру ID {id} не знайдено.");
                return;
            }

            _procedures.Remove(proc);
            _SaveChanges();
            Console.WriteLine($"[ProcedureService] Процедуру '{proc.Name}' видалено.");
        }

        //знайти процедуру
        public void BlockProcedure(int id, bool isBlocked)
        {
            var procedure = GetProcedureById(id);
            if (procedure == null)
            {
                Console.WriteLine($"[ProcedureService] Процедуру ID {id} не знайдено.");
                return;
            }

            procedure.IsBlocked = isBlocked;
            _SaveChanges();
            Console.WriteLine($"[ProcedureService] Процедура '{procedure.Name}' {(isBlocked ? "заблокована" : "доступна")}.");
        }
    }
}
