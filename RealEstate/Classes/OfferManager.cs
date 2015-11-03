using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Classes
{
    class OfferManager
    {
        public bool AddOffer(int listID, int clientID, int offerPrice, int offerStatus, string date)
        {
            DatabaseManager dbManager = new DatabaseManager();
            return dbManager.NonReturnQuery("INSERT INTO Offer (List_ID, Client_ID, Offer_Price, Offer_Status, Offer_Change_Date) VALUES (" + listID + "," + clientID + "," + offerPrice + "," + offerStatus + ",'" + date + "');");
        }
        public bool EditOffer(int offerID, int listID, int clientID, int offerPrice, int offerStatus, string date)
        {
            DatabaseManager dbManager = new DatabaseManager();
            return dbManager.NonReturnQuery("UPDATE Offer SET List_ID = " + listID + ", Client_ID = " + clientID + ", Offer_Price = " + offerPrice + ", Offer_Status = " + offerStatus + ", Offer_Change_Date = '" + date + "' WHERE Offer_ID = " + offerID + ";");
        }
        public bool DeleteOffer(int offerID)
        {
            DatabaseManager dbManager = new DatabaseManager();
            return dbManager.NonReturnQuery("DELETE FROM Offer WHERE Offer_ID = " + offerID + ";");
        }
        public List<int> GetListings(int agentID)
        {
            DatabaseManager dbManager = new DatabaseManager();
            List<int> listing_IDs = new List<int>();
            var listings = (dbManager.ReturnQuery("SELECT List_ID FROM Listing WHERE Agent_ID = " + agentID));
            foreach (var i in listings)
            {
                listing_IDs.Add(Convert.ToInt32(i[0]));
            }
            return listing_IDs;
        }
    }
}
