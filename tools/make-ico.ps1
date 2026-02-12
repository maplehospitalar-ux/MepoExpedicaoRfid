$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing

$inPng = 'C:\MepoExpedicaoRfid\src\MepoExpedicaoRfid\Assets\maple-logo.png'
$outIco = 'C:\MepoExpedicaoRfid\src\MepoExpedicaoRfid\Assets\maple.ico'

$img = [System.Drawing.Image]::FromFile($inPng)
$sizes = 16,24,32,48,64,128,256
$entries = @()

foreach($s in $sizes){
  $bmp = New-Object System.Drawing.Bitmap $s, $s
  $g = [System.Drawing.Graphics]::FromImage($bmp)
  $g.Clear([System.Drawing.Color]::Transparent)
  $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
  $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
  $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
  $g.DrawImage($img, 0, 0, $s, $s)

  $ms = New-Object System.IO.MemoryStream
  $bmp.Save($ms, [System.Drawing.Imaging.ImageFormat]::Png)
  $bytes = $ms.ToArray()

  $g.Dispose()
  $bmp.Dispose()
  $ms.Dispose()

  $entries += [PSCustomObject]@{ Size=$s; Bytes=$bytes }
}

$img.Dispose()

# ICO header + directory entries
$fs = [System.IO.File]::Open($outIco, [System.IO.FileMode]::Create)
$bw = New-Object System.IO.BinaryWriter($fs)

$bw.Write([UInt16]0)  # reserved
$bw.Write([UInt16]1)  # type: icon
$bw.Write([UInt16]$entries.Count)

$offset = 6 + (16 * $entries.Count)
foreach($e in $entries){
  $s = [int]$e.Size
  $w = [byte]($s -band 0xFF)
  if($s -eq 256){ $w = 0 }
  $h = $w

  $bw.Write($w)                # width
  $bw.Write($h)                # height
  $bw.Write([byte]0)           # color count
  $bw.Write([byte]0)           # reserved
  $bw.Write([UInt16]1)         # planes
  $bw.Write([UInt16]32)        # bit count
  $bw.Write([UInt32]$e.Bytes.Length)
  $bw.Write([UInt32]$offset)
  $offset += $e.Bytes.Length
}

# image data (PNG-compressed)
foreach($e in $entries){
  $bw.Write($e.Bytes)
}

$bw.Flush()
$bw.Close()
$fs.Close()

Write-Output "ICO criado em: $outIco"
