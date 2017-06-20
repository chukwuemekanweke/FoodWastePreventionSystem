using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodWastePreventionSystem.Models
{
    public enum TransactionType
    {
        Auction,
        Sale,        
    }

    public enum BatchAuctionState
    {
        OnAuction, AboutToBeAuctioned, NotReviewed
    }

    public enum Month
    {
        None,
        January,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December,

    }

    public enum AuctionState
    {
        Incomplete, Compleete,
    }
}