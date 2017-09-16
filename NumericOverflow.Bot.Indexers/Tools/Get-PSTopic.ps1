# Before using this command you must load NumericOverflow.Bot.dll
# Set $botDllLocation variable with the dll location
[Reflection.Assembly]::Load([System.IO.File]::ReadAllBytes($botDllLocation)) | Out-Null
# Get-Command Get-OperatingSystem | Get-PSTopic | ConvertTo-Json -Depth 3 | Out-File -FilePath .\PSTopics.json

function Get-PSTopic {
    [CmdletBinding()]
	param( 
        [Parameter(
                Position=0, 
                Mandatory=$true, 
                ValueFromPipeline=$true)
        ][System.Management.Automation.CommandInfo] $Command
	)
    process {
        $Command.Name | Out-Host
        try {
            $id = $Command.Name
            $help = Get-Help $Command.Name
            $title = $help.Details[0].Description[0].Text
            $content = $help.Description[0].Text
	        $topic = New-Object -TypeName NumericOverflow.Bot.Models.Topic -ArgumentList $id, $title, $content
            $topic
        } catch {
        }
    }
}
