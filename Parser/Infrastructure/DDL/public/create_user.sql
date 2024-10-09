create procedure create_user(IN p_login text, IN p_password text)
    language plpgsql
as
$$
BEGIN
    INSERT INTO users (username, password)
    VALUES (p_login, p_password);
END;
$$;

alter procedure create_user(text, text) owner to postgres;

