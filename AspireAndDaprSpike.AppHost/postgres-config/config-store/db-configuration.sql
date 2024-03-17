

-- Create the configtable table
CREATE TABLE IF NOT EXISTS configtable (
  KEY VARCHAR NOT NULL,
  VALUE VARCHAR NOT NULL,
  VERSION VARCHAR NOT NULL,
  METADATA JSON
);

-- Create the notify_event function
CREATE OR REPLACE FUNCTION notify_event() RETURNS TRIGGER AS $$
    DECLARE 
        data json;
        notification json;
    BEGIN
        IF (TG_OP = 'DELETE') THEN
            data = row_to_json(OLD);
        ELSE
            data = row_to_json(NEW);
        END IF;

        notification = json_build_object(
                          'table',TG_TABLE_NAME,
                          'action', TG_OP,
                          'data', data);
        PERFORM pg_notify('config',notification::text);
        RETURN NULL; 
    END;
$$ LANGUAGE plpgsql;

-- Create the config trigger
CREATE TRIGGER config
AFTER INSERT OR UPDATE OR DELETE ON configtable
    FOR EACH ROW EXECUTE PROCEDURE notify_event();

-- Insert example data
INSERT INTO configtable (KEY, VALUE, VERSION, METADATA) VALUES
('theme', 'dark', '1.0', '{"author": "John Doe", "created_at": "2024-03-16"}'),
('language', 'en', '1.1', '{"author": "Jane Smith", "created_at": "2024-03-17"}'),
('notifications', 'enabled', '1.0', '{"author": "Alice Johnson", "created_at": "2024-03-18"}'),
('timezone', 'UTC', '1.0', '{"author": "Bob Brown", "created_at": "2024-03-19"}'),
('currency', 'USD', '2.0', '{"author": "Charlie Davis", "created_at": "2024-03-20"}');




-- Create the StateStoreDb database
CREATE DATABASE StateStoreDb;