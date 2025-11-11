using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.Core;
using VetClinic.DAL;

namespace VetClinic.BLL;

public class StatisticsService
{
    private readonly VisitService _visitService;

    public StatisticsService(VisitService visitService)
    {
        _visitService = visitService;
    }

    public decimal GetTotalRevenueForDay(DateTime date)
    {
        var allVisits = _visitService.GetAllVisits();

        decimal totalRevenue = allVisits
            .Where(v => v.Status == VisitStatus.Completed)

            .Where(v => v.CompletionTime.HasValue &&
                        v.CompletionTime.Value.Date == date.Date)

            .Sum(v => v.TotalCost);

        return totalRevenue;
    }
    public int GetVisitsCountForPeriod(DateTime start, DateTime end)
    {
        return _visitService.GetAllVisits()
            .Count(v => v.VisitDate >= start && v.VisitDate <= end);
    }

    public Dictionary<int, int> GetVisitsPerCabinet()
    {
        return _visitService.GetAllVisits()
            .GroupBy(v => v.CabinetNumber)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public double GetAverageVisitDurationPerCabinet(int cabinetNumber)
    {
        var visits = _visitService.GetAllVisits()
            .Where(v => v.CabinetNumber == cabinetNumber && v.CompletionTime.HasValue)
            .ToList();

        if (visits.Count == 0) return 0;

        return visits.Average(v => (v.CompletionTime.Value - v.VisitDate).TotalMinutes);
    }

}
