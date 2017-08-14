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
            TransactionsForProduct SalesRecords = GetTransactionsForProduct(productId, out TurnoverReport);
            TransactionsForProductInYear SalesRecordForYear = SalesRecords.TransactionsForProductInYear.FirstOrDefault(x => x.Year == year);

            return year == 0 ? SalesRecords.TransactionsForProductInYear.Sum(x => x.TransactionsPerMonth.Sum(y => y.Value)) : (month == Month.None ? SalesRecordForYear.TransactionsPerMonth.Sum(x => x.Value) : SalesRecordForYear.TransactionsPerMonth[month]);

        }

        public TurnoverForProduct GetTurnoverForProduct(Guid id, int year = 0, Month month = Month.None)
        {
            Dictionary<int, Dictionary<Month, double>> TurnoverPerYear;
            GetSalesForProduct(id, out TurnoverPerYear);
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

        public TransactionsForProduct GetSalesForProduct(Guid productId, out Dictionary<int, Dictionary<Month, double>> turnoverReport)
        {
            return GetTransactionsForProduct(productId, out turnoverReport, x => x.TransactionType == TransactionType.Sale);
        }

        public List<TransactionsForProduct> GetSalesForProducts(Expression<Func<Product, bool>> ProductPredicate = null)
        {

            return GetTransactionsForProducts(ProductPredicate, x => x.TransactionType == TransactionType.Sale);
        }



        public List<TransactionsForProduct> GetTransactionsForProducts(Expression<Func<Product, bool>> ProductPredicate = null, Expression<Func<Transaction, bool>> transactionPredicate = null)
        {
            List<TransactionsForProduct> SaleRecordsForProducts = new List<TransactionsForProduct>();
            IEnumerable<Product> Products = ProductPredicate == null ? ProductRepo.GetAll() : ProductRepo.GetAll().Where(ProductPredicate);
            Dictionary<int, Dictionary<Month, double>> TurnoverPerYear = new Dictionary<int, Dictionary<Month, double>>();
            foreach (var item in Products)
            {
                SaleRecordsForProducts.Add(GetTransactionsForProduct(item.Id, out TurnoverPerYear, transactionPredicate));
            }
            return SaleRecordsForProducts;

        }



        public TransactionsForProduct GetTransactionsForProduct(Guid productId, out Dictionary<int, Dictionary<Month, double>> TurnoverPerYear, Expression<Func<Transaction, bool>> TransactionPredicate = null)
        {

            Product Product = ProductRepo.Get(x => x.Id == productId);

            TransactionsForProduct TransactionsRecords = new TransactionsForProduct();

            Batch[] ProductInStoreRecords = BatchRepo.GetAll(x => x.ProductId == productId).ToArray();
            Guid[] ProductInStoreIds = ProductInStoreRecords.Select(x => x.Id).ToArray();

            Transaction[] TransactionsForProduct = TransactionPredicate == null ? TransactionRepo.GetAll(x => x.Batch.ProductId == productId).OrderBy(x => x.DateOfTransaction).ToArray() : TransactionRepo.GetAll(x => x.Batch.ProductId == productId).Where(TransactionPredicate).OrderBy(x => x.DateOfTransaction).ToArray();

            Dictionary<int, Dictionary<Month, double>> TransactionsPerYear = new Dictionary<int, Dictionary<Month, double>>();
            TransactionsPerYear = GroupTransactionsPerYear(TransactionsForProduct, out TurnoverPerYear);

            if (TransactionsPerYear != null)
            {
                foreach (var item in TransactionsPerYear)
                {
                    int Year = item.Key;
                    Dictionary<Month, double> TransactionsPerMonth = item.Value;
                    TransactionsRecords.TransactionsForProductInYear.Add(new TransactionsForProductInYear() { Year = Year, TransactionsPerMonth = TransactionsPerMonth });
                }
                TransactionsRecords.Product = Product;

                return TransactionsRecords;
            }

            return null;
        }


        public Dictionary<int, Dictionary<Month, double>> GroupTransactionsPerYear(Transaction[] tansactions, out Dictionary<int, Dictionary<Month, double>> turnoverReport)
        {
            Dictionary<int, Dictionary<Month, double>> TransactionsPerYear = new Dictionary<int, Dictionary<Month, double>>();
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
                    Dictionary<Month, double> TransactionsPerMonth = new Dictionary<Month, double>();
                    Dictionary<Month, double> TurnoverPerMonth = new Dictionary<Month, double>();


                    foreach (var item in TransactionForYear)
                    {
                        Dictionary<string, double> Report = ProcessQuantityAndTurnoverFromTransaction(item);
                        double Quantity = Report["quantity"];
                        double Turnover = Report["turnover"];
                        switch (item.DateOfTransaction.Month)
                        {
                            case 1:
                                if (TransactionsPerMonth.ContainsKey(Month.January))
                                {
                                    TransactionsPerMonth[Month.January] += Quantity;
                                    TurnoverPerMonth[Month.January] += Turnover;
                                }
                                else
                                {
                                    TransactionsPerMonth.Add(Month.January, Quantity);
                                    TurnoverPerMonth.Add(Month.January, Turnover);
                                }
                                break;
                            case 2:
                                if (TransactionsPerMonth.ContainsKey(Month.February))
                                {
                                    TransactionsPerMonth[Month.February] += Quantity;
                                    TurnoverPerMonth[Month.February] += Turnover;

                                }
                                else
                                {
                                    TransactionsPerMonth.Add(Month.February, Quantity);
                                    TurnoverPerMonth.Add(Month.February, Turnover);

                                }
                                break;
                            case 3:
                                if (TransactionsPerMonth.ContainsKey(Month.March))
                                {
                                    TransactionsPerMonth[Month.March] += Quantity;
                                    TurnoverPerMonth[Month.March] += Turnover;

                                }
                                else
                                {
                                    TransactionsPerMonth.Add(Month.March, Quantity);
                                    TurnoverPerMonth.Add(Month.March, Turnover);

                                }
                                break;
                            case 4:
                                if (TransactionsPerMonth.ContainsKey(Month.April))
                                {
                                    TransactionsPerMonth[Month.April] += Quantity;
                                    TurnoverPerMonth[Month.April] += Turnover;

                                }
                                else
                                {
                                    TransactionsPerMonth.Add(Month.April, Quantity);
                                    TurnoverPerMonth.Add(Month.April, Turnover);

                                }
                                break;
                            case 5:
                                if (TransactionsPerMonth.ContainsKey(Month.May))
                                {
                                    TransactionsPerMonth[Month.May] += Quantity;
                                    TurnoverPerMonth[Month.May] += Turnover;

                                }
                                else
                                {
                                    TransactionsPerMonth.Add(Month.May, Quantity);
                                    TurnoverPerMonth.Add(Month.May, Turnover);

                                }
                                break;
                            case 6:
                                if (TransactionsPerMonth.ContainsKey(Month.June))
                                {
                                    TransactionsPerMonth[Month.June] += Quantity;
                                    TurnoverPerMonth[Month.June] += Turnover;

                                }
                                else
                                {
                                    TransactionsPerMonth.Add(Month.June, Quantity);
                                    TurnoverPerMonth.Add(Month.June, Turnover);

                                }
                                break;
                            case 7:
                                if (TransactionsPerMonth.ContainsKey(Month.July))
                                {
                                    TransactionsPerMonth[Month.July] += Quantity;
                                    TurnoverPerMonth[Month.July] += Turnover;

                                }
                                else
                                {
                                    TransactionsPerMonth.Add(Month.July, Quantity);
                                    TurnoverPerMonth.Add(Month.July, Turnover);

                                }
                                break;
                            case 8:
                                if (TransactionsPerMonth.ContainsKey(Month.August))
                                {
                                    TransactionsPerMonth[Month.August] += Quantity;
                                    TurnoverPerMonth[Month.August] += Turnover;

                                }
                                else
                                {
                                    TransactionsPerMonth.Add(Month.August, Quantity);
                                    TurnoverPerMonth.Add(Month.August, Turnover);

                                }
                                break;
                            case 9:
                                if (TransactionsPerMonth.ContainsKey(Month.September))
                                {
                                    TransactionsPerMonth[Month.September] += Quantity;
                                    TurnoverPerMonth[Month.September] += Turnover;

                                }
                                else
                                {
                                    TransactionsPerMonth.Add(Month.September, Quantity);
                                    TurnoverPerMonth.Add(Month.September, Turnover);

                                }
                                break;
                            case 10:
                                if (TransactionsPerMonth.ContainsKey(Month.October))
                                {
                                    TransactionsPerMonth[Month.October] += Quantity;
                                    TurnoverPerMonth[Month.October] += Turnover;

                                }
                                else
                                {
                                    TransactionsPerMonth.Add(Month.October, Quantity);
                                    TurnoverPerMonth.Add(Month.October, Turnover);

                                }
                                break;
                            case 11:
                                if (TransactionsPerMonth.ContainsKey(Month.November))
                                {
                                    TransactionsPerMonth[Month.November] += Quantity;
                                    TurnoverPerMonth[Month.November] += Turnover;

                                }
                                else
                                {
                                    TransactionsPerMonth.Add(Month.November, Quantity);
                                    TurnoverPerMonth.Add(Month.November, Turnover);

                                }
                                break;
                            case 12:
                                if (TransactionsPerMonth.ContainsKey(Month.December))
                                {
                                    TransactionsPerMonth[Month.December] += Quantity;
                                    TurnoverPerMonth[Month.December] += Turnover;

                                }
                                else
                                {
                                    TransactionsPerMonth.Add(Month.December, Quantity);
                                    TurnoverPerMonth.Add(Month.December, Turnover);

                                }
                                break;
                        }
                    }

                    TransactionsPerYear.Add(i, TransactionsPerMonth);
                    turnoverReport.Add(i, TurnoverPerMonth);

                }



                return TransactionsPerYear;
            }

            return null;
        }

        public Dictionary<string, double> ProcessQuantityAndTurnoverFromTransaction(Transaction transaction)
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


            TransactionsForProduct SalesRecord = GetTransactionsForProduct(id, out TurnoverReport, x => x.TransactionType == TransactionType.Sale);
            TransactionsForProduct AuctionRecords = GetTransactionsForProduct(id, out TurnoverReport, x => x.TransactionType == TransactionType.Auction);

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

        public void CompileMonthlySalesAuctionRatio(ref SalesAuctionViewModel monthlyRatioRecord, TransactionsForProduct salesRecords, TransactionsForProduct auctionRecords, Month month)
        {
            int Year = monthlyRatioRecord.Year;
            double SalesQuantity = salesRecords.TransactionsForProductInYear.FirstOrDefault(x => x.Year == Year).TransactionsPerMonth[month];
            double AuctionQuantity = auctionRecords.TransactionsForProductInYear.FirstOrDefault(x => x.Year == Year).TransactionsPerMonth[month];

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
                TransactionRecord.TotalCost = TransactionRecord.Quantity * Batch.SellingPrice * ((100 - Product.BulkPurchaseDiscountPercent) / 100);

            }
            else
            {
                TransactionRecord.TotalCost = TransactionRecord.Quantity * Batch.SellingPrice;

            }
            TransactionRecord.AgentId = agentId;
            TransactionRecord.TransactionType = TransactionType.Sale;
            Batch.QuantitySold += TransactionRecord.Quantity;

            await BatchRepo.UpdateAsync(Batch);
            return await TransactionRepo.AddAsync(TransactionRecord);

        }

    }
}