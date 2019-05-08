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