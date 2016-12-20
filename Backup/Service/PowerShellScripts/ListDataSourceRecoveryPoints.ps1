$dpmServer = '@server';
$PGName = '@protectionGroup';
$VMId = '@vmId';

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
 
$Results = @(); 
$RecoveryObjects = Get-DPMRecoverypoint -Datasource $DS; 
$RecoveryObject += Get-DPMRecoverypoint -Datasource $DS -Online; 
$Results += $RecoveryObjects | Sort -Property RepresentedPointInTime -Descending | select RecoverySourceId, ComponentName, RepresentedPointInTime, Location; 
$Results;