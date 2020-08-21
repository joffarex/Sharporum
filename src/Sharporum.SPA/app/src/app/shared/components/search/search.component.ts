import {Component, Input, OnDestroy, OnInit} from '@angular/core';

@Component({
  selector: 'app-search',
  templateUrl: 'search.component.html'
})
export class SearchComponent implements OnInit, OnDestroy {
  @Input() searchType: string;

  constructor() {
  }

  ngOnInit() {

  }

  ngOnDestroy() {

  }
}
