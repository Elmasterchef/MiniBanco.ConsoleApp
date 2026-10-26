ALTER TABLE contas
    ADD COLUMN IF NOT EXISTS limite_cheque_especial NUMERIC(18, 2),
    ADD COLUMN IF NOT EXISTS taxa_rendimento_mensal DOUBLE PRECISION,
    ADD COLUMN IF NOT EXISTS dia_aniversario INTEGER,
    ADD COLUMN IF NOT EXISTS empregador VARCHAR(150),
    ADD COLUMN IF NOT EXISTS limite_saques_mensais INTEGER,
    ADD COLUMN IF NOT EXISTS saques_realizados_no_mes INTEGER
        NOT NULL DEFAULT 0;