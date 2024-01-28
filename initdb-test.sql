CREATE TABLE airlines (
	id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	name VARCHAR(50) NOT NULL
);

CREATE TABLE cities (
	id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	name VARCHAR(50) NOT NULL,
	population INT NOT NULL
);

CREATE TABLE flights (
	id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	airline_id INT REFERENCES airlines(id),
	origin_id INT REFERENCES cities(id),
	destination_id INT REFERENCES cities(id),
	distance INT NOT NULL,
	flight_time INT NOT NULL,
	huf_per_km INT NOT NULL
);

INSERT INTO airlines(name) VALUES
('airline-1'),
('airline-2'),
('airline-3');

INSERT INTO cities(name, population) VALUES
('city-1', 1000),
('city-2', 2000),
('city-3', 3000),
('city-4', 4000),
('city-5', 5000),
('city-6', 6000);

INSERT INTO flights(airline_id, origin_id, destination_id, distance, flight_time, huf_per_km) VALUES
(1, 1, 2, 300, 30, 3),
(2, 3, 4, 500, 50, 5),
(3, 5, 6, 700, 70, 7);
