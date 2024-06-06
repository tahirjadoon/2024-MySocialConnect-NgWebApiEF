import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-confirm-modal',
  templateUrl: './confirm-modal.component.html',
  styleUrls: ['./confirm-modal.component.css']
})
export class ConfirmModalComponent implements OnInit {
  title = '';
  message = '';
  btnOkText = '';
  btnCancelText = '';

  result = false;

  constructor(public bsModalRef: BsModalRef){}

  ngOnInit(): void {
    
  }

  confirm(){
    this.result = true;
    this.bsModalRef.hide();
  }

  cancel(){
    this.bsModalRef.hide();
  }
}