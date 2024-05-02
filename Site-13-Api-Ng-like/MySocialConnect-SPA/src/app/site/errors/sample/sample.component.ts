import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';

import { HelperService } from '../../../core/services/helper.service';
import { HttpClientService } from '../../../core/services/http-client.service';

@Component({
  selector: 'app-sample',
  templateUrl: './sample.component.html',
  styleUrls: ['./sample.component.css']
})
export class SampleComponent implements OnInit, OnDestroy {

  constructor(private helperService: HelperService, private httpClientService: HttpClientService) {}

  modelValidationErrors: string[] = [];

  subscription404!: Subscription;
  subscription400!: Subscription;
  subscription500!: Subscription;
  subscription401!: Subscription;
  subscription40val!: Subscription;

  ngOnDestroy(): void {
    if(this.subscription404) this.subscription404.unsubscribe();
    if(this.subscription400) this.subscription400.unsubscribe();
    if(this.subscription500) this.subscription500.unsubscribe();
    if(this.subscription401) this.subscription401.unsubscribe();
    if(this.subscription40val) this.subscription40val.unsubscribe();
  }

  ngOnInit(): void {
    
  }

  onGet404Error(){
    const url = this.helperService.urlSampleUserNotFound;
    this.subscription404 = this.makeCallGet(url, 'get404Error');
  }

  onGet400Error(){
    const url = this.helperService.urlSampleBadRequest;
    this.subscription400 = this.makeCallGet(url, 'get400Error');
  }

  onGet500Error(){
    const url = this.helperService.urlSampleServerError;
    this.subscription500 = this.makeCallGet(url, 'get500Error');
  }

  onGet401Error(){
    const url = this.helperService.urlSampleAuth;
    this.subscription401 = this.makeCallGet(url, 'get401Error');
  }

  onGet400ValidationErrors(){
    const url = this.helperService.urlSampleLogin;
    this.subscription40val = this.makeCallPost(url, 'get400ValidationErrors',  {})
  }

  private makeCallGet(url: string, title: string){
    return this.httpClientService.get<any>(url).subscribe({
      next: (resposne: any) => {
        this.helperService.logIfFrom(resposne, `Sample - ${title}`);
      },
      error: e => {
        this.helperService.logIfError(e, `Sample - ${title}`)
      },
      complete: () => {}
    });
  }

  private makeCallPost(url: string, title: string, payload: any){
    this.modelValidationErrors = [];
    return this.httpClientService.post<any>(url, payload).subscribe({
      next: (resposne: any) => {
        this.helperService.logIfFrom(resposne, `Sample - ${title}`);
      },
      error: e => {
        this.helperService.logIfError(e, `Sample - ${title}`);
        this.modelValidationErrors = e;
      },
      complete: () => {}
    });
  }
}
