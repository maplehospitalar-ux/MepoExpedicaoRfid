$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing

$inPng = 'C:\MepoExpedicaoRfid\src\MepoExpedicaoRfid\Assets\maple-logo.png'
$outIco = 'C:\MepoExpedicaoRfid\src\MepoExpedicaoRfid\Assets\maple.ico'

$img = [System.Drawing.Image]::FromFile($inPng)
$size = 64
$bmp = New-Object System.Drawing.Bitmap $size, $size
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.Clear([System.Drawing.Color]::Transparent)
$g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
$g.DrawImage($img, 0, 0, $size, $size)

$hIcon = $bmp.GetHicon()
$icon = [System.Drawing.Icon]::FromHandle($hIcon)

$fs = [System.IO.File]::Open($outIco, [System.IO.FileMode]::Create)
$icon.Save($fs)
$fs.Close()

# Cleanup
$icon.Dispose()
$g.Dispose()
$bmp.Dispose()
$img.Dispose()

Write-Output "ICO (simples) criado em: $outIco"
