using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cotecna.Voc.Web.Models
{
    /// <summary>
    /// Contains information for pagination
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginatedList<T>
    {
        /// <summary>
        /// The collection that have been filtered
        /// </summary>
        public List<T> Collection { get; set; }

        /// <summary>
        /// Total numbers of elements
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// The number of pages
        /// </summary>
        public int NumberOfPages { get; set; }
        /// <summary>
        /// Get or Set Page
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// PageSize
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Get a new instance of the PaginatedList
        /// </summary>
        public PaginatedList()
        {
            TotalCount = 0;
            NumberOfPages = 0;
            Page = 0;
            Collection = new List<T>();
            PageSize = Cotecna.Voc.Web.Properties.Settings.Default.PageSize; 
        }
    }
}