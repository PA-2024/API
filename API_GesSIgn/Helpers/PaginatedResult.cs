﻿namespace API_GesSIgn.Helpers
{
    /// <summary>
    /// Pagination des résultats
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginatedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
    }
}
