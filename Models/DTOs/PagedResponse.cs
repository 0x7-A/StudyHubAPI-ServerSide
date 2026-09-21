namespace StudyHubAPI.Models.DTOs
{
    public class PagedResponse<T>
    {
        // The actual rows for the current page
        public IEnumerable<T> Data { get; set; }

        // Metadata
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / (PageSize > 0 ? PageSize : 1));
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        public PagedResponse(IEnumerable<T> data, int count, int pageNumber, int pageSize)
        {
            Data = data;
            TotalCount = count;
            CurrentPage = pageNumber;
            PageSize = pageSize;
        }
    }

}