# Airlines Management API
> Version 1.0.0

## Endpoints Table

| Method | Path | Description |
| --- | --- | --- |
| POST | [/airlines](#post-airlines) | Add a new airline. |
| GET | [/airlines](#get-airlines) | Get all airlines. |
| GET | [/airlines/{id}](#get-airlinesid) | Get an airline by id. |
| PUT | [/airlines/{id}](#put-airlinesid) | Overwrite an existing airline. |
| DELETE | [/airlines/{id}](#delete-airlinesid) | Delete an existing airline. |
| | |
| POST | [/cities](#post-cities) | Add a new city. |
| GET | [/cities](#get-cities) | Get all cities. |
| GET | [/cities/{id}](#get-citiesid) | Get a city by id. |
| PUT | [/cities/{id}](#put-citiesid) | Overwrite an existing city. |
| DELETE | [/cities/{id}](#delete-citiesid) | Delete an existing city. |
| | |
| POST | [/flights](#post-flights) | Add a new flight. |
| GET | [/flights](#get-flights) | Get all flights. |
| GET | [/flights/joined](#get-flightsjoined) | Get all flights with the corresponding airline and city information joined to them. |
| GET | [/flights/{id}](#get-flightsid) | Get a flight by id. |
| GET | [/flights/{id}/joined](#get-flightsidjoined) | Get a flight by id with the corresponding airline and city information joined to them. |
| PUT | [/flights/{id}](#put-flightsid) | Overwrite an existing flight. |
| DELETE | [/flights/{id}](#delete-flightsid) | Delete an existing flight. |
| | |
| POST | [/price](#post-price) | Calculate the price of a ticket. |
| GET | [/ping](#get-ping) | A quick way to check if the API is available. |
| GET | [/next-id/airlines](#get-next-idairlines) | Get the next available id for an airline. |
| GET | [/next-id/cities](#get-next-idcities) | Get the next available id for a city. |
| GET | [/next-id/flights](#get-next-idflights) | Get the next available id for a flight. |
| PATCH | [/modify](#patch-modify) | Perform all operations within a single transaction. |
| PATCH | [/modify/test](#patch-modifytest) | Perform all operations within a single transaction then rollback. |

## Schemas Table

| Name | Source File |
| --- | --- |
| [Airline](#airline-model) | [Airline.cs](AirportAPI.Models/Airline.cs) |
| [City](#city-model) | [City.cs](AirportAPI.Models/City.cs) |
| [Flight](#flight-model) | [Flight.cs](AirportAPI.Models/Flight.cs) |
| [FlightJoined](#flightjoined-model) | [FlightJoined.cs](AirportAPI.Models/FlightJoined.cs) |
| [ModificationResults](#modificationresults-model) | [ModificationResults.cs](AirportAPI.Models/ModificationResults.cs) |
| [OperationInfo](#operationinfo-model) | [Operations.cs](AirportAPI.Models/Operations.cs) |
| [Ticket](#ticket-model) | [Ticket.cs](AirportAPI.Models/Ticket.cs) |

### `POST` /airlines
> Add a new airline.

#### RequestBody
- application/json
```ts
{
  id?: integer
  name: string
}
```

#### Responses
- 201 Created
- 400 Bad Request

***

### `GET` /airlines
> Get all airlines.

#### Responses
- 200 OK
`application/json`
```ts
{
  id?: integer
  name: string
}[]
```

***

### `GET` /airlines/{id}
> Get an airline by id.

#### Responses
- 200 OK
`application/json`
```ts
{
  id?: integer
  name: string
}
```
- 404 Not Found

***

### `PUT` /airlines/{id}
> Overwrite an existing airline.

#### RequestBody
`application/json`
```ts
{
  id?: integer
  name: string
}
```

#### Responses
- 200 OK
- 400 Bad Request
- 404 Not Found

***

### `DELETE` /airlines/{id}
> Delete an existing airline.

#### Responses
- 200 OK
- 400 Bad Request

***

### `POST` /cities
> Add a new city.

#### RequestBody
`application/json`
```ts
{
  id?: integer
  name: string
  population: integer
}
```

#### Responses
- 201 Created
- 400 Bad Request

***

### `GET` /cities
> Get all cities.

#### Responses
- 200 OK
`application/json`
```ts
{
  id?: integer
  name: string
  population: integer
}[]
```

***

### `GET` /cities/{id}
> Get a city by id.

#### Responses
- 200 OK
`application/json`
```ts
{
  id?: integer
  name: string
  population: integer
}
```
- 404 Not Found

***

### `PUT` /cities/{id}
> Overwrite an existing city.

#### RequestBody
`application/json`
```ts
{
  id?: integer
  name: string
  population: integer
}
```

#### Responses
- 200 OK
- 400 Bad Request
- 404 Not Found

***

### `DELETE` /cities/{id}
> Delete an existing city.

#### Responses
- 200 OK
- 400 Bad Request

***

### `POST` /flights
> Add a new flight.

#### RequestBody
`application/json`
```ts
{
  id?: integer
  airline_id: integer
  origin_id: integer
  destination_id: integer
  distance: integer
  flight_time: integer
  huf_per_km: integer
}
```

#### Responses
- 201 Created
- 400 Bad Request

***

### `GET` /flights
> Get all flights.

#### Responses
- 200 OK
`application/json`
```ts
{
  id?: integer
  airline_id: integer
  origin_id: integer
  destination_id: integer
  distance: integer
  flight_time: integer
  huf_per_km: integer
}[]
```

***

### `GET` /flights/joined
> Get all flights with the corresponding airline and city information joined to them.

#### Responses
- 200 OK
`application/json`
```ts
{
  id?: integer
  airline: string
  origin_city: string
  origin_city_population: integer
  destination_city: string
  destination_city_population: integer
  distance: integer
  flight_time: integer
  huf_per_km: integer
}[]
```

***

### `GET` /flights/{id}
> Get a flight by id.

#### Responses
- 200 OK
`application/json`
```ts
{
  id?: integer
  airline_id: integer
  origin_id: integer
  destination_id: integer
  distance: integer
  flight_time: integer
  huf_per_km: integer
}[]
```
- 404 Not Found

***

### `GET` /flights/{id}/joined
> Get a flight by id with the corresponding airline and city information joined to them.

#### Responses
- 200 OK
`application/json`
```ts
{
  id?: integer
  airline: string
  origin_city: string
  origin_city_population: integer
  destination_city: string
  destination_city_population: integer
  distance: integer
  flight_time: integer
  huf_per_km: integer
}[]
```
- 404 Not Found

***

### `PUT` /flights/{id}
> Overwrite an existing flight.

#### RequestBody
`application/json`
```ts
{
  id?: integer
  airline_id: integer
  origin_id: integer
  destination_id: integer
  distance: integer
  flight_time: integer
  huf_per_km: integer
}
```

#### Responses
- 200 OK
- 400 Bad Request
- 404 Not Found

***

### `DELETE` /flights/{id}
> Delete an existing flight.

#### Responses
- 200 OK
- 400 Bad Request

***

### `POST` /price
> Calculate the price of a ticket.

#### RequestBody
- application/json
```ts
{
  id?: integer
  adults: integer
  children: integer
}
```

#### Responses
- 200 OK
`text/plain`
```
int32 as string
```
- 400 Bad Request

***

### `GET` /ping
> A quick way to check if the API is available.

#### Responses
- 200 OK

***

### `GET` /next-id/airlines
> Get the next available id for an airline.

#### Responses
- 200 OK
`text/plain`
```
int32 as string
```
- 404 Not Found

***

### `GET` /next-id/cities
> Get the next available id for a city.

#### Responses
- 200 OK
`text/plain`
```
int32 as string
```
- 404 Not Found

***

### `GET` /next-id/flights
> Get the next available id for a flight.

#### Responses
- 200 OK
`text/plain`
```
int32 as string
```
- 404 Not Found

***

### `PATCH` /modify
> Perform all operations within a single transaction.

#### RequestBody
- application/json

```ts
{
  operation_name: string
  model_name: string
  id?: integer
}[]
```

#### Responses
- 200 OK
`text/plain`
```
string
```
- 400 Bad Request
- 422 Unprocessable Entity
`text/plain`
```
string
```

***

### `PATCH` /modify/test
> Perform all operations within a single transaction then rollback.
> Returns the number of rows affected and the airlines, cities and flights tables in their pre-rollback state.

#### RequestBody
- application/json
```ts
{
  operation_name: string
  model_name: string
  id?: integer
}[]
```

#### Responses
- 200 OK
`application/json`
```ts
{
  rows_affected: integer
  airlines: {
    id?: integer
    name: string
  }[]
  cities: {
    id?: integer
    name: string
    population: integer
  }[]
  flights: {
    id?: integer
    airline_id: integer
    origin_id: integer
    destination_id: integer
    distance: integer
    flight_time: integer
    huf_per_km: integer
  }[]
}
```
- 400 Bad Request
- 422 Unprocessable Entity
`text/plain`
```
string
```

## Schemas

### Airline Model

```ts
{
  id?: integer
  name: string
}
```

### City Model

```ts
{
  id?: integer
  name: string
  population: integer
}
```

### Flight Model

```ts
{
  id?: integer
  airline_id: integer
  origin_id: integer
  destination_id: integer
  distance: integer
  flight_time: integer
  huf_per_km: integer
}
```

### FlightJoined Model

```ts
{
  id?: integer
  airline: string
  origin_city: string
  origin_city_population: integer
  destination_city: string
  destination_city_population: integer
  distance: integer
  flight_time: integer
  huf_per_km: integer
}
```

### ModificationResults Model

```ts
{
  rows_affected: integer
  airlines: {
    id?: integer
    name: string
  }[]
  cities: {
    id?: integer
    name: string
    population: integer
  }[]
  flights: {
    id?: integer
    airline_id: integer
    origin_id: integer
    destination_id: integer
    distance: integer
    flight_time: integer
    huf_per_km: integer
  }[]
}
```

### OperationInfo Model

```ts
{
  operation_name: string
  model_name: string
  id?: integer
  item?: object
}
```

### Ticket Model

```ts
{
  id?: integer
  adults: integer
  children: integer
}
```