namespace MSC.Core.Dtos.Pagination;

/// <summary>
/// When the response will go to the client the information will be taken from Pagination Header
/// </summary>
public class PaginationHeader
{
    public PaginationHeader(int currentPage, int totalPages, int itemsPerPage, int totalItems)
    {
        CurrentPage = currentPage;
        TotalPages = totalPages;
        ItemsPerPage = itemsPerPage;
        TotalItems = totalItems;
    }

    /// <summary>
    /// Current page number
    /// </summary>
    public int CurrentPage { get; set; }
    /// <summary>
    /// Total pages
    /// </summary>
    public int TotalPages { get; set; }
    /// <summary>
    /// Page size, total records to pull for the page
    /// </summary>
    public int ItemsPerPage { get; set; }
    /// <summary>
    /// Total records available
    /// </summary>
    public int TotalItems { get; set; }
}
