using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.Core;
using VetClinic.DAL;

namespace VetClinic.BLL
{
    // Сервіс для розрахунку всієї статистики
    public class StatisticsService
    {
        // Залежність від VisitService
        private readonly VisitService _visitService;

        // Конструктор
        public StatisticsService(VisitService visitService)
        {
            _visitService = visitService;
        }

        // Виручка за конкретний день
        public decimal GetTotalRevenueForDay(DateTime date)
        {
            var allVisits = _visitService.GetAllVisits();
            return allVisits
                .Where(v => v.Status == VisitStatus.Completed)
                .Where(v => v.CompletionTime.HasValue &&
                            v.CompletionTime.Value.Date == date.Date)
                .Sum(v => v.TotalCost);
        }

        // Виручка за довільний період
        public decimal GetRevenueForPeriod(DateTime start, DateTime end)
        {
            DateTime endDate = end;
            if (end.Date != DateTime.MaxValue.Date)
            {
                endDate = end.Date.AddDays(1).AddTicks(-1);
            }
            return _visitService.GetAllVisits()
                .Where(v => v.Status == VisitStatus.Completed && v.CompletionTime.HasValue)
                .Where(v => v.CompletionTime.Value >= start.Date &&
                            v.CompletionTime.Value <= endDate)
                .Sum(v => v.TotalCost);
        }

        // Кількість візитів за період
        public int GetVisitsCountForPeriod(DateTime start, DateTime end)
        {
            DateTime endDate = end;
            if (end.Date != DateTime.MaxValue.Date)
            {
                endDate = end.Date.AddDays(1).AddTicks(-1);
            }
            return _visitService.GetAllVisits()
                .Count(v => v.VisitDate >= start.Date && v.VisitDate <= endDate);
        }

        // Кількість візитів по кабінетах
        public Dictionary<int, int> GetVisitsPerCabinet()
        {
            return _visitService.GetAllVisits()
                .GroupBy(v => v.CabinetNumber)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        // Середній час роботи кабінету
        public double GetAverageVisitDurationPerCabinet(int cabinetNumber)
        {
            var visits = _visitService.GetAllVisits()
                .Where(v => v.CabinetNumber == cabinetNumber)
                .Where(v => v.CompletionTime.HasValue)
                .ToList();

            if (visits.Count == 0) return 0;
            return visits.Average(v => (v.CompletionTime.Value - v.VisitDate).TotalMinutes);
        }

        // Загальна завантаженість кабінетів
        public Dictionary<int, double> GetCabinetWorkloadStatistics(DateTime start, DateTime end)
        {
            DateTime endDate = end;
            if (end.Date != DateTime.MaxValue.Date)
            {
                endDate = end.Date.AddDays(1).AddTicks(-1);
            }
            return _visitService.GetAllVisits()
                .Where(v => v.Status == VisitStatus.Completed && v.CompletionTime.HasValue)
                .Where(v => v.CompletionTime.Value >= start.Date &&
                            v.CompletionTime.Value <= endDate)
                .GroupBy(v => v.CabinetNumber)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(v => (v.CompletionTime.Value - v.VisitDate).TotalMinutes)
                );
        }

        // Статистика по лікарях
        public Dictionary<int, (int VisitCount, decimal TotalRevenue, double AvgDuration)> GetVeterinarianStatistics()
        {
            var allVisits = _visitService.GetAllVisits();

            return allVisits
                .GroupBy(v => v.VeterinarianId)
                .ToDictionary(
                    g => g.Key, 
                    g =>
                    (
                        VisitCount: g.Count(),
                        TotalRevenue: g.Where(v => v.Status == VisitStatus.Completed)
                                       .Sum(v => v.TotalCost),
                        AvgDuration: g.Where(v => v.Status == VisitStatus.Completed && v.CompletionTime.HasValue)
                                      .DefaultIfEmpty()
                                      .Average(v => v == null ? 0 : (v.CompletionTime.Value - v.VisitDate).TotalMinutes)
                    )
                );
        }

        // Найчастіші процедури
        public Dictionary<string, int> GetMostFrequentProcedures()
        {
            return _visitService.GetAllVisits()
                .SelectMany(v => v.Procedures)
                .GroupBy(p => p.Name)
                .ToDictionary(
                    g => g.Key,
                    g => g.Count()
                )
                .OrderByDescending(kvp => kvp.Value)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        // Найприбутковіші процедури (Валовий дохід)
        public Dictionary<string, decimal> GetMostProfitableProcedures()
        {
            return _visitService.GetAllVisits()
                .Where(v => v.Status == VisitStatus.Completed)
                .SelectMany(v => v.Procedures)
                .GroupBy(p => p.Name)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(p => p.Price)
                )
                .OrderByDescending(kvp => kvp.Value)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        // Найприбутковіші процедури (Чистий прибуток)
        public Dictionary<string, decimal> GetMostNetProfitableProcedures()
        {
            return _visitService.GetAllVisits()
                .Where(v => v.Status == VisitStatus.Completed)
                .SelectMany(v => v.Procedures)
                .GroupBy(p => p.Name)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(p => p.Price - p.CostPrice)
                )
                .OrderByDescending(kvp => kvp.Value)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}
