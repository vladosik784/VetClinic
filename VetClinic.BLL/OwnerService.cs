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
    public class OwnerService
    {
        private const string OwnerFileName = "owners.json";
        private readonly FileRepository<Owner> _ownerRepository;
        private List<Owner> _owners;
        private int _nextOwnerId;

        public OwnerService()
        {
            _ownerRepository = new FileRepository<Owner>(OwnerFileName);
            _owners = _ownerRepository.ReadAll();
            _nextOwnerId = _GetNextId();
        }
        private void _SaveChanges()
        {
            _ownerRepository.SaveChanges(_owners);
        }

        private int _GetNextId()
        {
            if (_owners.Count == 0)
            {
                return 1;
            }
            return _owners.Max(o => o.Id) + 1;
        }

        public Owner RegisterOwner(string fullName, string phone)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(phone))
            {
                Console.WriteLine("[OwnerService] Ім'я та телефон не можуть бути порожніми.");
                return null;
            }

            var newOwner = new Owner
            {
                Id = _nextOwnerId++,
                FullName = fullName,
                ContactPhone = phone
            };

            _owners.Add(newOwner);
            _SaveChanges();

            Console.WriteLine($"[OwnerService] Зареєстровано власника (збережено у файл): {fullName}");
            return newOwner;
        }

        public Owner GetOwnerById(int id)
        {
            return _owners.FirstOrDefault(o => o.Id == id);
        }

        public List<Owner> GetAllOwners()
        {
            return new List<Owner>(_owners);
        }
    }
}
