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
    // Сервіс для керування Власниками
    public class OwnerService
    {
        private const string OwnerFileName = "owners.json";
        private readonly FileRepository<Owner> _ownerRepository;
        private List<Owner> _owners;
        private readonly IdCounterService _idService;

        // Конструктор
        public OwnerService(IdCounterService idService)
        {
            _idService = idService;
            _ownerRepository = new FileRepository<Owner>(OwnerFileName);
            _owners = _ownerRepository.ReadAll();
        }

        // Зберегти зміни
        private void _SaveChanges()
        {
            _ownerRepository.SaveChanges(_owners);
        }

        // Зареєструвати нового власника
        public Owner RegisterOwner(string fullName, string phone)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(phone))
            {
                return null;
            }

            var newOwner = new Owner
            {
                Id = _idService.GetNextId(nameof(Owner)),
                FullName = fullName,
                ContactPhone = phone
            };

            _owners.Add(newOwner);
            _SaveChanges();
            return newOwner;
        }

        // Знайти власника за ID
        public Owner GetOwnerById(int id)
        {
            return _owners.FirstOrDefault(o => o.Id == id);
        }

        // Отримати всіх власників
        public List<Owner> GetAllOwners()
        {
            return new List<Owner>(_owners);
        }
    }
}
