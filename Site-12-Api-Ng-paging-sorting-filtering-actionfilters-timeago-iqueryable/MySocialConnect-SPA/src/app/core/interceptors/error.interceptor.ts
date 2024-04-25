import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request)
    .pipe(
      catchError((error: HttpErrorResponse) => {
        if(error){
          switch (error.status){
            case 400:
              if(error.error.errors){
                //model valdation
                const modelStateErrors = [];
                for(const key in error.error.errors){
                  if(error.error.errors[key]){
                    modelStateErrors.push(error.error.errors[key]);
                  }
                }
                throw modelStateErrors.flat();
              }
              else{
                this.toastr.error(error.error, error.status.toString());
              }
              break;
            case 401:
              const message = error.error ? error.error : 'Unauthorized';
              this.toastr.error(message, error.status.toString());
              this.router.navigateByUrl("/errors/notloggedin");
              break;
            case 404:
              this.toastr.error(`${error.error.title} (${error.status}) : url ${error.url}`, error.status.toString());
              this.router.navigateByUrl('/errors/notfound');
              break;
            case 500:
              this.toastr.error("An error has happened", error.status.toString())
              const navExtras: NavigationExtras = {state: {error: error.error}};
              this.router.navigateByUrl("/errors/servererror", navExtras);
              break;
            default:
              this.toastr.error("Something unexpcted went wrong", error.status.toString());
              const navExtras2: NavigationExtras = {state: {error: error.error}};
              this.router.navigateByUrl("/errors/servererror", navExtras2);
              break;
          }
        }
        //return throwError(() => new Error(error.error));
        throw error;
      })
    );
  }
}
