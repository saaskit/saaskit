##################################################
#resources
##################################################
#http://www.powershellpro.com/powershell-tutorial-introduction/powershell-tutorial-conditional-logic/
#http://technet.microsoft.com/en-us/library/ee176935.aspx
#http://weblogs.asp.net/soever/archive/2007/02/06/powershell-regerencing-files-relative-to-the-currently-executing-script.aspx
##################################################
#resources
##################################################
param(
	$nugetApiKey = "$env:NUGET_API_KEY"
)

function require-param { 
    param($value, $paramName)
    
    if($value -eq $null) { 
        write-error "The parameter -$paramName is required."
    }
}


Write-Host $nugetApiKey
require-param $nugetApiKey -paramName "nugetApiKey"

#safely find the solutionDir
$ps1Dir = (Split-Path -parent $MyInvocation.MyCommand.Definition)
$solutionDir = Split-Path -Path $ps1Dir -Parent
$nugetExePath = resolve-path(join-path $solutionDir "\.nuget")

$packages = dir "$solutionDir\artifacts\packages\*.nupkg"

foreach($package in $packages) { 
    #$package is type of System.IO.FileInfo
    & "$nugetExePath\Nuget.exe" push $package.FullName $nugetApiKey
}