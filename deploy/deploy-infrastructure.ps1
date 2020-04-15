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
.PARAMETER $ApplicationInsightsName
  The name of the application insights instance. E.g. 'kmd-momentum-mea-shareddev-ai' or 'kmd-momentum-mea-shareddev-ai'.
.PARAMETER $DiagnosticSeqServerUrl
  The url of diagnostics seq instance (e.g. "https://myseq.kmdlogic.io/") which will help in diagnosing.
.PARAMETER $DiagnosticSeqApiKey
  Optional. An api key for diagnostics seq if required.
.PARAMETER $WebAppServicePlanSku
  Optional. F1,FREE,D1,SHARED,B1,B2,B3,S1,S2,S3,P1V2,P2V2,P3V2,PC2,PC3,PC4,I1,I2,I3
.PARAMETER $WebAppConfigAlwaysOn
  Optional. If set to $true, the web site will be 'always on' - this does not work with certain plans like D1 Shared
.INPUTS
  none
.OUTPUTS
  none
.NOTES
  Version:        1.0
  Author:         Ajay Aggarwal
  Creation Date:  02 March 2020
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
  $ResourceGroupLocation = "westeurope",

  [Parameter(Mandatory=$true)]
  [string]
  $DiagnosticSeqServerUrl,

  [Parameter(Mandatory=$true)]
  [string]
  $DiagnosticSeqApiKey,

  [Parameter(Mandatory=$false)]
  [string]
  [ValidateSet("F1","FREE","D1","SHARED","B1","B2","B3","S1","S2","S3","P1V2","P2V2","P3V2","PC2","PC3","PC4","I1","I2","I3")]
  $WebAppServicePlanSku = "F1",

  [Parameter(Mandatory=$false)]
  [bool]
  $WebAppConfigAlwaysOn = $false,

  [switch] 
  $ValidateOnly,

  [Parameter(Mandatory=$true)]
  [string]
  $ClientId,

  [Parameter(Mandatory=$true)]
  [string]
  $ClientSecret,

  [Parameter(Mandatory=$true)]
  [string]
  $Environment,

  [Parameter(Mandatory=$false)]
  [string]
  $DbRequired = 'false'
)

Push-Location $PSScriptRoot
Write-Host "Deploying from '$PSScriptRoot'"

$ResourceNamePrefix = "kmd-momentum-mea-$InstanceId"
$TemplateFile = "azuredeploy.json"

try {
  [Microsoft.Azure.Common.Authentication.AzureSession]::ClientFactory.AddUserAgent("VSAzureTools-$UI$($host.name)".replace(' ','_'), '3.0.0')
} catch { 
  Write-Host "An error occurred:"
  Write-Host $_
}

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3

function Format-ValidationOutput {
  param ($ValidationOutput, [int] $Depth = 0)
  Set-StrictMode -Off
  return @($ValidationOutput | Where-Object { $_ -ne $null } | ForEach-Object { @('  ' * $Depth + ': ' + $_.Message) + @(Format-ValidationOutput @($_.Details) ($Depth + 1)) })
}

$ResourceGroupName = "$ResourceNamePrefix-rg"
$ApplicationInsightsName="$ResourceNamePrefix-ai";
$DbServerName="$ResourceNamePrefix-dbsvr";
$DbName="$ResourceNamePrefix-db";
$DbConnection="Server=$($DbServerName).postgres.database.azure.com;Database=$($DbName);Port=5432;User Id=$($env:DbLoginId)@$($DbServerName);Password=$($env:DbLoginPassword);Ssl Mode=Require;"
# Set ARM template parameter values
$TemplateParameters = @{
  environment = $Environment;
  instanceId = $InstanceId;
  resourceNamePrefix = $ResourceNamePrefix;
  applicationInsightsName = $ApplicationInsightsName;
  diagnosticSeqServerUrl = $DiagnosticSeqServerUrl;
  diagnosticSeqApiKey = $DiagnosticSeqApiKey;
  webAppServicePlanSku = $WebAppServicePlanSku;
  webAppConfigAlwaysOn = $WebAppConfigAlwaysOn;
  clientId = $ClientId;
  clientSecret = $ClientSecret;
  mcaApiUri = $env:McaApiUri;
  dbServerName = $DbServerName;
  dbLoginId = $env:DbLoginId;
  dbLoginPassword = $env:DbLoginPassword;
  dbName = $DbName;
  dbConnection = $DbConnection;
  dbRequired = $DbRequired
}

# Create or update the resource group using the specified template file and template parameter values
$Tags = @{}
if ($MarkForAutoDelete) {
	$Tags["keep"] = "false";
} else {
	$Tags["important"] = "true";
}

try
{
	New-AzResourceGroup -Name $ResourceGroupName -Location $ResourceGroupLocation -Tags $Tags -Verbose -Force
  
	Write-Output '----1'
	if ($ValidateOnly) {
		Write-Output '----2'
		$ErrorMessages = Format-ValidationOutput (Test-AzResourceGroupDeployment -ResourceGroupName $ResourceGroupName `
                                                                                -TemplateFile $TemplateFile `
                                                                                @TemplateParameters)
		Write-Output '----3'
		if ($ErrorMessages) {
			Write-Output '', 'Validation returned the following errors:', @($ErrorMessages), '', 'Template is invalid.'
		}else {
			Write-Output '', 'Template is valid.'
		}
	
		Write-Output '----4'
	} else {
		Write-Output '----5'
		New-AzResourceGroupDeployment -Name ((Get-ChildItem $TemplateFile).BaseName + '-' + ((Get-Date).ToUniversalTime()).ToString('MMdd-HHmm')) `
                                      -ResourceGroupName $ResourceGroupName `
                                      -TemplateFile $TemplateFile `
                                      @TemplateParameters `
                                      -Force -Verbose `
                                      -ErrorVariable ErrorMessages
		Write-Output '----6'
		if ($ErrorMessages) {
			Write-Output '', 'Template deployment returned the following errors:', @(@($ErrorMessages) | ForEach-Object { $_.Exception.Message.TrimEnd("`r`n") })
		}
	}
}catch{
	Write-Host "An error occurred:"
	Write-Host $_
	exit 1
}
Pop-Location