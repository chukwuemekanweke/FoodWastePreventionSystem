using FoodWastePreventionSystem.Models;
using System;
using System.Collections.Generic;


namespace FoodWastePreventionSystem.Areas.Managers.Models
{
    public class SalesAuctionMonthlyChart
    {
        public Month Month { get; set; }
        public double Sales { get; set; }
        public double Auction { get; set; }
    }
    public class SalesAuctionYearlyChart
    {
        public int year { get; set; }
        public double Sales { get; set; }
        public double Auction { get; set; }
    }


    public class ProfitLossMonthlyChart
    {
        public Month Month { get; set; }
        public double Profit { get; set; }
        public double Loss { get; set; }
    }

    public class ProfitLossYearlyChart
    {
        public int year { get; set; }
        public double Profit { get; set; }
        public double Loss { get; set; }
    }

   
    public class MonthlyProfitChart
    {
        public Month Month { get; set; }
        public double Profit { get; set; }
        public string SeriesTitle { get; set; }
    }

    public class YearlyProfitChart
    {
        public int Year { get; set; }
        public double Profit { get; set; }
        public string SeriesTitle { get; set; }

    }

    public class MonthlyLossChart
    {
        public Month Month { get; set; }
        public double Loss { get; set; }
        public string SeriesTitle { get; set; }

    }

    public class YearlyLossChart
    {
        public int Year { get; set; }
        public double Loss { get; set; }
        public string SeriesTitle { get; set; }

    }










    public class ChartPlotter
    {
        public List<string> xValues { get; set; }
        public List<SeriesCreator> multipleSeries { get; set; }

        public ChartPlotter()
        {
            xValues = new List<string>();
            multipleSeries = new List<SeriesCreator>();
        }
    }

    public class SeriesCreator
    {
        public string SeriesName { get; set; }
        public List<string> yValues { get; set; }

        public SeriesCreator()
        {
            yValues = new List<string>();
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //ClientSide Request VM Here
    public class ReqColumnChart
    {
        public Guid productId { get; set; }
        public string chartType { get; set; }
        public int year { get; set; }

        public ReqColumnChart()
        {
            year = 0;
        }
    }





}