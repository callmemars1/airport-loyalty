ALTER TABLE flights ADD COLUMN cancelled boolean not null default false;

ALTER TABLE users ADD COLUMN balance double precision not null default 0;