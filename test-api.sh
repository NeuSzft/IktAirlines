#!/usr/bin/env bash
docker compose -f compose.test-api.yml down
docker compose -f compose.test-api.yml build test-api-build test-api-tests
docker compose -f compose.test-api.yml up -d
docker compose -f compose.test-api.yml logs -f test-api-tests
docker compose -f compose.test-api.yml down
docker run -v ./TestResults:/tr alpine /usr/bin/env chown -R $UID:$GID /tr
