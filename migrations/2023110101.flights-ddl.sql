CREATE
EXTENSION IF NOT EXISTS "pgcrypto";
CREATE
EXTENSION IF NOT EXISTS "uuid-ossp";

create table airports
(
    id        serial primary key,
    name      text not null,
    iata_code text not null unique,
    city      text not null,
    country   text not null
);

create table gates
(
    id         serial primary key,
    name       text     not null,
    terminal   text     not null,
    airport_id smallint not null references airports (id),
    unique (name, terminal, airport_id)
);

create table airlines
(
    id   serial primary key,
    name text not null,
    code text not null unique
);

create table airplane_models
(
    id           serial primary key,
    manufacturer text not null,
    title        text not null
);

create table airplanes
(
    id         serial primary key,
    model_id   int not null references airplane_models (id),
    airline_id int not null references airlines (id),
    airport_id int not null references airports (id)
);

create table rows_classes
(
    id                serial primary key,
    title             text     not null,
    service_level     smallint not null,
    rows_count        smallint not null,
    rows_offset       smallint not null,
    seats_per_row     smallint not null,
    airplane_model_id smallint not null references airplane_models (id)
);

create table flights
(
    id                      uuid primary key default uuid_generate_v4(),
    departure_airport_id    int       not null references airports (id),
    departure_date_time_utc timestamp not null,
    departure_gate_id       int       not null references gates (id),
    arrival_airport_id      int       not null references airports (id),
    arrival_date_time_utc   timestamp not null,
    arrival_gate_id         int       not null references gates (id),
    airplane_id             int       not null references airplanes (id)
);

