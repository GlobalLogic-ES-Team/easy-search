using EasySearch.Domain;
using System.Collections.Generic;

namespace EasySearch.Web.Models
{
    public class SearchViewModel
    {
        /// <summary>
        /// The total number of matching results
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        /// The total number of pages
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// The current page of package results
        /// </summary>
        public IEnumerable<Person> Persons { get; set; }

        /// <summary>
        /// Returns how long the elasticsearch query took in milliseconds
        /// </summary>
        public int ElapsedMilliseconds { get; set; }

        public SearchViewModel()
        {
        }
    }
}
