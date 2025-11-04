using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.BLL;
using VetClinic.Core;

namespace VetClinic.BLL
{
    public class OwnerService
    {
        private static List<Owner> _owners = new List<Owner>();
        private static int _nextOwnerId = 1;

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
            Console.WriteLine($"[OwnerService] Зареєстровано власника: {fullName}");
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
