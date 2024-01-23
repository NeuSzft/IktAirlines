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
('Delta Air Lines'),
('American Airlines'),
('United Airlines'),
('Emirates'),
('Southwest Airlines'),
('China Southern Airlines'),
('China Eastern Airlines'),
('Air China'),
('Air Canada'),
('Qatar Airways');

INSERT INTO cities(name, population) VALUES
('Tokyo', 13960000),
('New York', 8419000),
('Paris', 2148000),
('Moscow', 11920000),
('Beijing', 21540000),
('Sydney', 5312000),
('Rio de Janeiro', 6748000),
('Cairo', 10200000),
('Mumbai', 20411000),
('Los Angeles', 3971000),
('Shanghai', 26320000),
('Istanbul', 15460000),
('Karachi', 16000000),
('Seoul', 9776000),
('Lagos', 14000000),
('Jakarta', 10750000),
('São Paulo', 12300000),
('Mexico City', 9200000),
('Delhi', 19000000),
('Manila', 13900000),
('Toronto', 2930000),
('Berlin', 3645000),
('Madrid', 3265000),
('Bangkok', 10560000),
('Tehran', 8840000),
('Johannesburg', 9570000),
('Buenos Aires', 15100000),
('Taipei', 7870000),
('Lima', 10750000),
('Budapest', 1700000);

INSERT INTO flights(airline_id, origin_id, destination_id, distance, flight_time, huf_per_km) VALUES
(4, 1, 5, 2100, 300, 40),
(1, 2, 10, 4500, 540, 50),
(2, 2, 1, 10800, 650, 60),
(3, 2, 3, 5800, 420, 70),
(5, 10, 2, 4500, 540, 80),
(7, 3, 4, 2800, 320, 45),
(6, 5, 11, 1800, 240, 55),
(8, 4, 6, 9000, 660, 55),
(1, 7, 8, 11000, 780, 50),
(2, 9, 10, 14000, 840, 60),
(3, 11, 12, 2200, 300, 70),
(4, 13, 14, 2000, 280, 40),
(5, 15, 16, 12000, 720, 80),
(6, 17, 18, 9000, 660, 55),
(7, 19, 20, 5000, 360, 45);
