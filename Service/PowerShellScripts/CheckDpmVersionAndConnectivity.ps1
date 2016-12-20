$dpmServer = '@server';

Import-Module DataProtectionManager; 

# Check connectivity by getting a property
Get-DPMGlobalProperty -DPMServerName $dpmServer -PropertyName AllowLocalDataProtection | Out-Null;

# Get version number of cmdlets assembly
$DPMServer = Connect-DPMServer 'dpm01.dcs.corp'
$DPMProxy = $DPMServer.Proxy
$DPMInfo = $DPMProxy.GetRegistrationInfo()
$DPMInfo.Version