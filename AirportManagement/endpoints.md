# Airlines Management API

> Version 1.0.0

## Path Table

| Method | Path | Description |
| --- | --- | --- |
| GET | [/airlines](#getairlines) | Get all airlines. |
| POST | [/airlines](#postairlines) | Add a new airline. |
| DELETE | [/airlines/{id}](#deleteairlinesid) | Delete an existing airline. |
| GET | [/airlines/{id}](#getairlinesid) | Get an airline by id. |
| PUT | [/airlines/{id}](#putairlinesid) | Overwrite an existing airline. |
| GET | [/cities](#getcities) | Get all cities. |
| POST | [/cities](#postcities) | Add a new city. |
| DELETE | [/cities/{id}](#deletecitiesid) | Delete an existing city. |
| GET | [/cities/{id}](#getcitiesid) | Get a city by id. |
| PUT | [/cities/{id}](#putcitiesid) | Overwrite an existing city. |
| GET | [/flights](#getflights) | Get all flights. |
| POST | [/flights](#postflights) | Add a new flight. |
| GET | [/flights/joined](#getflightsjoined) | Get all flights with the corresponding airline and city information joined to them. |
| DELETE | [/flights/{id}](#deleteflightsid) | Delete an existing flight. |
| GET | [/flights/{id}](#getflightsid) | Get a flight by id. |
| PUT | [/flights/{id}](#putflightsid) | Overwrite an existing flight. |
| GET | [/flights/{id}/joined](#getflightsidjoined) | Get a flight by id with the corresponding airline and city information joined to them. |
| PATCH | [/modify](#patchmodify) | Perform all operations within a single transaction. |
| PATCH | [/modify/test](#patchmodifytest) | Perform all operations within a single transaction then rollback. |
| GET | [/next-id/airlines](#getnext-idairlines) | Get the next available id for an airline. |
| GET | [/next-id/cities](#getnext-idcities) | Get the next available id for a city. |
| GET | [/next-id/flights](#getnext-idflights) | Get the next available id for a flight. |
| GET | [/ping](#getping) | A quick way to check if the API is available. |
| POST | [/price](#postprice) | Calculate the price of a ticket. |

## Reference Table

| Name | Path | Description |
| --- | --- | --- |
| Airline | [#/components/schemas/Airline](#componentsschemasairline) |  |
| City | [#/components/schemas/City](#componentsschemascity) |  |
| Flight | [#/components/schemas/Flight](#componentsschemasflight) |  |
| FlightJoined | [#/components/schemas/FlightJoined](#componentsschemasflightjoined) |  |
| ModificationResults | [#/components/schemas/ModificationResults](#componentsschemasmodificationresults) |  |
| OperationInfo | [#/components/schemas/OperationInfo](#componentsschemasoperationinfo) |  |
| Ticket | [#/components/schemas/Ticket](#componentsschemasticket) |  |

### [GET]/airlines

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

### [POST]/airlines

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

### [DELETE]/airlines/{id}

> Delete an existing airline.

#### Responses

- 200 OK

- 400 Bad Request

***

### [GET]/airlines/{id}

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

### [PUT]/airlines/{id}

> Overwrite an existing airline.

#### RequestBody

- application/json

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

### [GET]/cities

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

### [POST]/cities

> Add a new city.

#### RequestBody

- application/json

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

### [DELETE]/cities/{id}

> Delete an existing city.

#### Responses

- 200 OK

- 400 Bad Request

***

### [GET]/cities/{id}

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

### [PUT]/cities/{id}

> Overwrite an existing city.

#### RequestBody

- application/json

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

### [GET]/flights

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

### [POST]/flights

> Add a new flight.

#### RequestBody

- application/json

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

### [GET]/flights/joined

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

### [DELETE]/flights/{id}

> Delete an existing flight.

#### Responses

- 200 OK

- 400 Bad Request

***

### [GET]/flights/{id}

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

### [PUT]/flights/{id}

> Overwrite an existing flight.

#### RequestBody

- application/json

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

### [GET]/flights/{id}/joined

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

### [PATCH]/modify

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

```ts
{
  "type": "integer",
  "format": "int32"
}
```

- 400 Bad Request

- 422 Unprocessable Entity

`text/plain`

```ts
{
  "type": "string"
}
```

***

### [PATCH]/modify/test

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

```ts
{
  "type": "string"
}
```

***

### [GET]/next-id/airlines

> Get the next available id for an airline.

#### Responses

- 200 OK

`text/plain`

```ts
{
  "type": "integer",
  "format": "int32"
}
```

- 404 Not Found

***

### [GET]/next-id/cities

> Get the next available id for a city.

#### Responses

- 200 OK

`text/plain`

```ts
{
  "type": "integer",
  "format": "int32"
}
```

- 404 Not Found

***

### [GET]/next-id/flights

> Get the next available id for a flight.

#### Responses

- 200 OK

`text/plain`

```ts
{
  "type": "integer",
  "format": "int32"
}
```

- 404 Not Found

***

### [GET]/ping

> A quick way to check if the API is available.

#### Responses

- 200 OK

***

### [POST]/price

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

`application/json`

```ts
{
  "type": "integer",
  "format": "int32"
}
```

- 400 Bad Request

## References

### #/components/schemas/Airline

```ts
{
  id?: integer
  name: string
}
```

### #/components/schemas/City

```ts
{
  id?: integer
  name: string
  population: integer
}
```

### #/components/schemas/Flight

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

### #/components/schemas/FlightJoined

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

### #/components/schemas/ModificationResults

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

### #/components/schemas/OperationInfo

```ts
{
  operation_name: string
  model_name: string
  id?: integer
  item?: object
}
```

### #/components/schemas/Ticket

```ts
{
  id?: integer
  adults: integer
  children: integer
}
```