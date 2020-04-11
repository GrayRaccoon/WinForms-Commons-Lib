
/*
Sample tables for unit testing.
*/

CREATE TABLE post (
    post_id INTEGER PRIMARY KEY AUTOINCREMENT,

    title VARCHAR(255) NOT NULL,
    content VARCHAR(255) NOT NULL,

    is_deleted INTEGER NOT NULL DEFAULT 0,
    created_at BIGINT NOT NULL,
    updated_at BIGINT NOT NULL
);
