CREATE EXTENSION IF NOT EXISTS "pgcrypto";
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

create table airports
(
    id      smallserial primary key,
    name    text not null,
    code    text not null unique,
    city    text not null,
    country text not null
);

create table gates
(
    id         smallserial primary key,
    name       text     not null,
    terminal   text     not null,
    airport_id smallint not null references airports (id),
    unique (name, terminal, airport_id)
);

create table airlines
(
    id   smallserial primary key,
    name text not null,
    code text not null unique
);

create table airplanes
(
    id         serial primary key,
    airline_id smallint not null references airlines,
    seats      smallint not null,
    model      text     not null
);

create table flights
(
    id                      uuid primary key default uuid_generate_v4(),
    departure_airport_id    smallint  not null references airports,
    departure_date_time_utc timestamp not null,
    arrival_date_time_utc   timestamp not null,
    arrival_airport_id      smallint  not null references airports,
    airplane_id             smallint references airplanes,
    gate_id                 smallint references gates,
    price                   money     not null
);

