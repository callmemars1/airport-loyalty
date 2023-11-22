CREATE
    EXTENSION IF NOT EXISTS "pgcrypto";
CREATE
    EXTENSION IF NOT EXISTS "uuid-ossp";

create table products
(
    id            uuid primary key  default uuid_generate_v4(),
    title         text     not null,
    quantity      int      not null,
    deleted       bool     not null default false,
    discriminator text     not null,
    created_by_id uuid     not null references staff,

    -- [[ Ticket ]]
    flight_id     uuid     null references flights (id),
    service_class smallint not null references rows_classes (id),

    -- [[ LoungePass ]]
    pass_number   int      null,
    access_date   date     null
);

create table price_changes
(
    id            uuid primary key default uuid_generate_v4(),
    changed_at    timestamp not null,
    product_id    uuid      not null references products (id),
    changed_by_id uuid      not null references staff,
    price         money     not null
);
CREATE INDEX price_changes_product_id_changed_at_idx ON price_changes (product_id, changed_at DESC);

create table purchases
(
    id                uuid primary key   default uuid_generate_v4(),
    created_at        timestamp not null,
    user_id           uuid      not null references users (id),
    paid_bonus_points int       not null default 0,
    total_price       money     not null
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
    seat_number     text null,
    ticket_number   text null
);

create table rewards
(
    id            uuid primary key default uuid_generate_v4(),
    title         text      not null,
    created_at    timestamp not null,
    starts_at     timestamp not null,
    ends_at       timestamp null,
    discriminator text      not null,

    -- FOR StaticPercentBonusPointsReward
    percent_value real      null
);

create table products_rewards
(
    product_id uuid not null references products,
    reward_id  uuid not null references rewards,
    primary key (product_id, reward_id)
);

create table bonus_points_changes
(
    id          uuid primary key default uuid_generate_v4(),
    user_id     uuid      not null references users,
    purchase_id uuid      not null references purchases,
    value       int       not null,
    changed_at  timestamp not null
);