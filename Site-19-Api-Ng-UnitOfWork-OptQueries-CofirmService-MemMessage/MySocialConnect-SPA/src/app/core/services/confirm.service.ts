import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ConfirmModalComponent } from '../../site/modals/confirm-modal/confirm-modal.component';
import { Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {

  //modal service also used in site/admin/userManagement component

  bsModalRef: BsModalRef<ConfirmModalComponent> = new BsModalRef<ConfirmModalComponent>();
  
  constructor(private modalService: BsModalService) { }

  confirm() : Observable<boolean>{
    return this.confirmCustom('Confirmation', 'Are you sure you want to do this?', 'Ok','Cancel');
  }

  confirmCustom (title: string, message: string, btnOkText: string, btnCancelText: string) : Observable<boolean>{

    const config = {
      //same property names
      initialState: {
        title,
        message, 
        btnOkText,
        btnCancelText
      }
    }    
    
    this.bsModalRef = this.modalService.show(ConfirmModalComponent, config);

    return this.bsModalRef.onHidden!.pipe(
      map(() => {
        return this.bsModalRef!.content!.result;
      })
    );
    
  }


}
