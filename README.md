# :spades: BridgeHouse - backend for real-time multiplayer bridge playing platform

## :spiral_notepad: Note
Although this repository contains frontend, currently it is not finished, therefore consider this project as **backend project** by now. 

## :star: Overview

Backend for real-time multiplayer bridge playing platform, that provides REST API for following functionalities:
- JWT authentication: registration, login, logout
- creating and deleting game tables
- inviting users to a table, accepting/declining invites
- playing full game of bridge (pacing bid, playing cards etc.)
- seeing your game history
- getting information abour user, game etc. that will be needed for fullstack application to work

All of requests are available only to eligible users and must be valid (eg. one can't make a move when it's not his turn or user can't invite others to a table unless he created it)

## :hammer_and_wrench: Used technologies
- .NET
- Entity Framework
- SignalR
- Redis
- Docker

## :globe_with_meridians: API endpoints

API provides following endpoints:


Endpoints can also be seen at: https://localhost:7200/swagger/index.html

