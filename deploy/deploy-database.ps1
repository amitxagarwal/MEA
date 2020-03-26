<#
.SYNOPSIS
  Deploys Azure Postgres Migration Scripts.
.DESCRIPTION
  Deploys the Azure resources for invoicing. If you haven't already logged in, execute `Connect-AzAccount`
  and `Select-AzSubscription -Subscription "LoGIC DEV"'. Depending on your account, you might need to use something
  like `Connect-AzAccount -Subscription "LoGIC DEV" -TenantId "1aaaea9d-df3e-4ce7-a55d-43de56e79442"`.
.PARAMETER $MarkForAutoDelete
  When $true, the resource group will be tagged for auto-deletion. Useful for temporary personal or phoenix environments.
.PARAMETER $InstanceId
  The unique instance identifier (e.g. "shareddev" or "udvdev" or "prod") which will be used to name the azure resources.

.INPUTS
  none
.OUTPUTS
  none
.NOTES
  Version:        1.0
  Author:         Ajay Aggarwal
  Creation Date:  26 Mar 2020
  Purpose/Change: Deploy sts bridge azure infrastructure.

.EXAMPLE
  ./deploy-database.ps1 -InstanceId udvdev -DiagnosticSeqServerUrl "https://xxx.kmdlogic.io/" -DiagnosticSeqApiKey "xxx" -MarkForAutoDelete -ResourceGroupLocation "australiaeast" -ApplicationInsightsName "kmd-momentum-mea-udvdev-ai" -ApplicationInsightsResourceGroup "kmd-momentum-mea-udvdev-rg" -WebAppServicePlanSku P1V2 -WebAppConfigAlwaysOn $true -AuditEventHubsConnectionString "xxx"

  Deploys a personal environment for 'udvdev', which is marked for auto-deletion, and uses a personal Seq and application insights.
#>
Param
(
  [Parameter(Mandatory=$true)]
  [string]
  $InstanceId,
)

Push-Location "$PSScriptRoot/src/PostgreSqlDb/Kmd.Momentum.Mea.DbAdmin"

$ResourceNamePrefix = "kmd-momentum-mea-$InstanceId"

$DbServerName="$ResourceNamePrefix-dbsvr";
$DbName="$ResourceNamePrefix-db";

Write-Host "Migrate Database '$DbName' with MigrationScripts"

& dotnet run -- migrate -s $DbServerName -d $DbName -u $env:DbLoginId -p $env:DbLoginPassword -f "$PSScriptRoot/MigrationScripts"

if($LASTEXITCODE -ne 0) { exit 1 }
