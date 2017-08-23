# Before using this command you must load NumericOverflow.Bot.dll
# Set $botDllLocation variable with the dll location
[Reflection.Assembly]::Load([System.IO.File]::ReadAllBytes($botDllLocation)) | Out-Null

function Get-PSTopics {
	param( 
	)
	(Get-Command Get-Date, Get-Clipboard, Set-Clipboard) | Select-Object -First 50 | ForEach-Object {
        $_.Name | Out-Host
        try {
            $help = Get-Help $_.Name
            $id = $_.Name
            $title = $help.Details[0].Description[0].Text
            $content = $help.Description[0].Text
	        $topic = New-Object -TypeName NumericOverflow.Bot.Models.Topic -ArgumentList $id, $title, $content
            $topic
        } catch {
        }
    } | ConvertTo-Json | Out-File -FilePath .\PSTopics.json
}
