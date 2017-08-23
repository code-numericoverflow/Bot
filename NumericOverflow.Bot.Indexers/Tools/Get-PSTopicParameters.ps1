# Before using this command you must load NumericOverflow.Bot.dll
# Set $botDllLocation variable with the dll location
[Reflection.Assembly]::Load([System.IO.File]::ReadAllBytes($botDllLocation)) | Out-Null

function Get-PSTopicParameters {
	param( 
	)
	(Get-Command Get-Date, Get-Clipboard, Set-Clipboard) | ForEach-Object {
        $command = $_
        $command.Name | Out-Host
        try {
			$topicId = $_.Name
            $parameters = $_.Parameters
			$parameters.Keys | ForEach-Object {
                $parameter = $parameters[$_]
                $parameterHelp = Get-Help $command.Name -Parameter $parameter.Name
				$topicParameter = New-Object -TypeName NumericOverflow.Bot.Models.TopicParameter
				$topicParameter.TopicId = $topicId
				$topicParameter.Id = $parameter.Name
                $topicParameter.Title = $topicParameter.Id
                $topicParameter.Description = $parameterHelp.Description[0].Text
                $topicParameter.Required = [bool]::Parse($parameterHelp.required)
                try {
                    $topicParameter.Default = New-Object -TypeName $parameter.ParameterType.Name
                } catch {
                }
                $topicParameter.Value = $topicParameter.Default
                $topicParameter.TypeName = $parameter.ParameterType.Name
                $matchesCommand = "$command -$($parameter.Name) "
                $matches = [System.Management.Automation.CommandCompletion]::CompleteInput($matchesCommand, $matchesCommand.Length, $null)
                $matches.CompletionMatches | ForEach-Object {
                    $match = $_
                    if (-not $match.CompletionText.StartsWith(".")) { 
                        $choiceItem = New-Object -TypeName NumericOverflow.Bot.Models.ChoiceItem
                        $choiceItem.Id = $match.CompletionText
                        $choiceItem.Description = $match.ListItemText
                        $topicParameter.Choices.Add($choiceItem)
                    }
                }
                $topicParameter
			}
        } catch {
        }
    } | ConvertTo-Json -Depth 3 | Out-File -FilePath .\PSTopicParameters.json
}
