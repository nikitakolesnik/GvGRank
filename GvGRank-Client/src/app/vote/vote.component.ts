import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { IVote } from './vote';

@Component({
  selector:    'VOTE-COMPONENT',
  templateUrl: './vote.component.html'
})
export class VoteComponent implements OnInit {
  //private apiVote: string = "https://gvgrank.azurewebsites.net/api/vote";
  private apiVote: string = "https://localhost:44329/api/vote";
  
  public player1Name:   string = '_';
  public player2Name:   string = 'Loading...';
  public player1Id:     number = 0;
  public player2Id:     number = 0;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.getVote();
  }


  // UTILITY

  vote1(): void { // Left button click
    this.vote(this.player1Id, this.player2Id);
  }

  vote2(): void { // Right button click
    this.vote(this.player2Id, this.player1Id);
  }

  vote(winId: number, loseId: number): void {
    this.postVote({ WinId: winId, LoseId: loseId });
    this.getVote();
  }


  // API

  

  getVote(): void {
    this.player1Name = '_';
    this.player2Name = 'Loading...';
    this.player1Id   = 0;
    this.player2Id   = 0;

    this.http.get<any>(this.apiVote).subscribe(
      response => {
        this.player1Name = response.name1;
        this.player2Name = response.name2;
        this.player1Id   = response.id1;
        this.player2Id   = response.id2;
      },
      error => {
        console.log(<any>error.message);
        this.player1Name = '_';
        this.player2Name = 'Failed to get data.';
        this.player1Id   = 0;
        this.player2Id   = 0;
      });
  }

  postVote(vote: IVote): void {
    this.http.post<any>(
      this.apiVote,
      JSON.stringify(vote),
      {
        headers: new HttpHeaders({
          'Content-Type': 'application/json',
          'Access-Control-Allow-Origin': '*'
        })
      }).subscribe();
  }

}
