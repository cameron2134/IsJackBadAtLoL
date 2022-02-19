# Overview
League of Feeders is a joke web application I built to track my friends and my own match history from playing League of Legends. You can click the dropdown at the top of the screen to pick a person's match history to browse, where the UI will update to reflect your choice. Along the top, the person's all-time matches played, total deaths and total time spent is displayed, and the rest of the UI contains their match history along with a highlight reel.

The highlight reel displays the worst three matches they have played in the past 7 days, ordered by the difference between kills and deaths, and the total number of deaths, in descending order.

Below the main statistics along the top is the "Feeder of the Week" which is updated every Friday at 5PM UTC, to showcase who performed the worst that week.

By default, in the match history section, the most recent 9 matches are shown. This can be expanded by clicking the "Load More" button at the bottom, which will load an additional 9 each time it is clicked. This can be collapsed by 9 at a time by clicking "Load Fewer".

I built this up pretty quickly and didn't invest a huge amount of time in it, so the code quality isn't amazing. 

# Project Structure
- Core: Contains the domain models and DTO's shared between the web app and Azure Functions.
- Database: Contains the full DB schema.
- MatchDataRequester: Contains the two Azure Functions and associated logic required for them to run.
- Repo: Pretty much just contains the shared DB context currently.
- WebApp: The Blazor web application.

# Design
As mentioned above, this was built pretty quickly so it most certainly wasn't designed to be scalable in any way.

## Managing API Data
As I needed to regularly updated data for multiple people, I obviously didn't want to pull a lot of match history data on every load of the application. So, I set up an Azure timer Function to run every 15 minutes that iterates through all summoners in my DB and pulls their match data from the Riot API, updating the match data in the DB. 

With how it is set up currently, it is not particularly scalable due to looping round X summoners pulling in Y matches each time, but it was only intended to supported a small number of people so it works just as planned.

## New Users
The design around new users was implemented so that I could simply pop someones summoner name into the DB, and their unique ID will automatically be pulled from the API along with their match history. However, as there's no account system in place, I did not want this publically exposed on the UI, so I manually add people on request (which is fine since it's just a web app for between friends).

## Entity Framework
I typically set up a repo/service layer when working with EF, however since the read/write operations are small and simple, instead of having additional project overhead, I have a single repo project that contains the DB context to share between the functions and web app, and just inject the context directly where it is required.


# Live Site
You can view it in action at https://league-of-feeders.com/

# Tech/Architecture
Everything is hosted on Azure, with the following:

- An Azure SQL DB being used to store all match data
- Two Azure Functions running on cron schedules:
  - One that runs every 15 mins, pulling in match data from Riot's API and storing it in the DB
  - Another one that runs once a week, calculating the weekly feeder
- Blazor web app to provide the UI for the match data

## Azure Resources
The following Azure resources are in use:
- The resource group to bunch everything together
- An App Service with an SSL certificate to host the web application
- An Azure SQL DB
- Azure Functions on the Consumption plan
