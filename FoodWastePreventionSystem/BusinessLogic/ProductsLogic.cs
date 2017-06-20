using FoodWastePreventionSystem.BusinessLogic.BusinessLogicModels;
using FoodWastePreventionSystem.Infrastructure;
using FoodWastePreventionSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace FoodWastePreventionSystem.BusinessLogic
{
    public class ProductsLogic : Logic
    {
        //ApplicationContext AppContext;
        //IRepository<Product> ProductRepo;
        //IRepository<ProductInStore> ProductInStoreRepo;
        //IRepository<Transaction> TransactionRepo;
        //public ProductsLogic(IRepository<Product> _ProductRepo, IRepository<ProductInStore> _ProductInStoreRepo, IRepository<Transaction> _TransactiontRepo)
        //{
        //    AppContext = new ApplicationContext();
        //    ProductRepo = _ProductRepo;
        //    ProductInStoreRepo = _ProductInStoreRepo;
        //    TransactionRepo = _TransactiontRepo;
        //}

        public ProductsLogic(IRepository<Product> _ProductRepo, IRepository<Batch> _ProductInStoreRepo, IRepository<Transaction> _TransactiontRepo, IRepository<Auction> _AuctionRepo, IRepository<ProductToBeAuctioned> _ProductToBeAuctionedRepo, IRepository<Loss> _LossRepo,IRepository<AuctionTransactionStatus> _AuctionTransactionStatusRepo) :
            base(_ProductRepo, _ProductInStoreRepo, _TransactiontRepo, _AuctionRepo, _ProductToBeAuctionedRepo, _LossRepo, _AuctionTransactionStatusRepo)
        {

        }
        public Product RegisterProduct(Product product)
        {
            Product NewProduct = ProductRepo.Add(product);
            return NewProduct;
        }

        public Product EditProduct(Product product)
        {             
            Product NewProduct = ProductRepo.Update(product);
            return NewProduct;
        }

        public void DeleteProduct(Guid ProductId)
        {
            ProductRepo.Delete(ProductId);
        }

        public IEnumerable<ProfitForProduct> ViewProfitForProducts(Expression<Func<Product, bool>> ProductPredicate = null, Expression<Func<Batch, bool>> ProductInStorePredicate = null)
        {
            IEnumerable<Product> Products = ProductPredicate == null ? ProductRepo.GetAll() : ProductRepo.GetAll().Where(ProductPredicate);
            return ViewProfitForProductsBasedOnDuration(Products, ProductInStorePredicate);
        }


        public IEnumerable<ProfitForProduct> ViewProfitForProductsBasedOnDuration(IEnumerable<Product> products, Expression<Func<Batch, bool>> predicate = null)
        {
            List<ProfitForProduct> ProfitForProductsList = new List<ProfitForProduct>();
            foreach (var item in products)
            {
                string[] ProfitForProducts = predicate == null ? BatchRepo.GetAll(x => x.ProductId == item.Id).Select(x => x.ProfitMargin).ToArray() :
                                                                BatchRepo.GetAll(x => x.ProductId == item.Id).Where(predicate).Select(x => x.ProfitMargin).ToArray();

                double[] ExpectedProfit = new double[ProfitForProducts.Count()];
                double[] ActualProfit = new double[ProfitForProducts.Count()];


                for (var i = 0; i < ProfitForProducts.Count(); ++i)
                {
                    ExpectedProfit[i] = GetProfitForProduct(ProfitForProducts[i])["Expected"];
                    ActualProfit[i] = GetProfitForProduct(ProfitForProducts[i])["Actual"];
                }

                double AverageExpectedProfitMargin = ExpectedProfit.Average();
                double AverageActualProfitMargin = ActualProfit.Average();

                ProfitForProductsList.Add(new ProfitForProduct
                {
                    ProductName = item.Name,
                    ActualProfit = AverageActualProfitMargin,
                    ExpectedProfit = AverageExpectedProfitMargin,
                });
            }
            return ProfitForProductsList;
        }





        public IEnumerable<Batch> ProductsPast75PercentOfShelfLife()
        {
            List<Batch> ProductsInStoreList = new List<Batch>();
            ProductsInStoreList = BatchRepo.GetAll(x => (x.QuantitySold + x.QuantityAuctioned+x.QuantityDisposedOf) < x.QuantityPurchased).ToList();
            List<Batch> SoonToExpireProductsInList = new List<Batch>();

            foreach (var item in ProductsInStoreList)
            {
                if (IsProductPast75PercentOfShelfLife(item))
                {
                    if (ProductToBeAuctionedRepo.GetAll(x => x.BatchId == item.Id).FirstOrDefault() == null &&
                        AuctionRepo.GetAll(x => x.BatchId == item.Id).FirstOrDefault() == null)
                    {
                        ProductToBeAuctionedRepo.Add(new ProductToBeAuctioned()
                        {
                            AuctionPrice = 0,
                            HasBeenReviewedByManager = false,
                            BatchId = item.Id,
                            //StoreId = new StoreManager().GetStoreId(),
                            DateOfAuction = ReturnAuctionDateUsingPercentageOfShelfLife(80, item),

                        });
                    }

                    SoonToExpireProductsInList.Add(item);
                }
            }
            return SoonToExpireProductsInList;
        }

        public bool IsProductPast75PercentOfShelfLife(Batch productInStore)
        {
            DateTime ManufactureDate = productInStore.ManufactureDate;
            DateTime ExpiryDate = productInStore.ExpiryDate;

            int NumberOfDays = (ExpiryDate - ManufactureDate).Days;
            int NumberOfDaysLeft = (ExpiryDate - DateTime.Now).Days;
            double PercentOfShelfUsed = ((NumberOfDays - NumberOfDaysLeft) / NumberOfDays) * 100;
            return PercentOfShelfUsed >= 75 ? true : false;
        }

        public DateTime ReturnAuctionDateUsingPercentageOfShelfLife(double DesiredPercentageOfShelfLife, Batch productInStore)
        {
            DateTime ManufactureDate = productInStore.ManufactureDate;
            DateTime ExpiryDate = productInStore.ExpiryDate;
            int NumberOfDays = (ExpiryDate - ManufactureDate).Days;
            int NumberOfDaysLeft = (ExpiryDate - DateTime.Now).Days;
            double dayToAuction = (NumberOfDays * DesiredPercentageOfShelfLife) / 100;
            DateTime AuctionDate = ManufactureDate.AddDays(dayToAuction);
            return AuctionDate;
        }




        public Dictionary<string, double> GetProfitForProduct(string Profit)
        {
            string[] Margins = Profit.Split(new char[',']);
            return new Dictionary<string, double>()
            {
                {"Expected", double.Parse(Margins[0]) },
                {"Actual", double.Parse(Margins[1]) },
            };
        }

        public IEnumerable<Batch> RetrieveProductInStoreRecordsForProduct(Guid productId)
        {
            return BatchRepo.GetAll(x => x.ProductId == productId);
        }

        public Guid[] RetrieveProductInStoreIdsForProduct(Guid productId)
        {
            return RetrieveProductInStoreRecordsForProduct(productId).Select(x => x.Id).ToArray();
        }




    }
}