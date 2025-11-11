using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VetClinic.BLL; 
using VetClinic.Core; 

namespace VetClinic.ConsoleApp
{
    internal class Program
    {
        // Ініціалізація всіх сервісів
        private static IdCounterService _idService = new IdCounterService();
        private static ProcedureService _procedureService = new ProcedureService(_idService);
        private static OwnerService _ownerService = new OwnerService(_idService);
        private static VeterinarianService _veterinarianService = new VeterinarianService(_idService);
        private static PetService _petService = new PetService(_ownerService, _idService);
        private static PetPassportService _petPassportService = new PetPassportService(_petService, _idService);
        private static VisitService _visitService = new VisitService(
            _petService, 
            _procedureService, 
            _veterinarianService
        );
        private static StatisticsService _statisticsService = new StatisticsService(_visitService);

        // Головна точка входу
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            // ---------------------------------
            
            RunMainMenu();
        }

        // Головний цикл програми
        private static void RunMainMenu()
        {
            bool isRunning = true;
            while (isRunning)
            {
                Console.Clear();
                Console.WriteLine("--- Система Управління Ветеринарною Клінікою ---");
                Console.WriteLine("1. Реєстрація (Клієнти та Тварини)");
                Console.WriteLine("2. Візити (Створення та Керування)");
                Console.WriteLine("3. Довідники (Послуги та Лікарі)");
                Console.WriteLine("4. Паспорти Тварин");
                Console.WriteLine("5. Статистика");
                Console.WriteLine("0. Вихід");
                Console.Write("\nВаш вибір: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": RunRegistrationMenu(); break;
                    case "2": RunVisitsMenu(); break;
                    case "3": RunDirectoryMenu(); break;
                    case "4": RunPassportsMenu(); break;
                    case "5": RunStatisticsMenu(); break;
                    case "0": isRunning = false; break;
                    default: ShowError("Невірний вибір. Спробуйте ще раз."); Pause(); break;
                }
            }
        }

        #region Реєстрація (Власники + Тварини)

        // Меню реєстрації з циклом
        private static void RunRegistrationMenu()
        {
            bool isRunning = true;
            while (isRunning)
            {
                Console.Clear();
                Console.WriteLine("--- 1. Реєстрація ---");
                Console.WriteLine("1. Додати нового Власника");
                Console.WriteLine("2. Додати нову Тварину");
                Console.WriteLine("3. Переглянути всіх Власників та їх Тварин");
                Console.WriteLine("0. Повернутись");
                Console.Write("\nВаш вибір: ");
                
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": AddNewOwner(); Pause(); break;
                    case "2": AddNewPet(); Pause(); break;
                    case "3": ListAllOwnersAndPets(); Pause(); break;
                    case "0": isRunning = false; break; 
                    default: ShowError("Невірний вибір."); Pause(); break;
                }
            }
        }

        // Додати власника
        private static void AddNewOwner()
        {
            string name = ReadString("Введіть ПІБ власника:");
            string phone = ReadString("Введіть телефон:");
            
            var owner = _ownerService.RegisterOwner(name, phone);
            if (owner != null)
                ShowSuccess($"Власника '{owner.FullName}' (ID: {owner.Id}) успішно додано.");
            else
                ShowError("Не вдалося додати власника. Перевірте дані.");
        }

        // Додати тварину
        private static void AddNewPet()
        {
            var owner = SelectOwner();
            if (owner == null) return; 

            string name = ReadString("Кличка тварини:");
            string species = ReadString("Вид (напр. 'Кіт'):");
            string breed = ReadString("Порода:");
            int age = ReadInt("Вік (років):", 0, 50);

            var pet = _petService.RegisterPet(name, species, breed, age, owner.Id);
            if (pet != null)
                ShowSuccess($"Тварину '{pet.Name}' (ID: {pet.Id}) додано власнику {owner.FullName}.");
            else
                ShowError("Не вдалося додати тварину.");
        }
        
        // Показати всіх
        private static void ListAllOwnersAndPets()
        {
            var owners = _ownerService.GetAllOwners();
            if (owners.Count == 0)
            {
                ShowMessage("Жодного власника ще не зареєстровано.");
                return;
            }
            
            foreach (var owner in owners)
            {
                Console.WriteLine($"\n[ID: {owner.Id}] {owner.FullName} - {owner.ContactPhone}");
                var pets = _petService.GetPetsByOwnerId(owner.Id);
                if (pets.Count > 0)
                {
                    foreach (var pet in pets)
                    {
                        Console.WriteLine($"    -> [PetID: {pet.Id}] {pet.Name} ({pet.Species}, {pet.Age} р.)");
                    }
                }
                else
                {
                    Console.WriteLine("    (Тварин не зареєстровано)");
                }
            }
        }


        #endregion

        #region Візити

        // Меню візитів з циклом
        private static void RunVisitsMenu()
        {
            bool isRunning = true;
            while(isRunning)
            {
                Console.Clear();
                Console.WriteLine("--- 2. Візити ---");
                Console.WriteLine("1. Зареєструвати Новий Візит");
                Console.WriteLine("2. Переглянути Всі Візити");
                Console.WriteLine("3. Оновити Статус Візиту");
                Console.WriteLine("4. Закрити Візит (Розрахувати)");
                Console.WriteLine("0. Повернутись");
                Console.Write("\nВаш вибір: ");
                
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": RegisterNewVisit(); Pause(); break;
                    case "2": ListAllVisits(); Pause(); break;
                    case "3": UpdateVisitStatus(); Pause(); break;
                    case "4": CloseVisit(); Pause(); break;
                    case "0": isRunning = false; break;
                    default: ShowError("Невірний вибір."); Pause(); break;
                }
            }
        }
        
        // Реєстрація візиту
        private static void RegisterNewVisit()
        {
            var pet = SelectPet();
            if (pet == null) return;
            
            var vet = SelectVeterinarian();
            if (vet == null) return;
            
            var procedures = SelectProcedures();
            if (procedures.Count == 0)
            {
                return;
            }
            
            int cabinet = ReadInt("Номер кабінету:", 1, 999);

            var visit = _visitService.RegisterVisit(pet.Id, procedures.Select(p => p.Id).ToList(), cabinet, vet.Id);
            
            if (visit != null)
            {
                ShowSuccess($"Візит {visit.Id} успішно зареєстровано!");
                Console.WriteLine($"Тварина: {pet.Name}, Лікар: {vet.FullName}, Кабінет: {cabinet}");
                Console.WriteLine("Обрані процедури:");
                foreach(var p in procedures) Console.WriteLine($"  - {p.Name} ({p.Price} грн)");
            }
            else
                ShowError("Не вдалося зареєструвати візит. Можливо, обрано заблоковані процедури або невірні дані.");
        }
        
        // Перегляд візитів
        private static void ListAllVisits()
        {
            var visits = _visitService.GetAllVisits();
            if(visits.Count == 0)
            {
                ShowMessage("Ще не було жодного візиту.");
                return;
            }

            foreach (var v in visits.OrderByDescending(x => x.VisitDate))
            {
                PrintVisit(v);
            }
        }
        
        // Оновлення статусу
        private static void UpdateVisitStatus()
        {
            var visit = SelectVisit(v => v.Status != VisitStatus.Completed && v.Status != VisitStatus.Cancelled);
            if (visit == null) return;

            Console.WriteLine($"Поточний статус: {visit.Status}");
            Console.WriteLine("Оберіть новий статус:");
            Console.WriteLine("1. В процесі");
            Console.WriteLine("2. Скасовано");
            string status = "";
            string choice = Console.ReadLine();
            switch(choice)
            {
                case "1": status = VisitStatus.InProgress; break;
                case "2": status = VisitStatus.Cancelled; break;
                default: ShowError("Невірний вибір."); return;
            }

            if (_visitService.UpdateVisitStatus(visit.Id, status))
                ShowSuccess($"Статус візиту {visit.Id} оновлено на '{status}'.");
            else
                ShowError("Не вдалося оновити статус.");
        }
        
        // Закриття візиту
        private static void CloseVisit()
        {
            var visit = SelectVisit(v => v.Status == VisitStatus.InProgress || v.Status == VisitStatus.Registered);
            if (visit == null) return;

            if (_visitService.CloseVisit(visit.Id))
            {
                var closedVisit = _visitService.GetVisitById(visit.Id);
                ShowSuccess($"Візит {visit.Id} успішно закрито!");
                ShowSuccess($"Загальна вартість до сплати: {closedVisit.TotalCost} грн.");
            }
            else
                ShowError("Не вдалося закрити візит.");
        }


        #endregion

        #region Довідники (Послуги + Лікарі)

        // Меню довідників
        private static void RunDirectoryMenu()
        {
            Console.Clear();
            Console.WriteLine("--- 3. Довідники ---");
            Console.WriteLine("1. Керування Послугами");
            Console.WriteLine("2. Керування Лікарями");
            Console.WriteLine("0. Повернутись");
            Console.Write("\nВаш вибір: ");
            
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1": RunProceduresMenu(); break; 
                case "2": RunVeterinariansMenu(); break; 
                case "0": return;
                default: ShowError("Невірний вибір."); Pause(); break;
            }
        }

        // Меню Послуг з циклом
        private static void RunProceduresMenu()
        {
            bool isRunning = true;
            while (isRunning)
            {
                Console.Clear();
                Console.WriteLine("--- 3.1 Керування Послугами ---");
                Console.WriteLine("1. Додати нову послугу");
                Console.WriteLine("2. Переглянути всі послуги");
                Console.WriteLine("3. Редагувати послугу");
                Console.WriteLine("4. Заблокувати/Розблокувати послугу");
                Console.WriteLine("5. Видалити послугу");
                Console.WriteLine("0. Повернутись");
                Console.Write("\nВаш вибір: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": AddNewProcedure(); Pause(); break;
                    case "2": ListAllProcedures(); Pause(); break;
                    case "3": UpdateProcedure(); Pause(); break;
                    case "4": BlockProcedure(); Pause(); break;
                    case "5": DeleteProcedure(); Pause(); break;
                    case "0": isRunning = false; break;
                    default: ShowError("Невірний вибір."); Pause(); break;
                }
            }
        }
        
        // Додавання послуги
        private static void AddNewProcedure()
        {
            string name = ReadString("Назва послуги:");
            decimal price = ReadDecimal("Ціна для клієнта (грн):", 1);
            decimal costPrice = ReadDecimal("Собівартість (грн):", 0, price);
            string tagsRaw = ReadString("Теги (через кому, напр. 'хірургія, діагностика'):");
            var tags = tagsRaw.Split(',').Select(t => t.Trim()).ToList();

            var proc = _procedureService.AddProcedure(name, price, costPrice, tags);
            if(proc != null)
                ShowSuccess($"Послугу '{proc.Name}' додано.");
            else
                ShowError("Не вдалося додати послугу.");
        }
        
        // Перегляд послуг
        private static void ListAllProcedures()
        {
            var procs = _procedureService.GetAllProcedures();
            if(procs.Count == 0)
            {
                ShowMessage("Жодної послуги ще не додано.");
                return;
            }
            foreach(var p in procs) PrintProcedure(p);
        }
        
        // Редагування
        private static void UpdateProcedure()
        {
            var proc = SelectProcedure();
            if (proc == null) return;

            ShowMessage("Введіть нові дані (або натисніть Enter, щоб залишити старі):");
            
            string name = ReadString($"Назва ({proc.Name}):", true);
            decimal price = ReadDecimal($"Ціна ({proc.Price}):", 0, 999999, true);
            decimal costPrice = ReadDecimal($"Собівартість ({proc.CostPrice}):", 0, 999999, true);
            string tagsRaw = ReadString($"Теги ({string.Join(", ", proc.Tags)}):", true);

            string newName = string.IsNullOrWhiteSpace(name) ? proc.Name : name;
            decimal newPrice = price == -1 ? proc.Price : price;
            decimal newCostPrice = costPrice == -1 ? proc.CostPrice : costPrice;
            var newTags = string.IsNullOrWhiteSpace(tagsRaw) ? proc.Tags : tagsRaw.Split(',').Select(t => t.Trim()).ToList();

            if(_procedureService.UpdateProcedure(proc.Id, newName, newPrice, newCostPrice, newTags, proc.IsBlocked))
                ShowSuccess("Послугу оновлено.");
            else
                ShowError("Не вдалося оновити послугу.");
        }
        
        // Блокування
        private static void BlockProcedure()
        {
            var proc = SelectProcedure();
            if (proc == null) return;
            
            bool newStatus = !proc.IsBlocked;
            if(_procedureService.BlockProcedure(proc.Id, newStatus))
                ShowSuccess($"Послугу '{proc.Name}' тепер {(newStatus ? "ЗАБЛОКОВАНО" : "РОЗБЛОКОВАНО")}.");
            else
                ShowError("Не вдалося змінити статус.");
        }
        
        // Видалення
        private static void DeleteProcedure()
        {
            var proc = SelectProcedure();
            if (proc == null) return;

            if (Confirm($"Ви впевнені, що хочете видалити '{proc.Name}'?"))
            {
                if(_procedureService.DeleteProcedure(proc.Id))
                    ShowSuccess("Послугу видалено.");
                else
                    ShowError("Не вдалося видалити послугу.");
            }
        }

        // Меню Лікарів з циклом
        private static void RunVeterinariansMenu()
        {
            bool isRunning = true;
            while(isRunning)
            {
                Console.Clear();
                Console.WriteLine("--- 3.2 Керування Лікарями ---");
                Console.WriteLine("1. Додати нового лікаря");
                Console.WriteLine("2. Переглянути всіх лікарів");
                Console.WriteLine("0. Повернутись");
                Console.Write("\nВаш вибір: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": AddNewVeterinarian(); Pause(); break;
                    case "2": ListAllVeterinarians(); Pause(); break;
                    case "0": isRunning = false; break;
                    default: ShowError("Невірний вибір."); Pause(); break;
                }
            }
        }
        
        // Додати лікаря
        private static void AddNewVeterinarian()
        {
            string name = ReadString("ПІБ лікаря:");
            string spec = ReadString("Спеціалізація:");
            
            var vet = _veterinarianService.AddVeterinarian(name, spec);
            if(vet != null)
                ShowSuccess($"Лікаря '{vet.FullName}' (ID: {vet.Id}) додано.");
            else
                ShowError("Не вдалося додати лікаря.");
        }
        
        // Перегляд лікарів
        private static void ListAllVeterinarians()
        {
            var vets = _veterinarianService.GetAllVeterinarians();
            if(vets.Count == 0)
            {
                ShowMessage("Жодного лікаря не додано.");
                return;
            }
            foreach(var v in vets) Console.WriteLine($"[ID: {v.Id}] {v.FullName} - {v.Specialization}");
        }

        #endregion

        #region Паспорти Тварин

        // Меню паспортів
        private static void RunPassportsMenu()
        {
            Console.Clear();
            Console.WriteLine("--- 4. Паспорти Тварин ---");
            var pet = SelectPet();
            if (pet == null)
            {
                Pause();
                return;
            }

            bool isRunning = true;
            while(isRunning)
            {
                var passport = _petPassportService.GetPassportByPetId(pet.Id);
                
                Console.Clear();
                Console.WriteLine($"--- Паспорт тварини: {pet.Name} (Власник: {pet.Owner.FullName}) ---");
                if (passport.MedicalHistory.Count == 0)
                    Console.WriteLine("\n(Історія хвороби порожня)\n");
                else
                {
                    foreach (var record in passport.MedicalHistory.OrderBy(r => r.Date))
                    {
                        Console.WriteLine($"* [{record.Date:dd.MM.yyyy}] ({record.RecordType}): {record.Description}");
                    }
                }
                
                Console.WriteLine("\n1. Додати запис в паспорт");
                Console.WriteLine("0. Повернутись (обрати іншу тварину)");
                Console.Write("\nВаш вибір: ");
                
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": AddMedicalRecord(pet.Id); Pause(); break;
                    case "0": isRunning = false; break;
                    default: ShowError("Невірний вибір."); Pause(); break;
                }
            }
        }

        // Додати запис
        private static void AddMedicalRecord(int petId)
        {
            string type = ReadString("Тип запису (напр. 'Вакцинація', 'Алергія'):");
            string desc = ReadString("Опис:");
            
            if(_petPassportService.AddMedicalRecord(petId, type, desc))
                ShowSuccess("Запис успішно додано до паспорта.");
            else
                ShowError("Не вдалося додати запис.");
        }

        #endregion

        #region Статистика

        // Меню статистики
        private static void RunStatisticsMenu()
        {
            Console.Clear();
            Console.WriteLine("--- 5. Статистика ---");
            Console.WriteLine("1. Статистика по Лікарях");
            Console.WriteLine("2. Статистика по Процедурах");
            Console.WriteLine("3. Статистика по Кабінетах");
            Console.WriteLine("4. Загальна статистика за період");
            Console.WriteLine("0. Повернутись");
            Console.Write("\nВаш вибір: ");
            
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1": ShowVeterinarianStats(); Pause(); break;
                case "2": ShowProcedureStats(); Pause(); break;
                case "3": ShowCabinetStats(); Pause(); break;
                case "4": ShowPeriodStats(); Pause(); break;
                case "0": return;
                default: ShowError("Невірний вибір."); Pause(); break;
            }
        }

        // Статистика по лікарях
        private static void ShowVeterinarianStats()
        {
            var stats = _statisticsService.GetVeterinarianStatistics();
            if(stats.Count == 0)
            {
                ShowMessage("Немає даних для аналізу.");
                return;
            }

            Console.WriteLine("\n--- Статистика по Лікарях ---");
            foreach (var kvp in stats)
            {
                var vet = _veterinarianService.GetVeterinarianById(kvp.Key);
                string vetName = vet != null ? vet.FullName : "Невідомий Лікар";
                var data = kvp.Value;
                Console.WriteLine($"\nЛікар: {vetName} [ID: {kvp.Key}]");
                Console.WriteLine($"  Кількість візитів: {data.VisitCount}");
                Console.WriteLine($"  Загальна виручка: {data.TotalRevenue} грн");
                Console.WriteLine($"  Середній час прийому: {data.AvgDuration:F2} хв.");
            }
        }
        
        // Статистика по процедурах
        private static void ShowProcedureStats()
        {
            Console.WriteLine("\n--- Статистика по Процедурах (Найчастіші) ---");
            var frequent = _statisticsService.GetMostFrequentProcedures();
            if (frequent.Count == 0) ShowMessage("(немає даних)");
            foreach (var kvp in frequent) Console.WriteLine($"  {kvp.Key}: {kvp.Value} раз(ів)");
            
            Console.WriteLine("\n--- Статистика по Процедурах (Валовий дохід) ---");
            var profitable = _statisticsService.GetMostProfitableProcedures();
            if (profitable.Count == 0) ShowMessage("(немає даних)");
            foreach (var kvp in profitable) Console.WriteLine($"  {kvp.Key}: {kvp.Value} грн");
            
            Console.WriteLine("\n--- Статистика по Процедурах (Чистий прибуток) ---");
            var netProfit = _statisticsService.GetMostNetProfitableProcedures();
            if (netProfit.Count == 0) ShowMessage("(немає даних)");
            foreach (var kvp in netProfit) Console.WriteLine($"  {kvp.Key}: {kvp.Value} грн (чистий прибуток)");
        }
        
        // Статистика по кабінетах
        private static void ShowCabinetStats()
        {
            Console.WriteLine("\n--- Статистика по Кабінетах (Кількість візитів) ---");
            var visitsPer = _statisticsService.GetVisitsPerCabinet();
            if (visitsPer.Count == 0) ShowMessage("(немає даних)");
            foreach (var kvp in visitsPer) Console.WriteLine($"  Кабінет #{kvp.Key}: {kvp.Value} візитів");
            
            Console.WriteLine("\n--- Статистика по Кабінетах (Завантаженість за весь час) ---");
            var workload = _statisticsService.GetCabinetWorkloadStatistics(DateTime.MinValue, DateTime.MaxValue);
            if (workload.Count == 0) ShowMessage("(немає даних)");
            foreach (var kvp in workload) Console.WriteLine($"  Кабінет #{kvp.Key}: Загальний час роботи: {kvp.Value:F2} хв.");
        }
        
        // Загальна статистика
        private static void ShowPeriodStats()
        {
            DateTime start = DateTime.MinValue;
            DateTime end = DateTime.MaxValue;

            decimal revenue = _statisticsService.GetRevenueForPeriod(start, end);
            int visits = _statisticsService.GetVisitsCountForPeriod(start, end);
            
            Console.WriteLine("\n--- Загальна статистика (за весь час) ---");
            Console.WriteLine($"Загальна виручка: {revenue} грн");
            Console.WriteLine($"Загальна кількість візитів: {visits}");
        }

        #endregion

        #region Хелпери (Вибір)

        private static Owner SelectOwner()
        {
            var owners = _ownerService.GetAllOwners();
            if (owners.Count == 0)
            {
                ShowError("Жодного власника не зареєстровано. Спочатку додайте власника.");
                return null;
            }
            
            Console.WriteLine("Оберіть власника:");
            for (int i = 0; i < owners.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {owners[i].FullName} ({owners[i].ContactPhone})");
            }
            
            int index = ReadInt("Ваш вибір:", 1, owners.Count) - 1;
            return owners[index];
        }
        
        private static Pet SelectPet()
        {
            var owner = SelectOwner();
            if (owner == null) return null;

            var pets = _petService.GetPetsByOwnerId(owner.Id);
            if (pets.Count == 0)
            {
                ShowError($"У власника {owner.FullName} немає тварин. Спочатку додайте тварину.");
                return null;
            }
            
            Console.WriteLine("Оберіть тварину:");
            for (int i = 0; i < pets.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {pets[i].Name} ({pets[i].Species})");
            }
            
            int index = ReadInt("Ваш вибір:", 1, pets.Count) - 1;
            return pets[index];
        }
        
        private static Veterinarian SelectVeterinarian()
        {
            var vets = _veterinarianService.GetAllVeterinarians();
            if (vets.Count == 0)
            {
                ShowError("Жодного лікаря не додано. Спочатку додайте лікаря у 'Довідниках'.");
                return null;
            }
            
            Console.WriteLine("Оберіть лікаря:");
            for (int i = 0; i < vets.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {vets[i].FullName} ({vets[i].Specialization})");
            }
            
            int index = ReadInt("Ваш вибір:", 1, vets.Count) - 1;
            return vets[index];
        }

        private static Procedure SelectProcedure()
        {
            var procs = _procedureService.GetAllProcedures();
            if (procs.Count == 0)
            {
                ShowError("Жодної послуги не додано. Спочатку додайте послугу у 'Довідниках'.");
                return null;
            }
            
            Console.WriteLine("Оберіть послугу:");
            for (int i = 0; i < procs.Count; i++)
            {
                PrintProcedure(procs[i], $"{i + 1}. ");
            }
            
            int index = ReadInt("Ваш вибір:", 1, procs.Count) - 1;
            return procs[index];
        }
        
        private static List<Procedure> SelectProcedures()
        {
            var allProcs = _procedureService.GetAllProcedures()
                .Where(p => !p.IsBlocked).ToList(); 
            
            if (allProcs.Count == 0)
            {
                ShowError("Немає доступних послуг для візиту.");
                return new List<Procedure>();
            }
            
            var selectedProcs = new List<Procedure>();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Оберіть послуги (введіть номер і натисніть Enter)");
                Console.WriteLine("Якщо завершили вибір, просто натисніть Enter.");
                
                if (selectedProcs.Count > 0)
                {
                    Console.WriteLine("\nОбрані послуги:");
                    foreach (var p in selectedProcs) Console.WriteLine($" - {p.Name}");
                }
                
                Console.WriteLine("\nДоступні послуги:");
                for (int i = 0; i < allProcs.Count; i++)
                {
                    if (!selectedProcs.Contains(allProcs[i]))
                        PrintProcedure(allProcs[i], $"{i + 1}. ");
                }
                
                Console.Write("\nВаш вибір (або Enter для завершення): ");
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    if (selectedProcs.Count > 0)
                        return selectedProcs;
                    else
                        ShowError("Потрібно обрати хоча б одну послугу.");
                }

                try
                {
                    int index = int.Parse(input) - 1;
                    if (index >= 0 && index < allProcs.Count)
                    {
                        var proc = allProcs[index];
                        if (!selectedProcs.Contains(proc))
                            selectedProcs.Add(proc);
                    }
                    else
                        ShowError("Невірний номер.");
                }
                catch { ShowError("Невірний формат."); }
            }
        }
        
        private static Visit SelectVisit(Func<Visit, bool> filter)
        {
            var visits = _visitService.GetAllVisits().Where(filter)
                .OrderByDescending(v => v.VisitDate).ToList();
            
            if (visits.Count == 0)
            {
                ShowMessage("Немає візитів, що відповідають критерію.");
                return null;
            }
            
            Console.WriteLine("Оберіть візит:");
            for (int i = 0; i < visits.Count; i++)
            {
                Console.WriteLine($"\n--- {i + 1} ---");
                PrintVisit(visits[i]);
            }
            
            int index = ReadInt("Ваш вибір:", 1, visits.Count) - 1;
            return visits[index];
        }


        #endregion

        #region Хелпери (Ввід та Вивід)

        private static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static void ShowSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        
        private static void ShowMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        // Пауза
        private static void Pause()
        {
            Console.WriteLine("\nНатисніть Enter, щоб продовжити...");
            Console.ReadLine(); 
        }
        
        private static void PrintVisit(Visit v)
        {
            Console.WriteLine($"Візит ID: {v.Id}");
            Console.WriteLine($"Дата: {v.VisitDate:g} | Статус: {v.Status}");
            Console.WriteLine($"Тварина: {v.Pet?.Name ?? "Невідомо"} (Власник: {v.Pet?.Owner?.FullName ?? "Невідомо"})");
            Console.WriteLine($"Лікар: {v.Veterinarian?.FullName ?? "Не призначено"} | Кабінет: {v.CabinetNumber}");
            Console.WriteLine("Послуги:");
            foreach (var p in v.Procedures) Console.WriteLine($"  - {p.Name} ({p.Price} грн)");
            if(v.Status == VisitStatus.Completed)
                Console.WriteLine($"Загальна вартість: {v.TotalCost} грн (Завершено: {v.CompletionTime:g})");
        }
        
        private static void PrintProcedure(Procedure proc, string prefix = "")
        {
            string status = proc.IsBlocked ? "[ЗАБЛОКОВАНО]" : "";
            Console.WriteLine($"{prefix}[ID: {proc.Id}] {proc.Name} - {proc.Price} грн (Собіварт: {proc.CostPrice} грн) {status}");
            Console.WriteLine($"      Теги: [{string.Join(", ", proc.Tags)}]");
        }
        
        // Читання рядка
        private static string ReadString(string prompt, bool allowEmpty = false)
        {
            while (true)
            {
                Console.Write(prompt + " ");
                string input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                    return input;
                if (allowEmpty)
                    return string.Empty;
                
                ShowError("Ввід не може бути порожнім.");
            }
        }

        // Читання числа
        private static int ReadInt(string prompt, int min = 0, int max = int.MaxValue)
        {
            while (true)
            {
                Console.Write(prompt + " ");
                string input = Console.ReadLine();
                if (int.TryParse(input, out int result))
                {
                    if (result >= min && result <= max)
                        return result;
                    else
                        ShowError($"Число має бути в діапазоні [{min}...{max}].");
                }
                else
                    ShowError("Це не схоже на число. Спробуйте ще раз.");
            }
        }
        
        // Читання грошей
        private static decimal ReadDecimal(string prompt, decimal min = 0, decimal max = decimal.MaxValue, bool allowEmpty = false)
        {
            while (true)
            {
                Console.Write(prompt + " ");
                string input = Console.ReadLine();
                
                if (allowEmpty && string.IsNullOrWhiteSpace(input))
                    return -1;
                
                if (decimal.TryParse(input, out decimal result))
                {
                    if (result >= min && result <= max)
                        return result;
                    else
                        ShowError($"Число має бути в діапазоні [{min}...{max}].");
                }
                else
                    ShowError("Це не схоже на число (використовуйте ',' для копійок).");
            }
        }
        
        // Підтвердження
        private static bool Confirm(string prompt)
        {
            Console.Write(prompt + " (y/n): ");
            return Console.ReadLine().Trim().ToLower() == "y"; 
        }

        #endregion
    }
}