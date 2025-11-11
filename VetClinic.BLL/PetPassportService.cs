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
    // Сервіс для керування Паспортами Тварин
    public class PetPassportService
{
    private const string PassportFileName = "petpassports.json";
    private readonly FileRepository<PetPassport> _passportRepository;
    private List<PetPassport> _passports;
    private readonly IdCounterService _idService;

    private readonly PetService _petService;

    // Конструктор
    public PetPassportService(PetService petService, IdCounterService idService)
    {
        _petService = petService;
        _idService = idService;

        _passportRepository = new FileRepository<PetPassport>(PassportFileName);
        _passports = _passportRepository.ReadAll();
        _RestoreRelationships();
    }

    // Зберегти зміни
    private void _SaveChanges()
    {
        _passportRepository.SaveChanges(_passports);
    }

    // Відновлює зв'язки (Паспорт -> Тварина/Власник)
    private void _RestoreRelationships()
    {
        foreach (var passport in _passports)
        {
            var pet = _petService.GetPetById(passport.PetId);
            if (pet != null)
            {
                passport.Pet = pet;
                passport.Owner = pet.Owner;
            }
        }
    }

    // Знайти паспорт за ID тварини (або створити, якщо немає)
    public PetPassport GetOrCreatePassport(int petId)
    {
        var passport = _passports.FirstOrDefault(p => p.PetId == petId);
        if (passport != null)
        {
            return passport;
        }

        var pet = _petService.GetPetById(petId);
        if (pet == null)
        {
            return null;
        }

        // Створюємо новий паспорт
        var newPassport = new PetPassport
        {
            Id = _idService.GetNextId(nameof(PetPassport)),
            PetId = pet.Id,
            Pet = pet,
            OwnerId = pet.OwnerId,
            Owner = pet.Owner
        };

        _passports.Add(newPassport);
        _SaveChanges();
        return newPassport;
    }

    // Просто знайти паспорт за ID тварини
    public PetPassport GetPassportByPetId(int petId)
    {
        return GetOrCreatePassport(petId);
    }

    // Додати медичний запис у паспорт
    public bool AddMedicalRecord(int petId, string recordType, string description)
    {
        var passport = GetOrCreatePassport(petId);
        if (passport == null)
        {
            return false;
        }

        var newRecord = new MedicalRecord
        {
            Date = DateTime.Now,
            RecordType = recordType,
            Description = description
        };

        passport.MedicalHistory.Add(newRecord);
        _SaveChanges();
        return true;
    }
}
}
