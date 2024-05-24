namespace Ecommerce.Models
{
    public class PaginatedList
    {
        public int PageIndex { get; set; }
        public int TotalPages { get;private set; }

        public int PageSize { get; set; }

        public int TotalItems {  get; private set; }

        PaginatedList()
        {
        }

        public PaginatedList(int pageIndex, int totalPages, int pageSize, int totalItems)
        {
            PageIndex = pageIndex;
            TotalPages = totalPages;
            PageSize = pageSize;
            TotalItems = totalItems;
        }


        public bool HasPreviousPage => (PageIndex > 1);

        public bool HasNextPage => (PageIndex < TotalPages);

        public int FirstItemIndex => (PageIndex - 1) * PageSize + 1;

        public int LastItemIndex => Math.Min(PageIndex * PageSize, TotalItems);

        

    }
}
