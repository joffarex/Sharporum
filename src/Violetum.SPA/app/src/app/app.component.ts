import {Component, OnInit} from '@angular/core';
import {OidcSecurityService} from 'angular-auth-oidc-client';
import {HttpClient, HttpHeaders} from '@angular/common/http';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  userId: string;

  constructor(
    public oidcSecurityService: OidcSecurityService,
    public http: HttpClient) {
  }

  ngOnInit() {
    this.oidcSecurityService
      .checkAuth()
      .subscribe((auth) => console.log('is authenticated', auth));
  }

  login() {
    this.oidcSecurityService.authorize();
  }

  callApi() {
    const token = this.oidcSecurityService.getToken();

    this.http.get("http://localhost:5001/api/v1/posts?OrderByDir=desc&SortBy=VoteCount&CurrentPage=1", {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + token,
      }),
      responseType: 'text',
    })
      .subscribe((data: any) => {
        console.log(JSON.parse(data));
        this.userId = this.oidcSecurityService.getPayloadFromIdToken().sub;
        console.log(this.userId);
      });
  }
}
