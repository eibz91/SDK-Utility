using System.Net;

using CherwellSDKUtility.WebAPI;

namespace CherwellSDKUtility.Helpers
{
    public class CherwellWebConnector
    {
        #region Properties

        /// <summary>Initialise Cherwell Web Service.</summary>
        private api _cherwellService;

        #endregion

        #region Constructor

        /// <summary>Constructor to initialise class.</summary>
        public CherwellWebConnector()
        {
        }

        #endregion
        
        #region Public Methods

        /// <summary>Establish a session and log in. Capture session cookie for later use.</summary>
        /// <returns>Returns true if login is successful</returns>
        internal bool Login(string username, string password)
        {
            _cherwellService = new api { CookieContainer = new CookieContainer() };
            return _cherwellService.Login(username, password);
        }

        /// <summary>Close down a session and Logout.</summary>
        /// <returns>Returns true if logout is successful, or if currently not logged in.</returns>
        internal bool Logout()
        {
            return !_cherwellService.Logout();
        }

        /// <summary>Create a new business object and populate it based on the passed XML</summary>
        /// <param name="busObName">Either the business object's name or its ID.</param>
        /// <param name="objectXml">XML of object to create.</param>
        /// <returns>The RecordID of the new record or an error if the create fails</returns>
        public string CreateBusinessObject(string busObName, string objectXml)
        {
            string response = _cherwellService.CreateBusinessObject(busObName, objectXml);
            return string.IsNullOrEmpty(response) ? GetLastError() : response;
        }

        /// <summary>Create a new business object and populate it based on the passed XML</summary>
        /// <param name="busObName">Either the business object's name or its ID.</param>
        /// <param name="recId">ID of record to update.</param>
        /// <param name="objectXml">XML of object to create.</param>
        /// <returns>The RecordID of the new record or an error if the update fails</returns>
        public string UpdateBusinessObject(string busObName, string recId, string objectXml)
        {
            var response = _cherwellService.UpdateBusinessObject(busObName, recId, objectXml);
            return response == false ? GetLastError() : "Record Updated Successfully";
        }

        /// <summary>Create a new business object and populate it based on the passed XML</summary>
        /// <param name="busObName">Either the business object's name or its ID.</param>
        /// <param name="publicId">Public ID of record to update.</param>
        /// <param name="objectXml">XML of object to create.</param>
        /// <returns>The RecordID of the new record or an error if the update fails</returns>
        public string UpdateBusinessObjectByPublicId(string busObName, string publicId, string objectXml)
        {
            var response = _cherwellService.UpdateBusinessObjectByPublicId(busObName, publicId, objectXml);
            return response == false ? GetLastError() : "Record Updated Successfully";
        }

        /// <summary>Query using a specific stored query.</summary>
        /// <param name="busObName">The name of the associated Business Object.</param>
        /// <param name="searchQuery">The name of the stored query.</param>
        /// <returns>A list of mapped Cherwell Business Object records.</returns>
        public string QueryByStoredQuery(string busObName, string searchQuery)
        {
            string response = _cherwellService.QueryByStoredQuery(busObName, searchQuery);
            return string.IsNullOrEmpty(response) ? GetLastError() : response;
        }

        /// <summary>Query using a specific field name and value.</summary>
        /// <param name="busObName">The name of the Business Object.</param>
        /// <param name="fieldName">The name of the field to search.</param>
        /// <param name="fieldValue">The value to search for.</param>
        /// <returns>A list of mapped Cherwell Business Object records.</returns>
        public string QueryByFieldValue(string busObName, string fieldName, string fieldValue)
        {
            string response = _cherwellService.QueryByFieldValue(busObName, fieldName, fieldValue);
            return string.IsNullOrEmpty(response) ? GetLastError() : response;
        }

        /// <summary>Get a specific Business Object.</summary>
        /// <param name="busObName">The name of the Business Object.</param>
        /// <param name="recId">The ID of the record to return.</param>
        /// <returns>A formatted string with the requested returnFields and values.</returns>
        public string GetBusinessObject(string busObName, string recId)
        {
            string response = _cherwellService.GetBusinessObject(busObName, recId);
            return string.IsNullOrEmpty(response) ? GetLastError() : response;
        }

        /// <summary>Retrieves results of a quick search for a particular busob type.</summary>
        /// <param name="busObName">The name of the Business Object.</param>
        /// <param name="searchText">The text for which to search.</param>
        /// <returns>A list of records matching the search text.</returns>
        public string QuickSearch(string busObName, string searchText)
        {
            string response = _cherwellService.QuickSearch(busObName, searchText, true, true, 30, "Seconds");
            return string.IsNullOrEmpty(response) ? GetLastError() : response;
        }

        #endregion

        #region Private Methods

        private string GetLastError()
        {
            string response = _cherwellService.GetLastError();
            return response; 
        }

        #endregion
    }
}
