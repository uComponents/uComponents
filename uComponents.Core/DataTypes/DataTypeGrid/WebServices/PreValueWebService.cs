// --------------------------------------------------------------------------------------------------------------------
// <summary>
// 10.04.2011 - Created [Ove Andersen]
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Web.Script.Services;
using System.Web.Services;

namespace uComponents.Core.DataTypes.DataTypeGrid.WebServices
{
    /// <summary>
    /// Web service for Prevalues.
    /// </summary>
    [ScriptService]
    [WebService]
    public class PreValueWebService : WebService
    {
        /// <summary>
        /// Reorders the prevalue.
        /// </summary>
        /// <param name="preValueId">The prevalue id.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public bool ReorderPreValue(string preValueId, string sortOrder)
        {
            return uQuery.ReorderPreValue(int.Parse(preValueId), int.Parse(sortOrder));
        }
    }
}
