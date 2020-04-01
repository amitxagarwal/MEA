<#
.SYNOPSIS
  Deploys the Azure resources for Momentum External API. This requires you to login to Azure first.
.DESCRIPTION
  Deploys the Azure resources for invoicing. If you haven't already logged in, execute `Connect-AzAccount`
  and `Select-AzSubscription -Subscription "KMD Momentum Internal"'. Depending on your account, you might need to use something
  like `Connect-AzAccount -Subscription "KMD Momentum Internal" -TenantId "1aaaea9d-df3e-4ce7-a55d-43de56e79442"`.
.PARAMETER $MarkForAutoDelete
  When $true, the resource group will be tagged for auto-deletion. Useful for temporary personal or phoenix environments.
.PARAMETER $InstanceId
  The unique instance identifier (e.g. "internal" or "external") which will be used to name the azure resources.
.PARAMETER $ResourceGroupLocation
  The azure location of the created resource group (e.g. "australiaeast" or "centralindia" or "westeurope").
.INPUTS
  none
.OUTPUTS
  none
.NOTES
  Version:        1.0
  Author:         Ajay Aggarwal
  Creation Date:  01 April 2020
  Purpose/Change: Deploys momentum mea azure infrastructure.

.EXAMPLE
  ./deploy-infrastructure.ps1 -InstanceId udvdev -DiagnosticSeqServerUrl "https://xxx.kmdlogic.io/" -DiagnosticSeqApiKey "xxx" -MarkForAutoDelete -ResourceGroupLocation "australiaeast" -ApplicationInsightsName "kmd-momentum-mea-udvdev-ai" -ApplicationInsightsResourceGroup "kmd-momentum-mea-udvdev-rg" -WebAppServicePlanSku P1V2 -WebAppConfigAlwaysOn $true -AuditEventHubsConnectionString "xxx"

  Deploys a personal environment for 'udvdev', which is marked for auto-deletion, and uses a personal Seq and application insights.
#>
Param
(
  [Parameter(Mandatory=$false)]
  [switch]
  $MarkForAutoDelete = $false,

  [Parameter(Mandatory=$true)]
  [string]
  $InstanceId,

  [Parameter(Mandatory=$false)]
  [string]
  $ResourceGroupLocation = "westeurope"
)

Push-Location $PSScriptRoot
Write-Host "Deploying from '$PSScriptRoot'"

$ResourceNamePrefix = "kmd-momentum-mea-$InstanceId"
$TemplateFile = "azuredeploy.json"

try {
  [Microsoft.Azure.Common.Authentication.AzureSession]::ClientFactory.AddUserAgent("VSAzureTools-$UI$($host.name)".replace(' ','_'), '3.0.0')
} catch { }

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3

function Format-ValidationOutput {
  param ($ValidationOutput, [int] $Depth = 0)
  Set-StrictMode -Off
  return @($ValidationOutput | Where-Object { $_ -ne $null } | ForEach-Object { @('  ' * $Depth + ': ' + $_.Message) + @(Format-ValidationOutput @($_.Details) ($Depth + 1)) })
}

$ResourceGroupName = "$ResourceNamePrefix-rg"
$StorageAccountName= $ResourceNamePrefix.ToLower() -replace "-",""

# Set ARM template parameter values
$TemplateParameters = @{
  storageAccountName = $StorageAccountName;
  accountType = "Standard_RAGRS"; 
  kind = "StorageV2";
  accessTier = "Cool";
  supportsHttpsTrafficOnly = $true
}

# Create or update the resource group using the specified template file and template parameter values
$Tags = @{}
if ($MarkForAutoDelete) {
  $Tags["keep"] = "false";
} else {
  $Tags["important"] = "true";
}

New-AzResourceGroup -Name $ResourceGroupName -Location $ResourceGroupLocation -Tags $Tags -Verbose -Force


$ErrorMessages = Format-ValidationOutput (Test-AzResourceGroupDeployment -ResourceGroupName $ResourceGroupName `
                                                                                -TemplateFile $TemplateFile `
                                                                                @TemplateParameters)
if ($ErrorMessages) {
      Write-Error '', 'Validation returned the following errors:', @($ErrorMessages), '', 'Template is invalid.'
}
else {
   Write-Output '', 'Template is valid.'
}

  New-AzResourceGroupDeployment -Name ((Get-ChildItem $TemplateFile).BaseName + '-' + ((Get-Date).ToUniversalTime()).ToString('MMdd-HHmm')) `
                                      -ResourceGroupName $ResourceGroupName `
                                      -TemplateFile $TemplateFile `
                                      @TemplateParameters `
                                      -Force -Verbose `
                                      -ErrorVariable ErrorMessages
  if ($ErrorMessages) {
      Write-Error '', 'Template deployment returned the following errors:', @(@($ErrorMessages) | ForEach-Object { $_.Exception.Message.TrimEnd("`r`n") })
  }


Pop-Location