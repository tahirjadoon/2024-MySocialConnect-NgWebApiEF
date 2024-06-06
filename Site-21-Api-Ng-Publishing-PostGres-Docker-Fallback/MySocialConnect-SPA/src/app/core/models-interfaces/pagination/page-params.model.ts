import { HttpParams } from "@angular/common/http";
import { AppConstants } from "../../constants/app-constants";

export class PageParams {
    pageNumber: number = AppConstants.PageNumber;
    pageSize: number = AppConstants.PageSize;

    constructor(){}

    //helper method to build search prams
    getPaginationSearchParams(): HttpParams{
        let params = new HttpParams();
        params = params.append('pageNumber', this.pageNumber.toString());
        params = params.append('pagesize', this.pageSize.toString());
        return params;
    }
}
