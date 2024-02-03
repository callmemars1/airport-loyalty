CREATE
    EXTENSION IF NOT EXISTS "pgcrypto";
CREATE
    EXTENSION IF NOT EXISTS "uuid-ossp";

create table products
(
    id               uuid primary key  default uuid_generate_v4(),
    title            text     not null,
    quantity         int      not null,
    deleted          bool     not null default false,
    discriminator    text     not null,
    created_by_id    uuid     not null references users,

    -- [[ Ticket ]]
    flight_id        uuid     null references flights (id),
    service_class_id smallint not null references rows_classes (id)
);

create table price_changes
(
    id            uuid primary key default uuid_generate_v4(),
    changed_at    timestamp        not null,
    product_id    uuid             not null references products (id),
    changed_by_id uuid             not null references users,
    price         double precision not null
);
CREATE INDEX price_changes_product_id_changed_at_idx ON price_changes (product_id, changed_at DESC);

create table purchases
(
    id                uuid primary key   default uuid_generate_v4(),
    created_at        timestamp not null,
    user_id           uuid      not null references users (id),
    paid_bonus_points int       not null default 0,
    total_price       double precision     not null
);

create table purchased_products
(
    id              uuid primary key default uuid_generate_v4(),
    product_id      uuid not null references products (id),
    purchase_id     uuid not null references purchases (id),
    quantity        int  not null,
    actual_price_id uuid not null references price_changes,
    discriminator   text not null,

    -- FOR [[ Ticket ]]
    seat_number     int2 null,
    ticket_number   text null
);