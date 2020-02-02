import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IPlayer } from './player';

@Component({
  selector: 'LEADERBOARD-COMPONENT',
  templateUrl: './leaderboard.component.html'
})
export class LeaderboardComponent implements OnInit {
  //private api: string = "https://gvgrank.azurewebsites.net/api/leaderboard";
  private api: string = "https://localhost:44329/api/leaderboard";

  public leaderboard:  Array<IPlayer>;
  public playerFilter: string;

  /*
   * Possible solutions for this...
   *   1) Refresh the leaderboard every X seconds
   *   2) Manual refresh button only
   *   3) Get a hash every X seconds, compare, update if needed.
   *   4) Server sends new leaderboard when the order is changed
   *      4.5) Hard mode -> only send list changes
   */

  constructor(private http: HttpClient) { // Refresh every minute
    setInterval(() => this.getLeaderboard(), 60000);
  }

  ngOnInit(): void {
    this.getLeaderboard();
  }


  // UTILITY

  clear() {
    this.playerFilter = '';
  }

  filterList(arr: IPlayer[]): IPlayer[] {
    return arr.filter(x => x.name.toLowerCase().includes(this.playerFilter.toLowerCase()));
  }

  numberArray(arr: IPlayer[]): IPlayer[] {
    var frontCount = 1, midCount = 1, backCount = 1;

    for (var i = 0; i < arr.length; i++) {
      var extraSpace = (i < 10) ? ' ' : '';
      switch (arr[i].role) {
        case 1: arr[i].name = frontCount++ + ') ' + extraSpace + arr[i].name; break;
        case 2: arr[i].name = midCount++   + ') ' + extraSpace + arr[i].name; break;
        case 3: arr[i].name = backCount++  + ') ' + extraSpace + arr[i].name; break;
      }
    }

    return arr;
  }


  // API

  getLeaderboard(): void {
    this.http.get<IPlayer[]>(this.api)
      .subscribe(
        response =>  this.leaderboard = this.numberArray(<IPlayer[]>response),
        () => this.leaderboard = [{ name: 'Failed to load.', role: 1 }, { name: 'Failed to load.', role: 2 }, { name: 'Failed to load.', role: 3 }]
    );
  }

}
