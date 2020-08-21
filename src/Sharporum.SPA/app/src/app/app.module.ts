import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {APP_INITIALIZER, NgModule} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {RouterModule} from '@angular/router';
import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {HttpClientModule} from '@angular/common/http';

import {PagesModule} from './pages/pages.module';
import {AuthModule, EventTypes, LogLevel, OidcConfigService, PublicEventsService} from 'angular-auth-oidc-client';
import {filter} from 'rxjs/operators';
import {CoreModule} from './core/core.module';

export function configureAuth(oidcConfigService: OidcConfigService) {
  return () =>
    oidcConfigService.withConfig({
      clientId: 'Sharporum.SPA',
      stsServer: 'http://localhost:5000',
      responseType: 'code',
      redirectUrl: window.location.origin,
      postLogoutRedirectUri: window.location.origin,
      scope: 'openid Sharporum.API offline_access',
      logLevel: LogLevel.Warn,
      silentRenew: true,
      useRefreshToken: true,
      forbiddenRoute: '/forbidden',
      unauthorizedRoute: '/unauthorized',
    });
}

@NgModule({
  declarations: [
    AppComponent,
  ],
  imports: [
    BrowserAnimationsModule,
    FormsModule,
    HttpClientModule,
    RouterModule,
    AppRoutingModule,
    AuthModule.forRoot(),
    // BsDropdownModule.forRoot(),
    // ProgressbarModule.forRoot(),
    // TooltipModule.forRoot(),
    // CollapseModule.forRoot(),
    // TabsModule.forRoot(),
    PagesModule,
    CoreModule,
    // PaginationModule.forRoot(),
    // AlertModule.forRoot(),
    // BsDatepickerModule.forRoot(),
    // CarouselModule.forRoot(),
    // ModalModule.forRoot()
  ],
  providers: [OidcConfigService,
    {
      provide: APP_INITIALIZER,
      useFactory: configureAuth,
      deps: [OidcConfigService],
      multi: true,
    },],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor(private readonly eventService: PublicEventsService) {
    this.eventService
      .registerForEvents()
      .pipe(filter((notification) => notification.type === EventTypes.ConfigLoaded))
      .subscribe((config) => {
        console.log('ConfigLoaded', config);
      });
  }
}
