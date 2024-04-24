import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, delay, finalize } from 'rxjs';

import { SpinnerBusyService } from '../services/spinner-busy.service';
import { HelperService } from '../services/helper.service';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {

  constructor(private spinnerBusyService: SpinnerBusyService, 
              private helperService: HelperService
  ) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const isSpinnerAllowed:boolean = this.helperService.isSpinnerallowed(request.url, request.method);
    
    if(isSpinnerAllowed)
      this.spinnerBusyService.busy();

    //return next.handle(request);
    return next.handle(request).pipe(
      delay(this.helperService.LoadingSpinnerDelayMiliSec),
      finalize(() => {
        if(isSpinnerAllowed)
          this.spinnerBusyService.idle();
      })
    );
  }
}
