using FoodWastePreventionSystem.BusinessLogic.BusinessLogicModels;
using FoodWastePreventionSystem.Infrastructure;
using FoodWastePreventionSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;


namespace FoodWastePreventionSystem.BusinessLogic
{
    public class SalesLogic : Logic
    {

        public SalesLogic(IRepository<Product> _ProductRepo, IRepository<Batch> _ProductInStoreRepo, IRepository<Transaction> _TransactiontRepo, IRepository<Auction> _AuctionRepo, IRepository<ProductToBeAuctioned> _ProductToBeAuctionedRepo, IRepository<Loss> _LossRepo, IRepository<AuctionTransactionStatus> _AuctionTransactionStatusRepo) :
            base(_ProductRepo, _ProductInStoreRepo, _TransactiontRepo, _AuctionRepo, _ProductToBeAuctionedRepo, _LossRepo, _AuctionTransactionStatusRepo)
        {

        }

        public Transaction[] TransactionForProductByCriteria(Expression<Func<Transaction, bool>> TransactionPredicate = null)
        {
            return TransactionRepo.GetAll().Where(TransactionPredicate).ToArray();
        }


        public Dictionary<TransactionType, int> GetSalesToAuctionInformationForProduct(Guid ProductId)
        {
            Product Product = ProductRepo.Get(x => x.Id == ProductId);
            Transaction[] SalesTransactions = TransactionForProductByCriteria(x => x.TransactionType == TransactionType.Sale);
            Transaction[] AuctionTransactions = TransactionForProductByCriteria(x => x.TransactionType == TransactionType.Auction);

            int SalesQuantity = SalesTransactions.Sum(x => x.Quantity);
            int AuctionQuantity = AuctionTransactions.Sum(x => x.Quantity);

            return new Dictionary<TransactionType, int>()
            {
                {TransactionType.Sale,SalesQuantity },
                {TransactionType.Auction,AuctionQuantity },
            };
        }


        public Dictionary<string, Dictionary<TransactionType, int>> GetSalesToAuctionInformationForProducts(Expression<Func<Product, bool>> ProductPredicate = null)
        {
            Dictionary<string, Dictionary<TransactionType, int>> SalesToAuctionInfoForProducts = new Dictionary<string, Dictionary<TransactionType, int>>();
            Product[] Product = ProductPredicate == null ? ProductRepo.GetAll().ToArray() :
                                                           ProductRepo.GetAll().Where(ProductPredicate).ToArray();
            foreach (var item in Product)
            {
                SalesToAuctionInfoForProducts.Add(item.Name, GetSalesToAuctionInformationForProduct(item.Id));
            }
            return SalesToAuctionInfoForProducts;
        }


        public double GetSaleQuantityForProduct(Guid productId, int year = 0, Month month = Month.None)
        {
            Dictionary<int, Dictionary<Month, double>> TurnoverReport = new Dictionary<int, Dictionary<Month, double>>();
            SalesForProduct SalesRecords = GetSalesForProduct(productId, out TurnoverReport);
            SalesForProductInYear SalesRecordForYear = SalesRecords.SalesForProductInYear.FirstOrDefault(x => x.Year == year);

            return year == 0 ? SalesRecords.SalesForProductInYear.Sum(x => x.SalesPerMonth.Sum(y => y.Value)) : (month == Month.None ? SalesRecordForYear.SalesPerMonth.Sum(x => x.Value) : SalesRecordForYear.SalesPerMonth[month]);

        }

        public TurnoverForProduct GetTurnoverForProduct(Guid id, int year=0, Month month = Month.None)
        {
            Dictionary<int, Dictionary<Month, double>> TurnoverPerYear;
            GetSalesForProduct(id,out TurnoverPerYear);
            TurnoverForProduct TurnoverForProduct = new TurnoverForProduct();
            if (TurnoverPerYear != null)
            {
                foreach (var item in TurnoverPerYear)
                {
                    TurnoverForProduct.TurnoverForProductInYear.Add(new TurnoverForProductInYear()
                    {
                        TurnoverPerMonth = item.Value,
                        Year = item.Key,
                    });

                    if (TurnoverForProduct.StartYear == 0 || TurnoverForProduct.StartYear > item.Key)
                    {
                        TurnoverForProduct.StartYear = item.Key;
                    }

                    if (TurnoverForProduct.EndYear == 0 || TurnoverForProduct.EndYear < item.Key)
                    {
                        TurnoverForProduct.EndYear = item.Key;
                    }
                }
                return TurnoverForProduct;
            }
            else
            {
                return null;
            }

        }


        public List<SalesForProduct> GetSalesForProducts(Expression<Func<Product, bool>> ProductPredicate = null)
        {
            List<SalesForProduct> SaleRecordsForProducts = new List<SalesForProduct>();
            IEnumerable<Product> Products = ProductPredicate == null ? ProductRepo.GetAll() : ProductRepo.GetAll().Where(ProductPredicate);
            Dictionary<int, Dictionary<Month, double>> TurnoverPerYear = new Dictionary<int, Dictionary<Month, double>>();
            foreach (var item in Products)
            {
                SaleRecordsForProducts.Add(GetSalesForProduct(item.Id,out TurnoverPerYear));
            }
            return SaleRecordsForProducts;

        }



        public SalesForProduct GetSalesForProduct(Guid productId, out Dictionary<int, Dictionary<Month, double>> TurnoverPerYear, Expression<Func<Transaction, bool>> TransactionPredicate = null)
        {
           
            Product Product = ProductRepo.Get(x => x.Id == productId);

            SalesForProduct SalesRecords = new SalesForProduct();

            Batch[] ProductInStoreRecords = BatchRepo.GetAll(x => x.ProductId == productId).ToArray();
            Guid[] ProductInStoreIds = ProductInStoreRecords.Select(x => x.Id).ToArray();

            Transaction[] TransactionsForProduct = TransactionPredicate == null ? TransactionRepo.GetAll(x => x.Batch.ProductId == productId).OrderBy(x => x.DateOfTransaction).ToArray() : TransactionRepo.GetAll(x => x.Batch.ProductId == productId).Where(TransactionPredicate).OrderBy(x => x.DateOfTransaction).ToArray();

            Dictionary<int, Dictionary<Month, double>> SalesPerYear = new Dictionary<int, Dictionary<Month, double>>();
            SalesPerYear = GroupSalesPerYear(TransactionsForProduct,out TurnoverPerYear);

            if (SalesPerYear != null)
            {
                foreach (var item in SalesPerYear)
                {
                    int Year = item.Key;
                    Dictionary<Month, double> SalesPerMonth = item.Value;
                    SalesRecords.SalesForProductInYear.Add(new SalesForProductInYear() { Year = Year, SalesPerMonth = SalesPerMonth });
                }
                SalesRecords.Product = Product;

                return SalesRecords;
            }

            return null;
        }


        public Dictionary<int, Dictionary<Month, double>> GroupSalesPerYear(Transaction[] tansactions, out Dictionary<int,Dictionary<Month,double>> turnoverReport)
        {
            Dictionary<int, Dictionary<Month, double>> SalesPerYear = new Dictionary<int, Dictionary<Month, double>>();
            turnoverReport = new Dictionary<int, Dictionary<Month, double>>();
            if (tansactions.Length > 0)
            {               
                DateTime StartDate = tansactions.FirstOrDefault().DateOfTransaction;
                DateTime EndDate = tansactions.LastOrDefault().DateOfTransaction;
                int StartYear = StartDate.Year;
                int EndYear = EndDate.Year;
                int YearSpan = EndYear - StartYear;

                for (int i = StartYear; i <= EndYear; ++i)
                {
                    Transaction[] TransactionForYear = tansactions.Where(x => x.DateOfTransaction.Year == i).ToArray();
                    Dictionary<Month, double> SalesPerMonth = new Dictionary<Month, double>();
                    Dictionary<Month, double> TurnoverPerMonth = new Dictionary<Month, double>();


                    foreach (var item in TransactionForYear)
                    {
                        Dictionary<string,double> Report = ProcessQuantityAndTurnoverFromTransaction(item);
                        double Quantity = Report["quantity"];
                        double Turnover = Report["turnover"];
                        switch (item.DateOfTransaction.Month)
                        {
                            case 1:
                                if (SalesPerMonth.ContainsKey(Month.January))
                                {
                                    SalesPerMonth[Month.January] += Quantity;
                                    TurnoverPerMonth[Month.January] += Turnover;
                                }
                                else
                                {
                                    SalesPerMonth.Add(Month.January, Quantity);
                                    TurnoverPerMonth.Add(Month.January ,Turnover);
                                }
                                break;
                            case 2:
                                if (SalesPerMonth.ContainsKey(Month.February))
                                {
                                    SalesPerMonth[Month.February] += Quantity;
                                    TurnoverPerMonth[Month.February] += Turnover;

                                }
                                else
                                {
                                    SalesPerMonth.Add(Month.February, Quantity);
                                    TurnoverPerMonth.Add(Month.February, Turnover);

                                }
                                break;
                            case 3:
                                if (SalesPerMonth.ContainsKey(Month.March))
                                {
                                    SalesPerMonth[Month.March] += Quantity;
                                    TurnoverPerMonth[Month.March] += Turnover;

                                }
                                else
                                {
                                    SalesPerMonth.Add(Month.March, Quantity);
                                    TurnoverPerMonth.Add(Month.March, Turnover);

                                }
                                break;
                            case 4:
                                if (SalesPerMonth.ContainsKey(Month.April))
                                {
                                    SalesPerMonth[Month.April] += Quantity;
                                    TurnoverPerMonth[Month.April] += Turnover;

                                }
                                else
                                {
                                    SalesPerMonth.Add(Month.April, Quantity);
                                    TurnoverPerMonth.Add(Month.April, Turnover);

                                }
                                break;
                            case 5:
                                if (SalesPerMonth.ContainsKey(Month.May))
                                {
                                    SalesPerMonth[Month.May] += Quantity;
                                    TurnoverPerMonth[Month.May] += Turnover;

                                }
                                else
                                {
                                    SalesPerMonth.Add(Month.May, Quantity);
                                    TurnoverPerMonth.Add(Month.May, Turnover);

                                }
                                break;
                            case 6:
                                if (SalesPerMonth.ContainsKey(Month.June))
                                {
                                    SalesPerMonth[Month.June] += Quantity;
                                    TurnoverPerMonth[Month.June] += Turnover;

                                }
                                else
                                {
                                    SalesPerMonth.Add(Month.June, Quantity);
                                    TurnoverPerMonth.Add(Month.June, Turnover);

                                }
                                break;
                            case 7:
                                if (SalesPerMonth.ContainsKey(Month.July))
                                {
                                    SalesPerMonth[Month.July] += Quantity;
                                    TurnoverPerMonth[Month.July] += Turnover;

                                }
                                else
                                {
                                    SalesPerMonth.Add(Month.July, Quantity);
                                    TurnoverPerMonth.Add(Month.July, Turnover);

                                }
                                break;
                            case 8:
                                if (SalesPerMonth.ContainsKey(Month.August))
                                {
                                    SalesPerMonth[Month.August] += Quantity;
                                    TurnoverPerMonth[Month.August] += Turnover;

                                }
                                else
                                {
                                    SalesPerMonth.Add(Month.August, Quantity);
                                    TurnoverPerMonth.Add(Month.August, Turnover);

                                }
                                break;
                            case 9:
                                if (SalesPerMonth.ContainsKey(Month.September))
                                {
                                    SalesPerMonth[Month.September] += Quantity;
                                    TurnoverPerMonth[Month.September] += Turnover;

                                }
                                else
                                {
                                    SalesPerMonth.Add(Month.September, Quantity);
                                    TurnoverPerMonth.Add(Month.September, Turnover);

                                }
                                break;
                            case 10:
                                if (SalesPerMonth.ContainsKey(Month.October))
                                {
                                    SalesPerMonth[Month.October] += Quantity;
                                    TurnoverPerMonth[Month.October] += Turnover;

                                }
                                else
                                {
                                    SalesPerMonth.Add(Month.October, Quantity);
                                    TurnoverPerMonth.Add(Month.October, Turnover);

                                }
                                break;
                            case 11:
                                if (SalesPerMonth.ContainsKey(Month.November))
                                {
                                    SalesPerMonth[Month.November] += Quantity;
                                    TurnoverPerMonth[Month.November] += Turnover;

                                }
                                else
                                {
                                    SalesPerMonth.Add(Month.November, Quantity);
                                    TurnoverPerMonth.Add(Month.November, Turnover);

                                }
                                break;
                            case 12:
                                if (SalesPerMonth.ContainsKey(Month.December))
                                {
                                    SalesPerMonth[Month.December] += Quantity;
                                    TurnoverPerMonth[Month.December] += Turnover;

                                }
                                else
                                {
                                    SalesPerMonth.Add(Month.December, Quantity);
                                    TurnoverPerMonth.Add(Month.December, Turnover);

                                }
                                break;
                        }
                    }

                    SalesPerYear.Add(i, SalesPerMonth);
                    turnoverReport.Add(i, TurnoverPerMonth);

                }



                return SalesPerYear;
            }

            return null;
        }

        public Dictionary<string,double> ProcessQuantityAndTurnoverFromTransaction(Transaction transaction)
        {
            Dictionary<string, double> Report = new Dictionary<string, double>();
            Report.Add("quantity", transaction.Quantity);
            Report.Add("turnover", transaction.TotalCost);
            return Report;
        }

        public List<SalesAuctionViewModel> GetSalesToAuctionRatio(Guid id)
        {
            List<SalesAuctionViewModel> TotalRatioRecords = new List<SalesAuctionViewModel>();
            Dictionary<int, Dictionary<Month, double>> TurnoverReport = new Dictionary<int, Dictionary<Month, double>>();


            SalesForProduct SalesRecord = GetSalesForProduct(id,out TurnoverReport,x => x.TransactionType == TransactionType.Sale);
            SalesForProduct AuctionRecords = GetSalesForProduct(id,out TurnoverReport, x => x.TransactionType == TransactionType.Auction);

            if (SalesRecord != null && AuctionRecords != null)
            {
                int StartYear = SalesRecord.StartYear < AuctionRecords.StartYear ? SalesRecord.StartYear : AuctionRecords.StartYear;
                int EndYear = SalesRecord.EndYear > AuctionRecords.EndYear ? SalesRecord.EndYear : AuctionRecords.EndYear;

                for (int i = StartYear; i <= EndYear; i++)
                {
                    SalesAuctionViewModel YearlyRatioRecord = new SalesAuctionViewModel()
                    {
                        Year = i,
                    };

                    for (Month j = Month.January; j <= Month.December; ++j)
                    {

                        switch (j)
                        {
                            case Month.January:
                                CompileMonthlySalesAuctionRatio(ref YearlyRatioRecord, SalesRecord, AuctionRecords, Month.January);
                                break;

                            case Month.February:
                                CompileMonthlySalesAuctionRatio(ref YearlyRatioRecord, SalesRecord, AuctionRecords, Month.February);

                                break;

                            case Month.March:
                                CompileMonthlySalesAuctionRatio(ref YearlyRatioRecord, SalesRecord, AuctionRecords, Month.March);
                                break;

                            case Month.April:
                                CompileMonthlySalesAuctionRatio(ref YearlyRatioRecord, SalesRecord, AuctionRecords, Month.April);
                                break;

                            case Month.May:
                                CompileMonthlySalesAuctionRatio(ref YearlyRatioRecord, SalesRecord, AuctionRecords, Month.May);
                                break;

                            case Month.June:
                                CompileMonthlySalesAuctionRatio(ref YearlyRatioRecord, SalesRecord, AuctionRecords, Month.June);
                                break;

                            case Month.July:
                                CompileMonthlySalesAuctionRatio(ref YearlyRatioRecord, SalesRecord, AuctionRecords, Month.July);
                                break;

                            case Month.August:
                                CompileMonthlySalesAuctionRatio(ref YearlyRatioRecord, SalesRecord, AuctionRecords, Month.August);
                                break;

                            case Month.September:
                                CompileMonthlySalesAuctionRatio(ref YearlyRatioRecord, SalesRecord, AuctionRecords, Month.September);
                                break;

                            case Month.October:
                                CompileMonthlySalesAuctionRatio(ref YearlyRatioRecord, SalesRecord, AuctionRecords, Month.October);
                                break;

                            case Month.November:
                                CompileMonthlySalesAuctionRatio(ref YearlyRatioRecord, SalesRecord, AuctionRecords, Month.November);
                                break;

                            case Month.December:
                                CompileMonthlySalesAuctionRatio(ref YearlyRatioRecord, SalesRecord, AuctionRecords, Month.December);
                                break;


                        }
                    }
                    TotalRatioRecords.Add(YearlyRatioRecord);

                }
            }

            return TotalRatioRecords.Count > 0 ? TotalRatioRecords : null;
        }

        public void CompileMonthlySalesAuctionRatio(ref SalesAuctionViewModel monthlyRatioRecord, SalesForProduct salesRecords, SalesForProduct auctionRecords, Month month)
        {
            int Year = monthlyRatioRecord.Year;
            double SalesQuantity = salesRecords.SalesForProductInYear.FirstOrDefault(x => x.Year == Year).SalesPerMonth[month];
            double AuctionQuantity = auctionRecords.SalesForProductInYear.FirstOrDefault(x => x.Year == Year).SalesPerMonth[month];

            monthlyRatioRecord.Record.Add(new SalesAuctionMonthlyViewModel()
            {
                Month = Month.February,
                AuctionQuantity = AuctionQuantity,
                SalesQuantity = SalesQuantity,
            });
        }

        //MessedUp Code...Logic shouldnt have write actions to database
        public async Task<Transaction> AddSalesTransactionAsync(Transaction model, string agentId)
        {
            Transaction TransactionRecord = model;
            Batch Batch = BatchRepo.Get(c => c.Id == model.BatchId);
            Product Product = Batch.Product;
            if (TransactionRecord.BulkPurchase)
            {
                TransactionRecord.Quantity *= Product.QuantityPerCarton;
                TransactionRecord.TotalCost = TransactionRecord.Quantity * Batch.SellingPrice;
            }
            else
            {
                TransactionRecord.TotalCost = TransactionRecord.Quantity * Batch.SellingPrice * (Product.BulkPurchaseDiscountPercent / 100);
            }
            TransactionRecord.AgentId = agentId;
            TransactionRecord.TransactionType = TransactionType.Sale;
            Batch.QuantitySold += TransactionRecord.Quantity;

            await BatchRepo.UpdateAsync(Batch);
            return await TransactionRepo.AddAsync(TransactionRecord);

        }

    }
}