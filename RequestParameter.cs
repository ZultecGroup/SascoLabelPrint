using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sasco
{
    class RequestParameter
    {
        public class LoginClass
        {
            public string loginName { get; set; }
            public string Password { get; set; }
            public string userName { get; set; }
            public string roleID { get; set; }
            public string roleName { get; set; }
            public string status { get; set; }
            public string storecode { get; set; }
            public string message { get; set; }
            public string validTill { get; set; }
            public string token { get; set; }
            public string rolrefreshTokeneName { get; set; }
            public string refreshTokenNew { get; set; }
            public string userID { get; set; }
        }

        public class generatetokenforemail
        {

            public string loginName { get; set; }
            public string message { get; set; }

        }

        public class forgotpassword
        {
            public string loginName { get; set; }
            public string newPassword { get; set; }
            public string token { get; set; }


            public string message { get; set; }
        }

        public class getallstoresClass
        {
            public int get { get; set; }
        }

        public class deleteuserClass
        {

            public int update { get; set; }
            public int delete { get; set; }
            public string message { get; set; }
            public int id { get; set; }
            public int isDeleted { get; set; }
            public int lastUpdatedBy { get; set; }

        }
        public class createuserClass
        {
            public int add { get; set; }
            public int update { get; set; }
            public string loginName { get; set; }
            public string userName { get; set; }
            public string storeCode { get; set; }
            public string password { get; set; }
            public string emailAddress { get; set; }
            public string message { get; set; }

            public int roleID { get; set; }

            public int userAccess { get; set; }

            public int id { get; set; }

        }
        public class createstoresClass
        {
            public int add { get; set; }
            public int update { get; set; }
            public string storeCode { get; set; }
            public string storeName { get; set; }
            public string message { get; set; }
            public string status { get; set; }
            public int createdBy { get; set; }
            public int storeID { get; set; }
            public int lastUpdatedBy { get; set; }
        }

        public class getalllabelsClass
        {
            public int get { get; set; }
        }
        public class getalllabelsagainstscodeClass
        {
            public string storeCode { get; set; }
        }
        public class getallstoreClassgainstStore
        {
            public string storeCode { get; set; }
        }
        public class getrowcount
        {
            public string date { get; set; }
            public string storeTypeID { get; set; }
        }
        public class createlabelClass
        {
            public int add { get; set; }
            public int update { get; set; }
            public int labelID { get; set; }
            public string labelCode { get; set; }
            public string labelName { get; set; }
            public string labelText { get; set; }
            public string message { get; set; }
            public string status { get; set; }
            public int createdBy { get; set; }
            public int lastUpdatedBy { get; set; }
            public int isDefault { get; set; }
        }
        public class createstoreClass
        {
            public int add { get; set; }
            public int update { get; set; }
            public string labelID { get; set; }
            public string labelCode { get; set; }
            public string storeCode { get; set; }
           
            public string storeName { get; set; }
            public List<printerss> printerList { get; set; } = new List<printerss> { };
            public string message { get; set; }
            public string status { get; set; }
            public int createdBy { get; set; }
            public int lastUpdatedBy { get; set; }
            public int storeID { get; set; }
            public int storeType { get; set; }
        }
        public class deletelabelClass
        {

            public int update { get; set; }
            public int delete { get; set; }
            public string message { get; set; }
            public int labelID { get; set; }
            public int lastUpdatedBy { get; set; }
        }
        public class deletestoreClass
        {

            public int update { get; set; }
            public int delete { get; set; }
            public string message { get; set; }
            public int storeID { get; set; }
            public int lastUpdatedBy { get; set; }
        }
        public class printerss
        {
            public string storeCode { get; set; }
            public string printerName { get; set; }
        }

        public class saveCompanyClass
        {
            public string companyName { get; set; }
            public string companyAddress { get; set; }
            public string country { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string email { get; set; }
            public string contactNo { get; set; }
            public string imageBase64 { get; set; }
            public string message { get; set; }
            public int add { get; set; }
            public int update { get; set; }
            public int id { get; set; }
            public string refreshTokenNew { get; set; }
            public string userID { get; set; }
        }
        public class createCompanyClass
        {
            public int get { get; set; }
            
        }

        public class importFileList
        {

            //public string StoreCode { get; set; }

            public string itemNo { get; set; }
            public string engName { get; set; }
            public string arName { get; set; }
            public string barcode { get; set; }
            public string unit { get; set; }

            public string price { get; set; }
            public string store { get; set; }
            public string storeTypeID { get; set; }
            //public string StoreName {get;set;}
            public string suppCode { get; set; }
            //public string BrandName {get;set;}
          
        }
        public class importStoreTypes
        {
            public List<importStoreList> importStoreTypesData { get; set; } = new List<importStoreList> { };

            public string message { get; set; }
        }
        public class importStoreList
        {

            //public string StoreCode { get; set; }

            public string storeType { get; set; }
        }
        public class importClass
        {
            public List<importFileList> importData { get; set; } = new List<importFileList> { };

            public string message { get; set; }

            public static implicit operator importClass(importStoreTypes v)
            {
                throw new NotImplementedException();
            }
        }
       
        public class savesettingClass
        {
            public int get { get; set; }
            public int add { get; set; }
            public int id { get; set; }
            public int update { get; set; }
            public string networkFolderPath { get; set; }
            //public string StoreName {get;set;}
            public string archiveFolderPath { get; set; }
            public string message { get; set; }
        }
    }
}
