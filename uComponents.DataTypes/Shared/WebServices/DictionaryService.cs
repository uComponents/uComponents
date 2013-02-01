// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 31.01.2013 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace uComponents.DataTypes.Shared.WebServices
{
    using System;
    using System.Web.Script.Services;
    using System.Web.Services;

    using umbraco;

    /// <summary>
    /// Web service for Dictionary items.
    /// </summary>
    [ScriptService]
    [WebService(Namespace = "http://umbraco.org/ucomponents/shared")]
    public class DictionaryService : WebService
    {
        /// <summary>
        /// Gets the dictionary item
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="fallback">The fallback text.</param>
        /// <returns>The translated dictionary item.</returns>
        /// <exception cref="System.UnauthorizedAccessException">Thrown if the user is not logged in.</exception>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public string GetDictionaryItem(string key, string fallback)
        {
            if (umbraco.BusinessLogic.User.GetCurrent() == null)
            {
                throw new UnauthorizedAccessException();
            }

            return uQuery.GetDictionaryItem(key, fallback);
        }
    }
}
