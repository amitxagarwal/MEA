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
  $ClientSecret,

  [Parameter(Mandatory=$true)]
  [string]
  $Environment,

  [Parameter(Mandatory=$false)]
  [string]
  $DbRequired = 'false',

  [Parameter(Mandatory=$false)]
  [string]
  $KeyVaultRequired = 'false',

  [Parameter(Mandatory=$false)]
  [string]
  $DbLoginId = 'MeaAdmin',

  [Parameter(Mandatory=$false)]
  [string]
  $DbLoginPassword = 'Admin@123',

  [Parameter(Mandatory=$true)]
  [string]  
  $MeaAuthorizationAudience,
  
  [Parameter(Mandatory=$false)]
  [string]  
  $IsKeyVaultPolicyRequired = 'false',
    
  [Parameter(Mandatory=$true)]
  [string]  
  $AppObjectId,

  [Parameter(Mandatory=$true)]
  [string]  
  $TenantId,

  [Parameter(Mandatory=$true)]
  [string]  
  $ApplicationId
)

Push-Location $PSScriptRoot
Write-Host "Deploying from '$PSScriptRoot'"

$ResourceNamePrefix = "kmd-momentum-mea-$InstanceId"
$TemplateFile = "azuredeploy.json"

try {
    #[Microsoft.Azure.Common.Authentication.AzureSession]::ClientFactory.AddUserAgent("VSAzureTools-$UI$($host.name)".replace(' ','_'), '3.0.0')
} catch {
    Write-Host '',"An error occurred:"
    Write-Host '',$_
}

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3

function Format-ValidationOutput {
    param ($ValidationOutput, [int] $Depth = 0)
    Set-StrictMode -Off
    return @($ValidationOutput | Where-Object { $_ -ne $null } | ForEach-Object { @('  ' * $Depth + ': ' + $_.Message) + @(Format-ValidationOutput @($_.Details) ($Depth + 1)) })
}
try
{
    Write-Host "Setting variables"

    $ResourceGroupName = "$ResourceNamePrefix-rg"
    $ApplicationInsightsName="$ResourceNamePrefix-ai";
    $DbServerName="$ResourceNamePrefix-dbsvr";
    $DbName="$ResourceNamePrefix-db";
    $DbConnection="Server=$($DbServerName).postgres.database.azure.com;Database=$($DbName);Port=5432;User Id=$($DbLoginId)@$($DbServerName);Password=$($DbLoginPassword);Ssl Mode=Require;"
    $KeyVaultName = "$($ResourceNamePrefix.replace('-',''))kv"

    Write-Host "Checking KeyVault Name Length"

    if($KeyVaultName.length -gt 24){

        Write-Host "Managing KeyVault Name Length"

        $KeyVaultName = $KeyVaultName.substring($KeyVaultName.length-24,24);

        Write-Host "Managing KeyVault Name Length completed"
    }

    Write-Host "Setting ARM Template parameters"

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
    clientSecret = $ClientSecret
    dbServerName = $DbServerName;
    dbLoginId = $DbLoginId;
    dbLoginPassword = $DbLoginPassword;
    dbName = $DbName;
    dbConnection = $DbConnection;
    dbRequired = $DbRequired;
    keyVaultRequired = $KeyVaultRequired;
    keyVaultName = $KeyVaultName;
    meaAuthorizationAudience = $MeaAuthorizationAudience;
    isKeyVaultPolicyRequired = $IsKeyVaultPolicyRequired;
    appObjectId = $AppObjectId;
    tenantId = $TenantId;
    applicationId= $ApplicationId;
    }

    Write-Host "Create or update ResourceGroup Tag"

    # Create or update the resource group using the specified template file and template parameter values
    $Tags = @{}
    if ($MarkForAutoDelete) {
	    $Tags["keep"] = "false";
    } else {
	    $Tags["important"] = "true";
    }

    Write-Host "Creating resource group '$ResourceGroupName'"

	New-AzResourceGroup -Name $ResourceGroupName -Location $ResourceGroupLocation -Tags $Tags -Verbose -Force -ErrorVariable ErrorMessages

    Write-Host "Resource group '$ResourceGroupName' created successfully"

    if ($ErrorMessages) {
    
        $ErrMsg =  @(@($ErrorMessages) | ForEach-Object { $_.Exception.Message.TrimEnd("`r`n") })
		Write-Host '', "Error on creating resource group '$ResourceGroupName'", $ErrMsg
        Write-Host '',"##vso[task.LogIssue type=error;]"$ErrMsg
        Write-Host '',"##vso[task.complete result=Failed]"
		exit 1
    }
    
	if ($ValidateOnly) {
        Write-Host '', 'Validating deployment Template.'

		$ErrorMessages = Format-ValidationOutput (Test-AzResourceGroupDeployment -ResourceGroupName $ResourceGroupName `
                                                                                -TemplateFile $TemplateFile `
                                                                                @TemplateParameters)

		if ($ErrorMessages) {            

			Write-Host '', 'Validation returned the following errors:', $ErrorMessages, '', 'Template is invalid.'
            Write-Host '',"##vso[task.LogIssue type=error;]"$ErrorMessages
            Write-Host '',"##vso[task.complete result=Failed]"
			exit 1

		}else {
			Write-Host '', 'Template is valid.'
		}        
	} else {

        Write-Host "Deploying resources."

		New-AzResourceGroupDeployment -Name ((Get-ChildItem $TemplateFile).BaseName + '-' + ((Get-Date).ToUniversalTime()).ToString('MMdd-HHmm')) `
                                      -ResourceGroupName $ResourceGroupName `
                                      -TemplateFile $TemplateFile `
                                      @TemplateParameters `
                                      -Force -Verbose `
                                      -ErrorVariable ErrorMessages `
                                      -Mode 'Incremental'

        Write-Host "Deploying of resources completed."

		if ($ErrorMessages) {
            $ErrMsg =  @(@($ErrorMessages) | ForEach-Object { $_.Exception.Message.TrimEnd("`r`n") })
			Write-Host '', 'Template deployment returned the following errors:', $ErrMsg
            Write-Host '',"##vso[task.LogIssue type=error;]"$ErrMsg
            Write-Host '',"##vso[task.complete result=Failed]"
			exit 1
		}     
	}
}catch{
	Write-Host '',"An error occurred:"
	Write-Host '',$_
    Write-Host '',"##vso[task.LogIssue type=error;]"$_
    Write-Host '',"##vso[task.complete result=Failed]"
    exit 1
}
Pop-Location
