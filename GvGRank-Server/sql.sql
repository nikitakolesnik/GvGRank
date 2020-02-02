/* USER VOTE COUNT */ --select count(*) from votes where userid=7
/* VOTES PER USER  */ --select userid, count(userid) from votes group by userid order by count(userid) desc
/* VOTES ON PLAYER */ --select  v.userid,format(v.[date], 'hh:mm:ss') as [time]  /*v.[date]*/, p1.[name] as W, p2.[name] as L from votes v join players p1 on v.winid = p1.id join players p2 on v.loseid = p2.id where p2.id=126 /*and v.[date] >= dateadd(day, -1, getdate())*/ order by [date] desc 

/* PLAYER NAME/ID  */ --select [name], id from players order by [name]
/* PLAYER STATS    */ --select shitlo, [name], wins, losses from players where role=3 order by shitlo desc

/* USER STATS      */ --select * from users where id=7
/* VOTE COUNT      */ --select sum(wins) as TotalVotes from players
/* USER COUNT      */ --select count(*) from users

/* USER'S VOTES    */ --select p1.[name] as win, p2.[name] as lose from votes join players p1 on votes.winid = p1.id join players p2 on votes.loseid = p2.id where userid = 377 order by [date] desc
/* BAN USER        */ --update users set antitamper=999999 where id = 155
/* RECENT VOTES    */ --select top 20 v.id, v.[date], v.userid, p1.[name], p2.[name], v.winid, v.loseid from votes v join players p1 on v.winid=p1.id join players p2 on v.loseid=p2.id order by [date] desc
/* USER VOTE PURGE */ --delete from votes where userid in (73,74,75,76,78,79,81,95,96,101,102,104,105,106,107,108,109,122,123,124,125,126,127,132,135,136,137,138,139,141,143,144,145,146,147,155,162,190,205,241,242,243,244,245,246,247,248,249,250,251,252,253,254,255,257,260,262,264)

select  count(*) from votes where [date] >= dateadd(day, -5, getdate())