function Get-MiniBancoDatabasePassword {
    $dotenvPath = Join-Path $PSScriptRoot '..\.env'

    if (-not (Test-Path -LiteralPath $dotenvPath -PathType Leaf)) {
        throw 'Arquivo .env não encontrado. Crie-o a partir de .env.example.'
    }

    $passwordLines = @(
        Get-Content -LiteralPath $dotenvPath |
            Where-Object { $_ -match '^\s*POSTGRES_PASSWORD\s*=' }
    )

    if ($passwordLines.Count -ne 1) {
        throw 'O .env deve conter exatamente uma entrada POSTGRES_PASSWORD.'
    }

    $password = $passwordLines[0] -replace '^\s*POSTGRES_PASSWORD\s*=\s*', ''
    $password = $password.Trim()

    if ($password.Length -ge 2 -and
        (($password[0] -eq '"' -and $password[-1] -eq '"') -or
         ($password[0] -eq "'" -and $password[-1] -eq "'"))) {
        $password = $password.Substring(1, $password.Length - 2)
    }

    if ([string]::IsNullOrWhiteSpace($password)) {
        throw 'POSTGRES_PASSWORD está vazia no .env.'
    }

    return $password
}
