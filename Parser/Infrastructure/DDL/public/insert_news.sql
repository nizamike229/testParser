create procedure insert_news(IN p_title text, IN p_content text, IN p_date_of_publish timestamp without time zone)
    language plpgsql
as
$$
BEGIN
    INSERT INTO news (title, content, date_of_publish)
    VALUES (p_title, p_content, p_date_of_publish);
END;
$$;

alter procedure insert_news(text, text, timestamp) owner to postgres;

