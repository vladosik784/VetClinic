using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.Core;

namespace VetClinic.BLL
{
    public class PetService
    {
        private static List<Pet> _pets = new List<Pet>();
        private static int _nextPetId = 1;

        private readonly OwnerService _ownerService;
        public PetService(OwnerService ownerService)
        {
            _ownerService = ownerService;
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

            Console.WriteLine($"[PetService] Зареєстровано тварину: {name} (Власник: {owner.FullName})");
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
