param (
	[string]$privateKeyPath = "./DaySpaPet.WebApi.RSA256.key",
	[string]$publicKeyPath = "./DaySpaPet.WebApi.RSA256.key.pub.pem",
	[string]$EnvironmentName
)

# Generate the private key
function Generate-NewRSAKeyPair {
	param (
		[string]$privateKeyPath
	)
	if (Test-Path -Path $privateKeyPath) {
		Write-Host "The private key file '$privateKeyPath' already exists. Skipping generation." -ForegroundColor Yellow
	} else {
		& ssh-keygen -t rsa -b 4096 -m PEM -f $privateKeyPath
		if ($?) {
			Write-Host "Private key generated successfully at '$privateKeyPath'." -ForegroundColor Green
		} else {
			Write-Host "Failed to generate private key at '$privateKeyPath'." -ForegroundColor Red
			exit 1
		}
	}
}

# Generate the public key
function Generate-NewPublicKeyPEM {
	param (
		[string]$privateKeyPath,
		[string]$publicKeyPath
	)
	if (Test-Path -Path $publicKeyPath) {
		Write-Host "The public key file '$publicKeyPath' already exists. Skipping generation." -ForegroundColor Yellow
	} else {
		& openssl rsa -in $privateKeyPath -pubout -outform PEM -out $publicKeyPath
		if ($?) {
			Write-Host "Public key generated successfully at '$publicKeyPath'." -ForegroundColor Green
		} else {
			Write-Host "Failed to generate public key at '$publicKeyPath'." -ForegroundColor Red
			exit 1
		}
	}
}

# Function to read the file content and remove line breaks
function Get-FileContentWithoutLineBreaks {
	param (
		[string]$filePath
	)
	$content = Get-Content -Path $filePath -Raw
	return ($content -replace "`r`n", "" -replace "`n", "")
}


# Ensure ssh-keygen and openssl are available
if (-not (Get-Command ssh-keygen -ErrorAction SilentlyContinue)) {
	Write-Host "ssh-keygen is not available in your PATH." -ForegroundColor Red
	Write-Host "Please install OpenSSH or ensure it is in your PATH." -ForegroundColor Red
	Write-Host "Alternatively, run the following command from a terminal using WSL on Windows:" -ForegroundColor Yellow
	Write-Host ""
	Write-Host " > ssh-keygen -t rsa -b 4096 -m PEM -f ./DaySpaPet.WebApi.RSA256.key" -ForegroundColor Yellow
	exit 1
}
if (-not (Get-Command openssl -ErrorAction SilentlyContinue)) {
	Write-Host "openssl is not available in your PATH." -ForegroundColor Red
	Write-Host "Please install OpenSSL or ensure it is in your PATH." -ForegroundColor Red
	Write-Host "Alternatively, run the following command from a terminal using WSL on Windows:" -ForegroundColor Yellow
	Write-Host ""
	Write-Host " > openssl rsa -in DaySpaPet.WebApi.RSA256.key -pubout -outform PEM -out DaySpaPet.WebApi.RSA256.key.pub.pem" -ForegroundColor Yellow
	exit 1
}

# Generate the keys
Generate-NewRSAKeyPair -privateKeyPath $privateKeyPath
Generate-NewPublicKeyPEM -privateKeyPath $privateKeyPath -publicKeyPath $publicKeyPath

# Get the keys without line breaks, to be used as JSON string (Development) or environment variable (CI/CD)
$privateKey = Get-FileContentWithoutLineBreaks -filePath $privateKeyPath
$publicKey = Get-FileContentWithoutLineBreaks -filePath $publicKeyPath

# Output the keys to appSettings.Development.json if Development, else to console
if ($EnvironmentName -eq "Development") {
	# Path to the appsettings JSON file
	$appSettingsPath = "appsettings.$EnvironmentName.json"

	# Check if the appsettings file exists
	if (-not (Test-Path -Path $appSettingsPath)) {
		Write-Host "The appsettings file '$appSettingsPath' does not exist." -ForegroundColor Red
		exit 1
	}

	# Read the JSON file
	$appSettings = Get-Content -Path $appSettingsPath -Raw | ConvertFrom-Json

	# Update the keys in the JSON
	$appSettings.Authentication.Schemes.Bearer.PrivateSigningKey = $privateKey
	$appSettings.Authentication.Schemes.Bearer.PublicSigningKey = $publicKey

	# Convert the JSON back to a string
	$jsonOutput = $appSettings | ConvertTo-Json -Depth 32

	# Write the updated JSON back to the file
	Write-Host "Embedding into application settings file: $appSettingsPath..."
	Set-Content -Path $appSettingsPath -Value $jsonOutput

	Write-Host "Keys have been embedded successfully in $appSettingsPath" -ForegroundColor Green
} else {
	Write-Host "Private key: $privateKey"
	Write-Host ""
	Write-Host "Public key: $publicKey"
	Write-Host ""
}