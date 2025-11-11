using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.DAL;

namespace VetClinic.BLL
{
    // Цей сервіс керує лічильниками ID
    public class IdCounterService
    {
        private const string CountersFileName = "id_counters.json";
        private readonly FileRepository<Counter> _counterRepository;
        private List<Counter> _counters;

        // Внутрішній клас для зберігання лічильників
        private class Counter
        {
            public string EntityName { get; set; } 
            public int NextId { get; set; } 
        }

        // Конструктор: завантажує лічильники
        public IdCounterService()
        {
            _counterRepository = new FileRepository<Counter>(CountersFileName);
            _counters = _counterRepository.ReadAll();
        }

        // Отримує наступний ID для сутності
        public int GetNextId(string entityName)
        {
            var counter = _counters.FirstOrDefault(c => c.EntityName == entityName);

            if (counter != null)
            {
                int id = counter.NextId;
                counter.NextId++;
                _counterRepository.SaveChanges(_counters);
                return id;
            }
            else
            {
                var newCounter = new Counter { EntityName = entityName, NextId = 2 };
                _counters.Add(newCounter);
                _counterRepository.SaveChanges(_counters);
                return 1;
            }
        }
    }
}