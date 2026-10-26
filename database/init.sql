
CREATE TABLE IF NOT EXISTS contas (
    numero VARCHAR(30) PRIMARY KEY,
    titular VARCHAR(150) NOT NULL,
    tipo VARCHAR(30) NOT NULL,
    saldo NUMERIC(18, 2) NOT NULL DEFAULT 0,
    criada_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT ck_contas_tipo
        CHECK (tipo IN ('Corrente', 'Poupanca', 'Salario')),

    CONSTRAINT ck_contas_saldo
        CHECK (saldo >= 0)
);

CREATE TABLE IF NOT EXISTS transacoes (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    conta_numero VARCHAR(30) NOT NULL,
    descricao VARCHAR(200) NOT NULL,
    tipo VARCHAR(30) NOT NULL,
    valor NUMERIC(18, 2) NOT NULL,
    data TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_transacoes_contas
        FOREIGN KEY (conta_numero)
        REFERENCES contas(numero),

    CONSTRAINT ck_transacoes_tipo
        CHECK (tipo IN ('Deposito', 'Saque', 'Tarifa', 'Rendimento')),

    CONSTRAINT ck_transacoes_valor
        CHECK (valor > 0)
);

CREATE INDEX IF NOT EXISTS idx_transacoes_conta_data
    ON transacoes (conta_numero, data DESC);
