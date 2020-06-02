import {Component, OnInit, OnDestroy, Inject} from '@angular/core';
import {OidcSecurityService} from 'angular-auth-oidc-client';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {DOCUMENT} from '@angular/common';

@Component({
  selector: 'app-index',
  templateUrl: 'index.component.html'
})
export class IndexComponent implements OnInit, OnDestroy {
  focus;
  date = new Date();
  pagination = 3;
  userId: string;

  constructor(
    @Inject(DOCUMENT) private document: Document,
    public oidcSecurityService: OidcSecurityService,
    public http: HttpClient
  ) {
  }

  ngOnInit() {
    this.oidcSecurityService
      .checkAuth()
      .subscribe((auth) => {
        console.log('is authenticated', auth);
        if (auth) {
          this.userId = this.oidcSecurityService.getPayloadFromIdToken().sub;
          console.log(this.userId);
        }
      });

    const body = document.getElementsByTagName('body')[0];
    body.classList.add('index-page');
  }

  login() {
    this.oidcSecurityService.authorize();
  }

  register() {
    this.document.location.href = `http://localhost:5000/auth/register?returnUrl=${window.location.href}`;
  }

  callApi() {
    const token = this.oidcSecurityService.getToken();

    this.http.get('http://localhost:5001/api/v1/posts?OrderByDir=desc&SortBy=VoteCount&CurrentPage=1', {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + token,
      }),
      responseType: 'text',
    })
      .subscribe((data: any) => {
        console.log(JSON.parse(data));

      });
  }

  ngOnDestroy() {
    const body = document.getElementsByTagName('body')[0];
    body.classList.remove('index-page');
  }
}
