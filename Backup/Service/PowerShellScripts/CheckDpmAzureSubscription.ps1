$dpmServer = '@server'; 

Import-Module DataProtectionManager; 
$sub = Get-DPMCloudSubscription -DPMServerName $dpmServer;
if ($sub -eq $null)
{
	$exception = Create-Exception -Message 'Unable to retrieve cloud subscription information' -ErrorID '10001' -ErrorCategory ResourceUnavailable -Target $dpmServer;
	throw $exception;
}

if ($sub.RegistrationStatus -ne 'AgentInstalledAndRegistered')
{
	$exception = Create-Exception -Message 'The cloud subscription status is not reporting agent installed and registered.' -ErrorID '10002' -ErrorCategory InvalidResult -Target $sub;
	throw $exception;
}
$true