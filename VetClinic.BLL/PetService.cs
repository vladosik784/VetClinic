using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.Core;
using VetClinic.DAL;

namespace VetClinic.BLL
{
    public class PetService
    {
        private const string PetFileName = "pets.json";
        private readonly FileRepository<Pet> _petRepository;
        private List<Pet> _pets;
        private int _nextPetId;

        private readonly OwnerService _ownerService;

        public PetService(OwnerService ownerService)
        {
            _ownerService = ownerService;

            _petRepository = new FileRepository<Pet>(PetFileName);
            _pets = _petRepository.ReadAll();
            _nextPetId = _GetNextId();

            _RestoreRelationships();
        }

        private void _SaveChanges()
        {
            _petRepository.SaveChanges(_pets);
        }

        private int _GetNextId()
        {
            if (_pets.Count == 0) return 1;
            return _pets.Max(p => p.Id) + 1;
        }
        private void _RestoreRelationships()
        {
            if (_pets.Count == 0) return;

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
        public Pet RegisterPet(string name, string species, string breed, int age, int ownerId)
        {
            var owner = _ownerService.GetOwnerById(ownerId);
            if (owner == null)
            {
                Console.WriteLine($"[PetService] Помилка: Власник з ID {ownerId} не знайдений.");
                return null;
            }

            var newPet = new Pet
            {
                Id = _nextPetId++,
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

            Console.WriteLine($"[PetService] Зареєстровано тварину (збережено у файл): {name}");
            return newPet;
        }

        public Pet GetPetById(int id)
        {
            return _pets.FirstOrDefault(p => p.Id == id);
        }

        public List<Pet> GetAllPets()
        {
            return new List<Pet>(_pets);
        }

        public List<Pet> GetPetsByOwnerId(int ownerId)
        {
            return _pets.Where(p => p.OwnerId == ownerId).ToList();
        }
    }
}
