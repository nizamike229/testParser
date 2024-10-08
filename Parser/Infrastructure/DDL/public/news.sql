create table news
(
    id              serial
        constraint news_pk
            primary key,
    title           varchar(300) not null,
    date_of_publish timestamp    not null,
    content         varchar      not null
);

alter table news
    owner to postgres;

