using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using MSC.Core.Constants;
using MSC.Core.Dtos.Pagination;

namespace MSC.Core.Extensions;

public static class HttpExtensions
{
    public static void AddPaginationHeader(this HttpResponse response, PaginationHeader header)
    {
        var headerJson = header.Serialize(); //toJson
        //response.Headers.Add(HeaderNameConstants.Pagination, headerJson);
        response.Headers.TryAdd(HeaderNameConstants.Pagination, headerJson);
        //since custom header, allow cors policy. Otherwise the clients will not be able to access the header
        response.Headers.TryAdd(HeaderNameConstants.AccessControlExposeHeaders, HeaderNameConstants.Pagination);
    }

    public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
    {
        var header = new PaginationHeader(currentPage, totalPages, itemsPerPage, totalItems);
        var headerJson = header.Serialize(); //toJson
        //response.Headers.Add(HeaderNameConstants.Pagination, headerJson);
        response.Headers.TryAdd(HeaderNameConstants.Pagination, headerJson);
        //since custom header, allow cors policy. Otherwise the clients will not be able to access the header
        response.Headers.TryAdd(HeaderNameConstants.AccessControlExposeHeaders, HeaderNameConstants.Pagination);
    }
}
