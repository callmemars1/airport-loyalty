-- Аэропорты
INSERT INTO airports (name, code, city, country)
VALUES ('Международный аэропорт Шереметьево', 'SVO', 'Москва', 'Россия'),
       ('Международный аэропорт Домодедово', 'DME', 'Москва', 'Россия'),
       ('Аэропорт Пулково', 'LED', 'Санкт-Петербург', 'Россия'),
       ('Международный аэропорт Казань', 'KZN', 'Казань', 'Россия'),
       ('Международный аэропорт Сочи', 'AER', 'Сочи', 'Россия'),
       ('Международный аэропорт Внуково', 'VKO', 'Москва', 'Россия'),
       ('Международный аэропорт Платов', 'ROV', 'Ростов-на-Дону', 'Россия'),
       ('Аэропорт Кольцово', 'SVX', 'Екатеринбург', 'Россия'),
       ('Аэропорт Толмачёво', 'OVB', 'Новосибирск', 'Россия'),
       ('Международный аэропорт Курумоч', 'KUF', 'Самара', 'Россия'),

       ('Международный аэропорт Пекина', 'PEK', 'Пекин', 'Китай'),
       ('Международный аэропорт Шанхай Пудун', 'PVG', 'Шанхай', 'Китай'),
       ('Международный аэропорт Гуанчжоу Байюн', 'CAN', 'Гуанчжоу', 'Китай'),
       ('Международный аэропорт Чэнду Шуанглиу', 'CTU', 'Чэнду', 'Китай'),
       ('Международный аэропорт Шэньчжэнь Баоань', 'SZX', 'Шэньчжэнь', 'Китай'),

       ('Аэропорт Хитроу', 'LHR', 'Лондон', 'Великобритания'),
       ('Аэропорт Шарль де Голль', 'CDG', 'Париж', 'Франция'),
       ('Аэропорт Франкфурт', 'FRA', 'Франкфурт', 'Германия'),
       ('Аэропорт Мадрид-Барахас', 'MAD', 'Мадрид', 'Испания'),
       ('Аэропорт Рим-Фьюмичино', 'FCO', 'Рим', 'Италия'),

       ('Международный аэропорт Хартсфилд-Джексон Атланта', 'ATL', 'Атланта', 'США'),
       ('Международный аэропорт Лос-Анджелес', 'LAX', 'Лос-Анджелес', 'США'),
       ('Международный аэропорт О’Хара', 'ORD', 'Чикаго', 'США'),
       ('Международный аэропорт Даллас-Форт Уэрт', 'DFW', 'Даллас', 'США'),
       ('Международный аэропорт Денвер', 'DEN', 'Денвер', 'США'),

       ('Международный аэропорт Кейптаун', 'CPT', 'Кейптаун', 'Южная Африка'),
       ('Международный аэропорт Лагос', 'LOS', 'Лагос', 'Нигерия'),
       ('Международный аэропорт Касабланка', 'CMN', 'Касабланка', 'Марокко'),
       ('Международный аэропорт Найроби', 'NBO', 'Найроби', 'Кения'),
       ('Международный аэропорт Каир', 'CAI', 'Каир', 'Египет');

-- Генерируем гейты
CREATE OR REPLACE FUNCTION generate_gates() RETURNS VOID AS $$
DECLARE
    airport_id INTEGER;
    terminal_count INTEGER;
    gate_count INTEGER;
    terminal_index INTEGER;
    gate_index INTEGER;
BEGIN
    FOR airport_id IN 1..30
        LOOP
            terminal_count := 1 + floor(random() * 4)::INTEGER;
            FOR terminal_index IN 1..terminal_count
                LOOP
                    gate_count := 1 + floor(random() * 4)::INTEGER;
                    FOR gate_index IN 1..gate_count
                        LOOP
                            INSERT INTO gates (name, terminal, airport_id)
                            VALUES ('Gate ' || gate_index, 'Terminal ' || terminal_index, airport_id);
                        END LOOP;
                END LOOP;
        END LOOP;
END;
$$ LANGUAGE plpgsql;

SELECT generate_gates();

DROP FUNCTION generate_gates();

-- Авиалинии
INSERT INTO airlines (name, code)
VALUES ('Aeroflot', 'SU'),
       ('S7 Airlines', 'S7'),
       ('UTair', 'UT'),
       ('Ural Airlines', 'U6'),
       ('Pobeda', 'DP'),
       ('Rossiya Airlines', 'FV'),
       ('Nordwind Airlines', 'N4'),
       ('Red Wings', 'WZ'),
       ('Azur Air', 'ZF'),
       ('Yamal Airlines', 'YC');

-- Генерируем самолеты

-- Predefined airplane models
CREATE TEMPORARY TABLE airplane_models
(
    id    serial primary key,
    model text not null,
    seats smallint not null
);

INSERT INTO airplane_models (model, seats)
VALUES ('Airbus A320', 180),
       ('Airbus A321', 220),
       ('Airbus A330', 290),
       ('Airbus A350', 325),
       ('Airbus A380', 853),
       ('Boeing 737', 200),
       ('Boeing 747', 660),
       ('Boeing 757', 295),
       ('Boeing 767', 375),
       ('Boeing 777', 550),
       ('Boeing 787', 440),
       ('Bombardier CRJ900', 90),
       ('Embraer E190', 114),
       ('Embraer E195', 132),
       ('Sukhoi Superjet 100', 108),
       ('Ilyushin Il-96', 350),
       ('Tupolev Tu-204', 210),
       ('Antonov An-148', 85),
       ('Irkut MC-21', 211),
       ('Comac C919', 190);

CREATE OR REPLACE FUNCTION generate_airplanes() RETURNS VOID AS $$
DECLARE
    airplane_index INTEGER;
    random_airline_id INTEGER;
    random_model RECORD;
BEGIN
    FOR airplane_index IN 1..100
        LOOP
            random_airline_id := 1 + floor(random() * 10)::INTEGER;
            SELECT * INTO random_model FROM airplane_models ORDER BY random() LIMIT 1;

            INSERT INTO airplanes (airline_id, seats, model)
            VALUES (random_airline_id, random_model.seats, random_model.model);
        END LOOP;
END;
$$ LANGUAGE plpgsql;

SELECT generate_airplanes();

DROP FUNCTION generate_airplanes();
DROP TABLE airplane_models;

-- Генерируем полеты
CREATE OR REPLACE FUNCTION generate_flights() RETURNS VOID AS $$
DECLARE
    departure_airport_id INTEGER;
    arrival_airport_id INTEGER;
    airplane RECORD;
    gate RECORD;
    departure_date_time_utc TIMESTAMP;
    arrival_date_time_utc TIMESTAMP;
    price NUMERIC;
    flight_count INTEGER;
BEGIN
    FOR departure_airport_id IN 1..30
        LOOP
            FOR arrival_airport_id IN 1..30
                LOOP
                    IF departure_airport_id <> arrival_airport_id THEN
                        SELECT * INTO airplane FROM airplanes ORDER BY random() LIMIT 1;
                        SELECT * INTO gate FROM gates WHERE airport_id = departure_airport_id ORDER BY random() LIMIT 1;

                        -- Generate flights before and after November 10, 2023
                        FOR flight_count IN 1..10
                            LOOP
                                IF flight_count <= 2 THEN
                                    -- 20% of flights before November 10, 2023
                                    departure_date_time_utc := '2023-11-10 00:00:00'::timestamp - INTERVAL '1 day' * floor(random() * 100)::INTEGER;
                                ELSE
                                    -- 80% of flights after November 10, 2023
                                    departure_date_time_utc := '2023-11-10 00:00:00'::timestamp + INTERVAL '1 day' * floor(random() * 100)::INTEGER;
                                END IF;

                                arrival_date_time_utc := departure_date_time_utc + INTERVAL '1 hour' * (1 + floor(random() * 10)::INTEGER);
                                price := 100 + floor(random() * 900)::INTEGER;

                                INSERT INTO flights (departure_airport_id, departure_date_time_utc, arrival_date_time_utc, arrival_airport_id, airplane_id, gate_id, price)
                                VALUES (departure_airport_id, departure_date_time_utc, arrival_date_time_utc, arrival_airport_id, airplane.id, gate.id, price);
                            END LOOP;
                    END IF;
                END LOOP;
        END LOOP;
END;
$$ LANGUAGE plpgsql;

-- Call the function to generate flights
SELECT generate_flights();

-- Drop the function after generating flights
DROP FUNCTION generate_flights();
