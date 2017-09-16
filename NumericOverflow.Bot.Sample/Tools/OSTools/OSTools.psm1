
function Get-SystemInformation {

<#
.SYNOPSIS
Retrieves system information
.DESCRIPTION
Retrieves system information
.EXAMPLE
Get-SystemInformation -Property OSName
.PARAMETER Property
Specify the property to retrieve: Name or Architecture
#>
    [CmdletBinding()]
	param(
        [ValidateSet('OSName','OSArchitecture')] [String] $Property = 'OSName'
	)
	Get-ComputerInfo -Property $Property
}


function Get-OperatingSystem {

<#
.SYNOPSIS
Retrieves operating system name
.DESCRIPTION
Retrieves OS name
.EXAMPLE
Get-OperatingSystem
#>
    [CmdletBinding()]
	param(
	)
	Get-ComputerInfo -Property 'OSName'
}

function Get-SystemArchitecture {

<#
.SYNOPSIS
Retrieves system architecture
.DESCRIPTION
Retrieves OS information
.EXAMPLE
Get-SystemArchitecture
#>
    [CmdletBinding()]
	param(
	)
	Get-ComputerInfo -Property 'OSArchitecture'
}
