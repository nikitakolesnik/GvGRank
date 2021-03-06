An API & web client which identified users by their IP address, and would repeatedly present them with a vote between two similarly rated players while showing other users' activity in real time. It also built a ladder from the resulting rating, and allowed users to search for recent votes on specific players.

This was live for three weeks. I expected it to accrue 5,000 votes over its lifetime, which it broke in the first hour. By the time I shut it down, there had been over 50,000 votes made (60,000 including votes from users which I purged for extreme low-integrity use) and a few hundred unique users.

While I'm happy with the results and have even given the source code a much needed "face-lift" for the sake of being presentable, I've learned a lot since writing this and feel that my other project ("slambot") is more representative of my organization and coding style.

Screenshot: 
https://i.imgur.com/iqfWkmM.png

Rating system implementation: 
https://github.com/nikitakolesnik/GvGRank/blob/master/GvGRank-Server/Services/VoteRepository.cs#L281

Matchmaking implementation: 
https://github.com/nikitakolesnik/GvGRank/blob/master/GvGRank-Server/Services/VoteRepository.cs#L149
