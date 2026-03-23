-- 1. Inserir Contas Correntes (Checking Accounts)
-- account_number alterado para conter apenas números conforme solicitado
INSERT INTO checking_account (account_number, name, document, is_active, password, salt) 
VALUES 
('10015', 'João Silva', '12345678901', 1, 'hash_pwd_01', 'salt_01'),
('20020', 'Maria Santos', '98765432109', 1, 'hash_pwd_02', 'salt_02'),
('30039', 'Ricardo Pereira', '55566677788', 0, 'hash_pwd_03', 'salt_03');

-- 2. Inserir Transações (Transactions)
-- Relacionado via account_number (TEXT) conforme definido no seu schema
INSERT INTO transactions (request_id, account_number, transaction_type, amount)
VALUES 
('REQ-TX-001', '10015', 'C', 1500.00),
('REQ-TX-002', '10015', 'D', 200.00),
('REQ-TX-003', '20020', 'C', 5000.00);

-- 3. Inserir Transferências (Transfers)
-- Utiliza os IDs numéricos gerados automaticamente (1=João, 2=Maria)
INSERT INTO transfers (id_origin_account, id_destination_account, amount)
VALUES 
(1, 2, 350.00);

-- 4. Inserir Tarifas (Fees)
-- Vinculado ao ID numérico da conta
INSERT INTO fees (id_checking_account, amount)
VALUES 
(1, 12.50),
(2, 12.50);

-- 5. Inserir Registos de Idempotência
-- Exemplos para as tabelas de Account e Transfer
INSERT INTO idempotency_checking_account (idempotency_key, request_body, response_body)
VALUES 
('key-acc-001', '{"name": "João Silva", "account": "10015"}', '{"status": "success"}');

INSERT INTO idempotency_transfer (idempotency_key, request_body, response_body)
VALUES 
('key-tx-999', '{"from": 1, "to": 2, "amount": 350}', '{"transfer_id": 1}');