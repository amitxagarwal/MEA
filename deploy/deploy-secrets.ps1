<#
.SYNOPSIS
  Deploys client secrets to azure key vault
.DESCRIPTION
  Deploys the client secrets to azure key vault. If you haven't already logged in, execute `Connect-AzAccount`
  and `Select-AzSubscription -Subscription "KMD Momentum Internal"'. Depending on your account, you might need to use something
  like `Connect-AzAccount -Subscription "KMD Momentum Internal" -TenantId "1aaaea9d-df3e-4ce7-a55d-43de56e79442"`.
.PARAMETER $InstanceId
  The unique instance identifier (e.g. "internal" or "external") which will be used to name the azure resources.

.INPUTS
  none
.OUTPUTS
  none
.NOTES
  Version:        1.0
  Author:         Ashish Verma
  Creation Date:  22 Apr 2020
  Purpose/Change: Deploy client secrets to azure key vault
#>

Param
(
  [Parameter(Mandatory=$true)]
  [string]
  $InstanceId
)

$ResourceNamePrefix = "kmd-momentum-mea-$InstanceId"
$KeyVaultName = "$($ResourceNamePrefix.replace('-',''))kv"

Write-Host "Storing the client secret in '$key'"

Set-AzKeyVaultSecret -VaultName $KeyVaultName -Name $ResourceNamePrefix -SecretValue (ConvertTo-SecureString -String $env:$McaClientSecret -AsPlainText -Force)