$dpmServer = '@server';
$PGName = '@protectionGroup';
$VMId = '@vmId';

Import-Module DataProtectionManager;
$prodSvrs = @(Get-DPMProductionServer -DPMServerName $dpmServer); 
foreach ($prodServer in $prodSvrs)
{
    $Foo = Get-DPMDatasource -ProductionServer $prodServer -Inquire;
}

$ExistingPG = Get-DPMProtectionGroup -DPMServerName $dpmServer | ?{$_.Name -eq $PGName};
if ($ExistingPG -eq $null)
{
    throw '10001: PG not found!';
};

$PG = Get-ModifiableProtectionGroup $ExistingPG;
$DPMDisks = Get-DPMDisk -DPMServerName $dpmServer | where{$_.IsInStoragePool -eq $true};
[int64]$AvailableStorage = ($DPMDisks.UnallocatedSpace | Measure-Object -Sum).Sum;
if($AvailableStorage -le 0 )
{
    throw '10003: No space available';
};

$DS = Get-DPMDatasource | ?{$_.ComponentName -eq $VMId};
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

Add-DPMChildDatasource -ProtectionGroup $PG -ChildDatasource $DS -ErrorAction Stop;
Set-DPMProtectionType -ProtectionGroup $PG -ShortTerm disk -ErrorAction Stop;
Get-DatasourceDiskAllocation -Datasource $DS -ErrorAction Stop | Out-Null;
Set-DPMDatasourceDiskAllocation -Datasource $DS -ProtectionGroup $PG -ErrorAction Stop;
Set-DPMReplicaCreationMethod -ProtectionGroup $PG -Now;
Set-DPMProtectionGroup -ProtectionGroup $PG -ErrorAction Stop;

$PG = Get-DPMProtectionGroup -DPMServerName $dpmServer | ?{$_.Name -eq $PGName};
$DS = @(Get-DPMDatasource -ProtectionGroup $PG); 
if ($DS -eq $null) 
{ 
    throw '10002: VM not found!'; 
}; 
$DS | select DataSourceId, Name;

