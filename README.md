# IsJackBadAtLoL
Joke app that I built to showcase my friend Jack's League of Legends match history because I thought it would be funny. It features a header which highlights the total number of matches
tracked by the app since its inception, and across those the total number of deaths and time spent dead.



The highlight reel shows the worst 3 manages in the past week, ordered by number of deaths and where the number of deaths is greater than the number of kills.

By default, in the match history section, the most recent 9 matches are shown. This can be expanded by clicking the "Load More" button at the bottom, which will load an addition
9 each time it is clicked. This can be collapsed by 9 at a time by clicking "Load Fewer".

I built this in a few hours, so the code quality is pretty poor. However I might extend it's features, and clean it up a bit in the future.

# Live Site
You can view it in action at https://isjackbadatlol.azurewebsites.net/

# Tech/Architecture
Everything is hosted on Azure, with the following:

- An Azure SQL DB being used to store match data
- Azure Function that runs on a cron schedule (currently every 15 minutes) that pulls match data from the League of Legends API and stores it in the DB
- Blazor web app to provide the UI for the match data
