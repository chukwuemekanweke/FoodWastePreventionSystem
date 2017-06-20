namespace FoodWastePreventionSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class project : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Auctions",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        BatchId = c.Guid(nullable: false),
                        AuctionPrice = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Batches", t => t.BatchId, cascadeDelete: true)
                .Index(t => t.BatchId);
            
            CreateTable(
                "dbo.Batches",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ProductId = c.Guid(nullable: false),
                        PurchasePrice = c.Double(nullable: false),
                        SellingPrice = c.Double(nullable: false),
                        ManufactureDate = c.DateTime(nullable: false),
                        ExpiryDate = c.DateTime(nullable: false),
                        QuantityPurchased = c.Int(nullable: false),
                        QuantitySold = c.Int(nullable: false),
                        QuantityAuctioned = c.Int(nullable: false),
                        QuantityDisposedOf = c.Int(nullable: false),
                        ProfitMargin = c.String(),
                        DateWhichInventoryWasPurchased = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        BulkPurchaseDiscountPercent = c.Double(nullable: false),
                        QuantityPerCarton = c.Int(nullable: false),
                        StoreId = c.Guid(nullable: false),
                        Category = c.String(nullable: false),
                        DateProductWasRegistered = c.DateTime(nullable: false),
                        Description = c.String(),
                        Image1 = c.Binary(),
                        Image2 = c.Binary(),
                        extension1 = c.String(),
                        extension2 = c.String(),
                        Blacklisted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stores", t => t.StoreId, cascadeDelete: true)
                .Index(t => t.StoreId);
            
            CreateTable(
                "dbo.Stores",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Address = c.String(),
                        State = c.String(),
                        City = c.String(),
                        Area = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        BatchId = c.Guid(nullable: false),
                        Quantity = c.Int(nullable: false),
                        BulkPurchase = c.Boolean(nullable: false),
                        TotalCost = c.Double(nullable: false),
                        DateOfTransaction = c.DateTime(nullable: false),
                        TransactionType = c.Int(nullable: false),
                        AgentId = c.String(),
                        AuctioneeId = c.String(),
                        ProfitMade = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Batches", t => t.BatchId, cascadeDelete: true)
                .Index(t => t.BatchId);
            
            CreateTable(
                "dbo.AuctionTransactionStatus",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        TransactionId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Transactions", t => t.TransactionId, cascadeDelete: true)
                .Index(t => t.TransactionId);
            
            CreateTable(
                "dbo.Losses",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        BatchId = c.Guid(nullable: false),
                        Quantity = c.Int(nullable: false),
                        DateOfLoss = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Batches", t => t.BatchId, cascadeDelete: true)
                .Index(t => t.BatchId);
            
            CreateTable(
                "dbo.ProductToBeAuctioneds",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        BatchId = c.Guid(nullable: false),
                        DateOfAuction = c.DateTime(nullable: false),
                        Auctionprice = c.Double(nullable: false),
                        HasBeenReviewedByManager = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Batches", t => t.BatchId, cascadeDelete: true)
                .Index(t => t.BatchId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductToBeAuctioneds", "BatchId", "dbo.Batches");
            DropForeignKey("dbo.Losses", "BatchId", "dbo.Batches");
            DropForeignKey("dbo.AuctionTransactionStatus", "TransactionId", "dbo.Transactions");
            DropForeignKey("dbo.Auctions", "BatchId", "dbo.Batches");
            DropForeignKey("dbo.Transactions", "BatchId", "dbo.Batches");
            DropForeignKey("dbo.Products", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.Batches", "ProductId", "dbo.Products");
            DropIndex("dbo.ProductToBeAuctioneds", new[] { "BatchId" });
            DropIndex("dbo.Losses", new[] { "BatchId" });
            DropIndex("dbo.AuctionTransactionStatus", new[] { "TransactionId" });
            DropIndex("dbo.Transactions", new[] { "BatchId" });
            DropIndex("dbo.Products", new[] { "StoreId" });
            DropIndex("dbo.Batches", new[] { "ProductId" });
            DropIndex("dbo.Auctions", new[] { "BatchId" });
            DropTable("dbo.ProductToBeAuctioneds");
            DropTable("dbo.Losses");
            DropTable("dbo.AuctionTransactionStatus");
            DropTable("dbo.Transactions");
            DropTable("dbo.Stores");
            DropTable("dbo.Products");
            DropTable("dbo.Batches");
            DropTable("dbo.Auctions");
        }
    }
}
