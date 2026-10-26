$environmentNames = @(
    'MINIBANCO_DB_HOST',
    'MINIBANCO_DB_PORT',
    'MINIBANCO_DB_NAME',
    'MINIBANCO_DB_USER',
    'MINIBANCO_DB_PASSWORD'
)

. (Join-Path $PSScriptRoot 'Database-Environment.ps1')
$databasePassword = Get-MiniBancoDatabasePassword

$previousEnvironment = @{}
foreach ($name in $environmentNames) {
    $previousEnvironment[$name] = [Environment]::GetEnvironmentVariable(
        $name,
        [EnvironmentVariableTarget]::Process
    )
}

$testExitCode = 1

try {
    $env:MINIBANCO_DB_HOST = '127.0.0.1'
    $env:MINIBANCO_DB_PORT = '5434'
    $env:MINIBANCO_DB_NAME = 'minibanco'
    $env:MINIBANCO_DB_USER = 'minibanco_user'
    $env:MINIBANCO_DB_PASSWORD = $databasePassword

    $solutionPath = Join-Path $PSScriptRoot '..\MiniBanco.slnx'
    dotnet test $solutionPath
    $testExitCode = $LASTEXITCODE
}
finally {
    foreach ($name in $environmentNames) {
        [Environment]::SetEnvironmentVariable(
            $name,
            $previousEnvironment[$name],
            [EnvironmentVariableTarget]::Process
        )
    }

    $databasePassword = $null
}

# Keep the result available to the caller without exiting its PowerShell session.
$global:LASTEXITCODE = $testExitCode
