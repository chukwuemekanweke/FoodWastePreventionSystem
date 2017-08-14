using FoodWastePreventionSystem.BusinessLogic.BusinessLogicModels;
using FoodWastePreventionSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace FoodWastePreventionSystem.Areas.Managers.Models
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public IEnumerable<Batch> InStoreRecords { get; set; }
        public IEnumerable<ProductToBeAuctioned> ToBeAuctionedRecords { get; set; }
        public IEnumerable<Auction> AuctionRecords { get; set; }
        public TransactionsForProduct Sales { get; set; }

        public ProductViewModel()
        {
            InStoreRecords = new List<Batch>();
            ToBeAuctionedRecords = new List<ProductToBeAuctioned>();
            AuctionRecords = new List<Auction>();
            Sales = new TransactionsForProduct();

        }
    }

    public class RegisterProductViewModel
    {
        public Product Product { get; set; }
        public string NewCategory { get; set; }

        public RegisterProductViewModel()
        {
            NewCategory = "";
        }

    }

    public class BatchInformationVM
    {
        public Batch BatchInfo { get; set; }
        public IEnumerable<TransactionVM> Transactions { get; set; }
        public IEnumerable<TransactionVM> AuctionRecords { get; set; }
        public ProductToBeAuctioned ToBeAuctionedRecord { get; set; }

        public Auction AuctionDetails { get; set; }

        public BatchInformationVM()
        {
        }

    }

    public class TransactionVM
    {
        public Transaction Transaction { get; set; }
        public ApplicationUser Agent { get; set; }
        public ApplicationUser Auctionee { get; set; }
    }


    public class AddTransactionVM
    {
        public Transaction Transaction { get; set; }
        public Batch Batch { get; set; }
    }


    public class AuctionManagementVM
    {
        public Batch Batch { get; set; }
        public BatchAuctionState State { get; set; }
    }

    public class EditViewAuctionVM
    {
        public Auction Auction { get; set; }
        public List<TransactionVM> TransactionRecords { get; set; }
    }

    public class ProfitForProductAnalysisVM
    {
        public List<ProfitLossForBatch> BatchProfitRecords { get; set; }
        public ProfitsForProduct YearlyProfitRecords { get; set; }
    }



}