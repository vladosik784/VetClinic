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
}