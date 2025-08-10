# :spades: BridgeHouse - backend for real-time multiplayer bridge playing platform

## :spiral_notepad: Note
Although this repository contains frontend, currently it is not finished, therefore consider this project as **backend project** by now. 

## :star: Overview

Backend for real-time multiplayer bridge playing platform, that provides REST API for following functionalities:
- registration, login, logout
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

- GET /api/bridge-tables/{bridgeTableId}
- POST /api/bridge-tables
- POST /api/bridge-tables/{bridgeTableId}/invite/{userId}
- POST /api/bridge-tables/invite/accept/me
- PATCH /api/bridge-tables/{bridgeTableId}/remove-user/{userId}
- PATCH /api/bridge-tables/{bridgeTableId}/leave
- DELETE /api/bridge-tables/{bridgeTableId}
- DELETE /api/bridge-tables/invite/decline/me

- GET /api/game/{gameId}/biddingState
- GET /api/game/{gameId}/playingState
- GET /api/game/{gameId}/contract
- GET /api/game/{gameId}/cards/me
- GET /api/game/{gameId}/cards/dummy
- GET /api/game/{gameId}/playerInfo/current
- GET /api/game/{gameId}/playerInfo/me
- GET /api/game/{gameId}/phase
- POST /api/game/startGame
- POST /api/game/{gameId}/bid
- POST /api/game/{gameId}/bid

- GET /api/game-history/{gameId}
- GET /api/game-history/short-info/me

- GET /api/player-state/invite/me
- GET /api/player-state/table/me
- GET /api/player-state/game/me

- GET /api/users/id/me
- GET /api/users/id/{nickname}

Endpoints can also be seen at: https://localhost:7200/swagger/index.html

