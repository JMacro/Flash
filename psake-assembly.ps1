Include "psake-common.ps1"

function Collect-Assembly-Localizations(){
	$nupkgs = $pack_config.Nupkgs
	foreach ($item in $nupkgs) {
		if (!$item.IsPush){
            Write-Host "Ignore pack name $packName"
            continue
        }

		$targetFrameworks = $item.TargetFrameworks -Split ","
		foreach ($targetFramework in $targetFrameworks) {
			Collect-Assembly $item.PackName $targetFramework
			Collect-Localizations $item.PackName $targetFramework
		}
	}
}