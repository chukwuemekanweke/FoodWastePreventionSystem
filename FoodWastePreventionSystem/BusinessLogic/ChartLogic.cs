using FoodWastePreventionSystem.Areas.Managers.Models;
using FoodWastePreventionSystem.BusinessLogic.BusinessLogicModels;
using FoodWastePreventionSystem.Infrastructure;
using FoodWastePreventionSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace FoodWastePreventionSystem.BusinessLogic
{
    public class ChartLogic : Logic
    {
        private ProfitLogic ProfitLogic;
        private SalesLogic SalesLogic;
        private LossLogic LossLogic;
        ChartsRenderer ChartsRenderer;


        public ChartLogic(IRepository<Product> _ProductRepo, IRepository<Batch> _ProductInStoreRepo, IRepository<Transaction> _TransactiontRepo, IRepository<Auction> _AuctionRepo, IRepository<ProductToBeAuctioned> _ProductToBeAuctionedRepo, IRepository<Loss> _LossRepo, IRepository<AuctionTransactionStatus> _AuctionTransactionStatusRepo) :
            base(_ProductRepo, _ProductInStoreRepo, _TransactiontRepo, _AuctionRepo, _ProductToBeAuctionedRepo, _LossRepo, _AuctionTransactionStatusRepo)
        {
            ProfitLogic = new ProfitLogic(_ProductRepo, _ProductInStoreRepo, _TransactiontRepo, _AuctionRepo, _ProductToBeAuctionedRepo, _LossRepo,_AuctionTransactionStatusRepo);
            SalesLogic = new SalesLogic(_ProductRepo, _ProductInStoreRepo, _TransactiontRepo, _AuctionRepo, _ProductToBeAuctionedRepo, _LossRepo, _AuctionTransactionStatusRepo);
            LossLogic = new LossLogic(_ProductRepo, _ProductInStoreRepo, _TransactiontRepo, _AuctionRepo, _ProductToBeAuctionedRepo, _LossRepo, _AuctionTransactionStatusRepo);
            ChartsRenderer = new ChartsRenderer();
        }

        public Chart ProfitChart(ReqColumnChart chartData)
        {
            int Year = chartData.year;
            Chart Chart = null;
            YearlyProfitChart[] YearlyData;
            MonthlyProfitChart[] MonthlyData;
            ProfitsForProduct ProfitRecords = ProfitLogic.GetProfitForProduct(chartData.productId);

            int i = 0;
            if (Year != 0)
            {
                YearlyData = new YearlyProfitChart[ProfitRecords.ProfitsForProductInYear.Count];
                foreach (var item in ProfitRecords.ProfitsForProductInYear)
                {
                    YearlyData[i] = new YearlyProfitChart()
                    {
                        Year = item.Year,
                        Profit = item.ProfitsPerMonth.Sum(x => x.Value),
                        SeriesTitle = $"Profit For {ProductRepo.Get(x => x.Id == chartData.productId).Name}",
                    };
                    ++i;
                }

                Chart = ChartsRenderer.DrawChart(YearlyData.ToList(), "Profit Chart [YEARLY]", chartData.chartType);
            }
            else
            {
                MonthlyData = new MonthlyProfitChart[12];
                ProfitsForProductInYear ProfitsForProductInYear = ProfitRecords.ProfitsForProductInYear.FirstOrDefault(x => x.Year == Year);
                foreach (var item in ProfitsForProductInYear.ProfitsPerMonth)
                {
                    MonthlyData[i] = new MonthlyProfitChart()
                    {
                        Month = item.Key,
                        Profit = item.Value,
                        SeriesTitle = $"Profit For {ProductRepo.Get(x => x.Id == chartData.productId).Name}",
                    };
                    ++i;
                }

                Chart = ChartsRenderer.DrawChart(MonthlyData.ToList(), $"Profit Chart [FOR {chartData.year}]", chartData.chartType);
            }

            return Chart;

        }

        public Chart LossChart(ReqColumnChart chartData)
        {
            int Year = chartData.year;
            Chart Chart = null;
            YearlyLossChart[] YearlyData;
            MonthlyLossChart[] MonthlyData;
            LossForProduct Lossrecords = LossLogic.GetLossForProduct(chartData.productId);


            int i = 0;
            if (Year != 0)
            {
                YearlyData = new YearlyLossChart[Lossrecords.LossForProductInYear.Count];
                foreach (var item in Lossrecords.LossForProductInYear)
                {
                    YearlyData[i] = new YearlyLossChart()
                    {
                        Year = item.Year,
                        Loss = item.LossPerMonth.Sum(x => x.Value),
                        SeriesTitle = $"Loss For {ProductRepo.Get(x => x.Id == chartData.productId).Name}",
                    };
                    ++i;
                }

                Chart = ChartsRenderer.DrawChart(YearlyData.ToList(), "Loss Chart [YEARLY]", chartData.chartType);
            }
            else
            {
                MonthlyData = new MonthlyLossChart[12];
                LossForProductInYear ProfitsForProductInYear = Lossrecords.LossForProductInYear.FirstOrDefault(x => x.Year == Year);
                foreach (var item in ProfitsForProductInYear.LossPerMonth)
                {
                    MonthlyData[i] = new MonthlyLossChart()
                    {
                        Month = item.Key,
                        Loss = item.Value,
                        SeriesTitle = $"Loss For {ProductRepo.Get(x => x.Id == chartData.productId).Name}",
                    };
                    ++i;
                }
                Chart = ChartsRenderer.DrawChart(MonthlyData.ToList(), $"Loss Chart [FOR {chartData.year}]", chartData.chartType);
            }

            return Chart;
        }

        public Chart SalesAuctionChart(ReqColumnChart chartData)
        {
            Chart Chart = null;
            int year = chartData.year;
            SalesAuctionYearlyChart[] YearlyData;
            SalesAuctionMonthlyChart[] MonthlyData;
            List<SalesAuctionViewModel> SalesAuctionRatioRecords = new List<SalesAuctionViewModel>();
            SalesAuctionViewModel SalesAuctionRecord = new SalesAuctionViewModel();

            int i = 0;

            if (year != 0)
            {
                SalesAuctionRecord = SalesLogic.GetSalesToAuctionRatio(chartData.productId).FirstOrDefault(x => x.Year == year);
                MonthlyData = new SalesAuctionMonthlyChart[12];
                foreach (var item in SalesAuctionRecord.Record)
                {
                    MonthlyData[i] = new SalesAuctionMonthlyChart()
                    {
                        Month = item.Month,
                        Auction = item.AuctionQuantity,
                        Sales = item.SalesQuantity,


                    }; ++i;
                }
                Chart = ChartsRenderer.DrawChart(MonthlyData.ToList(), $"Sales/Auction Chart [FOR {chartData.year}]", chartData.chartType);
            }
            else
            {
                SalesAuctionRatioRecords = SalesLogic.GetSalesToAuctionRatio(chartData.productId);
                YearlyData = new SalesAuctionYearlyChart[SalesAuctionRatioRecords.Count()];
                foreach (var item in SalesAuctionRatioRecords)
                {

                    YearlyData[i] = new SalesAuctionYearlyChart()
                    {
                        year = item.Year,
                        Sales = item.Record.Sum(x => x.SalesQuantity),
                        Auction = item.Record.Sum(x => x.AuctionQuantity),
                    };

                    ++i;
                }
                ChartsRenderer chart = new ChartsRenderer();
                Chart = chart.DrawChart(YearlyData.ToList(), "Sales/Auction Chart [YEARLY]", chartData.chartType);
            }
            return Chart;
        }


        public Chart ProfitLossChart(ReqColumnChart chartData)
        {
            Chart Chart = null;
            int year = chartData.year;
            ProfitLossYearlyChart[] YearlyData;
            ProfitLossMonthlyChart[] MonthlyData;
            List<ProfitLossVM> ProfitLossRatioRecords = new List<ProfitLossVM>();
            ProfitLossVM ProfitLossRecord = new ProfitLossVM();

            int i = 0;

            if (year != 0)
            {
                ProfitLossRecord = ProfitLogic.GetProfitLossRatio(chartData.productId).FirstOrDefault(x => x.Year == year);
                MonthlyData = new ProfitLossMonthlyChart[12];
                foreach (var item in ProfitLossRecord.Records)
                {
                    MonthlyData[i] = new ProfitLossMonthlyChart()
                    {
                        Month = item.Month,
                        Profit = item.Profit,
                        Loss = item.Loss,
                    };
                    ++i;
                }

                Chart = ChartsRenderer.DrawChart(MonthlyData.ToList(), $"Profit/Loss Chart [FOR {chartData.year}]", chartData.chartType);
            }
            else
            {
                ProfitLossRatioRecords = ProfitLogic.GetProfitLossRatio(chartData.productId);
                YearlyData = new ProfitLossYearlyChart[ProfitLossRatioRecords.Count()];
                foreach (var item in ProfitLossRatioRecords)
                {

                    YearlyData[i] = new ProfitLossYearlyChart()
                    {
                        year = item.Year,
                        Profit = item.Records.Sum(x => x.Profit),
                        Loss = item.Records.Sum(x => x.Loss),
                    };

                    ++i;
                }
                Chart = ChartsRenderer.DrawChart(YearlyData.ToList(), "Profit/Loss Chart [YEARLY]", chartData.chartType);
            }
            return Chart;
        }


    }
}