import {Component, HostListener, Inject, OnDestroy, OnInit, Renderer2} from '@angular/core';
import {DOCUMENT} from '@angular/common';
import {OidcSecurityService} from 'angular-auth-oidc-client';
import {HttpClient, HttpHeaders} from '@angular/common/http';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
  userId: string;

  constructor(
    private renderer: Renderer2,
    @Inject(DOCUMENT) private document: Document,
    public oidcSecurityService: OidcSecurityService,
    public http: HttpClient,
  ) {
  }

  @HostListener('window:scroll', ['$event'])
  onWindowScroll(e) {
    const element = document.getElementById('navbar-top');

    if (window.pageYOffset > 100) {
      if (element) {
        element.classList.remove('navbar-transparent');
        element.classList.add('bg-primary');
      }
    } else {
      if (element) {
        element.classList.add('navbar-transparent');
        element.classList.remove('bg-primary');
      }
    }
  }

  ngOnInit() {
    this.onWindowScroll(event);
    this.oidcSecurityService
      .checkAuth()
      .subscribe((auth) => {
        console.log('is authenticated', auth);
        if (auth) {
          this.userId = this.oidcSecurityService.getPayloadFromIdToken().sub;
          console.log(this.userId);
        }
      });
  }

  ngOnDestroy() {
    const body = document.getElementsByTagName('body')[0];
    body.classList.remove('index-page');
  }
}
