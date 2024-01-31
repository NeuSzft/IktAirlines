#!/usr/bin/env bash
docker compose -f compose.test-web.yml down
docker compose -f compose.test-web.yml build api-build-test selenium-tests
docker compose -f compose.test-web.yml up -d

#TODO: Fix this error
#Initialization method AirportWebTest.SeleniumTests.InitializeTest threw exception.                                                     'http://selenium-tests:4444/session' won't work either.
#OpenQA.Selenium.WebDriverException: An unknown exception was encountered sending an HTTP request to the remote WebDriver server for URL http://localhost:4444/session. 
#The exception message was: Cannot assign requested address (localhost:4444) 
#    ---> System.Net.Http.HttpRequestException: Cannot assign requested address (localhost:4444) 
#    ---> System.Net.Sockets.SocketException: Cannot assign requested address.

#Maybe can't reach SutHub="http://localhost:4444" when initializing RemoteWebDriver()

#docker compose -f compose.test-web.yml logs -f selenium-tests  

dotnet test AirportWebTest/AirportWebTest/AirportWebTest.csproj
docker compose -f compose.test-web.yml down
