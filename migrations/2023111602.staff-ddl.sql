CREATE
    EXTENSION IF NOT EXISTS "pgcrypto";
CREATE
    EXTENSION IF NOT EXISTS "uuid-ossp";

create table roles
(
    id          smallserial primary key,
    title       text not null,
    description text not null,
    system_name text not null unique
);

create table positions
(
    id          smallserial primary key,
    title       text not null,
    description text not null
);

create table positions_roles
(
    position_id smallint not null references positions,
    role_id     smallint not null references roles,
    primary key (position_id, role_id)
);

create table staff
(
    id            uuid primary key default uuid_generate_v4(),
    name          text      not null,
    surname       text      not null,
    patronymic    text      null,
    created_at    timestamp not null,
    position_id   smallint  not null references positions,
    login         text      not null unique,
    password_hash text      not null,
    salt          text      not null
);