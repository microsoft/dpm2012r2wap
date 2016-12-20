function Create-Exception 
{
    Param(
        [string] $Message,
        [string] $ErrorID,
        [Management.Automation.ErrorCategory] $ErrorCategory,
        [object] $Target
    )

    $exception = New-Object Exception $Message
    $errorRecord = New-Object Management.Automation.ErrorRecord $exception, $ErrorID, $ErrorCategory, $Target
    return  $errorRecord;
}
