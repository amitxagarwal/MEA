<#
.SYNOPSIS
  Builds the project and runs the tests, optionally publishing artifacts and packages for later deployment.
.DESCRIPTION
  Builds the project and runs the tests, optionally publishing artifacts and packages for later deployment.
.PARAMETER $ArtifactsStagingPath
  If specified, the staged artifacts are placed into this folder as a temporary output storage location.
.PARAMETER $PublishArtifactsToAzureDevOps
  If $true, the build publishes artifacts to Azure DevOps by writing the necessary lines to the console output.
.PARAMETER $BuildVerbosity
  The dotnet --verbosity= value: q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic].
.PARAMETER $SrcBranchName
  The source branch name e.g. "local" or "3123-issue", usually $env:BUILD_SOURCEBRANCHNAME from Azure DevOps.
.PARAMETER $BuildId
  The build ID e.g. 12345, defaults to $env:BUILD_BUILDID from Azure DevOps.
.INPUTS
  none
.OUTPUTS
  none
.EXAMPLE
  build.ps1
.EXAMPLE
  build.ps1 -ArtifactsStagingPath ./artifacts
  
  Builds and tests everything and writes the generated artifacts to a specified path.
.EXAMPLE
  build.ps1 -pubtodevops
  
  Builds and tests everything and publishes artifacts to Azure DevOps by writing the magic output lines it requires.
.EXAMPLE
  build.ps1 -BuildVerbosity normal -ArtifactsStagingPath C:\temp\artifacts-staging -PublishArtifactsToAzureDevOps
  
  Just another example using more options.
#>
Param(
    # If specified, the artifacts output into this folder
    [Parameter(Mandatory=$false, Position=1)]
    [System.IO.FileInfo]
    $ArtifactsStagingPath = "artifacts",

    # If $true, then we will write output lines so Azure DevOps takes the artifacts.
    [Parameter(Mandatory=$false, Position=2)]
    [switch]
    [Alias("pubtodevops")]
    $PublishArtifactsToAzureDevOps = $false,

    # The dotnet --verbosity= value: q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic].
    [Parameter(Mandatory=$false, Position=3)]
    [string]
    [ValidateSet("quiet", "q", "minimal", "m", "normal", "n", "detailed", "d", "diagnostic", "diag")]
    $BuildVerbosity = "minimal",

    # The source branch name e.g. "local" or "3123-issue", usually $env:BUILD_SOURCEBRANCHNAME from Azure DevOps
    [Parameter(Mandatory=$false, Position=4)]
    [string]
    $SrcBranchName = $env:BUILD_SOURCEBRANCHNAME,

    # The build ID e.g. 12345, defaults to $env:BUILD_BUILDID from Azure DevOps
    [Parameter(Mandatory=$false, Position=5)]
    [string]
    $BuildId = $env:BUILD_BUILDID,

    # The client id for integration tests to run
    [Parameter(Mandatory=$true)]
    $McaClientId,

    # The client secret for integration tests to run
    [Parameter(Mandatory=$true)]
    $McaClientSecret,

    # The MCA Api Uri for integration tests to run
    [Parameter(Mandatory=$true)]
    $McaApiUri,

    # The scope for integration tests to run
    [Parameter(Mandatory=$true)]
    $McaScope,

    # The environment for integration tests to run only in phoenix environment
    [Parameter(Mandatory=$true)]
    $Environment,

     # The client id to get token
    [Parameter(Mandatory=$true)]
    $MeaClientId,

    # The client secret to get token
    [Parameter(Mandatory=$true)]
    $MeaClientSecret,

    # The scope to get token
    [Parameter(Mandatory=$true)]
    $MeaScope,

    # The DbRequired for build database if it is true
    [Parameter(Mandatory=$false)]
    [string]
    $DbRequired = "false"
)

function Compress-Directory {
    param (
        [string]$directoryToZip
    )
    $zippedArtifactOutputFolder = [System.IO.Directory]::GetParent("$directoryToZip")
    $name = [System.IO.Path]::GetFileName($directoryToZip)
    $zipFileName = "$name.zip"
    $zippedArtifactFullFilePath = [System.IO.Path]::Combine("$zippedArtifactOutputFolder", "$zipFileName")
    Write-Host "build: zipping '$directoryToZip' into '$zippedArtifactFullFilePath'"
    Push-Location $directoryToZip
    Compress-Archive -Path "./*" -DestinationPath "$zippedArtifactFullFilePath" -Force
    Pop-Location
    return $zippedArtifactFullFilePath
}

Write-Host "build: Build started"

if ($VerbosePreference) {
    & dotnet --info
}


try{
   
    if($DbRequired -eq 'true')
    {
        Write-Host "Database is required"

        Push-Location "$PSScriptRoot/src/PostgreSqlDb"

        Write-Host "build: Starting in folder Kmd.Momentum.Mea.DbAdmin"
    
        if(Test-Path ./artifacts) {
            Write-Host "build: Cleaning ./artifacts"
            Remove-Item ./artifacts -Force -Recurse
        }

        $branch = @{ $true = $SrcBranchName; $false = $(git symbolic-ref --short -q HEAD) }[$SrcBranchName -ne $NULL];    
        $suffix = @{ $true = ""; $false = "$($branch.Substring(0, [math]::Min(10,$branch.Length)))-$revision"}[$branch -eq "master" -and $revision -ne "lo"]
        $commitHash = $(git rev-parse --short HEAD)
        $buildSuffix = @{ $true = "$($suffix)-$($commitHash)"; $false = "$($branch)-$($commitHash)" }[$suffix -ne ""]

        & dotnet build "Kmd.Momentum.Mea.DbAdmin.sln" -c Release --verbosity "$BuildVerbosity" --version-suffix "$buildSuffix"
  
        if($LASTEXITCODE -ne 0) { exit 1 }

        $now = Get-Date
        $nowStr = $now.ToUniversalTime().ToString("yyyyMMddHHmmss")
        $BuildDatabaseName = "$nowStr-$ImageName-$buildSuffix"
        $DbServer = "kmd-momentum-api-build-dbsvr"
    
        Push-Location "./Kmd.Momentum.Mea.DbAdmin"

        Write-Host "Creating Database '$BuildDatabaseName'"

        & dotnet run -- create -s $DbServer -d $BuildDatabaseName -u $BuildDatabaseName -p RtAhL8j9946W
    
        if($LASTEXITCODE -ne 0) { exit 1 }

        Write-Host "Migrate Database '$BuildDatabaseName' with MigrationScripts"

        & dotnet run -- migrate -s $DbServer -d $BuildDatabaseName -u $BuildDatabaseName -p RtAhL8j9946W -f "$PSScriptRoot/MigrationScripts"
    
        if($LASTEXITCODE -ne 0) { exit 1 }  
       
        $expiryMinutes = 120;
    
        Write-Host "cleanup: database '$BuildDatabaseName'"
        Write-Host "Also looking for any databases with a timestamp <=" $expiryMinutes "minutes ago"
    
        & dotnet run -- delete -s $DbServer -d $BuildDatabaseName -r "^\\d{14}\\-" -e $expiryMinutes -f "yyyyMMddHHmmss-"

        if($LASTEXITCODE -ne 0) { exit 1 }
    
        if ($PublishArtifactsToAzureDevOps) {

            Write-Host "Creating artifacts for database"

            foreach ($item in Get-ChildItem "./bin/*/*") {

                Write-Host "##vso[artifact.upload artifactname=dbApp;]$item"

                if($LASTEXITCODE -ne 0) { exit 1 }
            }
        
            Write-Host "Creating artifacts for database migration scripts"

            foreach ($item in Get-ChildItem "$PSScriptRoot/MigrationScripts") {
        
                Write-Host "##vso[artifact.upload artifactname=migrationScripts;]$item"

                if($LASTEXITCODE -ne 0) { exit 1 }
            }

        }

        Pop-Location
    }
    else
    {
        Write-Host "Database is not required"
    }
}
catch{

    Write-Host "An error occurred:"
	Write-Host $_
    Write-Host "##vso[task.LogIssue type=error;]"$_
    Write-Host "##vso[task.complete result=Failed]"

    exit 1
}

Push-Location $PSScriptRoot

try {
    Write-Host "build: Starting in folder '$PSScriptRoot'"
    
    if(Test-Path ./artifacts) {
        Write-Host "build: Cleaning ./artifacts"
        Remove-Item ./artifacts -Force -Recurse
    }
    
    $ArtifactsStagingPath = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($ArtifactsStagingPath)
    
    Write-Host "build: staging artifacts to '$ArtifactsStagingPath'"
    
    $branch = @{ $true = $SrcBranchName; $false = $(git symbolic-ref --short -q HEAD) }[$SrcBranchName -ne $NULL];
    $revision = @{ $true = "{0:00000}" -f [convert]::ToInt32("0" + $BuildId, 10); $false = "lo" }[$BuildId -ne $NULL];
    $suffix = @{ $true = ""; $false = "$($branch.Substring(0, [math]::Min(10,$branch.Length)))-$revision"}[$branch -eq "master" -and $revision -ne "lo"]
    $commitHash = $(git rev-parse --short HEAD)
    $buildSuffix = @{ $true = "$($suffix)-$($commitHash)"; $false = "$($branch)-$($commitHash)" }[$suffix -ne ""]

    if($LASTEXITCODE -ne 0) { exit 1 }

    Write-Host "build: Package version suffix is $suffix"
    Write-Host "build: Build version suffix is $buildSuffix"

    & dotnet build "kmd-momentum-mea.sln" -c Release --verbosity "$BuildVerbosity" --version-suffix "$buildSuffix"

    if($LASTEXITCODE -ne 0) {
    
        Write-Host "build again to fix dependencies if exist"
        
        & dotnet build "kmd-momentum-mea.sln" -c Release --verbosity "$BuildVerbosity" --version-suffix "$buildSuffix"
    }

    if($LASTEXITCODE -ne 0) { exit 1 }

    if ($PublishArtifactsToAzureDevOps) {

        Write-Host '', "Publish is started for Application"
        
        $PublishedApplications = $(
            "Kmd.Momentum.Mea.Api"
        )
        
        foreach ($srcProjectName in $PublishedApplications) {
            try {
            
                Push-Location "./src/$srcProjectName"
                Write-Host "build: publishing output of '$srcProjectName' into '$ArtifactsStagingPath/$srcProjectName'"

                if ($suffix) {
                    & dotnet publish -c Release --verbosity "$BuildVerbosity" --no-build --no-restore -o "$ArtifactsStagingPath/$srcProjectName" --version-suffix "$suffix"
                }
                else {
                    & dotnet publish -c Release --verbosity "$BuildVerbosity" --no-build --no-restore -o "$ArtifactsStagingPath/$srcProjectName"
                }
                
                if($LASTEXITCODE -ne 0) { exit 1 }

                Write-Host '', "Publish: Compression is started for Application"
                
                $compressedArtifactFileName = Compress-Directory "$ArtifactsStagingPath/$srcProjectName"
                
                Write-Host '', "Publish: Uploading is started for Application"

                Write-Host "##vso[artifact.upload artifactname=Applications;]$compressedArtifactFileName"
                
            }
            catch{
            
                Write-Host "An error occurred:"
                Write-Host $_
                Write-Host "##vso[task.LogIssue type=error;]"$_
                Write-Host "##vso[task.complete result=Failed]"
                
                exit 1
            }
            finally {
                Pop-Location
            }
        }
    }

    foreach ($testFolder in Get-ChildItem "./test/*.Tests") {
        Push-Location "$testFolder"
        try {
            Write-Host "build: Testing project in '$testFolder'"
            
            ($env:KMD_MOMENTUM_MEA_McaClientSecret = $McaClientSecret); 
            ($env:KMD_MOMENTUM_MEA_McaClientId = $McaClientId); 
            ($env:KMD_MOMENTUM_MEA_McaApiUri = $McaApiUri); 
            ($env:KMD_MOMENTUM_MEA_McaScope = $McaScope);
            ($env:KMD_MOMENTUM_MEA_ClientSecret = $MeaClientSecret); 
            ($env:KMD_MOMENTUM_MEA_ClientId = $MeaClientId); 
            ($env:KMD_MOMENTUM_MEA_Scope = $MeaScope);
            ($env:ASPNETCORE_ENVIRONMENT = $Environment) | dotnet test -c Release --logger trx --verbosity="$BuildVerbosity" --no-build --no-restore
            
            if($LASTEXITCODE -ne 0) { exit 1 }
        }
        catch{
        
            Write-Host "An error occurred:"
            Write-Host $_
            Write-Host "##vso[task.LogIssue type=error;]"$_
            Write-Host "##vso[task.complete result=Failed]"
            
            exit 1

        }
        finally {
            Pop-Location
        }
    }

    if($PublishArtifactsToAzureDevOps) {
        $deployScriptSourcePath = "$PSScriptRoot/deploy"
        $artifactsOutputPath = "$ArtifactsStagingPath/deploy"
        
        Write-Host "build: publishing files from '$deployScriptSourcePath' to '$artifactsOutputPath'"
        
        If(!(test-path $artifactsOutputPath)) {
            Write-Host "build: creating folder '$artifactsOutputPath'"
            New-Item -ItemType Directory -Force -Path $artifactsOutputPath
        }

        if($LASTEXITCODE -ne 0) { exit 1 }
        
        $resolvedArtifactsOutputPath = Resolve-Path -Path "$artifactsOutputPath"

        if($LASTEXITCODE -ne 0) { exit 1 }

        Copy-Item "$deployScriptSourcePath/*" -Destination $resolvedArtifactsOutputPath -Recurse

        if($LASTEXITCODE -ne 0) { exit 1 }
        
        Write-Host "##vso[artifact.upload containerfolder=deploy;artifactname=deploy;]$resolvedArtifactsOutputPath"
    }
}
catch{
    
    Write-Host "An error occurred:"
    Write-Host $_
    Write-Host "##vso[task.LogIssue type=error;]"$_
    Write-Host "##vso[task.complete result=Failed]"
    exit 1

}finally {
    Pop-Location
}