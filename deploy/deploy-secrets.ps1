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
  $InstanceId,

  [Parameter(Mandatory=$true)]
  [string]
  $ClientSecret
)

$ErrorActionPreference = 'Stop'

try{
    $ResourceNamePrefix = "kmd-momentum-mea-$InstanceId"
    $KeyVaultName = "$($ResourceNamePrefix.replace('-',''))kv"

    if($KeyVaultName.length -gt 24)
    {
        $KeyVaultName = $KeyVaultName.substring($KeyVaultName.length-24,24);
    }

    
    $SecretName = $ResourceNamePrefix
    $SecretValue = $ClientSecret
    
    Write-Host "Storing the client secret in '$SecretName'"

    Write-Host "Storing the client secret '$SecretValue'"

    $SecretValue1 = "$($env:McaClientSecret)" | ConvertTo-SecureString -AsPlainText -Force

    Write-Host "Storing the client secret -1- '$SecretValue1'"

    Write-Host "test 2 '$($env:DbLoginId)'"
    Write-Host "test 3 '$env:DbLoginId'"
    Write-Host "test 4 '$DbLoginId'"
    
    Write-Host "test 2 '$($env:McaClientSecret)'"
    Write-Host "test 3 '$env:McaClientSecret'"
    Write-Host "test 4 '$McaClientSecret'"

    Set-AzKeyVaultSecret -VaultName $KeyVaultName -Name $SecretName -SecretValue (ConvertTo-SecureString $SecretValue -AsPlainText -Force)

    Write-Host "test 5"

    Set-AzKeyVaultSecret -VaultName $KeyVaultName -Name $SecretName -SecretValue $SecretValue1

}catch{

	Write-Host "An error occurred:"
	Write-Host $_
    Write-Host "##vso[task.LogIssue type=error;]"$_
    Write-Host "##vso[task.complete result=Failed]"
    exit 1

}