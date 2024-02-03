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


INSERT INTO roles (title, description, system_name)
VALUES
    ('Admin', 'Все права', 'Admin'),
    ('Редактор', 'Просмотр и редактирование данных', 'Editor'),
    ('Аналитик', 'Просмотр данных', 'Analyst'),
    ('Клиент', 'Совершает покупки и пользуется системой', 'Client');

create table users
(
    id uuid primary key default uuid_generate_v4(),
    name text not null,
    surname text not null,
    patronymic text null,
    passport_number text not null,
    created_at timestamp not null,
    role_id smallint not null references roles (id),
    login text not null unique,
    password_hash text not null,
    salt text not null 
);

INSERT INTO users ("name", surname, patronymic, passport_number, created_at, role_id, login, password_hash, "salt")
VALUES ('Admin', 'Admin', null, '0000 000000', CURRENT_TIMESTAMP, 1, 'admin', 'BD9efOAjVMwH/QALNE1ttOtJ+LuS9HOURXDXJ38E5cM=', '3xUOTm977LzPgCdo336JZg==');
/*admin:Admin1234*/

