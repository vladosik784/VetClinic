using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.BLL;
using VetClinic.Core;

namespace VetClinic.BLL
{
    public class ProcedureService
    {
        private static List<Procedure> _procedures = new List<Procedure>();

        private static int _nextProcedureId = 1;
        public Procedure AddProcedure(string name, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Название процедуры не может быть пустым.");
                return null;
            }

            if (price <= 0)
            {
                Console.WriteLine("Стоимость процедуры должна быть больше нуля.");
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

            Console.WriteLine($"[ProcedureService] Добавлена услуга: {name}");
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
                    Console.WriteLine("Неверные данные для обновления.");
                    return false;
                }

                procedureToUpdate.Name = newName;
                procedureToUpdate.Price = newPrice;

                Console.WriteLine($"[ProcedureService] Обновлена услуга ID: {id}");
                return true;
            }

            Console.WriteLine($"[ProcedureService] Услуга с ID {id} не найдена.");
            return false; 
        }

        public bool DeleteProcedure(int id)
        {
            var procedureToDelete = GetProcedureById(id);

            if (procedureToDelete != null)
            {
                _procedures.Remove(procedureToDelete);
                Console.WriteLine($"[ProcedureService] Удалена услуга ID: {id}");
                return true;
            }

            Console.WriteLine($"[ProcedureService] Услуга с ID {id} не найдена.");
            return false;
        }
    }
}
