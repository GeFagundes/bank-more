-- --- SCHEMA: CHECKING_ACCOUNT ---

CREATE TABLE IF NOT EXISTS checking_account (
    id_checking_account INTEGER PRIMARY KEY AUTOINCREMENT,
    account_number TEXT NOT NULL UNIQUE,
    name TEXT NOT NULL,
	document TEXT NOT NULL,
    is_active BOOLEAN DEFAULT 1,
    password TEXT NOT NULL,
    salt TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS transactions (
    id_transaction INTEGER PRIMARY KEY AUTOINCREMENT,
    request_id TEXT NOT NULL UNIQUE,           
    account_number TEXT NOT NULL,       
    transaction_date DATETIME DEFAULT CURRENT_TIMESTAMP,
    transaction_type TEXT NOT NULL,
    amount REAL NOT NULL,
    FOREIGN KEY (account_number) REFERENCES checking_account (account_number)
);

CREATE TABLE IF NOT EXISTS idempotency_checking_account (
    idempotency_key TEXT PRIMARY KEY,
    request_body TEXT,
    response_body TEXT
);

-- --- SCHEMA: TRANSFER ---

CREATE TABLE IF NOT EXISTS transfers (
    id_transfer INTEGER PRIMARY KEY AUTOINCREMENT,
    id_origin_account INTEGER NOT NULL,
    id_destination_account INTEGER NOT NULL,
    transfer_date DATETIME DEFAULT CURRENT_TIMESTAMP,
    amount REAL,
    FOREIGN KEY (id_origin_account) REFERENCES checking_account (id_checking_account),
    FOREIGN KEY (id_destination_account) REFERENCES checking_account (id_checking_account)
);

CREATE TABLE IF NOT EXISTS idempotency_transfer (
    idempotency_key TEXT PRIMARY KEY,
    request_body TEXT,
    response_body TEXT
);

-- --- SCHEMA: FEES ---

CREATE TABLE IF NOT EXISTS fees (
    id_fee INTEGER PRIMARY KEY AUTOINCREMENT,
    id_checking_account INTEGER NOT NULL,
    fee_date DATETIME DEFAULT CURRENT_TIMESTAMP,
    amount REAL,
    FOREIGN KEY (id_checking_account) REFERENCES checking_account (id_checking_account)
);