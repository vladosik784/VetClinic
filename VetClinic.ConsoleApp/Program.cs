using System;
using System.Collections.Generic;
using VetClinic.BLL;
using VetClinic.Core;

namespace VetClinic.ConsoleApp
{
    internal class Program
    {
        private static ProcedureService _procedureService = new ProcedureService();
        private static OwnerService _ownerService = new OwnerService();

        private static PetService _petService = new PetService(_ownerService);

        private static VisitService _visitService = new VisitService(_petService, _procedureService);


        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("--- Система управління ветеринарною клінікою ---");

            TestProcedureCRUD();

            TestVisitRegistration();

            Console.WriteLine("\n--- Всі тести завершено. Натисніть Enter для виходу. ---");
            Console.ReadLine();
        }

        private static void TestProcedureCRUD()
        {
            Console.WriteLine("\n--- (Етап 1.1: Тестування CRUD для Процедур) ---");

            _procedureService.AddProcedure("Первинний осмотр", 150);
            _procedureService.AddProcedure("Вакцинація (комплекс)", 350);
            _procedureService.AddProcedure("Чипірування", 500);
            _procedureService.AddProcedure("Стрижка кігтів", 100);

            Console.WriteLine("\nПоточний список процедур:");
            PrintAllProcedures();

            _procedureService.UpdateProcedure(3, "Чипірування (з реєстрацією)", 550);

            _procedureService.DeleteProcedure(4);

            Console.WriteLine("\nФінальний список процедур (після Update/Delete):");
            PrintAllProcedures();
        }

        private static void TestVisitRegistration()
        {
            Console.WriteLine("\n--- (Етап 1.2: Тестування Реєстрації Візиту) ---");

            Console.WriteLine("\n1. Реєстрація власників...");
            var owner1 = _ownerService.RegisterOwner("Іван Петров", "050-123-45-67");
            var owner2 = _ownerService.RegisterOwner("Анна Сидоренко", "067-987-65-43");

            Console.WriteLine("\n2. Реєстрація тварин...");
            var pet1 = _petService.RegisterPet("Рекс", "Собака", "Вівчарка", 3, owner1.Id);
            var pet2 = _petService.RegisterPet("Мурзик", "Кіт", "Британський", 5, owner2.Id);

            var procedureIdsForVisit1 = new List<int> { 1, 2 };

            Console.WriteLine($"\n3. Реєструємо візит для {pet1.Name}...");
            var visit1 = _visitService.RegisterVisit(pet1.Id, procedureIdsForVisit1);

            if (visit1 != null)
            {
                Console.WriteLine($"Успішно! ID візиту: {visit1.Id}");
                Console.WriteLine($"Власник: {visit1.Pet.Owner.FullName}");
                Console.WriteLine($"Тварина: {visit1.Pet.Name}");
                Console.WriteLine($"Статус: {visit1.Status}");
                Console.WriteLine("Призначені процедури:");
                foreach (var p in visit1.Procedures)
                {
                    Console.WriteLine($"  - {p.Name} ({p.Price} грн)");
                }
            }

            var procedureIdsForVisit2 = new List<int> { 1 };

            Console.WriteLine($"\n4. Реєструємо візит для {pet2.Name}...");
            var visit2 = _visitService.RegisterVisit(pet2.Id, procedureIdsForVisit2);

            if (visit2 != null)
            {
                Console.WriteLine($"Успішно! ID візиту: {visit2.Id}");
                Console.WriteLine($"Статус: {visit2.Status}");
                Console.WriteLine("Призначені процедури:");
                foreach (var p in visit2.Procedures)
                {
                    Console.WriteLine($"  - {p.Name} ({p.Price} грн)");
                }
            }
        }
        private static void PrintAllProcedures()
        {
            var allProcedures = _procedureService.GetAllProcedures();
            if (allProcedures.Count == 0)
            {
                Console.WriteLine("  (Список процедур порожній)");
                return;
            }

            foreach (var proc in allProcedures)
            {
                Console.WriteLine($"  [ID: {proc.Id}] {proc.Name} - {proc.Price} грн");
            }
        }
    }
}