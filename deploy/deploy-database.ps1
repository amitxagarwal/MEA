<#
.SYNOPSIS
  Deploys Azure Postgres Migration Scripts.
.DESCRIPTION
  Deploys the Azure resources for invoicing. If you haven't already logged in, execute `Connect-AzAccount`
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
  Author:         Ajay Aggarwal
  Creation Date:  26 Mar 2020
  Purpose/Change: Deploy momentum mea azure infrastructure.
#>

Param
(
  [Parameter(Mandatory=$true)]
  [string]
  $InstanceId,

  [Parameter(Mandatory=$true)]
  [string]
  $ArtifactPath
)

Write-Host "Migrate Database '$DbName' with MigrationScripts"

Push-Location "$ArtifactPath/dbApp"

$ResourceNamePrefix = "kmd-momentum-mea-$InstanceId"

$DbServerName="$ResourceNamePrefix-dbsvr";
$DbName="$ResourceNamePrefix-db";

& dotnet Kmd.Momentum.Mea.DbAdmin.dll migrate -s $DbServerName -d $DbName -u $env:DbLoginId -p $env:DbLoginPassword -f "$ArtifactPath/migrationScripts"

if($LASTEXITCODE -ne 0) { exit 1 } 
