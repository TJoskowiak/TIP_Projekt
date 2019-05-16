drop database if exists voip;
create database voip;
use voip;
CREATE TABLE IF NOT EXISTS users(
    user_id INT auto_increment,
    username VARCHAR(50) NOT NULL,
    email VARCHAR(50) NOT NULL,
    password CHAR(128) NOT NULL,
    salt CHAR(128) NOT NULL,
    ip_address INT UNSIGNED NOT NULL,
    status TINYINT(4) DEFAULT 0,
    profile_picture BLOB NULL,
    last_login_date DATETIME NOT NULL,
    created_date DATETIME NOT NULL,
    PRIMARY KEY (user_id)
) ENGINE=INNODB;

CREATE TABLE IF NOT EXISTS calls(
	call_id INT auto_increment,
    caller_id int not null,
    receiver_id int not null,
    call_date DATETIME NOT NULL,
    end_date DATETIME NOT NULL,
    status INT NOT NULL DEFAULT 0,
    FOREIGN KEY (caller_id) REFERENCES users(user_id),
    FOREIGN KEY (receiver_id) REFERENCES users(user_id),
    PRIMARY KEY (call_id)
) ENGINE=INNODB;

CREATE TABLE IF NOT EXISTS contacts(
	contact_id INT auto_increment,
    owner_id int not null,
    subject_id int not null,
    created_date DATETIME NOT NULL,
    is_favourite BOOL NOT NULL DEFAULT FALSE,
    FOREIGN KEY (owner_id) REFERENCES users(user_id),
    FOREIGN KEY (subject_id) REFERENCES users(user_id),
    PRIMARY KEY (contact_id)
) ENGINE=INNODB;
