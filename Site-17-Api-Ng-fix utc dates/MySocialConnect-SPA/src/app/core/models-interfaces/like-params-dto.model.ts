import { HttpParams } from "@angular/common/http";
import { AppConstants } from "../constants/app-constants";
import { ZUserLikeType } from "../enums/z-user-like-type";

export class LikeParamsDto {
    pageNumber: number = AppConstants.PageNumber;
    pageSize: number = AppConstants.PageSize;
    userLikeType: ZUserLikeType = ZUserLikeType.liked;

    constructor(){}

    //helper method to build search prams
    getPaginationSearchParams(): HttpParams{
        let params = new HttpParams();
        params = params.append('pageNumber', this.pageNumber.toString());
        params = params.append('pagesize', this.pageSize.toString());
        return params;
    }

    //helper method to build search params
    getLikeSearchParams(): HttpParams{
        let params = this.getPaginationSearchParams();
        params = params.append('userLikeType', ZUserLikeType[this.userLikeType]);
        return params;
    }
}
