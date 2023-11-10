$content = Get-Content "$PSSCriptRoot.\log.txt"
$var = $false
$lines = @()
foreach ($line in $content)
{
     if ($line -match "IMPROVE PATH FROM ALTERNATIVE BROKEN EDGE 2")
     {
        Write-Output "Checking"
        if($var){
            foreach($_ in $lines) {Write-Output $_}
            $lines = @()
            continue
        }
        $var = $true
    }
    if($var -and  ($line -match "Not updating partial sum"))
    {
        Write-Output "Unsuccessful"
        $var = $false
    }
    if($var -and  ($line -match "Updating partial sum"))
    {
        if($var){
            foreach($_ in $lines) {Write-Output $_}
            $lines = @()
            continue
        }
        $var = $false
    }
}