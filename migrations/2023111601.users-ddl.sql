CREATE
    EXTENSION IF NOT EXISTS "pgcrypto";
CREATE
    EXTENSION IF NOT EXISTS "uuid-ossp";

create table users
(
    id uuid primary key default uuid_generate_v4(),
    name text not null,
    surname text not null,
    patronymic text null,
    passport_number text not null,
    created_at timestamp not null,
    login text not null unique,
    password_hash text not null,
    salt text not null 
);