# :spades: BridgeHouse - backend for real-time multiplayer bridge playing platform

## :spiral_notepad: Note
Although this repository contains frontend, currently it is not finished, therefore consider this project as **backend project** by now. 

## :star: Overview

Backend for real-time multiplayer bridge playing platform, that provides REST API for following functionalities:
- JWT auth: registration, login, logout
- creating and deleting game tables
- inviting users to a table, accepting/declining invites, leaving table
- playing full game of bridge (pacing bid, playing cards etc.)
- seeing your game history
- getting information abour user, game etc. that will be needed for fullstack application to work

All of requests are available only to eligible users and must be valid (eg. one can't make a move when it's not his turn or user can't invite others to a table unless he created it)

## :hammer_and_wrench: Used technologies
- .NET
- Entity Framework
- SignalR
- Microsoft SQL Server
- Redis
- Docker

## :globe_with_meridians: API endpoints

API provides following endpoints:
- POST /api/authentication/register
- POST /api/authentication/login
- POST /api/authentication/logout

- GET /api/bridge-tables/{id}
- POST /api/bridge-tables
- DELETE /api/bridge-tables/{id}
- POST /api/bridge-tables/{id}/invitations
- DELETE /api/bridge-tables/{id}/invitations

- GET /api/games/{id}/bidding-state
- GET /api/games/{id}/playing-state
- GET /api/games/{id}/cards
- GET /api/games/{id}/current-player
- GET /api/games/{id}/phase
- GET /api/games/{id}/contract
- POST /api/games
- POST /api/games/{id}/bids
- POST /api/games/{id}/card-plays

- GET /api/users/me/id
- GET /api/users/id
- GET /api/users/me/state
- PATCH /api/users/me/invitation
- DELETE /api/users/me/invitation
- GET /api/users/me/game-info
- GET /api/users/me/game-histories/short-info

Endpoints can also be seen at: https://localhost:7200/swagger/index.html

