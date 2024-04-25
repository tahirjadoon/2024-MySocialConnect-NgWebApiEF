import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, take } from 'rxjs';

import { AccountService } from '../services/account.service';

import { LoggedInUserDto } from '../models-interfaces/logged-in-user-dto.model';
import { AppConstants } from '../constants/app-constants';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private accountService: AccountService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let loggedInUser: LoggedInUserDto;

    //subscribe. only taking the first item. no need to unsubscribe here, subscription will complete
    this.accountService.currentLoggedInUser$.pipe(take(1)).subscribe({
      next: user => {
        if(user){
          request = request.clone({
            setHeaders: {
              Authorization: `${AppConstants.Bearer}${user.token}`
            }
          })
        }
      }
    });

    return next.handle(request);
  }
}
