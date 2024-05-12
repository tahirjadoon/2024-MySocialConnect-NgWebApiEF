﻿using System.Net;

namespace MSC.Core.Dtos;

public class BusinessResponse
{
    public BusinessResponse()
    {
    }

    public BusinessResponse(HttpStatusCode httpStatusCode, string message = "", object data = null)
    {
        HttpStatusCode = httpStatusCode;
        Message = message;
        Data = data;
    }

    public HttpStatusCode HttpStatusCode { get; set; }
    public string Message { get; set; }
    public object Data {get; set;}

    public T ConvertDataToType<T>()
    {
        if(Data == null) return default(T);
        var newData = (T)Data;
        return newData;
    }
}
