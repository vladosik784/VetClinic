using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.DAL;
using VetClinic.Core;

namespace VetClinic.BLL
{
    // Сервіс для керування Лікарями
    public class VeterinarianService
    {
        private const string VetFileName = "veterinarians.json";
        private readonly FileRepository<Veterinarian> _vetRepository;
        private List<Veterinarian> _veterinarians;
        private readonly IdCounterService _idService;

        // Конструктор
        public VeterinarianService(IdCounterService idService)
        {
            _idService = idService;
            _vetRepository = new FileRepository<Veterinarian>(VetFileName);
            _veterinarians = _vetRepository.ReadAll();
        }

        // Зберегти зміни
        private void _SaveChanges()
        {
            _vetRepository.SaveChanges(_veterinarians);
        }

        // Додати нового лікаря
        public Veterinarian AddVeterinarian(string fullName, string specialization)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(specialization))
            {
                return null;
            }

            var newVet = new Veterinarian
            {
                Id = _idService.GetNextId(nameof(Veterinarian)),
                FullName = fullName,
                Specialization = specialization
            };

            _veterinarians.Add(newVet);
            _SaveChanges();
            return newVet;
        }

        // Знайти лікаря за ID
        public Veterinarian GetVeterinarianById(int id)
        {
            return _veterinarians.FirstOrDefault(v => v.Id == id);
        }

        // Отримати всіх лікарів
        public List<Veterinarian> GetAllVeterinarians()
        {
            return new List<Veterinarian>(_veterinarians);
        }
    }
}