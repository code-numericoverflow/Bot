# Before using this command you must load NumericOverflow.Bot.dll
# Set $botDllLocation variable with the dll location
[Reflection.Assembly]::Load([System.IO.File]::ReadAllBytes($botDllLocation)) | Out-Null
# Get-Command Get-OperatingSystem | Get-PSTopicParameters | ConvertTo-Json -Depth 3 | Out-File -FilePath .\PSTopicParameters.json

function Get-PSTopicParameters {
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
			$topicId = $Command.Name
            $parameters = $Command.Parameters
			$parameters.Keys | ForEach-Object {
                $parameter = $parameters[$_]
                $parameterHelp = Get-Help $Command.Name -Parameter $parameter.Name
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
                $matchesCommand = "$Command -$($parameter.Name) "
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
    }
}
