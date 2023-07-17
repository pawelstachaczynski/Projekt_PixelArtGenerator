import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { ConfigStore } from 'src/app/app-config/config-store';


@Component({
  selector: 'app-spinner',
  templateUrl: './spinner.component.html',
  styleUrls: ['./spinner.component.scss']
})
export class SpinnerComponent implements OnInit, OnDestroy{
  loadingPanelVisible : boolean;

  loadingPanelSub : Subscription;

  constructor(private configStore: ConfigStore)
  { }

  ngOnInit(): void {
    this.loadingPanelSub = this.configStore.loadingPanel.subscribe((value) => {
      console.log('spinner')
      this.loadingPanelVisible = value;
    })
  }

  ngOnDestroy(): void {
    this.loadingPanelSub.unsubscribe();
  }
}
