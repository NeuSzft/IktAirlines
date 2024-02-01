#!/usr/bin/env bash
docker compose -f compose.test-web.yml down
docker compose -f compose.test-web.yml build api-build-test selenium-tests
docker compose -f compose.test-web.yml up -d
docker compose -f compose.test-web.yml logs -f selenium-tests
docker compose -f compose.test-web.yml down