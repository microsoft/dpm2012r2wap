$dpmServer = '@server';
$PGName = '@protectionGroup';

Import-Module DataProtectionManager;
$PG = Get-DPMProtectionGroup -DPMServerName $dpmServer | ?{$_.Name -eq $PGName};
if ($PG -eq $null) 
{ 
    throw '10001: PG not found!';
};
 
$DS = @(Get-DPMDatasource -ProtectionGroup $PG); 
if ($DS -eq $null) 
{ 
    throw '10002: VM not found!'; 
};
 
$Schedule = $PG.Performance.Split('|')[2].Split('-')[1];
$Results = @(); 
foreach($dataSource in $DS)
{
    $RecoveryObject = Get-DPMRecoverypoint -Datasource $dataSource
    $measures = $RecoveryObject | measure RepresentedPointInTime -Maximum | select Count, Maximum
    $Results += $dataSource| select  @{name='Schedule';expression={$Schedule}}, DatasourceId, ComponentName, @{name='TotalRecoveryPoints';expression={$measures.Count}}, @{name='LatestRecoveryPoint';expression={$measures.Maximum}}
}
$Results; 
