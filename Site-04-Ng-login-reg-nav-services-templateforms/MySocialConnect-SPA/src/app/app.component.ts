import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HelperService } from './core/services/helper.service';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = '';

  usersPreferred: any;
  errorPreferred: string = "";
  completePreferred: string = "";

  usersNonPreferred: any;
  errorNonPreferred: string = "";
  completeNonPreferred: string = "";

  constructor(private helperService: HelperService, private http: HttpClient){}

  ngOnInit(): void {
    this.title = this.helperService.Title;

    this.getUsersPreferred();
    this.getUsersNonPreferred();
  }

  getUsersPreferred(){
    const url = this.helperService.urlUsersAll;
    this.http.get(url).subscribe({
      next: r => {
        this.usersPreferred = r;
      }, error: e => {
        this.errorPreferred = e;
      }, complete: () => {
        this.completePreferred = "Users Preferred Complete"
      }
    });
  }

  getUsersNonPreferred(){
    const url = this.helperService.urlUsersAll;
    this.http.get(url).subscribe(response => {
        this.usersNonPreferred = response;
      }, error => {
        this.errorNonPreferred = error;
      }, () => {
        this.completeNonPreferred = "Users Non Preferred Complete";
      }
    );
  }
}
