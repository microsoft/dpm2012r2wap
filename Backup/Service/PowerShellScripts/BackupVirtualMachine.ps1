$dpmServer = '@server';
$PGName = '@protectionGroup';
$VMId = '@vmId';

Import-Module DataProtectionManager; 
$prodSvrs = @(Get-DPMProductionServer -DPMServerName 'dpm01.dcs.corp')
foreach ($prodServer in $prodSvrs)
{
    $Foo = Get-DPMDatasource -ProductionServer $prodServer -Inquire
}

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
 
$job = New-DPMRecoveryPoint -Datasource $DS -Disk -BackupType ExpressFull
while ($job.HasCompleted -eq $false) { sleep 10 }
if ($job.Status -ne 'Succeeded')
{
    throw $job.Error;
};