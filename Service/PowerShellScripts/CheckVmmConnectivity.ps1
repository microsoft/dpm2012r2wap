$vmmServer = '@server';

Import-Module VirtualMachineManager; 

Get-SCVMMServer -ComputerName $vmmServer | Out-Null;