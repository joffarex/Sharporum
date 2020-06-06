import {Component, OnInit} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {OidcSecurityService} from 'angular-auth-oidc-client';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
})
export class NavbarComponent implements OnInit {
  isCollapsed = true;

  constructor(
    public oidcSecurityService: OidcSecurityService,
    public http: HttpClient,
  ) {
  }

  ngOnInit(): void {
  }

  login() {
    this.oidcSecurityService.authorize();
  }

  logout() {
    this.oidcSecurityService.logoff();
  }

  callApi() {
    const token = this.oidcSecurityService.getToken();

    this.http.get('http://localhost:5001/secret', {
      headers: new HttpHeaders({
        Authorization: 'Bearer ' + token,
      }),
      responseType: 'text',
    })
      .subscribe((data: any) => {
        console.log(JSON.parse(data));

      });
  }

}
