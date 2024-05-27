import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouteReuseStrategy } from '@angular/router';

import { SharedModule } from './core/modules/shared.module';

import { AppComponent } from './app.component';
import { NavComponent } from './site/nav/nav.component';

import { NotFoundComponent } from './site/errors/not-found/not-found.component';
import { ServerErrorComponent } from './site/errors/server-error/server-error.component';

import { ErrorInterceptor } from './core/interceptors/error.interceptor';
import { JwtInterceptor } from './core/interceptors/jwt.interceptor';
import { LoadingInterceptor } from './core/interceptors/loading.interceptor';

import { CustomRouteReuseStrategy } from './core/Strategy/customRouteReuseStrategy';

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
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true },
    { provide: RouteReuseStrategy, useClass: CustomRouteReuseStrategy},
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
