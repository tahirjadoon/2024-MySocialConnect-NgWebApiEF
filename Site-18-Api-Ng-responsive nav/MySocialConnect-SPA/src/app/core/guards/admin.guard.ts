import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { map } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

import { AccountService } from '../services/account.service';
import { ZRoles } from '../enums/z-roles';

export const adminGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const toastr = inject(ToastrService);

  //account service is firing the logged in user
  //inside the component/interceptor we subscribe
  //inside guards
  return accountService.currentLoggedInUser$.pipe(
    map(user => {
      if(!user) return false;

      if(user.roles && (user.roles.includes(ZRoles.Admin) || user.roles.includes(ZRoles.Moderator)))
        return true;

      toastr.error('You cannot enter this area', 'Restricted Area');
      return false;
    })
  );

  return true;
};
