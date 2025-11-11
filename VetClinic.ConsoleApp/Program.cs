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

        private static StatisticsService _statisticsService = new StatisticsService(_visitService);


        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("--- Система управління ветеринарною клінікою ---");

            TestProcedureCRUD();

            TestVisitRegistration();

            TestVisitLifecycle();

            TestDailyStatistics();

            Console.WriteLine("\n--- Всі тести завершено. Натисніть Enter для виходу. ---");
            Console.ReadLine();
        }

        private static void TestProcedureCRUD()
        {
            Console.WriteLine("\n--- (Тестування CRUD для Процедур) ---");

            _procedureService.AddProcedure("Первинний осмотр", 150,50);
            _procedureService.AddProcedure("Вакцинація (комплекс)", 350, 250);
            _procedureService.AddProcedure("Чипірування", 500, 350 );
            _procedureService.AddProcedure("Стрижка кігтів", 100, 20);

            Console.WriteLine("\nПоточний список процедур:");
            PrintAllProcedures();

            _procedureService.UpdateProcedure(3, "Чипірування (з реєстрацією)", 550);

            _procedureService.DeleteProcedure(4);

            Console.WriteLine("\nФінальний список процедур (після Update/Delete):");
            PrintAllProcedures();
        }

        private static void TestVisitRegistration()
        {
            Console.WriteLine("\n--- (Тестування Реєстрації Візиту) ---");

            Console.WriteLine("\n1. Реєстрація власників...");
            var owner1 = _ownerService.RegisterOwner("Іван Петров", "050-123-45-67");
            var owner2 = _ownerService.RegisterOwner("Анна Сидоренко", "067-987-65-43");

            Console.WriteLine("\n2. Реєстрація тварин...");
            var pet1 = _petService.RegisterPet("Рекс", "Собака", "Вівчарка", 3, owner1.Id);
            var pet2 = _petService.RegisterPet("Мурзик", "Кіт", "Британський", 5, owner2.Id);

            Console.WriteLine($"\n3. Реєструємо візит для {pet1.Name}...");
            var visit1 = _visitService.RegisterVisit(pet1.Id, new List<int> { 1, 2 });

            if (visit1 != null)
            {
                Console.WriteLine($"Успішно! ID візиту: {visit1.Id}");
                Console.WriteLine($"Власник: {visit1.Pet.Owner.FullName}");
                Console.WriteLine($"Тварина: {visit1.Pet.Name}");
                Console.WriteLine($"Статус: {visit1.Status}");
            }

            Console.WriteLine($"\n4. Реєструємо візит для {pet2.Name}...");
            var visit2 = _visitService.RegisterVisit(pet2.Id, new List<int> { 1 });

            if (visit2 != null)
            {
                Console.WriteLine($"Успішно! ID візиту: {visit2.Id}");
                Console.WriteLine($"Статус: {visit2.Status}");
            }
        }
        private static void TestVisitLifecycle()
        {
            Console.WriteLine("\n--- (Тестування Життєвого Циклу Візиту) ---");

            var visitToTest = _visitService.GetAllVisits()
                                 .FirstOrDefault(v => v.Status == VisitStatus.Registered);

            if (visitToTest == null)
            {
                Console.WriteLine("Не знайдено візитів у статусі 'Зареєстрований' для тесту.");
                Console.WriteLine("Запустіть програму ще раз, щоб створити нові візити.");
                return;
            }

            Guid testVisitId = visitToTest.Id;
            Console.WriteLine($"\nТестуємо візит ID: {testVisitId} (Тварина: {visitToTest.Pet.Name})");

            Console.WriteLine("\n1. Зміна статусу -> 'В процесі'");
            _visitService.UpdateVisitStatus(testVisitId, VisitStatus.InProgress);

            Console.WriteLine("\n2. Закриття візиту...");
            _visitService.CloseVisit(testVisitId);

            var closedVisit = _visitService.GetVisitById(testVisitId);

            Console.WriteLine("\n--- Результат ---");
            Console.WriteLine($"Статус: {closedVisit.Status}");
            Console.WriteLine($"Загальна вартість: {closedVisit.TotalCost} грн");
            Console.WriteLine($"Час завершення: {closedVisit.CompletionTime}");

            Console.WriteLine("\nІсторія статусів:");
            foreach (var history in closedVisit.StatusHistory)
            {
                Console.WriteLine($"  [{history.Timestamp}] - {history.Status}");
            }
        }
        private static void TestDailyStatistics()
        {
            Console.WriteLine("\n--- (Тестування Статистики) ---");

            DateTime today = DateTime.Today;

            decimal revenueToday = _statisticsService.GetTotalRevenueForDay(today);
            Console.WriteLine($"Загальна виручка за сьогодні ({today:dd.MM.yyyy}): {revenueToday} грн");

            DateTime yesterday = DateTime.Today.AddDays(-1);
            decimal revenueYesterday = _statisticsService.GetTotalRevenueForDay(yesterday);
            Console.WriteLine($"Загальна виручка за вчора ({yesterday:dd.MM.yyyy}): {revenueYesterday} грн");
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