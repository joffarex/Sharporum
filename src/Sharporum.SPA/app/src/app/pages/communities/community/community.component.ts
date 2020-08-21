import {Component, Input, OnDestroy, OnInit} from '@angular/core';

@Component({
  selector: 'app-community',
  templateUrl: 'community.component.html'
})
export class CommunityComponent implements OnInit, OnDestroy {
  @Input() community: any;

  constructor() {
  }

  ngOnInit() {

  }

  ngOnDestroy() {

  }
}
