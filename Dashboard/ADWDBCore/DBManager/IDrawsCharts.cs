using System;
using System.Collections.Generic;
using DBConstants;

namespace DBManager
{
    public interface IDrawsCharts
    {
        List<object> GetDailyChart(DataPoint fields = DataPoint.all);
        
        List <object> GetWeeklyChart(DataPoint fields = DataPoint.all);
        
        List<object> GetPastNDaysChart(int days, DataPoint fields = DataPoint.all);

        List<object> GetYearlyChart(DataPoint fields = DataPoint.all);

        List<object> GetMontlyChart(DataPoint fields = DataPoint.all);
    }
}