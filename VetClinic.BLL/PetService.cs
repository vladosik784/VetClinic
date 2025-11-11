using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.Core;
using VetClinic.DAL;

namespace VetClinic.BLL
{
    // Сервіс для керування Тваринами
    public class PetService
    {
        private const string PetFileName = "pets.json";
        private readonly FileRepository<Pet> _petRepository;
        private List<Pet> _pets;
        private readonly IdCounterService _idService;

        // Залежність від OwnerService
        private readonly OwnerService _ownerService;

        // Конструктор
        public PetService(OwnerService ownerService, IdCounterService idService)
        {
            _ownerService = ownerService;
            _idService = idService;

            _petRepository = new FileRepository<Pet>(PetFileName);
            _pets = _petRepository.ReadAll();
            _RestoreRelationships();
        }

        // Зберегти зміни
        private void _SaveChanges()
        {
            _petRepository.SaveChanges(_pets);
        }

        // Відновлює зв'язки Pet.Owner, які ігноруються в JSON
        private void _RestoreRelationships()
        {
            foreach (var pet in _pets)
            {
                var owner = _ownerService.GetOwnerById(pet.OwnerId);
                if (owner != null)
                {
                    pet.Owner = owner;
                    if (!owner.Pets.Any(p => p.Id == pet.Id))
                    {
                        owner.Pets.Add(pet);
                    }
                }
            }
        }

        // Зареєструвати нову тварину
        public Pet RegisterPet(string name, string species, string breed, int age, int ownerId)
        {
            var owner = _ownerService.GetOwnerById(ownerId);
            if (owner == null)
            {
                return null;
            }

            var newPet = new Pet
            {
                Id = _idService.GetNextId(nameof(Pet)),
                Name = name,
                Species = species,
                Breed = breed,
                Age = age,
                OwnerId = ownerId,
                Owner = owner
            };

            _pets.Add(newPet);
            owner.Pets.Add(newPet);
            _SaveChanges();
            return newPet;
        }

        // Знайти тварину за ID
        public Pet GetPetById(int id)
        {
            return _pets.FirstOrDefault(p => p.Id == id);
        }

        // Отримати всіх тварин
        public List<Pet> GetAllPets()
        {
            return new List<Pet>(_pets);
        }

        // Отримати тварин конкретного власника
        public List<Pet> GetPetsByOwnerId(int ownerId)
        {
            return _pets.Where(p => p.OwnerId == ownerId).ToList();
        }
    }
}
