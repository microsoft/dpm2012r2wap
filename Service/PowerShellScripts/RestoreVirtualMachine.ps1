$dpmServer = '@server';
$PGName = '@protectionGroup';
$VMId = '@vmId';
$RecoverySourceId = '@recoverySourceId';

Import-Module DataProtectionManager; 
$PG = Get-DPMProtectionGroup -DPMServerName $dpmServer | ?{$_.Name -eq $PGName};
if ($PG -eq $null) 
{ 
    throw '10001: PG not found!';
}; 

$DS = Get-DPMDatasource -ProtectionGroup $PG | ?{$_.ComponentName -eq $VMId}; 
if ($DS -eq $null) 
{ 
    throw '10002: VM not found!';  
}; 

$RecoveryObject = Get-DPMRecoverypoint -Datasource $DS | ?{$_.RecoverySourceId -eq $RecoverySourceId};
if ($RecoveryObject -eq $null)
{
    throw '10004: Recovery Point not found!';
};

$RecoveryOption = New-DPMRecoveryOption -HyperVDatasource -RecoveryLocation OriginalServer -RecoveryType Recover -TargetServer $DS.ProductionServerName;
$job = Restore-DPMRecoverableItem -RecoverableItem $RecoveryObject -RecoveryOption $RecoveryOption;

while ($job.HasCompleted -eq $false) { sleep 10 };
if ($job.Status -ne 'Succeeded')
{
    throw $job.Error;
};