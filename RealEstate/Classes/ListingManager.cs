using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Classes
{
    class ListingManager
    {
        #region Add
        public bool AddListing(int propertyID, int agentID, int listPrice, int isNegotiable, int isSold, string listDescription)
        {
            DatabaseManager dbManager = new DatabaseManager();
            return dbManager.NonReturnQuery("INSERT INTO Listing (Property_ID, Agent_ID, List_Price, List_isNegotiable, List_isSold, List_Description) VALUES (" + propertyID + "," + agentID + "," + listPrice + "," + isNegotiable + "," + isSold + ",'" + listDescription + "');");
        }
        public bool AddListingProperty(int clientID, int addressID, int complexID, int propertyUnitNo, int bedroomCount, int bathroomCount, int garageCount, int hasPool, int plotSize, int houseSize, int propertyValue, string propertyDescription)
        {
            DatabaseManager dbManager = new DatabaseManager();
            return (dbManager.NonReturnQuery("INSERT INTO Property (Client_ID, Address_ID, Complex_ID, Property_Unit_No, Property_Bedroom_Count, Property_Bathroom_Count, Property_Garage_Count, Property_hasPool, Property_Plot_size, Property_House_Size, Property_Value, Property_Description) values (" + clientID + "," + addressID + "," + complexID + "," + propertyUnitNo + "," + bedroomCount + "," + bathroomCount + "," + garageCount + "," + hasPool + "," + plotSize + "," + houseSize + "," + propertyValue + ",'" + propertyDescription + "');"));
        }
        public bool AddListingAddress(int areaID, string streetName, int streetNo)
        {
            DatabaseManager dbManager = new DatabaseManager();
            return (dbManager.NonReturnQuery("INSERT INTO Address (Area_ID, Address_Streetname, Address_Streetno) values (" + areaID + ",'" + streetName + "'," + streetNo + ");"));
        }
        public bool AddListingComplex(string complexName, int addressID)
        {
            DatabaseManager dbManager = new DatabaseManager();
            return (dbManager.NonReturnQuery("INSERT INTO Complex (Complex_Name, Address_ID) values ('" + complexName + "'," + addressID + ");"));
        }
        public bool AddListingImage(int propertyID, string url, string caption)
        {
            DatabaseManager dbManager = new DatabaseManager();
            return (dbManager.NonReturnQuery("INSERT INTO Image (Property_ID, Image_URL, Image_Caption) values (" + propertyID.ToString() + ",'" + url + "','" + caption + "');"));
        }
        #endregion

        #region Edit
        public bool EditListing(int listingID, int propertyID, int agentID, int listPrice, int isNegotiable, int isSold, string listDescription)
        {
            DatabaseManager dbManager = new DatabaseManager();
            return dbManager.NonReturnQuery("UPDATE Listing SET Property_ID = " + propertyID + ", Agent_ID = " + agentID + ", List_Price = " + listPrice + ", List_isNegotiable = " + isNegotiable + ", List_isSold = " + isSold + ", List_Description = '" + listDescription + "' WHERE List_ID = " + listingID + ";");
        }
        public bool EditListingProperty(int propertyID, int clientID, int addressID, int complexID, int propertyUnitNo, int bedroomCount, int bathroomCount, int garageCount, int hasPool, int plotSize, int houseSize, int propertyValue, string propertyDescription)
        {
            DatabaseManager dbManager = new DatabaseManager();
            return (dbManager.NonReturnQuery("UPDATE Property SET Client_ID = " + clientID + ", Address_ID = " + addressID + ", Complex_ID = " + complexID + ", Property_Unit_No = " + propertyUnitNo + ", Property_Bedroom_Count = " + bedroomCount + ", Property_Bathroom_Count = " + bathroomCount + ", Property_Garage_Count = " + garageCount + ", Property_hasPool = " + hasPool + ", Property_Plot_size = " + plotSize + ", Property_House_Size = " + houseSize + ", Property_Value = " + propertyValue + ", Property_Description = '" + propertyDescription + "' WHERE Property_ID = " + propertyID + ";"));
        }
        public bool EditListingAddress(int addressID, int areaID, string streetName, int streetNo)
        {
            DatabaseManager dbManager = new DatabaseManager();
            return (dbManager.NonReturnQuery("UPDATE Address SET Area_ID = " + areaID + ", Address_Streetname = '" + streetName + "', Address_Streetno = " + streetNo + " WHERE Address_ID = " + addressID + ";"));
        }
        public bool EditListingComplex(int complexID, string complexName, int addressID)
        {
            DatabaseManager dbManager = new DatabaseManager();
            return (dbManager.NonReturnQuery("UPDATE Complex SET Complex_Name = '" + complexName + "', Address_ID = " + addressID + " WHERE Complex_ID = " + complexID + ";"));
        }
        public bool EditListingImage(int imageID, string caption)
        {
            DatabaseManager dbManager = new DatabaseManager();
            return (dbManager.NonReturnQuery("UPDATE Image SET Image_Caption = '" + caption + "' WHERE Image_ID = " + imageID + ";"));
        }
        #endregion

        #region Get
        public List<string> GetClients()
        {
            DatabaseManager dbManager = new DatabaseManager();
            List<string> clients_Email = new List<string>();
            var clients = (dbManager.ReturnQuery("SELECT Client_Name , Client_Email FROM Clients"));
            string clientName, clientEmail;
            foreach (var i in clients)
            {
                clientName = Convert.ToString(i[0]);
                clientEmail = Convert.ToString(i[1]);
                clients_Email.Add(clientName + ", " + clientEmail);
            }
            return clients_Email;
        }
        public List<string> GetProvinces()
        {
            DatabaseManager dbManager = new DatabaseManager();
            List<string> province_Names = new List<string>();
            var provinces = (dbManager.ReturnQuery("SELECT Province_Name FROM Province ORDER BY Province_Name"));
            string provinceName;
            foreach (var i in provinces)
            {
                provinceName = Convert.ToString(i[0]);
                province_Names.Add(provinceName);
            }
            return province_Names;
        }
        public List<string> GetCities(int city_Province_ID)
        {
            DatabaseManager dbManager = new DatabaseManager();
            List<string> city_Names = new List<string>();
            var citiesName = (dbManager.ReturnQuery("SELECT City_Name FROM City WHERE City_Province_ID = " + city_Province_ID + " ORDER BY City_Name"));
            string cityName;
            foreach (var i in citiesName)
            {
                cityName = Convert.ToString(i[0]);
                city_Names.Add(cityName);
            }
            return city_Names;
        }
        public List<string> GetAreas(int area_City_ID)
        {
            DatabaseManager dbManager = new DatabaseManager();
            List<string> area_Names = new List<string>();
            var areasName = (dbManager.ReturnQuery("SELECT Area_Name FROM Area WHERE Area_City_ID = " + area_City_ID + " ORDER BY Area_Name"));
            string areaName;
            foreach (var i in areasName)
            {
                areaName = Convert.ToString(i[0]);
                area_Names.Add(areaName);
            }
            return area_Names;
        }
        public List<int> GetClientsID()
        {
            DatabaseManager dbManager = new DatabaseManager();
            List<int> clients_ID = new List<int>();
            var clientsID = (dbManager.ReturnQuery("SELECT Client_ID FROM Clients"));
            int clientID;
            foreach (var i in clientsID)
            {
                clientID = Convert.ToInt32(i[0]);
                clients_ID.Add(clientID);
            }
            return clients_ID;
        }
        public List<int> GetProvincesID()
        {
            DatabaseManager dbManager = new DatabaseManager();
            List<int> provinces_ID = new List<int>();
            var provincesID = (dbManager.ReturnQuery("SELECT Province_ID, Province_Name  FROM Province ORDER BY Province_Name"));
            int provinceID;
            foreach (var i in provincesID)
            {
                provinceID = Convert.ToInt32(i[0]);
                provinces_ID.Add(provinceID);
            }
            return provinces_ID;
        }
        public List<int> GetCitiesID(int city_Province_ID)
        {
            DatabaseManager dbManager = new DatabaseManager();
            List<int> cities_ID = new List<int>();
            var citiesID = (dbManager.ReturnQuery("SELECT City_ID, City_Name FROM City WHERE City_Province_ID = " + city_Province_ID + " ORDER BY City_Name"));
            int cityID;
            foreach (var i in citiesID)
            {
                cityID = Convert.ToInt32(i[0]);
                cities_ID.Add(cityID);
            }
            return cities_ID;
        }
        public List<int> GetAreasID(int area_City_ID)
        {
            DatabaseManager dbManager = new DatabaseManager();
            List<int> areas_ID = new List<int>();
            var areasID = (dbManager.ReturnQuery("SELECT Area_ID, Area_Name FROM Area WHERE Area_City_ID = " + area_City_ID + " ORDER BY Area_Name"));
            int areaID;
            foreach (var i in areasID)
            {
                areaID = Convert.ToInt32(i[0]);
                areas_ID.Add(areaID);
            }
            return areas_ID;
        }
        public int GetAddressID(int AreaID, string streetName, int streetNo)
        {
            DatabaseManager dbManager = new DatabaseManager();
            int address_ID = -1;
            var addressID = (dbManager.ReturnQuery("SELECT Address_ID FROM Address WHERE (Area_ID = " + AreaID + " AND Address_Streetname = '" + streetName + "' AND Address_StreetNo = " + streetNo + ")"));
            foreach (var i in addressID)
            {
                address_ID = Convert.ToInt32(i[0]);
            }
            return address_ID;
        }
        public int GetComplexID(string complexName, int addressID)
        {
            DatabaseManager dbManager = new DatabaseManager();
            int complex_ID = -1;
            var complexID = (dbManager.ReturnQuery("SELECT Complex_ID FROM Complex WHERE (Complex_Name = '" + complexName + "' AND Address_ID = " + addressID + ")"));
            foreach (var i in complexID)
            {
                complex_ID = Convert.ToInt32(i[0]);
            }
            return complex_ID;

        }
        public int GetPropertyID(int clientID)
        {
            DatabaseManager dbManager = new DatabaseManager();
            int propertyID = 0;
            var properties = (dbManager.ReturnQuery("SELECT Property_ID FROM Property WHERE Client_ID = " + clientID.ToString() + " ORDER BY Property_ID ASC;"));
            foreach (var i in properties)
            {
                propertyID = Convert.ToInt32(i[0]);
            }
            return propertyID;
        }
        public int GetPropertyID(int agentID, int listingID)
        {
            DatabaseManager dbManager = new DatabaseManager();
            int propertyID = 0;
            var properties = (dbManager.ReturnQuery("SELECT Property_ID FROM Listing WHERE Agent_ID = " + agentID + " AND List_ID = " + listingID + " ORDER BY Property_ID ASC;"));
            foreach (var i in properties)
            {
                propertyID = Convert.ToInt32(i[0]);
            }
            return propertyID;
        }
        public int GetAgentID(string agentEmail)
        {
            DatabaseManager dbManager = new DatabaseManager();
            int agentID = -1;
            var agent = (dbManager.ReturnQuery("SELECT Agent_ID FROM Agent WHERE Agent_Email = '" + agentEmail + "'"));
            foreach (var i in agent)
            {
                agentID = Convert.ToInt32(i[0]);
            }
            return agentID;
        }
        #endregion

        #region Count
        private int countPropertiesWithAddress(int addressID)
        {
            int count = -1;
            DatabaseManager dbManager = new DatabaseManager();
            var addressess = (dbManager.ReturnQuery("SELECT * FROM Address WHERE (Adress_ID = " + addressID + ")"));
            foreach (var address in addressess)
            {
                count++;
            }
            return count;
        }
        private int countPropertiesInComplex(int complexID)
        {
            int count = -1;
            DatabaseManager dbManager = new DatabaseManager();
            var complexes = (dbManager.ReturnQuery("SELECT * FROM Complex WHERE (Complex_ID = '" + complexID + ")"));
            foreach (var complex in complexes)
            {
                count++;
            }
            return count;
        }
        #endregion

        public bool DeleteListing(int listingID)
        {
            int propID = -1;
            int addID = -1;
            int compID = -1;
            DatabaseManager dbManager = new DatabaseManager();
            var propIDs = dbManager.ReturnQuery("SELECT Property_ID FROM Listing WHERE List_ID = " + listingID);
            foreach (var i in propIDs)
            {
                propID = Convert.ToInt32(i[0]);
            }
            var compIDs = dbManager.ReturnQuery("SELECT Complex_ID FROM Property WHERE Property_ID = " + propID);
            foreach (var i in compIDs)
            {
                compID = Convert.ToInt32(i[0]);
            }
            var addIDs = dbManager.ReturnQuery("SELECT Address_ID FROM Property WHERE Property_ID = " + propID);
            foreach (var i in addIDs)
            {
                addID = Convert.ToInt32(i[0]);
            }
            if (countPropertiesInComplex(compID) == 0 && countPropertiesWithAddress(addID) == 0)
                return (dbManager.NonReturnQuery("DELETE FROM Image, Complex, Address, Property, Listing USING Image, Complex, Address, Property, Listing WHERE ( Image.Property_ID = Property.Property_ID AND Complex.Complex_ID = Property.Complex_ID AND Property.Property_ID = Listing.Property_ID AND Property.Address_ID = Address.Address_ID AND Listing.List_ID =" + listingID + ");"));
            else if (countPropertiesInComplex(compID) == 0 && countPropertiesWithAddress(addID) != 0)
                return (dbManager.NonReturnQuery("DELETE FROM Image, Complex, Property, Listing USING Image, Complex, Property, Listing WHERE ( Image.Property_ID = Property.Property_ID AND  Complex.Complex_ID = Property.Complex_ID AND Property.Property_ID = Listing.Property_ID AND Listing.List_ID =" + listingID + ");"));
            else if (countPropertiesInComplex(compID) != 0 && countPropertiesWithAddress(addID) == 0)
                return (dbManager.NonReturnQuery("DELETE FROM Image, Address, Property, Listing USING Image, Address, Property, Listing WHERE ( Image.Property_ID = Property.Property_ID AND  Property.Property_ID = Listing.Property_ID AND Property.Address_ID = Address.Address_ID AND Listing.List_ID =" + listingID + ");"));
            else
                return (dbManager.NonReturnQuery("DELETE FROM Image, Complex, Address, Property, Listing USING Image, Complex, Address, Property, Listing WHERE ( Image.Property_ID = Property.Property_ID AND  Complex.Complex_ID = Property.Complex_ID AND Property.Property_ID = Listing.Property_ID AND Property.Address_ID = Address.Address_ID AND Listing.List_ID =" + listingID + ");"));
        }

    }
}
