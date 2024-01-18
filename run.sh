#!/usr/bin/env bash
docker-compose down || docker compose down
docker-compose build api-build || docker compose build api-build
docker-compose up -d || docker compose up -d
