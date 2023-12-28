using System.Collections.Generic;

namespace Kissarekisteri.DTOs
{
    public class CatQueryParamsDTO
    {
        /// <summary>
        /// Optional name filter
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Optional breed filter
        /// </summary>
        public string? Breed { get; set; }
        /// <summary>
        /// Optional limit for the number of results
        /// </summary>
        public int? Limit { get; set; }

        public string? Sex { get; set; }

        /// <summary>
        /// A comma-separated list of additional data to include in the response.
        /// For example, "parents,results" to include both parents and results data.
        /// </summary>
        public List<string>? Include { get; set; } = [];
    }
}
