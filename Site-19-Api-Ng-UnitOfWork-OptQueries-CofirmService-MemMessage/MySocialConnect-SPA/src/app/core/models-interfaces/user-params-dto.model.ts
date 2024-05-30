import { HttpParams } from "@angular/common/http";
import { LoggedInUserDto } from "./logged-in-user-dto.model";
import { AppConstants } from "../constants/app-constants";

export class UserParamsDto {
    gender: string;
    minAge: number = AppConstants.MinAge;
    maxAge: number = AppConstants.MaxAge;
    pageNumber: number = AppConstants.PageNumber;
    pagesize: number = AppConstants.PageSize;
    orderBy: string = AppConstants.membersOrderByLastActive;
    displayName: string;

    constructor(user: LoggedInUserDto) {
        //when logged in user is male then get the female members otherwise male members
        this.gender = user.gender === AppConstants.Female ? AppConstants.Male : AppConstants.Female;
        this.displayName = user.displayName;
    }

    //helper metod to build search params
    getDefaultPaginationSearchParams() : HttpParams {
        let params = new HttpParams();
        params = params.append('pageNumber', this.pageNumber.toString());
        params = params.append('pageSize', this.pagesize.toString());
        return params;
    }

    //helper metod to build search params
    getDefaultMemberSearchParams() : HttpParams {
        let params = this.getDefaultPaginationSearchParams();
        params = params.append('gender', this.gender);
        params = params.append('minAge', this.minAge.toString());
        params = params.append('maxAge', this.maxAge.toString());
        params = params.append('orderBy', this.orderBy);
        return params;
    }
}
