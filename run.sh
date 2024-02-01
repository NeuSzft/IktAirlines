#!/usr/bin/env bash
docker compose down
docker compose build api-build web-build
docker compose up -d
docker compose logs
