create table users
(
    id       serial
        constraint users_pk
            primary key,
    username varchar(255),
    password varchar(300)
);

alter table users
    owner to postgres;

