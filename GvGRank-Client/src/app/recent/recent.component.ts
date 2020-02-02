import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as signalR from "@aspnet/signalr";
import { LeaderboardComponent } from '../leaderboard/leaderboard.component';

@Component({
  selector:    'RECENT-COMPONENT',
  templateUrl: './recent.component.html'
})
export class RecentComponent implements OnInit {
  //private apiRecent:   string = "https://gvgrank.azurewebsites.net/api/recentvotes";
  //private apiVCount:   string = "https://gvgrank.azurewebsites.net/api/voteCount";
  //private signalrConn: string = "https://gvgrank.azurewebsites.net/recentvote";
  private apiRecent:   string = "https://localhost:44329/api/recentvotes";
  private apiVCount:   string = "https://localhost:44329/api/voteCount";
  private signalrConn: string = "https://localhost:44329/recentvote";

  private hubConnection: signalR.HubConnection;
  private count: number = 12; // Number of entries to display, dictated by card size

  public recentVotes:         string[][];
  public filteredRecentVotes: string[][];
  public filteringResults:    boolean = false;
  public playerFilter:        string;
  public voteCount:           number = -1; // Value of -1 will hide the element

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.startConnection();
    this.addTransferChartDataListener();

    this.getVoteCount();
  }


  // UTILITY

  update(vote: string[]): void {
    if (this.recentVotes.length == this.count) {
      this.recentVotes.pop();
    }
    this.recentVotes.unshift(vote);
  }

  search(): void {
    if (this.playerFilter) {
      this.filteringResults = true;
      this.getRecentVotes();
    }
  }

  reset(): void {
    this.filteringResults = false;
    this.playerFilter = '';
  }


  // API

  getVoteCount(): void {
    this.http.get<any>(this.apiVCount).subscribe(
      response => {
        this.voteCount = response
        this.getRecentVotes();
      },
      error => {
        this.voteCount = -1; // Value of -1 will hide the element
        //console.log(<any>error.message);
      });
  }

  getRecentVotes(): void {
    this.filteredRecentVotes = [];

    var apiString: string = this.apiRecent + '?count=' + this.count;

    if (this.filteringResults)
      apiString += '&player=' + this.playerFilter;

    this.http.get<any>(apiString).subscribe(
      response => {
        (this.filteringResults) ? this.filteredRecentVotes = response : this.recentVotes = response;
      },
      error => {
        //console.log(<any>error.message);
        (this.filteringResults) ? this.filteredRecentVotes = [['Failed to load recent votes', '']] : this.recentVotes = [['Failed to load recent votes','']];
      }
    );
  }


  // SIGNALR

  public startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.signalrConn
        , {                                               // https://stackoverflow.com/a/52913505/10874809
          skipNegotiation: true,                          // ^
          transport: signalR.HttpTransportType.WebSockets // ^
        }                                                 // ^
      )
      .build();

    this.hubConnection
      .start()
      //.then(() => console.log('SignalR Connection started'))
      //.catch(err => console.log('SignalR  Connection failed: ' + err))
      ;
  }

  public addTransferChartDataListener = () => {
    this.hubConnection.on('recentvote', (data) => {
      this.update(data);
      this.voteCount++;
    });
  }
}
