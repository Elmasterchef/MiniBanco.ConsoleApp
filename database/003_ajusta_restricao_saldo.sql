ALTER TABLE contas
    DROP CONSTRAINT IF EXISTS ck_contas_saldo;

ALTER TABLE contas
    ADD CONSTRAINT ck_contas_saldo_por_tipo
    CHECK (
        (tipo = 'Corrente')
        OR (saldo >= 0)
    );