import {Component, OnDestroy, OnInit} from '@angular/core';

@Component({
  selector: 'app-communities',
  templateUrl: 'communities.component.html'
})
export class CommunitiesComponent implements OnInit, OnDestroy {
  searchType = 'Community Name';
  communities: any[] = [
    {
      name: 'community 1',
    },
    {
      name: 'community 2',
    },
  ];

  constructor() {
  }

  ngOnInit() {

  }

  ngOnDestroy() {

  }
}
