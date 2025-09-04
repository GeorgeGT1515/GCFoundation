param(
  [Parameter(Mandatory=$true)][string]$ResourceGroupName,
  [Parameter(Mandatory=$true)][string]$WebAppName,
  [Parameter(Mandatory=$true)][string]$Slot,
  [Parameter(Mandatory=$true)][string]$UseSlots,
  [Parameter(Mandatory=$true)][string]$VirtualDirectoryFullPath,
  [bool]$PreloadEnabled = $false
)

Function CreateVirtualApplication {
  Param(
    [string]$ARG,
    [string]$AWAN,
    [string]$WS,
    [bool]$PE,
    [string]$PP,
    [string]$VP,
    [string]$US
  )

  $updateWebAppSlot = "true"
  if ($WS -eq "Production" -or $US -eq "false") {
    $updateWebAppSlot = "false"
  }

  if ($updateWebAppSlot -eq "false") {
    $webAppSlot = Get-AzureRmWebApp -ResourceGroupName $ARG -Name $AWAN
  } else {
    $webAppSlot = Get-AzureRmWebAppSlot -ResourceGroupName $ARG -Name $AWAN -Slot $WS
  }

  if ($webAppSlot.siteConfig.VirtualApplications.VirtualPath -notcontains $VP) {
    $virtApp = New-Object Microsoft.Azure.Management.WebSites.Models.VirtualApplication
    $virtApp.VirtualPath = $VP
    $virtApp.PhysicalPath = $PP
    $virtApp.PreloadEnabled = $PE

    [void]$webAppSlot.siteConfig.VirtualApplications.Add($virtApp)

    if ($updateWebAppSlot -eq "false") {
      Set-AzureRmWebApp -WebApp $webAppSlot
    } else {
      Set-AzureRmWebAppSlot -WebApp $webAppSlot
    }
  } else {
    for ($i = 0; $i -lt $webAppSlot.siteConfig.VirtualApplications.Count; $i++) {
      $webAppSlotVirtualApp = $webAppSlot.siteConfig.VirtualApplications[$i]
      if ($webAppSlotVirtualApp.VirtualPath -eq $VP) {
        $webAppSlotVirtualApp.PhysicalPath = $PP
      }
    }
    if ($updateWebAppSlot -eq "false") {
      Set-AzureRmWebApp -WebApp $webAppSlot
    } else {
      Set-AzureRmWebAppSlot -WebApp $webAppSlot
    }
  }
}

# Map parameters to variables used in existing logic
$AzureResGroup = $ResourceGroupName
$AzureWebAppName = $WebAppName
$WebSlot = $Slot
$usesSlots = $UseSlots

# Normalize empty path to "/"
if ([string]::IsNullOrWhiteSpace($VirtualDirectoryFullPath)) {
  $VirtualDirectoryFullPath = "/"
}

$separators = "/"
$separators2 = ";"
$separators3 = "&"
$option = [System.StringSplitOptions]::RemoveEmptyEntries
$distinctVirtualDirectories = $VirtualDirectoryFullPath.Split($separators2)

for ($dvds = 0; $dvds -lt $distinctVirtualDirectories.Count; $dvds++) {
  $distinctVirtualDirectory = $distinctVirtualDirectories[$dvds]

  if ($distinctVirtualDirectory.StartsWith("%")) {
    $virtualDirectories = $distinctVirtualDirectory.Split($separators3, $option)
    $PhysicalPath = "site\wwwroot\" + $virtualDirectories[0].Substring(1)
    $VirtualPath = $virtualDirectories[1]

    if ($VirtualPath -eq "/") { $VirtualPath = "" }

    $PhysicalPath = $PhysicalPath.Replace('/', '\\')
    $VirtualPath = $VirtualPath.Replace('\\', '/')

    CreateVirtualApplication -ARG $AzureResGroup -AWAN $AzureWebAppName -WS $WebSlot -PE $PreloadEnabled -PP $PhysicalPath -VP $VirtualPath -US $usesSlots
  }
  else {
    $virtualDirectories = $distinctVirtualDirectory.Split($separators, $option)

    if ($virtualDirectories.Count -eq 0) {
      $PhysicalPath = "site\wwwroot"
      $VirtualPath = ""
      #CreateVirtualApplication -ARG $AzureResGroup -AWAN $AzureWebAppName -WS $WebSlot -PE $PreloadEnabled -PP $PhysicalPath -VP $VirtualPath -US $usesSlots
    }
    else {
      for ($vds = 0; $vds -lt $virtualDirectories.Count; $vds++) {
        $VirtualPath = "/"
        $PhysicalPath = ""

        for ($vps = 0; $vps -lt $vds + 1; $vps++) {
          $VirtualPath = $VirtualPath + $virtualDirectories[$vps] + "/"
          $PhysicalPath = $PhysicalPath + $virtualDirectories[$vps] + "/"
        }

        $PhysicalPath = "site\wwwroot\$PhysicalPath"
        $PhysicalPath = $PhysicalPath.Replace('/', '\\')
        $VirtualPath = $VirtualPath.Replace('\\', '/')

        if ($PhysicalPath.Length -gt 1) {
          do {
            $PhysicalPath = $PhysicalPath.Substring(0, $PhysicalPath.Length - 1)
          } while ($PhysicalPath.Substring($PhysicalPath.Length - 1, 1) -eq "\\")
        }

        if ($VirtualPath.Length -gt 1) {
          do {
            $VirtualPath = $VirtualPath.Substring(0, $VirtualPath.Length - 1)
          } while ($VirtualPath.Substring($VirtualPath.Length - 1, 1) -eq "/")
        }

        CreateVirtualApplication -ARG $AzureResGroup -AWAN $AzureWebAppName -WS $WebSlot -PE $PreloadEnabled -PP $PhysicalPath -VP $VirtualPath -US $usesSlots
      }
    }
  }
}


