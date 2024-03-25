import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { SharedModule } from './core/modules/shared.module';

import { AppComponent } from './app.component';
import { NavComponent } from './site/nav/nav.component';

import { NotFoundComponent } from './site/errors/not-found/not-found.component';
import { ServerErrorComponent } from './site/errors/server-error/server-error.component';


@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    NotFoundComponent, 
    ServerErrorComponent,  
  ],
  imports: [
    BrowserModule,
    SharedModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
