using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

using System.Linq;
using System.Web;

namespace FoodWastePreventionSystem.Models
{
    public class Store
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
    }

    public class Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public double BulkPurchaseDiscountPercent { get; set; }
        [Required]
        public int QuantityPerCarton { get; set; }
        public Guid StoreId { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public DateTime DateProductWasRegistered { get; set; }

        public string Description { get; set; }

        public byte[] Image1 { get; set; }
        public byte[] Image2 { get; set; }

        public string extension1 { get; set; }
        public string extension2 { get; set; }

        public bool Blacklisted { get; set; }

        //[ForeignKey(nameof(StoreId))]
        public virtual Store Store { get; set; }

        public virtual ICollection<Batch> Batches { get; set; }


    }

    public class Batch
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Display(Name = "Product")]
        public Guid ProductId { get; set; }
        [Display(Name = "Purchase price")]
        public double PurchasePrice { get; set; }
        [Display(Name = "Selling Price")]
        public double SellingPrice { get; set; }
        [Display(Name = "Manufacture Date")]
        public DateTime ManufactureDate { get; set; }
        [Display(Name = "Expiry Date")]
        public DateTime ExpiryDate { get; set; }
        [Display(Name = "Quantity Purchased")]
        public int QuantityPurchased { get; set; }
        [Display(Name = "Quantity Sold")]
        public int QuantitySold { get; set; }
        [Display(Name = "Quantity Auctioned")]
        public int QuantityAuctioned { get; set; }
        [Display(Name = "Quantity Lost")]
        public int QuantityDisposedOf { get; set; }
        [Display(Name = "Profit Margin")]
        public string ProfitMargin { get; set; } //[ExpectedProfitMargin,ActualProfitMargin]
        [Display(Name = "Date")]
        public DateTime DateWhichInventoryWasPurchased { get; set; }

        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
    }

    public class Transaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid BatchId { get; set; }
        public int Quantity { get; set; }
        public bool BulkPurchase { get; set; }
        public double TotalCost { get; set; }
        public DateTime DateOfTransaction { get; set; }
        public TransactionType TransactionType { get; set; }
        public string AgentId { get; set; }
        public string AuctioneeId { get; set; }
        //public Guid AuctionId { get; set; }

        //public Guid StoreId { get; set; }
        public double ProfitMade { get; set; }

        [ForeignKey(nameof(BatchId))]
        public virtual Batch Batch { get; set; }
        //[ForeignKey(nameof(AuctionId))]
        //public virtual Auction Auction { get; set; }
        //public  ApplicationUser User { get; set; }
        //public  ApplicationUser Auctionee { get; set; }

        //public virtual Store Store { get; set; }
    }



    public class AuctionTransactionStatus
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid TransactionId { get; set; }
        public AuctionState Status { get; set; }

        [ForeignKey(nameof(TransactionId))]
        public virtual Transaction Transaction { get; set; }


    }


    public class Loss
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid BatchId { get; set; }
        public int Quantity { get; set; }
        //public Guid StoreId { get; set; }
        public DateTime DateOfLoss { get; set; }

        [ForeignKey(nameof(BatchId))]
        public virtual Batch Batch { get; set; }
        //public virtual Store Store { get; set; }
    }

    public class ProductToBeAuctioned
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid BatchId { get; set; }
        public DateTime DateOfAuction { get; set; }
        public double AuctionPrice { get; set; }
        //public Guid StoreId { get; set; }
        public bool HasBeenReviewedByManager { get; set; }

        [ForeignKey(nameof(BatchId))]
        public virtual Batch Batch { get; set; }
        //public virtual Store Store { get; set; }
    }

    public class Auction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid BatchId { get; set; }
        public double AuctionPrice { get; set; }

        [ForeignKey(nameof(BatchId))]
        public virtual Batch Batch { get; set; }
        //public virtual Store Store { get; set; }
    }

    public class Cart
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string CustomerId { get; set; }
        public Guid BatchId { get; set; }
        public Guid AuctionId { get; set; }
        public int Quantity { get; set; }

        [ForeignKey(nameof(BatchId))]
        public virtual Batch Batch { get; set; }
        [ForeignKey(nameof(AuctionId))]
        public virtual Auction Auction { get; set; }
    }

}