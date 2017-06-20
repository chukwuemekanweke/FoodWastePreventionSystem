using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace FoodWastePreventionSystem.Areas.Managers.Models
{
    public class ChartsRenderer
    {
        string _3dTheme = "<Chart BackColor=\"#345\" BackGradientStyle=\"TopBottom\" BorderColor=\"181, 64, 1\" BorderWidth=\"2\" BorderlineDashStyle=\"Solid\" Palette=\"SemiTransparent\" AntiAliasing=\"All\">\r\n    <ChartAreas>\r\n        <ChartArea Name=\"Default\" _Template_=\"All\" BackColor=\"blue\" BackSecondaryColor=\"red\" BorderColor=\"64, 24, 24, 64\" BorderDashStyle=\"Solid\" ShadowColor=\"Transparent\">\r\n            <Area3DStyle LightStyle=\"Simplistic\" Enable3D=\"True\" Inclination=\"30\" IsClustered=\"False\" IsRightAngleAxes=\"False\" Perspective=\"10\" Rotation=\"-30\" WallWidth=\"0\" />\r\n        </ChartArea>\r\n    </ChartAreas>\r\n</Chart> ";



        public Chart DrawChart<T>(List<T> data, string title, string chartType)
        {
            Chart chart = new Chart(600, 400, _3dTheme).AddTitle(title);
            ChartPlotter plotter = new ChartPlotter();

            if (typeof(T) == typeof(SalesAuctionYearlyChart) || typeof(T) == typeof(SalesAuctionMonthlyChart))
            {
                plotter = processRatioData(data);
            }
            else if (typeof(T) == typeof(MonthlyProfitChart) || typeof(T) == typeof(YearlyProfitChart))
            {
                plotter = processData(data);
            }
            else if (typeof(T) == typeof(MonthlyLossChart) || typeof(T) == typeof(YearlyLossChart))
            {

            }


            List<string> xValues = plotter.xValues;
            foreach (var item in plotter.multipleSeries)
            {
                chart.AddTitle(title);
                chart.AddLegend();
                chart.AddSeries(name: item.SeriesName, chartType: chartType, xValue: xValues, yValues: item.yValues);
            }




            return chart;
        }

        public ChartPlotter processData<T>(List<T> data)
        {
            ChartPlotter plotter = new ChartPlotter();
            SeriesCreator profit = new SeriesCreator();

            if (typeof(T) == typeof(MonthlyProfitChart))
            {
                var MonthlyData = data as List<MonthlyProfitChart>;
                foreach (var item in MonthlyData)
                {
                    profit.yValues.Add(item.Profit.ToString());

                    if (MonthlyData.First() == item)
                        profit.SeriesName = item.SeriesTitle;
                }
            }
            else if (typeof(T) == typeof(YearlyProfitChart))
            {
                var YearlyData = data as List<YearlyProfitChart>;
                foreach (var item in YearlyData)
                {
                    profit.yValues.Add(item.Profit.ToString());
                    if (YearlyData.First() == item)
                        profit.SeriesName = item.SeriesTitle;
                }

            }




            plotter.multipleSeries.Add(profit);

            return plotter;
        }



        public ChartPlotter processRatioData<T>(List<T> data)
        {

            ChartPlotter plotter = new ChartPlotter();

            SeriesCreator firstRatio = new SeriesCreator();
            SeriesCreator secondRatio = new SeriesCreator();

            if (typeof(T) == typeof(SalesAuctionYearlyChart) || typeof(T) == typeof(SalesAuctionMonthlyChart)){
                if (typeof(T) == typeof(SalesAuctionYearlyChart))
                {
                    var YearlyData = data as List<SalesAuctionYearlyChart>;
                    foreach (var item in YearlyData)
                    {
                        plotter.xValues.Add(item.year.ToString());
                        firstRatio.yValues.Add(item.Auction.ToString());
                        secondRatio.yValues.Add(item.Sales.ToString());
                    }
                }
                else if (typeof(T) == typeof(SalesAuctionMonthlyChart))
                {
                    var MonthlyData = data as List<SalesAuctionMonthlyChart>;
                    foreach (var item in MonthlyData)
                    {
                        plotter.xValues.Add(item.Month.ToString());
                        firstRatio.yValues.Add(item.Auction.ToString());
                        secondRatio.yValues.Add(item.Sales.ToString());
                    }
                }

                firstRatio.SeriesName = "Auction";
                secondRatio.SeriesName = "Sales";
            }

            else if (typeof(T) == typeof(ProfitLossYearlyChart) || typeof(T) == typeof(ProfitLossYearlyChart)){
                if (typeof(T) == typeof(ProfitLossYearlyChart))
                {
                    var YearlyData = data as List<ProfitLossYearlyChart>;
                    foreach (var item in YearlyData)
                    {
                        plotter.xValues.Add(item.year.ToString());
                        firstRatio.yValues.Add(item.Profit.ToString());
                        secondRatio.yValues.Add(item.Loss.ToString());
                    }
                }
                else if (typeof(T) == typeof(ProfitLossMonthlyChart))
                {
                    var MonthlyData = data as List<ProfitLossMonthlyChart>;
                    foreach (var item in MonthlyData)
                    {
                        plotter.xValues.Add(item.Month.ToString());
                        firstRatio.yValues.Add(item.Profit.ToString());
                        secondRatio.yValues.Add(item.Loss.ToString());
                    }
                }
                firstRatio.SeriesName = "Profit";
                secondRatio.SeriesName = "Loss";
            }


            plotter.multipleSeries.Add(firstRatio);
            plotter.multipleSeries.Add(secondRatio);


            return plotter;
        }


    }
}