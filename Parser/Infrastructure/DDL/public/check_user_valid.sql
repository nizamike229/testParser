create function check_user_valid(p_username character varying, p_password character varying) returns boolean
    language plpgsql
as
$$
DECLARE
    stored_hash VARCHAR(255);
BEGIN
    SELECT password INTO stored_hash
    FROM users
    WHERE username = p_username;

    IF stored_hash IS NULL THEN
        RETURN false;
    END IF;

    RETURN stored_hash = p_password;
END;
$$;

alter function check_user_valid(varchar, varchar) owner to postgres;

