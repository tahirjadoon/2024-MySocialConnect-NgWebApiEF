import { Injectable } from '@angular/core';
import { map } from 'rxjs';
import { HttpParams } from '@angular/common/http';

import { HttpClientService } from './http-client.service';
import { HelperService } from './helper.service';

import { PaginatedResult } from '../models-interfaces/pagination/paginated-result.model';
import { AppConstants } from '../constants/app-constants';

@Injectable({
  providedIn: 'root'
})
export class PageService {

  constructor(private httpClientService: HttpClientService, private helperService: HelperService) { }

  getPaginatedResult<T>(url: string, params: HttpParams){
    this.helperService.logIfFrom(url, "PageService getPaginatedResult Request");
    this.helperService.logIf(params);
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
    return this.httpClientService.getWithFullResponse<T>(url, params).pipe(
      map(response => {
        //result - members list
        if(response.body)
          paginatedResult.result = response.body;
        //pagination info from the header
        const header = response.headers.get(AppConstants.PaginationHeader);
        if(header != null)
          paginatedResult.pagination = JSON.parse(header)
        this.helperService.logIfFrom(paginatedResult, "PageService getPaginatedResult");
        return paginatedResult;
      })
    );
  }

}
