﻿namespace MSC.Core.Dtos.Pagination;

/// <summary>
/// Base class to be used by search  classes like UserSearchParamsDto
/// </summary>
public class PaginationParams
{
    //set a constant for the max page size
    private const int MaxPageSize = 50;

    /// <summary>
    /// page number user is requesting. Default is page #1
    /// </summary>
    public int PageNumber { get; set; } = 1;

    //default for the page size
    private int _pageSize = 10;

    public int PageSize
    {
        //the default in this case will be 10
        get => _pageSize;
        //when pageSize is greater than MaxPageSize then return MaxPageSize
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
}
