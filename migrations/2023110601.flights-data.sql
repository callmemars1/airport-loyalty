-- Аэропорты
INSERT INTO airports (name, iata_code, city, country)
VALUES ('Москва Домодедово', 'DME', 'Москва', 'Россия'),
       ('Пулково', 'LED', 'Санкт-Петербург', 'Россия'),
       ('Толмачево', 'OVB', 'Новосибирск', 'Россия'),
       ('Кольцово', 'SVX', 'Екатеринбург', 'Россия'),
       ('Казань', 'KZN', 'Казань', 'Россия'),
       ('Курумоч', 'KUF', 'Самара', 'Россия'),
       ('Внуково', 'VKO', 'Москва', 'Россия'),
       ('Шереметьево', 'SVO', 'Москва', 'Россия'),
       ('Ростов-на-Дону', 'ROV', 'Ростов-на-Дону', 'Россия'),
       ('Платов', 'POV', 'Ростов-на-Дону', 'Россия'),

       ('John F. Kennedy International Airport', 'JFK', 'New York', 'USA'),
       ('Los Angeles International Airport', 'LAX', 'Los Angeles', 'USA'),
       ('Chicago O''Hare International Airport', 'ORD', 'Chicago', 'USA'),
       ('Miami International Airport', 'MIA', 'Miami', 'USA'),
       ('San Francisco International Airport', 'SFO', 'San Francisco', 'USA'),

       ('Beijing Capital International Airport', 'PEK', 'Beijing', 'China'),
       ('Tokyo Haneda Airport', 'HND', 'Tokyo', 'Japan'),
       ('Indira Gandhi International Airport', 'DEL', 'Delhi', 'India'),
       ('Dubai International Airport', 'DXB', 'Dubai', 'United Arab Emirates'),
       ('Hong Kong International Airport', 'HKG', 'Hong Kong', 'China'),

       ('Heathrow Airport', 'LHR', 'London', 'United Kingdom'),
       ('Charles de Gaulle Airport', 'CDG', 'Paris', 'France'),
       ('Frankfurt Airport', 'FRA', 'Frankfurt', 'Germany'),
       ('Amsterdam Airport Schiphol', 'AMS', 'Amsterdam', 'Netherlands'),
       ('Istanbul Ataturk Airport', 'IST', 'Istanbul', 'Turkey'),

       ('Cairo International Airport', 'CAI', 'Cairo', 'Egypt'),
       ('O.R. Tambo International Airport', 'JNB', 'Johannesburg', 'South Africa'),
       ('Cape Town International Airport', 'CPT', 'Cape Town', 'South Africa'),
       ('Murtala Muhammed International Airport', 'LOS', 'Lagos', 'Nigeria'),
       ('Addis Ababa Bole International Airport', 'ADD', 'Addis Ababa', 'Ethiopia'),

       ('Sydney Kingsford Smith Airport', 'SYD', 'Sydney', 'Australia'),
       ('Melbourne Airport', 'MEL', 'Melbourne', 'Australia');


-- Авиалинии
INSERT INTO airlines (name, code)
VALUES ('Аэрофлот', 'SU'),
       ('Сибирь (S7 Airlines)', 'S7'),
       ('Уральские авиалинии', 'U6'),
       ('Победа', 'DP'),
       ('Россия', 'FV'),

       ('American Airlines', 'AA'),
       ('Delta Air Lines', 'DL'),
       ('United Airlines', 'UA'),
       ('Southwest Airlines', 'WN'),
       ('Lufthansa', 'LH'),
       ('Air France', 'AF'),
       ('Emirates', 'EK'),
       ('Qatar Airways', 'QR'),
       ('Cathay Pacific', 'CX'),
       ('Qantas', 'QF');



-- Генерируем гейты
CREATE OR REPLACE FUNCTION generate_gates(min_terminals INTEGER, max_terminals INTEGER, min_gates INTEGER, max_gates INTEGER)
    RETURNS VOID AS $$
DECLARE
    r_airport RECORD;
    i INTEGER;
    j INTEGER;
    num_terminals INTEGER;
    num_gates INTEGER;
BEGIN
    FOR r_airport IN (SELECT id FROM airports) LOOP
            num_terminals := floor(random() * (max_terminals - min_terminals + 1) + min_terminals)::INTEGER;

            FOR i IN 1..num_terminals LOOP
                    num_gates := floor(random() * (max_gates - min_gates + 1) + min_gates)::INTEGER;

                    FOR j IN 1..num_gates LOOP
                            INSERT INTO gates (name, terminal, airport_id)
                            VALUES ('G' || j, 'T' || i, r_airport.id);
                        END LOOP;
                END LOOP;
        END LOOP;
END;
$$ LANGUAGE plpgsql;

SELECT generate_gates(1, 5, 2, 6);


-- Модели самолетов
INSERT INTO airplane_models (title, brand)
VALUES
    ('Boeing 737-800', 'Boeing'),
    ('Boeing 777-300ER', 'Boeing'),
    ('Airbus A320-200', 'Airbus'),
    ('Airbus A330-300', 'Airbus'),
    ('Boeing 787-9', 'Boeing'),
    ('Airbus A350-900', 'Airbus'),
    ('Boeing 767-300ER', 'Boeing'),
    ('Airbus A319-100', 'Airbus'),
    ('Boeing 747-8', 'Boeing'),
    ('Airbus A380-800', 'Airbus')
;


-- Генерируем карту мест с классами.
CREATE OR REPLACE FUNCTION generate_rows_classes()
    RETURNS VOID AS $$
DECLARE
    r_model RECORD;
    economy_rows_count INTEGER;
    business_rows_count INTEGER;
    first_rows_count INTEGER;
BEGIN
    FOR r_model IN (SELECT id FROM airplane_models) LOOP
            -- Economy class
            economy_rows_count := floor(random() * (30 - 20 + 1) + 20)::INTEGER;
            INSERT INTO rows_classes (title, service_level, rows_count, rows_offset, seats_per_row, airplane_model_id)
            VALUES ('Economy', 1, economy_rows_count, 0, floor(random() * (6 - 4 + 1) + 4)::INTEGER, r_model.id);

            -- Business class
            business_rows_count := floor(random() * (8 - 4 + 1) + 4)::INTEGER;
            INSERT INTO rows_classes (title, service_level, rows_count, rows_offset, seats_per_row, airplane_model_id)
            VALUES ('Business', 2, business_rows_count, economy_rows_count, floor(random() * (5 - 3 + 1) + 3)::INTEGER, r_model.id);

            -- First class (not for all models)
            IF (random() < 0.5) THEN
                first_rows_count := floor(random() * (4 - 2 + 1) + 2)::INTEGER;
                INSERT INTO rows_classes (title, service_level, rows_count, rows_offset, seats_per_row, airplane_model_id)
                VALUES ('First', 3, first_rows_count, economy_rows_count + business_rows_count, floor(random() * (4 - 2 + 1) + 2)::INTEGER, r_model.id);
            END IF;
        END LOOP;
END;
$$ LANGUAGE plpgsql;

-- Call the function to generate rows classes for airplane models
SELECT generate_rows_classes();

SELECT generate_rows_classes();

-- Генерируем самолеты
CREATE OR REPLACE FUNCTION generate_airplanes()
    RETURNS VOID AS $$
DECLARE
    r_airport RECORD;
    r_airline RECORD;
    r_model RECORD;
    airport_airplane_count INTEGER;
    airline_airplane_count INTEGER;
    i INTEGER;
BEGIN

    i := 0;
    FOR r_airport IN (SELECT id FROM airports) LOOP
            FOR r_airline IN (SELECT id FROM airlines) LOOP
                    airport_airplane_count := floor(random() * (15 - 10 + 1) + 10)::INTEGER;
                    airline_airplane_count := floor(random() * (75 - 50 + 1) + 50)::INTEGER;

                    FOR j IN 1..airport_airplane_count LOOP
                            SELECT INTO r_model id FROM airplane_models OFFSET floor(random() * (SELECT COUNT(*) FROM airplane_models)) LIMIT 1;

                            INSERT INTO airplanes (airline_id, model_id, airport_id)
                            VALUES (r_airline.id, r_model.id, r_airport.id);

                            i := i + 1;
                        END LOOP;
                END LOOP;
        END LOOP;
END;
$$ LANGUAGE plpgsql;

SELECT generate_airplanes();


-- Генерируем полеты
CREATE OR REPLACE FUNCTION generate_flights()
    RETURNS VOID AS $$
DECLARE
    r_departure RECORD;
    r_arrival RECORD;
    r_airplane RECORD;
    r_flight RECORD;
    r_departure_gate RECORD;
    r_arrival_gate RECORD;
    departure_date TIMESTAMP;
    arrival_date TIMESTAMP;
    flight_count INTEGER;
    i INTEGER;
BEGIN
    departure_date := '2023-11-15 00:00:00';

    FOR i IN 1..365 LOOP
            flight_count := floor(random() * (150 - 80 + 1) + 80)::INTEGER;

            FOR j IN 1..flight_count LOOP
                    SELECT INTO r_departure * FROM airports OFFSET floor(random() * (SELECT COUNT(*) FROM airports)) LIMIT 1;
                    SELECT INTO r_arrival * FROM airports WHERE city != r_departure.city AND id != r_departure.id OFFSET floor(random() * (SELECT COUNT(*) FROM airports WHERE city != r_departure.city AND id != r_departure.id)) LIMIT 1;

                    SELECT INTO r_airplane * FROM airplanes WHERE airport_id = r_departure.id OFFSET floor(random() * (SELECT COUNT(*) FROM airplanes WHERE airport_id = r_departure.id)) LIMIT 1;

                    departure_date := departure_date + interval '2 hours' * random();
                    arrival_date := departure_date + interval '1 hour' * (1 + floor(random() * 12)::INTEGER);

                    SELECT INTO r_departure_gate * FROM gates WHERE airport_id = r_departure.id OFFSET floor(random() * (SELECT COUNT(*) FROM gates WHERE airport_id = r_departure.id)) LIMIT 1;
                    SELECT INTO r_arrival_gate * FROM gates WHERE airport_id = r_arrival.id OFFSET floor(random() * (SELECT COUNT(*) FROM gates WHERE airport_id = r_arrival.id)) LIMIT 1;

                    INSERT INTO flights (departure_airport_id, departure_date_time_utc, arrival_airport_id, arrival_date_time_utc, airplane_id, departure_gate_id, arrival_gate_id)
                    VALUES (r_departure.id, departure_date, r_arrival.id, arrival_date, r_airplane.id, r_departure_gate.id, r_arrival_gate.id);
                END LOOP;
        END LOOP;
END;
$$ LANGUAGE plpgsql;

-- Call the function to generate flights
SELECT generate_flights();
