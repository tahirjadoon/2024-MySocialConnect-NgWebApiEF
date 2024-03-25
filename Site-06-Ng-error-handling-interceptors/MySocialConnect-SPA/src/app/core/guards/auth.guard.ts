import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot } from '@angular/router';
import { AccountService } from '../services/account.service';
import { ToastrService } from 'ngx-toastr';
import { map } from 'rxjs';
import { LoggedInUserDto } from '../models-interfaces/logged-in-user-dto.model';

export const authGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {

  const accountService = inject(AccountService);
  const toastr = inject(ToastrService);
  const router = inject(Router);
  
  return accountService.currentLoggedInUser$.pipe(
    map(user => {
      if(user)
        return true;
      else{
        //in real time we do not do this.
        toastr.error("You are not authorized to view the resource. Login please!");
        //redirect to the error page
        router.navigate(["/errors/notloggedin"], {queryParams: { returnUrl: state.url } });
        return false;
      }
    })
  );
};
