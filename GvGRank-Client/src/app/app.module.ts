import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from "@angular/forms";
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { VoteComponent } from './vote/vote.component';
import { LeaderboardComponent } from './leaderboard/leaderboard.component';
import { RecentComponent } from './recent/recent.component';

@NgModule({
  declarations: [ AppComponent, VoteComponent, LeaderboardComponent, RecentComponent ],
  imports:      [ BrowserModule, FormsModule, HttpClientModule ],
  bootstrap:    [ AppComponent ]
})
export class AppModule { }
