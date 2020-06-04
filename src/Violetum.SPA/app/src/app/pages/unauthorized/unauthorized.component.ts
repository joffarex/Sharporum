import {Component, OnDestroy, OnInit} from '@angular/core';

@Component({
  selector: 'app-unauthorized',
  templateUrl: 'unauthorized.component.html'
})
export class UnauthorizedComponent implements OnInit, OnDestroy {
  source = 'unauthorized';

  constructor() {
  }

  ngOnInit() {
  }

  ngOnDestroy() {
  }
}
