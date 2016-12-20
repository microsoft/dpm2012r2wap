$dpmServer = '@server';
$PGName = '@protectionGroup';
$VMId = '@vmId';

Import-Module DataProtectionManager; 
$ExistingPG = Get-DPMProtectionGroup -DPMServerName $dpmServer | ?{$_.Name -eq $PGName}; 
if ($ExistingPG -eq $null) 
{ 
    throw '10001: PG not found!';
}; 

$PG = Get-ModifiableProtectionGroup $ExistingPG; 
$DS = Get-DPMDatasource -ProtectionGroup $ExistingPG | ?{$_.ComponentName -eq $VMId}; 
if ($DS -eq $null) 
{ 
    throw '10002: VM not found!';  
}; 

$jobs = @(Get-DPMJob -DPMServerName $dpmServer -Status InProgress | ?{$_.ProtectionGroupName -eq $PGName})
if ($jobs.Count -gt 0)
{
    $message = 'One or more jobs that could be affected by this operation are running so the job will not be run.'
    $exception = New-Object InvalidOperationException $message
    $errorID = '10005'
    $errorCategory = [Management.Automation.ErrorCategory]::DeadlockDetected
    $target = $PGName
    $errorRecord = New-Object Management.Automation.ErrorRecord $exception, $errorID, $errorCategory, $target
    throw $errorRecord
}

Remove-DPMChildDatasource -ProtectionGroup $PG -ChildDatasource $DS;
Set-DPMProtectionGroup -ProtectionGroup $PG -ErrorAction Stop;
