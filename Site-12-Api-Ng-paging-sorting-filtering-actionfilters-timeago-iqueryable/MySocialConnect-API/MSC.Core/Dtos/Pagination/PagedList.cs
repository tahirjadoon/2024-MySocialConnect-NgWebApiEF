using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MSC.Core.Dtos.Pagination;

// This will work with any type of object/class so PagedList<T> where T is UserDto
// We are getting users (so List<T>) and will send through this class which will help with pagination
// There is static metod which will receive the data, it will call the contructor and then return the data
public class PagedList<T> : List<T>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="items">The data</param>
    /// <param name="count">The total number of records</param>
    /// <param name="pageNumber">The pageNumber interested in - currentPage</param>
    /// <param name="pageSize">The total number of records per page</param>
    public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        CurrentPage = pageNumber;
        PageSize = pageSize;
        TotalCount = count;
        //lets say we have count=10, and pageSize=4, we will get TotalPages=3
        TotalPages = (int) Math.Ceiling(count/(double) pageSize);

        //to have access to the items in the page list
        AddRange(items);
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
    public int PageSize { get; set; }
    /// <summary>
    /// Total records available
    /// </summary>
    public int TotalCount { get; set; }

    //static method which will receive the IQueryable pageNumber and pageSize and return the page data
    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        //total count
        var count = await source.CountAsync();
        //items, when on page #1 we do not want to skip any thing. So use pagenumber - 1
        var items = await source.Skip((pageNumber-1)*pageSize).Take(pageSize).ToListAsync();
        //return new paged list
        var data = new PagedList<T>(items, count, pageNumber, pageSize);
        return data;
    }

}
