param (
	[string]$PrivateKeyPath = "./DaySpaPet.WebApi.RSA256.key",
	[string]$PublicKeyPath = "./DaySpaPet.WebApi.RSA256.key.pub.pem",
	[string]$EnvironmentName = "Development",
	[switch]$Force
)

# Generate the private key
function Generate-NewRSAKeyPair {
	param (
		[string]$PrivateKeyPath,
		[switch]$Force
	)
	if ((Test-Path -Path $PrivateKeyPath) -and $Force) {
		Write-Host "The private key file '$PrivateKeyPath' already exists. Skipping generation." -ForegroundColor Yellow
	} else {
		& ssh-keygen -t rsa -b 4096 -m PEM -f $PrivateKeyPath
		if ($?) {
			Write-Host "Private key generated successfully at '$PrivateKeyPath'." -ForegroundColor Green
		} else {
			Write-Host "Failed to generate private key at '$PrivateKeyPath'." -ForegroundColor Red
			exit 1
		}
	}
}

# Generate the public key
function Generate-NewPublicKeyPEM {
	param (
		[string]$PrivateKeyPath,
		[string]$PublicKeyPath,
		[switch]$Force
	)
	if ((Test-Path -Path $PublicKeyPath) -and $Force) {
		Write-Host "The public key file '$PublicKeyPath' already exists. Skipping generation." -ForegroundColor Yellow
	} else {
		& openssl rsa -in $PrivateKeyPath -pubout -outform PEM -out $PublicKeyPath
		if ($?) {
			Write-Host "Public key generated successfully at '$PublicKeyPath'." -ForegroundColor Green
		} else {
			Write-Host "Failed to generate public key at '$PublicKeyPath'." -ForegroundColor Red
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
Generate-NewRSAKeyPair -privateKeyPath $PrivateKeyPath
Generate-NewPublicKeyPEM -privateKeyPath $PrivateKeyPath -publicKeyPath $PublicKeyPath

# Get the keys without line breaks, to be used as JSON string (Development) or environment variable (CI/CD)
$privateKey = Get-FileContentWithoutLineBreaks -filePath $PrivateKeyPath
$publicKey = Get-FileContentWithoutLineBreaks -filePath $PublicKeyPath

# Output the keys to .NET User Secrets if Development, else to console
if ($EnvironmentName -eq "Development") {
	# Write the updated JSON back to the file
	Write-Host "Storing private and public keys in .NET Secrets Manager (aka User Secrets)..."
	dotnet user-secrets init
	Write-Host ""

	dotnet user-secrets set "Authentication.Schemes.Bearer.PrivateSigningKey" $privateKey
	Write-Host ""

	dotnet user-secrets set "Authentication.Schemes.Bearer.PublicSigningKey" $publicKey
	Write-Host ""
} else {
	Write-Host "Private key: $privateKey"
	Write-Host ""
	Write-Host "  `$env:Authentication__Schemes__Bearer__PrivateSigningKey=$privateKey"
	Write-Host "  `export Authentication__Schemes__Bearer__PrivateSigningKey=$privateKey"
	Write-Host "  [System.Environment]::SetEnvironmentVariable('Authentication__Schemes__Bearer__PrivateSigningKey','$privateKey')
	Write-Host ""
	Write-Host ""
	Write-Host "Public key: $publicKey"
	Write-Host "  `$env:Authentication__Schemes__Bearer__PublicSigningKey=$publicKey"
	Write-Host "  `export Authentication__Schemes__Bearer__PublicSigningKey=$publicKey"
	Write-Host "  [System.Environment]::SetEnvironmentVariable('Authentication__Schemes__Bearer__PublicSigningKey','$publicKey')
	Write-Host ""
}