
/*
Last Successful Sync table
*/

CREATE TABLE last_successful_sync (
    id INTEGER PRIMARY KEY,
    
    backend_id VARCHAR(250) NOT NULL DEFAULT 'default',
    table_id VARCHAR(250) NOT NULL,
    
    sync_timestamp BIGINT NOT NULL
);

 